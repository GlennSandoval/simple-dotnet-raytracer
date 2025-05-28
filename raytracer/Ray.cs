using Avalonia;

namespace raytracer;

public class Ray
{

    public Ray(Vector3D e, Vector3D d)
    {
        Source = e;

        Direction = Vector3D.Normalize(d);
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