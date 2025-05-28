using System;
using Avalonia;

namespace raytracer.geometry;

public class Sphere(Vector3D pos, double r) : Geometry
{
    public Vector3D Center
    {
        get;
        set;
    } = pos;

    public double Radius
    {
        get;
        set;
    } = r;

    override public Vector3D GetSurfaceNormalAtPoint(Vector3D point)
    {
        Vector3D normal = Vector3D.Normalize(point - Center);
        return normal;
    }

    override public bool Intersects(Ray ray, ref Vector3D intPoint)
    {
        double distance = double.NaN;

        Vector3D originOffset = ray.Source - Center;

        // a = 1 since  ray.D.Dot() = 1
        double b = 2.0 * Vector3D.Dot(ray.Direction, originOffset);
        double c = Vector3D.Dot(originOffset, originOffset) - (Radius * Radius);

        double discriminant = b * b - 4.0 * c;
        if (discriminant < 0)
        {
            return false;
        }

        // compute q as described above
        double distSqrt = Math.Sqrt(discriminant);
        double q;
        if (b > 0)
        {
            q = (-b - distSqrt) / 2.0;
        }
        else
        {
            q = (-b + distSqrt) / 2.0;
        }

        // compute t0 and t1
        double t0 = q;
        double t1 = c / q;

        // make sure t0 is smaller than t1
        if (t0 > t1)
        {

            // if t0 is bigger than t1 swap them around
            (t1, t0) = (t0, t1);
        }

        // if t1 is less than zero, the object is in the ray's negative direction
        // and consequently the ray misses the sphere
        if (t1 < 0)
        {
            return false;
        }

        // if t0 is less than zero, the intersection point is at t1
        if (t0 < 0)
        {
            distance = t1;
        }
        else
        {

            // else the intersection point is at t0
            distance = t0;
        }
        intPoint = ray.Source + ray.Direction * distance;
        return true;
    }
}
