namespace RayTracer {

	public class Camera {
		public Vector3 m_Location;
		public Vector3 m_LookAt;

		public Vector3 Location {
			get {
				return m_Location;
			}
			set {
				m_Location = value;
			}
		}

		public Vector3 LookAt {
			get {
				return m_LookAt;
			}
			set {
				m_LookAt = value;
			}
		}

		private Vector3 zaxis;

		private void Init() {
			zaxis = m_LookAt - m_Location;
		}

		public Ray GetCameraRay( int x, int y ) {
			Vector3 lookAt = new Vector3( x - Location.x, -( y - Location.y ), 1 - Location.z );
			return new Ray( Location, lookAt );
		}
	}
}