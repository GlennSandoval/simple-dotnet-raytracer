
namespace RayTracer {
    public class Ray {
        public Vector3 E;
        public Vector3 D;
 
        public Ray(Vector3 e, Vector3 d) {
            E = e;
            D = d;
            D.Normalize();
        }
    }
}
