using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

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

        private void btnGo_Click(object sender, EventArgs e) {
            renderSurface = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            pictureBox1.Image = renderSurface;
            RayTrace();
        }
        
        private void RayTrace() {
            btnGo.Enabled = false;
            pictureBox1.Image = null;
            pictureBox1.Update();

            rt.size = renderSurface.Size;

            int centerX = pictureBox1.Width / 2;
            int centerY = pictureBox1.Height / 2;

            Scene sc = new Scene();

            Sphere sp1 = new Sphere(new Vector3(centerX + 300, centerY, 2000), 200);
            Sphere sp2 = new Sphere(new Vector3(centerX - 300, centerY, 2000), 200);
            Sphere sp3 = new Sphere(new Vector3(centerX, centerY, 3500), 500);


            Vector3 plnm = new Vector3(0, 0, -1);
            Vector3 point = new Vector3(0, 0, 4000);
            plnm.Normalize();
            Plane pl = new Plane(plnm, point);

            Camera cam = new Camera();
            cam.Origin = new Vector3(centerX, centerY, -2500);
            sc.camera = cam;

            Light ltRed = new Light();
            ltRed.location = new Vector3(centerX + 300, centerY - 200, 1000);
            ltRed.color = Color.Red;

            Light ltGreen = new Light();
            ltGreen.location = new Vector3(centerX, centerY + 200, 2000);
            ltGreen.color = Color.FromArgb(0, 255, 0);

            Light ltBlue = new Light();
            ltBlue.location = new Vector3(centerX - 300, centerY - 200, 1000);
            ltBlue.color = Color.Blue;            

            sc.geoms.Add(sp1);
            sc.geoms.Add(sp2);
            sc.geoms.Add(sp3);
            sc.geoms.Add(pl);

            sc.lights.Add(ltBlue);
            sc.lights.Add(ltGreen);
            sc.lights.Add(ltRed);

            rt.scene = sc;
            rt.BackColor = Color.Black;

            using (Graphics g = Graphics.FromImage(renderSurface)) {
                rt.RayTrace(g);
            }

            pictureBox1.Image = renderSurface;
            pictureBox1.Update();
            btnGo.Enabled = true;
        }

    }

}
