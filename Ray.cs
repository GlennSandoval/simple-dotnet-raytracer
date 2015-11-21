namespace RayTracer
{

    public class Ray
    {

        public Ray(Vector3 e, Vector3 d)
        {
            Source = e;
            d.Normalize();
            Direction = d;
        }

        public Vector3 Direction
        {
            get; set;
        }

        public Vector3 Source
        {
            get; set;
        }
    }
}