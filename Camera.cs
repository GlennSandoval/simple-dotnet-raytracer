using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RayTracer {
    class Camera {

        public Vector3 Origin { get;  set;}

        public Vector3 LookAt { get; set; }

        public Vector2 Start { get; set; }

        public Vector2 End { get; set; }
    }
}
