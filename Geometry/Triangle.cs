using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace RayTracer
{
    public class Triangle : Geometry
    {
        private Vector3D _p1;
        private Vector3D _p2;
        private Vector3D _p3;
        private Vector3D m_Normal;

        public Triangle(Vector3D p1, Vector3D p2, Vector3D p3)
        {
            _p1 = p1;
            _p2 = p2;
            _p3 = p3;
        }

        public override Vector3D GetSurfaceNormalAtPoint(Vector3D point)
        {
            // Don't precalculate the normal, only calculate when needed.
            if (m_Normal == null)
            {
                var vec = Vector3D.CrossProduct(Vector3D.Subtract(_p2, _p1), Vector3D.Subtract(_p3, _p1));
                vec.Normalize();
                m_Normal = vec;
            }
            return m_Normal;

        }


        /*
        Taken from:  https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm
        Haven't taken the time to understand it. Hope it works.
        */
        public override bool Intersects(Ray ray, ref Vector3D intPoint)
        {
            Vector3D e1, e2;  //Edge1, Edge2
            Vector3D P, Q, T;
            double det, inv_det, u, v;
            double t;

            //Find vectors for two edges sharing V1
            e1 = Vector3D.Subtract(_p2, _p1);
            e2 = Vector3D.Subtract(_p3, _p1);
            //Begin calculating determinant - also used to calculate u parameter
            P = Vector3D.CrossProduct(ray.Direction, e2);
            //if determinant is near zero, ray lies in plane of triangle
            det = Vector3D.DotProduct(e1, P);
            //NOT CULLING
            if (det == 0) return false;
            inv_det = 1.0d / det;

            //calculate distance from V1 to ray origin
            T = Vector3D.Subtract(ray.Source, _p1);

            //Calculate u parameter and test bound
            u = Vector3D.DotProduct(T, P) * inv_det;
            //The intersection lies outside of the triangle
            if (u < 0.0d || u > 1.0d) return false;

            //Prepare to test v parameter
            Q = Vector3D.CrossProduct(T, e1);

            //Calculate V parameter and test bound
            v = Vector3D.DotProduct(ray.Direction, Q) * inv_det;
            //The intersection lies outside of the triangle
            if (v < 0.0d || u + v > 1.0d) return false;

            t = Vector3D.DotProduct(e2, Q) * inv_det;

            if (t > 0)
            { //ray intersection
                intPoint = ray.Source + ray.Direction * t;
                return true;
            }

            // No hit, no win
            return false;
        }
    }
}
