namespace RayTracer {

    public interface IGeometry {

        IMaterial Material {
            get;
            set;
        }

        void GetColor( Vector3 point, ref int r, ref int g, ref int b );

        Vector3 GetSurfaceNormalAtPoint( Vector3 point );

        bool Intersects( Ray ray, ref Vector3 intPoint );
    }
}