namespace RayTracer {

    public interface IGeometry {

        bool Intersects( Ray ray, ref Vector3 intPoint );

        Vector3 GetSurfaceNormalAtPoint( Vector3 point );

        void GetColor( Vector3 point, ref int r, ref int g, ref int b );

        IMaterial material {
            get;
            set;
        }
    }
}