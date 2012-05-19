using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace RayTracer {

    internal class Raytracer {
        int rayDepth = 3;
        private object m_callbackLock = new object();
        private Action m_callback;
        public Scene scene;
        public Size size;
        public Color BackColor;
        private int m_processorCount;
        Graphics m_graphic;
        public bool stop = false;

        public Raytracer() {
            m_processorCount = Environment.ProcessorCount;
        }

        public void RayTrace( Image image ) {
            RayTrace( image, null, null );
        }

        public void RayTrace( Image image, Action onUpdate, Action onFinished ) {
            size = new Size( image.Width, image.Height );
            m_callback = onUpdate;
            m_graphic = Graphics.FromImage( image );

            new Thread( () => {
                Go( onFinished );
            } ).Start();
        }

        private void Go( Action onFinished ) {
            double segmentsize = Math.Ceiling( size.Height / (double)m_processorCount );
            List<Thread> threads = new List<Thread>();
            for( int i = 0; i < m_processorCount; i++ ) {
                int start = 0 + (int)( i * segmentsize );
                int stop = Math.Min( (int)segmentsize + (int)( i * segmentsize ), size.Height );

                Thread t = new Thread( () => {
                    RenderRows( start, stop );
                } );
                t.Start();
                threads.Add( t );
            }

            foreach( Thread t in threads ) {
                t.Join();
            }

            if( onFinished != null && !this.stop ) {
                try {
                    onFinished();
                } catch {
                    //carry on
                }
            }
        }

        private void RenderRows( int start, int stop ) {
            for( int j = start; j < stop; j++ ) {
                RenderColumns( j );
            }
        }

        private void RenderColumns( int row ) {
            if( this.stop ) {
                return;
            }
            for( int i = 0; i < size.Width; i++ ) {
                CastCameraRay( i, row );
            }

            if( m_callback != null ) {
                try {
                    lock( m_callbackLock ) {
                        if( !this.stop ) {
                            m_callback();
                        }
                    }
                } catch {
                    return;
                }
            }
        }

        private Ray GetCameraRay( int x, int y ) {
            Vector3 lookAt = new Vector3( x, y, 0 ) - scene.camera.Location;
            lookAt.Normalize();
            return new Ray( scene.camera.Location, lookAt );
        }

        private HitInfo FindHitObject( Ray ray ) {
            return FindHitObject( ray, null, HitMode.Closest );
        }

        private HitInfo FindHitObject( Ray ray, IGeometry originator, HitMode mode ) {
            Vector3 intPoint = new Vector3( double.MaxValue, double.MaxValue, double.MaxValue );
            HitInfo info = new HitInfo( null, intPoint, ray );
            double dist = double.MaxValue;
            foreach( IGeometry geom in scene.geoms ) {
                if( geom != originator && geom.Intersects( ray, ref intPoint ) ) {
                    double distToObj = ray.E.DistanceTo( intPoint );
                    if( distToObj < dist ) {
                        info.hitPoint = intPoint;
                        dist = distToObj;
                        info.hitObj = geom;
                        if( mode == HitMode.Any ) {
                            break;
                        }
                    }
                }
            }
            return info;
        }

        private ColorAccumulator CalculateLighting( HitInfo info, int count ) {
            ColorAccumulator ca = new ColorAccumulator();
            foreach( Light lt in scene.lights ) {
                GetColor( info, lt, ca, count );
            }
            return ca;
        }

        private void GetColor( HitInfo info, Light lt, ColorAccumulator ca, int count ) {
            Vector3 lightNormal = info.hitPoint - lt.location;
            lightNormal.Normalize();

            if( InShadow( info, lt, lightNormal ) ) {
                return;
            }

            double lambert = lightNormal.Dot( info.normal );
            if( lambert <= 0 ) {
                int r, g, b;
                r = b = g = 0;

                int r2 = 0;
                int g2 = 0;
                int b2 = 0;

                info.hitObj.GetColor( info.hitPoint, ref r, ref g, ref b );
                if( info.hitObj.material != null && info.hitObj.material is SolidColor ) {
                    double phongTerm = Math.Pow( lambert, 20 ) * ( info.hitObj.material as SolidColor ).Phong * 2;
                    r2 = (int)( lt.color.R * phongTerm );
                    g2 = (int)( lt.color.G * phongTerm );
                    b2 = (int)( lt.color.B * phongTerm );
                    double reflet = 2.0f * ( info.normal * info.ray.D );
                    Vector3 dir = info.ray.D - info.normal * reflet;
                    Ray reflect = new Ray( info.hitPoint + dir, dir );
                    ColorAccumulator rca = CastRay( reflect, ++count );
                    if( rca != null ) {
                        ca.accumR = ca.accumR + rca.accumR;
                        ca.accumG = ca.accumG + rca.accumG;
                        ca.accumB = ca.accumB + rca.accumB;
                    }
                }
                ca.accumR += (int)( ( lt.color.R * r * -lambert ) / 255 ) + r2;
                ca.accumG += (int)( ( lt.color.G * g * -lambert ) / 255 ) + g2;
                ca.accumB += (int)( ( lt.color.B * b * -lambert ) / 255 ) + b2;
            }
        }

        private bool InShadow( HitInfo info, Light lt, Vector3 lightNormal ) {
            Ray shadowRay = new Ray( lt.location, lightNormal );
            HitInfo shadinfo = FindHitObject( shadowRay, info.hitObj, HitMode.Closest );
            if( shadinfo.hitObj != null && lt.location.DistanceTo( info.hitPoint ) > lt.location.DistanceTo( shadinfo.hitPoint ) ) {
                return true;
            }
            return false;
        }

        private ColorAccumulator CastRay( Ray ray, int count ) {
            if( count > rayDepth ) {
                return null;
            }

            ColorAccumulator ca = null;
            HitInfo info = FindHitObject( ray );

            if( info.hitObj != null ) {
                ca = CalculateLighting( info, count );
                ca.Clamp();
            } else {
                ca = new ColorAccumulator( BackColor.R, BackColor.G, BackColor.B );
            }

            return ca;
        }

        private void CastCameraRay( int col, int row ) {
            if( this.stop ) {
                return;
            }

            Ray ray = GetCameraRay( col, row );
            HitInfo info = FindHitObject( ray );

            ColorAccumulator ca = CastRay( ray, 1 );

            SolidBrush brush = new SolidBrush( Color.FromArgb( ca.accumR, ca.accumG, ca.accumB ) );
            lock( m_graphic ) {
                m_graphic.FillRectangle( brush, col, row, 1, 1 );
            }
        }
    }

    internal class ColorAccumulator {
        public int accumR = 0;
        public int accumG = 0;
        public int accumB = 0;

        public void Clamp() {
            accumR = Clamp( accumR );
            accumG = Clamp( accumG );
            accumB = Clamp( accumB );
        }

        public ColorAccumulator() {
        }

        public ColorAccumulator( int r, int g, int b ) {
            accumR = r;
            accumG = g;
            accumB = b;
        }

        private static int Clamp( int num ) {
            int clamped = num;
            if( clamped > 255 ) {
                clamped = 255;
            }
            if( clamped < 0 ) {
                clamped = 0;
            }
            return clamped;
        }

        public static ColorAccumulator operator +( ColorAccumulator left, ColorAccumulator right ) {
            ColorAccumulator sum = new ColorAccumulator();
            sum.accumR = left.accumR + right.accumR;
            sum.accumG = left.accumG + right.accumG;
            sum.accumB = left.accumB + right.accumB;
            return sum;
        }
    }

    internal class HitInfo {
        public IGeometry hitObj;
        public Vector3 hitPoint;
        public Ray ray;

        public HitInfo( IGeometry hitObj, Vector3 hitPoint, Ray ray ) {
            this.hitObj = hitObj;
            this.hitPoint = hitPoint;
            this.ray = ray;
        }

        public Vector3 normal {
            get {
                if( hitObj != null ) {
                    return hitObj.GetSurfaceNormalAtPoint( hitPoint );
                } else {
                    throw new Exception( "hitObj is null" );
                }
            }
        }
    }

    internal enum HitMode {
        Any,
        Closest
    }
}