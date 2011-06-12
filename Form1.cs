using System;
using System.Drawing;
using System.Windows.Forms;

namespace RayTracer {
    public partial class Form1 : Form {

        Bitmap renderSurface;

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            renderSurface = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);

            pictureBox1.Image = renderSurface;
        }

        private void pictureBox1_Resize(object sender, EventArgs e) {
            if (pictureBox1.Size.Width == 0 || pictureBox1.Size.Height == 0) {
                return;
            }
            Bitmap newRenderSurface = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            using (Graphics g = Graphics.FromImage(newRenderSurface)) {
                g.DrawImage(renderSurface, 0, 0);
            }
            renderSurface.Dispose();
            renderSurface = newRenderSurface;
            pictureBox1.Image = renderSurface;
        }

        bool mouseDown = false;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e) {
            mouseDown = true;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e) {
            mouseDown = false;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e) {
            mouseDown = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e) {
            if (mouseDown) {
                using (Graphics g = Graphics.FromImage(renderSurface)) {
                    Bitmap bm = new Bitmap(1, 1);
                    bm.SetPixel(0, 0, Color.Red);
                    g.DrawImageUnscaled(bm, e.X, e.Y);
                }
            }
            pictureBox1.Invalidate();
        }

        private void btnGo_Click(object sender, EventArgs e) {
            renderSurface = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            pictureBox1.Image = renderSurface;
            RayTrace();
        }



        private void RayTrace() {
            btnGo.Enabled = false;

            int centerX = pictureBox1.Width / 2;
            int centerY = pictureBox1.Height / 2;

            Sphere s = new Sphere(new Vector3(centerX, 150, 70), 25);
            //Vector3 camera = new Vector3(centerX, centerY, -3);
            Vector3 light = new Vector3(centerX, 0, 0);
            using (Graphics g = Graphics.FromImage(renderSurface)) {
                Bitmap bm = new Bitmap(1, 1);
                for (int j = 0; j < renderSurface.Height; j++) {
                    for (int i = 0; i < renderSurface.Width; i++) {
                        Vector3 camera = new Vector3(i, j, -3);
                        Vector3 lookAt = new Vector3(i, j, 5) - camera;
                        lookAt.Normalize();
                        Ray r = new Ray(camera, lookAt);
                        Vector3 intPoint = new Vector3();
                        if (s.Intersects(r, ref intPoint)) {
                            Pen p = new Pen(Color.FromArgb(255, 0, 0, 0));
                            bm.SetPixel(0, 0, Color.Red);
                            g.DrawImageUnscaled(bm, i, j);
                        }
                    }
                }
            }
            pictureBox1.Invalidate();
            btnGo.Enabled = true;
        }

    }


}
