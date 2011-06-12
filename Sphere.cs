using System;

namespace RayTracer {
    public class Sphere : IGeometry {

        public double Radius { get; set; }
        Vector3 m_Center;

        public Sphere(Vector3 pos, double r) {
            Radius = r;
            m_Center = pos;
        }

        #region IGeometry Members
        public bool Intersects(Ray ray, ref Vector3 intPoint) {

            double distance = double.NaN;

            Vector3 originOffset = ray.m_Origin - m_Center;
            ray.m_Direction.Normalize();
            double a = ray.m_Direction.Dot();
            double b = 2 * (ray.m_Direction.Dot(originOffset));
            double c = originOffset.Dot() - (Radius * Radius);

            double discriminant = b * b - 4 * a * c;
            if (discriminant < 0) { return false; }

            // compute q as described above
            double distSqrt = Math.Sqrt(discriminant);
            double q;
            if (b < 0) {
                q = (-b - distSqrt) / 2.0;
            } else {
                q = (-b + distSqrt) / 2.0;
            }

            // compute t0 and t1
            double t0 = q / a;
            double t1 = c / q;

            // make sure t0 is smaller than t1
            if (t0 > t1) {
                // if t0 is bigger than t1 swap them around
                double temp = t0;
                t0 = t1;
                t1 = temp;
            }

            // if t1 is less than zero, the object is in the ray's negative direction
            // and consequently the ray misses the sphere
            if (t1 < 0) {
                return false;
            }

            // if t0 is less than zero, the intersection point is at t1
            if (t0 < 0) {
                distance = t1;
            } else {
                // else the intersection point is at t0
                distance = t0;
            }
            intPoint = ray.m_Origin + ray.m_Direction * distance;
            return true;
        }

        public Vector3 GetSurfaceNormalAtPoint(Vector3 point) {
            Vector3 normal = point - m_Center;
            normal.Normalize();
            return normal;
        }

        public Material Mat {
            get {
                return null;
            }
            set {
                return;
            }
        }

        #endregion
    }
}
