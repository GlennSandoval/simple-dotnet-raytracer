using System;
using System.Collections.Generic;
using System.Threading;
using Avalonia;
using Avalonia.Media;

namespace raytracer;

using raytracer.geometry;

public delegate void RenderBack(int x, int y, Color c);

internal class Raytracer(Scene scene)
{
    private readonly Lock m_CallbackLock = new();
    private readonly Lock m_GraphicsLock = new();
    private readonly int m_ProcessorCount = Math.Max(Environment.ProcessorCount - 1, 1);
    private bool m_Stop = false;
    private Action? m_UpdateCallback;
    private volatile int current = 0;

    private readonly Scene m_scene = scene;

    public delegate void ProgressHandler(double percent);
    public event ProgressHandler? OnProgress;

    private void DoProgress(double percent)
    {
        OnProgress?.Invoke(percent);
    }

    private void ReportProgress()
    {
        if (current == 0)
        {
            DoProgress(0);
        }
        else
        {
            DoProgress((double)current / (double)(Size.Width * Size.Height));
        }
    }

    public Color BackColor { get; set; }

    public int RayDepth { get; set; } = 5;

    public Scene Scene { get { return m_scene; } }

    public Size Size { get; set; }

    public bool Stop { get { return m_Stop; } set { m_Stop = value; } }

    /// <summary>
    /// Raytrace the scene onto the given image.
    /// </summary>
    /// <param name="onUpdate">Called after each row is rendered.</param>
    /// <param name="onFinished">Called when rendering is complete.</param>
    public void Start(RenderBack render, Size size, Action onUpdate, Action onFinished)
    {
        m_Stop = false;
        Size = size;
        m_UpdateCallback = onUpdate;

        new Thread(() =>
        {
            Raytrace(render, onFinished);
        }).Start();
    }

    private ColorAccumulator CalculateLighting(HitInfo info, int count)
    {
        ColorAccumulator ca = new();
        foreach (Light lt in Scene.Lights)
        {
            GetColor(info, lt, ca, count);
        }
        return ca;
    }

    private Color? CastCameraRay(int col, int row)
    {
        if (m_Stop)
        {
            return null;
        }

        Ray ray = Scene.Camera.GetCameraRay(col, row);

        ColorAccumulator? ca = CastRay(ray, 1);

        if (ca == null)
        {
            return null;
        }

        return Color.FromRgb((byte)ca.accumR, (byte)ca.accumG, (byte)ca.accumB);

    }

    private ColorAccumulator? CastRay(Ray ray, int count)
    {
        if (count > RayDepth)
        {
            return null;
        }

        HitInfo info = FindHitObject(ray);

        ColorAccumulator? ca;
        if (info.hitObj != null)
        {
            ca = CalculateLighting(info, count);
            ca.Clamp();
        }
        else
        {
            ca = new ColorAccumulator(BackColor.R, BackColor.G, BackColor.B);
        }

        return ca;
    }

    private HitInfo FindHitObject(Ray ray) => FindHitObject(ray, null, HitMode.Closest);

    private HitInfo FindHitObject(Ray ray, Geometry? originator, HitMode mode)
    {
        Vector3D intPoint = new(double.MaxValue, double.MaxValue, double.MaxValue);
        HitInfo info = new(null, intPoint, ray);
        double dist = double.MaxValue;
        foreach (Geometry geom in Scene.Geoms)
        {
            if (geom != originator && geom.Intersects(ray, ref intPoint))
            {
                double distToObj = (ray.Source - intPoint).Length;
                if (distToObj < dist)
                {
                    info.hitPoint = intPoint;
                    dist = distToObj;
                    info.hitObj = geom;
                    if (mode == HitMode.Any)
                    {
                        break;
                    }
                }
            }
        }
        return info;
    }

    private void GetColor(HitInfo info, Light lt, ColorAccumulator ca, int count)
    {
        if (info.hitObj == null)
        {
            return;
        }
        Vector3D lightNormal = Vector3D.Normalize(info.hitPoint - lt.Location);

        if (InShadow(info, lt, lightNormal))
        {
            return;
        }

        double lambert = Vector3D.Dot(lightNormal, info.Normal);
        if (lambert <= 0)
        {
            int r, g, b;
            r = b = g = 0;

            int r2 = 0;
            int g2 = 0;
            int b2 = 0;

            info.hitObj.GetColor(info.hitPoint, ref r, ref g, ref b);

            if (info.hitObj.Material is SolidColor solidColor)
            {
                double phongTerm = Math.Pow(lambert, 20) * solidColor.Phong * 2;
                r2 = (int)(lt.Color.R * phongTerm);
                g2 = (int)(lt.Color.G * phongTerm);
                b2 = (int)(lt.Color.B * phongTerm);
                double reflet = 2.0f * Vector3D.Dot(info.Normal, info.ray.Direction);
                Vector3D dir = info.ray.Direction - info.Normal * reflet;
                Ray reflect = new(info.hitPoint + dir, dir);
                ColorAccumulator? rca = CastRay(reflect, ++count);
                if (rca != null)
                {
                    ca.accumR += rca.accumR;
                    ca.accumG += rca.accumG;
                    ca.accumB += rca.accumB;
                }
            }
            ca.accumR += (int)(lt.Color.R * r * -lambert / 255) + r2;
            ca.accumG += (int)(lt.Color.G * g * -lambert / 255) + g2;
            ca.accumB += (int)(lt.Color.B * b * -lambert / 255) + b2;
        }
    }

    private bool InShadow(HitInfo info, Light lt, Vector3D lightNormal)
    {
        Ray shadowRay = new(lt.Location, lightNormal);
        HitInfo shadinfo = FindHitObject(shadowRay, info.hitObj, HitMode.Closest);
        if (shadinfo.hitObj != null && (lt.Location - info.hitPoint).Length > (lt.Location - shadinfo.hitPoint).Length)
        {
            return true;
        }
        return false;
    }

    private void Raytrace(RenderBack render, Action onFinished)
    {
        current = 0;
        double segmentsize = Math.Ceiling(Size.Height / m_ProcessorCount);
        List<Thread> threads = [];
        for (int i = 0; i < m_ProcessorCount; i++)
        {
            int start = 0 + (int)(i * segmentsize);
            int stop = (int)Math.Min((int)segmentsize + (int)(i * segmentsize), Size.Height);

            Thread t = new(() =>
            {
                RenderRows(render, start, stop);
            });
            t.Start();
            threads.Add(t);
        }

        foreach (Thread t in threads)
        {
            t.Join();
        }

        if (onFinished != null && !m_Stop)
        {
            onFinished.Invoke();
        }
    }

    private void RenderRow(RenderBack render, int row)
    {
        if (m_Stop)
        {
            return;
        }
        for (int i = 0; i < Size.Width; i++)
        {
            var color = CastCameraRay(i, row);
            if (color != null)
            {
                lock (m_GraphicsLock)
                {
                    render(row, i, color.Value);
                }
            }
            current++;
        }

        if (m_UpdateCallback != null)
        {
            try
            {
                lock (m_CallbackLock)
                {
                    if (!m_Stop)
                    {
                        m_UpdateCallback.Invoke();
                    }
                }
            }
            catch
            {
                return;
            }
        }
    }

    private void RenderRows(RenderBack render, int start, int stop)
    {
        for (int j = start; j < stop; j++)
        {
            RenderRow(render, j);
            ReportProgress();
        }
    }
}

internal enum HitMode
{
    Any,
    Closest
}

internal class ColorAccumulator
{
    public int accumB = 0;
    public int accumG = 0;
    public int accumR = 0;

    public ColorAccumulator()
    {
    }

    public ColorAccumulator(int r, int g, int b)
    {
        accumR = r;
        accumG = g;
        accumB = b;
    }

    public static ColorAccumulator operator +(ColorAccumulator left, ColorAccumulator right)
    {
        ColorAccumulator sum = new()
        {
            accumR = left.accumR + right.accumR,
            accumG = left.accumG + right.accumG,
            accumB = left.accumB + right.accumB
        };
        return sum;
    }

    public void Clamp()
    {
        double ratio = 1;
        ratio = Math.Max(accumR / 255.0, ratio);
        ratio = Math.Max(accumG / 255.0, ratio);
        ratio = Math.Max(accumB / 255.0, ratio);

        accumR = (int)(accumR / ratio);
        accumG = (int)(accumG / ratio);
        accumB = (int)(accumB / ratio);
    }
}

internal class HitInfo(Geometry? hitObj, Vector3D hitPoint, Ray ray)
{
    public Geometry? hitObj = hitObj;
    public Vector3D hitPoint = hitPoint;
    public Ray ray = ray;

    public Vector3D Normal
    {
        get
        {
            if (hitObj != null)
            {
                return hitObj.GetSurfaceNormalAtPoint(hitPoint);
            }
            else
            {
                throw new Exception("hitObj is null");
            }
        }
    }
}

