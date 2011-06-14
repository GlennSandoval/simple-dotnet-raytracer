using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace RayTracer {
    public partial class Form1 : Form {

        Raytracer rt;

        Bitmap renderSurface;

        public Form1() {
            InitializeComponent();
            rt = new Raytracer();
        }

        private void Form1_Load(object sender, EventArgs e) {
            renderSurface = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            pictureBox1.Image = renderSurface;
        }

        private void pictureBox1_Resize(object sender, EventArgs e) {
            ResetPicture();
        }

        private void ResetPicture() {
            if (pictureBox1.Size.Width == 0 || pictureBox1.Size.Height == 0) {
                return;
            }
            return;
            //Bitmap newRenderSurface = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            //using (Graphics g = Graphics.FromImage(newRenderSurface)) {
            //    g.DrawImage(renderSurface, 0, 0, pictureBox1.Size.Width, pictureBox1.Size.Height);
            //}
            //renderSurface.Dispose();
            //renderSurface = newRenderSurface;
            //pictureBox1.Image = renderSurface;
            //pictureBox1.Invalidate();
        }

        private void btnGo_Click(object sender, EventArgs e) {
            renderSurface = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            pictureBox1.Image = renderSurface;
            RayTrace();
        }

        private void RayTrace() {
            ResetPicture();
            btnGo.Enabled = false;
            rt.size = renderSurface.Size;

            int centerX = pictureBox1.Width / 2;
            int centerY = pictureBox1.Height / 2;

            Sphere sp1 = new Sphere(new Vector3(centerX + 200, centerY, 500), 100);
            Sphere sp2 = new Sphere(new Vector3(centerX - 200, centerY, 500), 100);

            Vector3 camera = new Vector3(centerX, centerY, -5000);
            Vector3 light = new Vector3(centerX, centerY / 2, 400);

            using (Graphics g = Graphics.FromImage(renderSurface)) {
                rt.RayTrace(g);
            }
            pictureBox1.Invalidate();
            btnGo.Enabled = true;
        }

    }

}
