using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace RayTracer {
    class Raytracer {
        public Scene scene;

        public Size size;

        public void RayTrace(Graphics g) {
            int centerX = size.Width / 2;
            int centerY = size.Height / 2;

            Sphere sp1 = new Sphere(new Vector3(centerX + 200, centerY, 500), 100);
            Sphere sp2 = new Sphere(new Vector3(centerX - 200, centerY, 500), 100);

            Vector3 camera = new Vector3(centerX, centerY, -5000);
            Vector3 light = new Vector3(centerX, centerY / 2, 400);

            Bitmap bm = new Bitmap(1, 1);
            for (int j = 0; j < size.Height; j++) {
                for (int i = 0; i < size.Width; i++) {
                    Vector3 lookAt = new Vector3(i, j, 0) - camera;
                    lookAt.Normalize();
                    Ray r = new Ray(camera, lookAt);
                    Vector3 intPoint = new Vector3();
                    if (sp1.Intersects(r, ref intPoint)) {
                        Vector3 normal = sp1.GetSurfaceNormalAtPoint(intPoint);
                        Vector3 lightNormal = light - intPoint;
                        lightNormal.Normalize();
                        double derp = lightNormal.Dot(normal);
                        if (derp > 0) {
                            int red = (int)(255 * derp);
                            Pen p = new Pen(Color.FromArgb(255, red, 0, 0));
                            bm.SetPixel(0, 0, p.Color);
                            g.DrawImageUnscaled(bm, i, j);
                        } else {
                            Pen p = new Pen(Color.FromArgb(255, 0, 0, 0));
                            bm.SetPixel(0, 0, p.Color);
                            g.DrawImageUnscaled(bm, i, j);
                        }
                    } else if (sp2.Intersects(r, ref intPoint)) {
                        Vector3 normal = sp2.GetSurfaceNormalAtPoint(intPoint);
                        Vector3 lightNormal = light - intPoint;
                        lightNormal.Normalize();
                        double derp = lightNormal.Dot(normal);
                        if (derp > 0) {
                            int red = (int)(255 * derp);
                            Pen p = new Pen(Color.FromArgb(255, red, 0, 0));
                            bm.SetPixel(0, 0, p.Color);
                            g.DrawImageUnscaled(bm, i, j);
                        } else {
                            Pen p = new Pen(Color.FromArgb(255, 0, 0, 0));
                            bm.SetPixel(0, 0, p.Color);
                            g.DrawImageUnscaled(bm, i, j);
                        }
                    }
                }
            }
        }
    }
}
