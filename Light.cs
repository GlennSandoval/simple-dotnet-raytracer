using System.Drawing;

namespace RayTracer {

	public class Light {
		private Color m_Color = Color.White;
		private Vector3 m_Location;

		public Color Color {
			get {
				return m_Color;
			}
			set {
				m_Color = value;
			}
		}

		public Vector3 Location {
			get {
				return m_Location;
			}
			set {
				m_Location = value;
			}
		}
	}
}