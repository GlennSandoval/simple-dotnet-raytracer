using System;
using System.Drawing;
using System.Windows.Media.Media3D;

namespace RayTracer
{

    internal class Plane : Geometry
    {
        private Vector3D m_Normal;
        private Vector3D m_Point;

        /// <summary>
        /// Constructor for Plane
        /// </summary>
        /// <param name="n">The normal of the plane.</param>
        /// <param name="q">A point on the plane.</param>
        public Plane(Vector3D n, Vector3D q)
        {
            m_Normal = n;
            m_Point = q;
            m_Normal.Normalize();
            Material = new SolidColor(64, 64, 64);
        }

        /// <summary>
        /// Gets or sets the Planes normal.
        /// </summary>
        public Vector3D Normal
        {
            get
            {
                return m_Normal;
            }
            set
            {
                m_Normal = value;
            }
        }

        /// <summary>
        /// Gets or sets the point used to define the Plane.
        /// </summary>
        public Vector3D Point
        {
            get
            {
                return m_Point;
            }
            set
            {
                m_Point = value;
            }
        }

        override public Vector3D GetSurfaceNormalAtPoint(Vector3D point)
        {
            return m_Normal;
        }

        override public bool Intersects(Ray ray, ref Vector3D intPoint)
        {

            // Intersection formula from http://www.cl.cam.ac.uk/teaching/1999/AGraphHCI/SMAG/node2.html
            // t = (N * ( Q - E )) / (E - D)
            Vector3D E = ray.Source;
            Vector3D D = ray.Direction;
            Vector3D NegN = -m_Normal;
            if (Vector3D.DotProduct(m_Normal, D) != 0)
            {
                double t = (Vector3D.DotProduct(m_Normal, (Vector3D.Subtract(m_Point, E)))) / (Vector3D.DotProduct(m_Normal, D));
                if (t >= 0)
                {
                    intPoint = E + (D * t);
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// A Square is a section of a plane.
    /// </summary>
    internal class Square : Plane
    {
        public Size size;

        public Square(Vector3D n, Vector3D q, int width, int height)
            : base(n, q)
        {
            size = new Size(width, height);
            Material = new SolidColor(64, 64, 128);
        }

        public override bool Intersects(Ray ray, ref Vector3D intPoint)
        {
            Vector3D E = ray.Source;
            Vector3D D = ray.Direction;

            // Do the intersection test as for a Plane, but check that the intersection point lies within the region of a square on the plane.
            if (Vector3D.DotProduct(Normal, D) != 0)
            {
                double t = (Vector3D.DotProduct(Normal, (Vector3D.Subtract(Point, E)))) / (Vector3D.DotProduct(Normal, D));
                if (t >= 0)
                {
                    intPoint = E + (D * t);
                    if (Math.Abs(intPoint.X - Point.X) < size.Width && Math.Abs(intPoint.Y - Point.Y) < size.Height)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}