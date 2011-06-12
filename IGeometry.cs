
namespace RayTracer {
    interface IGeometry {
        bool Intersects(Ray ray, ref Vector3 intPoint);

        Vector3 GetSurfaceNormalAtPoint(Vector3 point);

        Material Mat { get; set; }

    }
}
