using System.Windows.Media.Media3D;

namespace RayTracer
{

    public class Ray
    {

        public Ray(Vector3D e, Vector3D d)
        {
            Source = e;
            d.Normalize();
            Direction = d;
        }

        public Vector3D Direction
        {
            get; set;
        }

        public Vector3D Source
        {
            get; set;
        }
    }
}