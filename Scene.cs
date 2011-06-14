using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer {
    class Scene {

        public Camera camera;

        public List<IGeometry> geoms = new List<IGeometry>();

        public List<Light> lights = new List<Light>();
    }
}
