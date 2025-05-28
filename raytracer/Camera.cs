using Avalonia;

namespace raytracer;

public class Camera
{

    public Vector3D Location
    {
        get; set;
    }

    public Vector3D LookAt
    {
        get; set;
    }

    private Vector3D zaxis;

    private void Init()
    {
        zaxis = LookAt - Location;
    }

    public Ray GetCameraRay(int x, int y)
    {
        Vector3D lookAt = new(x - Location.X, -(y - Location.Y), 1 - Location.Z);
        return new Ray(Location, lookAt);
    }
}