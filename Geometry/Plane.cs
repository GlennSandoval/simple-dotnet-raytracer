using System;
using System.Drawing;

namespace RayTracer {

    internal class Plane : Geometry {
        private Vector3 m_Normal;
        private Vector3 m_Point;

        /// <summary>
        /// Constructor for Plane
        /// </summary>
        /// <param name="n">The normal of the plane.</param>
        /// <param name="q">A point on the plane.</param>
        public Plane( Vector3 n, Vector3 q ) {
            m_Normal = n;
            m_Point = q;
            m_Normal.Normalize();
            Material = new SolidColor( 64, 64, 64 );
        }

        /// <summary>
        /// Gets or sets the Planes normal.
        /// </summary>
        public Vector3 Normal {
            get {
                return m_Normal;
            }
            set {
                m_Normal = value;
            }
        }

        /// <summary>
        /// Gets or sets the point used to define the Plane.
        /// </summary>
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
            // Intersection formula from http://www.cl.cam.ac.uk/teaching/1999/AGraphHCI/SMAG/node2.html
            // t = (N * ( Q - E )) / (E - D)
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
    }

    /// <summary>
    /// A Square is a section of a plane.
    /// </summary>
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

            // Do the intersection test as for a Plane, but check that the intersection point lies within the region of a square on the plane.
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