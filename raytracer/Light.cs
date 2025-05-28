
using Avalonia;
using Avalonia.Media;

namespace raytracer;

public class Light
{
    private Color m_Color = Colors.White;

    public Color Color
    {
        get
        {
            return m_Color;
        }
        set
        {
            m_Color = value;
        }
    }

    public Vector3D Location
    {
        get; set;
    }
}