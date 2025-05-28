using Avalonia;

namespace raytracer;

public interface IMaterial
{
    void GetColor(Vector3D point, ref int r, ref int g, ref int b);
}

public class SolidColor(int r, int g, int b) : IMaterial
{
    public int r = r, g = g, b = b;
    public double Phong = 0;

    public static readonly SolidColor Default = new(255, 255, 255);

    public void GetColor(Vector3D point, ref int r, ref int g, ref int b)
    {
        r = this.r;
        g = this.g;
        b = this.b;
    }

}