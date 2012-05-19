using System.Collections.Generic;

namespace RayTracer {

    internal class Scene {
        public Camera camera;

        public List<IGeometry> geoms = new List<IGeometry>();

        public List<Light> lights = new List<Light>();
    }
}