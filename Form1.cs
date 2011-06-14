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
            rt.size = renderSurface.Size;

            int centerX = pictureBox1.Width / 2;
            int centerY = pictureBox1.Height / 2;

            Scene sc = new Scene();            

            Sphere sp1 = new Sphere(new Vector3(centerX + 300, centerY, 2000), 200);
            Sphere sp2 = new Sphere(new Vector3(centerX - 300, centerY, 2000), 200);
            sc.geoms.Add(sp1);
            sc.geoms.Add(sp2);
            
            Camera cam = new Camera();
            cam.Origin = new Vector3(centerX, centerY, -2500);
            sc.camera = cam;

            Light lt3 = new Light();
            lt3.location = new Vector3(centerX + 300, centerY, 1000);
            lt3.color = Color.Red;
            sc.lights.Add(lt3);

            //Light lt = new Light();
            //lt.location = new Vector3(centerX, centerY, 1000);
            //lt.color = Color.Green;
            //sc.lights.Add(lt);

            Light lt2 = new Light();
            lt2.location = new Vector3(centerX - 300, centerY, 1000);
            lt2.color = Color.Blue;
            sc.lights.Add(lt2);

            rt.scene = sc;
            rt.BackColor = Color.Black;

            using (Graphics g = Graphics.FromImage(renderSurface)) {
                rt.RayTrace(g);
            }

            pictureBox1.Invalidate();
            btnGo.Enabled = true;
        }

    }

}
