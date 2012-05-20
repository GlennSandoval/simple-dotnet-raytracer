using System.Collections.Generic;

namespace RayTracer {

    public class Scene {
        private Camera m_Camera;
        private List<IGeometry> m_Geoms = new List<IGeometry>();
        private List<Light> m_Lights = new List<Light>();        

        public Camera Camera {
            get {
                return m_Camera;
            }
            set {
                m_Camera = value;
            }
        }

        public List<IGeometry> Geoms {
            get {
                return m_Geoms;
            }
            set {
                m_Geoms = value;
            }
        }

        public List<Light> Lights {
            get {
                return m_Lights;
            }
            set {
                m_Lights = value;
            }
        }
    }
}