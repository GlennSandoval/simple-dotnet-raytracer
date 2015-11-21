namespace RayTracer
{

    public class Camera
    {

        public Vector3 Location
        {
            get; set;
        }

        public Vector3 LookAt
        {
            get; set;
        }

        private Vector3 zaxis;

        private void Init()
        {
            zaxis = LookAt - Location;
        }

        public Ray GetCameraRay(int x, int y)
        {
            Vector3 lookAt = new Vector3(x - Location.x, -(y - Location.y), 1 - Location.z);
            return new Ray(Location, lookAt);
        }
    }
}