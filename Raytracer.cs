using System.Drawing;

namespace RayTracer {
    class Raytracer {
        public Scene scene;

        public Size size;

        public Color BackColor;

        public void RayTrace(Graphics graphic) {
            for (int j = 0; j < size.Height; j++) {
                for (int i = 0; i < size.Width; i++) {

                    Vector3 lookAt = new Vector3(i, j, 0) - scene.camera.Origin;
                    lookAt.Normalize();
                    Ray ray = new Ray(scene.camera.Origin, lookAt);
                    Vector3 intPoint = new Vector3();

                    bool hit = false;
                    foreach (IGeometry geom in scene.geoms) {
                        if (geom.Intersects(ray, ref intPoint)) {
                            int accumR = 0;
                            int accumG = 0;
                            int accumB = 0;
                            hit = true;
                            Vector3 normal = geom.GetSurfaceNormalAtPoint(intPoint);                            
                            foreach (Light lt in scene.lights) {
                                Vector3 lightNormal = lt.location - intPoint;
                                lightNormal.Normalize();
                                double lambert = lightNormal.Dot(normal);
                                if (lambert > 0) {
                                    int r, g, b;
                                    r = b = g = 0;
                                    geom.GetColor(intPoint, ref r, ref g, ref b);
                                    accumR += (int)(lt.color.R * r * lambert)/255;
                                    accumG += (int)(lt.color.G * g * lambert)/255;
                                    accumB += (int)(lt.color.B * b * lambert)/255;
                                }
                            }
                            if (accumR > 255) { accumR = 255; }
                            if (accumG > 255) { accumG = 255; }
                            if (accumB > 255) { accumB = 255; }
                            if (accumR < 15) { accumR = 15; }
                            if (accumG < 15) { accumG = 15; }
                            if (accumB < 15) { accumB = 15; }

                            SolidBrush brush = new SolidBrush(Color.FromArgb(255, accumR, accumG, accumB));                            
                            graphic.FillRectangle(brush, i, j, 1, 1);
                        }
                    }
                    if (!hit) {
                        graphic.FillRectangle(new SolidBrush(BackColor), i, j, 1, 1);
                    }
                }
            }
        }
    }
}
