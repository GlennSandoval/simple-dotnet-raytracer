namespace RayTracer {

    public abstract class Geometry {

        private IMaterial m_Material = SolidColor.Default;

        public virtual IMaterial Material {
            get {
                return m_Material;
            }
            set {
                m_Material = value;
            }
        }
        
        virtual public void GetColor( Vector3 point, ref int r, ref int g, ref int b ) {
            Material.GetColor(point, ref r, ref g, ref b);
        }

        abstract public Vector3 GetSurfaceNormalAtPoint( Vector3 point );

        abstract public bool Intersects( Ray ray, ref Vector3 intPoint );
    }
}