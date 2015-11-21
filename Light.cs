using System.Drawing;

namespace RayTracer
{

    public class Light
    {
        private Color m_Color = Color.White;

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

        public Vector3 Location
        {
            get; set;
        }
    }
}