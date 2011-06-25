using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer {
    class Plane : IGeometry {

        public Vector3 N;
        public Vector3 Q;

        public Plane(Vector3 n, Vector3 q) {
            N = n;
            Q = q;
            N.Normalize();
        }

        #region IGeometry Members

        public bool Intersects(Ray ray, ref Vector3 intPoint) {            
            Vector3 E = ray.E;
            Vector3 D = ray.D;            

            if (N * D != 0) {
                double t = (N * (Q - E)) / (N * D);
                if (t >= 0) {
                    intPoint = E + (D * t);
                    return true;
                }
            }

            return false;
        }

        public Vector3 GetSurfaceNormalAtPoint(Vector3 point) {
            return N;
        }

        public void GetColor(Vector3 point, ref int r, ref int g, ref int b) {
            r = 64;
            g = 64;
            b = 64;
        }

        #endregion
    }
}
