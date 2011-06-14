
namespace RayTracer {
    public class Ray {
        public Vector3 m_Origin;
        public Vector3 m_Direction;
 
        public Ray(Vector3 Origin, Vector3 Direction) {
            m_Origin = Origin;
            m_Direction = Direction;
        }
    }
}
