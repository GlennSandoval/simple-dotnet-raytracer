using System.Drawing;

namespace RayTracer {

    public interface IMaterial {

        void GetColor( Vector3 point, ref int r, ref int g, ref int b );
    }

    public class SolidColor : IMaterial {
        public Color Color;
        public double Phong = 0;

        public void GetColor( Vector3 point, ref int r, ref int g, ref int b ) {
            r = Color.R;
            g = Color.G;
            b = Color.B;
        }
        #region IMaterial Members
        #endregion IMaterial Members
    }
}