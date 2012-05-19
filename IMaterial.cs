using System.Drawing;

namespace RayTracer {

    public interface IMaterial {

        void GetColor( Vector3 point, ref int r, ref int g, ref int b );
    }

    public class SolidColor : IMaterial {
        public double Phong = 0;
        public Color color;

        #region IMaterial Members

        public void GetColor( Vector3 point, ref int r, ref int g, ref int b ) {
            r = color.R;
            g = color.G;
            b = color.B;
        }

        #endregion IMaterial Members
    }
}