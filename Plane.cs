using System;
using System.Drawing;

namespace RayTracer {

    internal class Plane : Geometry {
        private Vector3 m_Normal;
        private Vector3 m_Point;

        public Plane( Vector3 n, Vector3 q ) {
            m_Normal = n;
            m_Point = q;
            m_Normal.Normalize();
            Material = new SolidColor( 64, 64, 64 );
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

        override public Vector3 GetSurfaceNormalAtPoint( Vector3 point ) {
            return m_Normal;
        }

        override public bool Intersects( Ray ray, ref Vector3 intPoint ) {
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

            return false;
        }
        #region Geometry Members
        #endregion Geometry Members
    }

    internal class Square : Plane {
        public Size size;

        public Square( Vector3 n, Vector3 q, int width, int height )
            : base( n, q ) {
            size = new Size( width, height );
            Material = new SolidColor( 64, 64, 128 );
        }

        public override bool Intersects( Ray ray, ref Vector3 intPoint ) {
            Vector3 E = ray.Source;
            Vector3 D = ray.Direction;

            if( Normal * D != 0 ) {
                double t = ( Normal * ( Point - E ) ) / ( Normal * D );
                if( t >= 0 ) {
                    intPoint = E + ( D * t );
                    if( Math.Abs( intPoint.x - Point.x ) < size.Width && Math.Abs( intPoint.y - Point.y ) < size.Height ) {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}