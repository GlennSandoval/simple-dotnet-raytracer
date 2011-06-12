
namespace RayTracer {
    public class Ray {
        public Vector3 m_Origin;
        public Vector3 m_Direction;
        public double t;

        public Ray(Vector3 Origin, Vector3 Direction) {
            m_Origin = Origin;
            m_Direction = Direction;
            // Normalize();
        }

        private void Normalize() {
            m_Direction.Normalize();
            m_Direction = m_Origin + m_Direction;
        }

    }
}
