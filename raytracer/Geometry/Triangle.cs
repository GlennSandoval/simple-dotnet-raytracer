
using Avalonia;

namespace raytracer.geometry;

public class Triangle(Vector3D p1, Vector3D p2, Vector3D p3) : Geometry
{
    private readonly Vector3D _p1 = p1;
    private readonly Vector3D _p2 = p2;
    private readonly Vector3D _p3 = p3;
    private Vector3D? m_Normal;

    public override Vector3D GetSurfaceNormalAtPoint(Vector3D point)
    {
        // Don't precalculate the normal, only calculate when needed.
        if (m_Normal == null)
        {
            var vec = (_p2 - _p1).CrossProduct(_p3 - _p1);
            m_Normal = Vector3D.Normalize(vec);
        }
        return (Vector3D)m_Normal;

    }


    /*
    Taken from:  https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm
    Haven't taken the time to understand it. Hope it works.
    */
    public override bool Intersects(Ray ray, ref Vector3D intPoint)
    {
        Vector3D e1, e2;  //Edge1, Edge2
        Vector3D P, Q, T;
        double det, inv_det, u, v;
        double t;

        //Find vectors for two edges sharing V1
        e1 = _p2 - _p1;
        e2 = _p3 - _p1;
        //Begin calculating determinant - also used to calculate u parameter
        P = ray.Direction.CrossProduct(e2);
        //if determinant is near zero, ray lies in plane of triangle
        det = Vector3D.Dot(e1, P);
        //NOT CULLING
        if (det == 0) return false;
        inv_det = 1.0d / det;

        //calculate distance from V1 to ray origin
        T = (ray.Source - _p1);

        //Calculate u parameter and test bound
        u = Vector3D.Dot(T, P) * inv_det;
        //The intersection lies outside of the triangle
        if (u < 0.0d || u > 1.0d) return false;

        //Prepare to test v parameter
        Q = T.CrossProduct(e1);

        //Calculate V parameter and test bound
        v = Vector3D.Dot(ray.Direction, Q) * inv_det;
        //The intersection lies outside of the triangle
        if (v < 0.0d || u + v > 1.0d) return false;

        t = Vector3D.Dot(e2, Q) * inv_det;

        if (t > 0)
        { //ray intersection
            intPoint = ray.Source + ray.Direction * t;
            return true;
        }

        // No hit, no win
        return false;
    }
}

