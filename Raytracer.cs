using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Media.Media3D;

namespace RayTracer
{

    internal class Raytracer
    {
        private object m_CallbackLock = new object();
        private Graphics m_Graphic;
        private Object m_GraphicsLock = new object();
        private int m_ProcessorCount;
        private bool m_Stop = false;
        private Action m_UpdateCallback;
        private volatile int current = 0;

        public delegate void ProgressHandler(double percent);
        public event ProgressHandler OnProgress;

        public Raytracer()
        {
            m_ProcessorCount = Math.Max(Environment.ProcessorCount - 1, 1);
        }

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

        public int RayDepth { get; set; } = 3;

        public Scene Scene { get; set; }

        public Size Size { get; set; }

        public bool Stop { get; set; }

        /// <summary>
        /// Raytrace the scene onto the given image.
        /// </summary>
        /// <param name="image">Image that will be rendered to.</param>
        public void Raytrace(Image image)
        {
            Raytrace(image, null, null);
        }

        /// <summary>
        /// Raytrace the scene onto the given image.
        /// </summary>
        /// <param name="image">Image that will be rendered to.</param>
        /// <param name="onUpdate">Called after each row is rendered.</param>
        /// <param name="onFinished">Called when rendering is complete.</param>
        public void Raytrace(Image image, Action onUpdate, Action onFinished)
        {
            Size = new Size(image.Width, image.Height);
            m_UpdateCallback = onUpdate;
            m_Graphic = Graphics.FromImage(image);

            new Thread(() =>
            {
                Raytrace(onFinished);
            }).Start();
        }

        private ColorAccumulator CalculateLighting(HitInfo info, int count)
        {
            ColorAccumulator ca = new ColorAccumulator();
            foreach (Light lt in Scene.Lights)
            {
                GetColor(info, lt, ca, count);
            }
            return ca;
        }

        private void CastCameraRay(int col, int row)
        {
            if (this.m_Stop)
            {
                return;
            }

            Ray ray = Scene.Camera.GetCameraRay(col, row);

            ColorAccumulator ca = CastRay(ray, 1);

            SolidBrush brush = new SolidBrush(Color.FromArgb(ca.accumR, ca.accumG, ca.accumB));
            lock (m_GraphicsLock)
            {
                m_Graphic.FillRectangle(brush, col, row, 1, 1);
            }
        }

        private ColorAccumulator CastRay(Ray ray, int count)
        {
            if (count > RayDepth)
            {
                return null;
            }

            ColorAccumulator ca = null;
            HitInfo info = FindHitObject(ray);

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

        private HitInfo FindHitObject(Ray ray, Geometry originator, HitMode mode)
        {
            Vector3D intPoint = new Vector3D(double.MaxValue, double.MaxValue, double.MaxValue);
            HitInfo info = new HitInfo(null, intPoint, ray);
            double dist = double.MaxValue;
            foreach (Geometry geom in Scene.Geoms)
            {
                if (geom != originator && geom.Intersects(ray, ref intPoint))
                {
                    double distToObj = Vector3D.Subtract(ray.Source, intPoint).Length;
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
            Vector3D lightNormal = info.hitPoint - lt.Location;
            lightNormal.Normalize();

            if (InShadow(info, lt, lightNormal))
            {
                return;
            }

            double lambert = Vector3D.DotProduct(lightNormal, info.normal);
            if (lambert <= 0)
            {
                int r, g, b;
                r = b = g = 0;

                int r2 = 0;
                int g2 = 0;
                int b2 = 0;

                info.hitObj.GetColor(info.hitPoint, ref r, ref g, ref b);
                if (info.hitObj.Material != null && info.hitObj.Material is SolidColor)
                {
                    double phongTerm = Math.Pow(lambert, 20) * (info.hitObj.Material as SolidColor).Phong * 2;
                    r2 = (int)(lt.Color.R * phongTerm);
                    g2 = (int)(lt.Color.G * phongTerm);
                    b2 = (int)(lt.Color.B * phongTerm);
                    double reflet = 2.0f * (Vector3D.DotProduct(info.normal, info.ray.Direction));
                    Vector3D dir = info.ray.Direction - info.normal * reflet;
                    Ray reflect = new Ray(info.hitPoint + dir, dir);
                    ColorAccumulator rca = CastRay(reflect, ++count);
                    if (rca != null)
                    {
                        ca.accumR = ca.accumR + rca.accumR;
                        ca.accumG = ca.accumG + rca.accumG;
                        ca.accumB = ca.accumB + rca.accumB;
                    }
                }
                ca.accumR += (int)((lt.Color.R * r * -lambert) / 255) + r2;
                ca.accumG += (int)((lt.Color.G * g * -lambert) / 255) + g2;
                ca.accumB += (int)((lt.Color.B * b * -lambert) / 255) + b2;
            }
        }

        private bool InShadow(HitInfo info, Light lt, Vector3D lightNormal)
        {
            Ray shadowRay = new Ray(lt.Location, lightNormal);
            HitInfo shadinfo = FindHitObject(shadowRay, info.hitObj, HitMode.Closest);
            if (shadinfo.hitObj != null && Vector3D.Subtract(lt.Location, info.hitPoint).Length > Vector3D.Subtract(lt.Location, shadinfo.hitPoint).Length)
            {
                return true;
            }
            return false;
        }

        private void Raytrace(Action onFinished)
        {
            current = 0;
            double segmentsize = Math.Ceiling(Size.Height / (double)m_ProcessorCount);
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < m_ProcessorCount; i++)
            {
                int start = 0 + (int)(i * segmentsize);
                int stop = Math.Min((int)segmentsize + (int)(i * segmentsize), Size.Height);

                Thread t = new Thread(() =>
                {
                    RenderRows(start, stop);
                });
                t.Start();
                threads.Add(t);
            }

            foreach (Thread t in threads)
            {
                t.Join();
            }

            if (onFinished != null && !this.m_Stop)
            {
                onFinished.Invoke();
            }
        }

        private void RenderRow(int row)
        {
            if (this.m_Stop)
            {
                return;
            }
            for (int i = 0; i < Size.Width; i++)
            {
                CastCameraRay(i, row);
                current++;
            }

            if (m_UpdateCallback != null)
            {
                try
                {
                    lock (m_CallbackLock)
                    {
                        if (!this.m_Stop)
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

        private void RenderRows(int start, int stop)
        {
            for (int j = start; j < stop; j++)
            {
                RenderRow(j);
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
            ColorAccumulator sum = new ColorAccumulator();
            sum.accumR = left.accumR + right.accumR;
            sum.accumG = left.accumG + right.accumG;
            sum.accumB = left.accumB + right.accumB;
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

    internal class HitInfo
    {
        public Geometry hitObj;
        public Vector3D hitPoint;
        public Ray ray;

        public HitInfo(Geometry hitObj, Vector3D hitPoint, Ray ray)
        {
            this.hitObj = hitObj;
            this.hitPoint = hitPoint;
            this.ray = ray;
        }

        public Vector3D normal
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
}