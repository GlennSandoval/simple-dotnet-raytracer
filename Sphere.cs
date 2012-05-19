using System;

namespace RayTracer {

    public class Sphere : IGeometry {

        public double Radius {
            get;
            set;
        }

        public Vector3 m_Center;

        public Sphere( Vector3 pos, double r ) {
            Radius = r;
            m_Center = pos;
        }

        #region IGeometry Members

        public bool Intersects( Ray ray, ref Vector3 intPoint ) {
            double distance = double.NaN;

            Vector3 originOffset = ray.E - m_Center;
            // a = 1 since  ray.D.Dot() = 1
            double b = 2.0 * ( ray.D.Dot( originOffset ) );
            double c = originOffset.Dot() - ( Radius * Radius );

            double discriminant = b * b - 4.0 * c;
            if( discriminant < 0 ) {
                return false;
            }

            // compute q as described above
            double distSqrt = Math.Sqrt( discriminant );
            double q;
            if( b > 0 ) {
                q = ( -b - distSqrt ) / 2.0;
            } else {
                q = ( -b + distSqrt ) / 2.0;
            }

            // compute t0 and t1
            double t0 = q;
            double t1 = c / q;

            // make sure t0 is smaller than t1
            if( t0 > t1 ) {
                // if t0 is bigger than t1 swap them around
                double temp = t0;
                t0 = t1;
                t1 = temp;
            }

            // if t1 is less than zero, the object is in the ray's negative direction
            // and consequently the ray misses the sphere
            if( t1 < 0 ) {
                return false;
            }

            // if t0 is less than zero, the intersection point is at t1
            if( t0 < 0 ) {
                distance = t1;
            } else {
                // else the intersection point is at t0
                distance = t0;
            }
            intPoint = ray.E + ray.D * distance;
            return true;
        }

        public Vector3 GetSurfaceNormalAtPoint( Vector3 point ) {
            Vector3 normal = point - m_Center;
            normal.Normalize();
            return normal;
        }

        public void GetColor( Vector3 point, ref int r, ref int g, ref int b ) {
            if( material != null ) {
                material.GetColor( point, ref r, ref g, ref b );
            } else {
                r = 255;
                g = 255;
                b = 255;
            }
        }

        public IMaterial material {
            get;
            set;
        }

        #endregion IGeometry Members
    }
}