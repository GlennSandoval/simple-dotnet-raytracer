using System;
using System.Drawing;

namespace RayTracer {

    internal class Plane : IGeometry {
        private Vector3 m_Normal;
        private Vector3 m_Point;

        public Plane( Vector3 n, Vector3 q ) {
            m_Normal = n;
            m_Point = q;
            m_Normal.Normalize();
        }

        public IMaterial Material {
            get;
            set;
        }

        public Vector3 Normal {
            get {
                return m_Normal;
            }
            set {
                m_Normal = value;
            }
        }

        public Vector3 Point {
            get {
                return m_Point;
            }
            set {
                m_Point = value;
            }
        }

        public void GetColor( Vector3 point, ref int r, ref int g, ref int b ) {
            r = 64;
            g = 64;
            b = 64;
        }

        public Vector3 GetSurfaceNormalAtPoint( Vector3 point ) {
            return m_Normal;
        }

        public bool Intersects( Ray ray, ref Vector3 intPoint ) {
            Vector3 E = ray.Source;
            Vector3 D = ray.Direction;
            Vector3 NegN = -m_Normal;
            if( m_Normal * D != 0 ) {
                double t = ( m_Normal * ( m_Point - E ) ) / ( m_Normal * D );
                if( t >= 0 ) {
                    intPoint = E + ( D * t );
                    return true;
                }
            }

            //if (NegN * D != 0) {
            //    double t = (NegN * (Q - E)) / (NegN * D);
            //    if (t >= 0) {
            //        intPoint = E + (D * t);
            //        return true;
            //    }
            //}

            return false;
        }
        #region IGeometry Members
        #endregion IGeometry Members
    }

    internal class Square : IGeometry {
        public Vector3 N;
        public Vector3 Q;
        public Size size;

        public Square( Vector3 n, Vector3 q, int width, int height ) {
            N = n;
            Q = q;
            size = new Size( width, height );

            N.Normalize();
        }

        public IMaterial Material {
            get;
            set;
        }

        public void GetColor( Vector3 point, ref int r, ref int g, ref int b ) {
            r = 64;
            g = 64;
            b = 128;
        }

        public Vector3 GetSurfaceNormalAtPoint( Vector3 point ) {
            return N;
        }

        public bool Intersects( Ray ray, ref Vector3 intPoint ) {
            Vector3 E = ray.Source;
            Vector3 D = ray.Direction;

            if( N * D != 0 ) {
                double t = ( N * ( Q - E ) ) / ( N * D );
                if( t >= 0 ) {
                    intPoint = E + ( D * t );
                    if( Math.Abs( intPoint.x - Q.x ) < size.Width && Math.Abs( intPoint.y - Q.y ) < size.Height ) {
                        return true;
                    }
                }
            }

            return false;
        }
        #region IGeometry Members
        #endregion IGeometry Members
    }
}