namespace RayTracer {

    public class Ray {
        private Vector3 m_Direction;
        private Vector3 m_Source;

        public Ray( Vector3 e, Vector3 d ) {
            m_Source = e;
            m_Direction = d;
            m_Direction.Normalize();
        }

        public Vector3 Direction {
            get {
                return m_Direction;
            }
            set {
                m_Direction = value;
            }
        }

        public Vector3 Source {
            get {
                return m_Source;
            }
            set {
                m_Source = value;
            }
        }
    }
}