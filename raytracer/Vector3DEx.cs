using Avalonia;

namespace raytracer;

public static class Vector3DEX
{
    public static Vector3D CrossProduct(this Vector3D v1, Vector3D v2)
    {
        double x = v1.Y * v2.Z - v1.Z * v2.Y;
        double y = v1.Z * v2.X - v1.X * v2.Z;
        double z = v1.X * v2.Y - v1.Y * v2.X;
        return new Vector3D(x, y, z);
    }

}