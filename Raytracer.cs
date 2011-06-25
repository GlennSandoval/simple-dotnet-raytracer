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
                    Vector3 intPoint = new Vector3(double.MaxValue, double.MaxValue, double.MaxValue);

                    IGeometry hitObj = null;
                    Vector3 hitPoint = intPoint;
                    double dist = double.MaxValue;
                    foreach (IGeometry geom in scene.geoms) {
                        if (geom.Intersects(ray, ref intPoint)) {
                            double distToObj = ray.E.DistanceTo(intPoint);
                            if (distToObj < dist) {
                                hitPoint = intPoint;
                                dist = distToObj;
                                hitObj = geom;
                            }
                        }
                    }

                    if (hitObj != null) {
                        int accumR = 0;
                        int accumG = 0;
                        int accumB = 0;
                        Vector3 normal = hitObj.GetSurfaceNormalAtPoint(hitPoint);
                        foreach (Light lt in scene.lights) {
                            Vector3 lightNormal = hitPoint - lt.location;
                            lightNormal.Normalize();

                            bool shadow = false;
                            foreach (IGeometry geom in scene.geoms) {
                                if (geom != hitObj) {
                                    Ray shadowRay = new Ray(lt.location, lightNormal);
                                    Vector3 shadint = new Vector3();
                                    if (geom.Intersects(shadowRay, ref shadint)) {
                                        if (lt.location.DistanceTo(hitPoint) > lt.location.DistanceTo(shadint)) {
                                            shadow = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (shadow) {
                                continue;
                            }
                            double lambert = lightNormal.Dot(normal);
                            if (lambert <= 0) {
                                int r, g, b;
                                r = b = g = 0;
                                hitObj.GetColor(hitPoint, ref r, ref g, ref b);
                                accumR += (int)(lt.color.R * r * -lambert) / 255;
                                accumG += (int)(lt.color.G * g * -lambert) / 255;
                                accumB += (int)(lt.color.B * b * -lambert) / 255;
                            } 
                                if (hitObj is Sphere) {
                                    accumR += 15;
                                    accumG += 15;
                                    accumB += 15;
                                }
                            
                        }
                        if (accumR > 255) { accumR = 255; }
                        if (accumG > 255) { accumG = 255; }
                        if (accumB > 255) { accumB = 255; }
                        if (accumR < 0) { accumR = 0; }
                        if (accumG < 0) { accumG = 0; }
                        if (accumB < 0) { accumB = 0; }

                        SolidBrush brush = new SolidBrush(Color.FromArgb(accumR, accumG, accumB));
                        graphic.FillRectangle(brush, i, j, 1, 1);
                    } else {
                        graphic.FillRectangle(new SolidBrush(BackColor), i, j, 1, 1);
                    }
                }
            }
        }
    }
}
