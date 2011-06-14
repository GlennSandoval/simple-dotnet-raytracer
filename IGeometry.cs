
namespace RayTracer {
    public interface IGeometry {
        bool Intersects(Ray ray, ref Vector3 intPoint);

        Vector3 GetSurfaceNormalAtPoint(Vector3 point);

        IMaterial Mat { get; set; }

    }
}
