using System;
using System.Drawing;
using System.Windows.Forms;

namespace RayTracer {

    public partial class Form1 : Form {
        Raytracer rt;

        public Form1() {
            InitializeComponent();
            rt = new Raytracer();
        }

        private void btnGo_Click( object sender, EventArgs e ) {
            RayTrace();
        }

        private void RayTrace() {
            btnGo.Enabled = false;

            pictureBox1.Image = new Bitmap( pictureBox1.Size.Width, pictureBox1.Size.Height );
            pictureBox1.Update();

            rt.Size = pictureBox1.Image.Size;

            int centerX = pictureBox1.Width / 2;
            int centerY = pictureBox1.Height / 2;

            Scene sc = new Scene();

            Sphere sp1 = new Sphere( new Vector3( centerX + 300, centerY - 200, 2000 ), 200 );
            sp1.material = new SolidColor() {
                color = Color.Red,
                Phong = .5
            };

            Sphere sp2 = new Sphere( new Vector3( centerX - 300, centerY + 200, 1000 ), 200 );
            sp2.material = new SolidColor() {
                color = Color.Blue,
                Phong = .5
            };

            Sphere sp3 = new Sphere( new Vector3( centerX, centerY, 3000 ), 500 );
            sp3.material = new SolidColor() {
                color = Color.Green,
                Phong = .25
            };

            Sphere sp4 = new Sphere( new Vector3( centerX + 200, centerY, -6000 ), 750 );
            sp4.material = new SolidColor() {
                color = Color.Yellow,
                Phong = .25
            };

            Square sq = new Square( new Vector3( 0, 0, -1 ), new Vector3( centerX - 75, centerY - 200, 1500 ), 100, 100 );
            sq.material = new SolidColor() {
                color = Color.Green,
                Phong = .01
            };

            Vector3 plnm = new Vector3( 0, -1, 0 );
            Vector3 point = new Vector3( 0, centerY + 500, 0 );
            plnm.Normalize();
            Plane pl = new Plane( plnm, point );

            Vector3 plnm2 = new Vector3( 0, 0, -1 );
            Vector3 point2 = new Vector3( 0, 0, 6000 );
            plnm.Normalize();
            Plane pl2 = new Plane( plnm2, point2 );

            Camera cam = new Camera();
            cam.Location = new Vector3( centerX, centerY, -2500 );
            sc.camera = cam;

            Light ltRed = new Light();
            ltRed.location = new Vector3( centerX, centerY - 200, 1000 );
            ltRed.color = Color.Red;

            Light ltGreen = new Light();
            ltGreen.location = new Vector3( centerX, centerY + 200, 2000 );
            ltGreen.color = Color.Green;

            Light ltWhite = new Light();
            ltWhite.location = new Vector3( centerX, centerY - 400, 1000 );
            ltWhite.color = Color.White;

            Light ltBlue = new Light();
            ltBlue.location = new Vector3( centerX, centerY - 200, 1000 );
            ltBlue.color = Color.Blue;

            sc.geoms.Add( sp1 );
            sc.geoms.Add( sp2 );
            sc.geoms.Add( sp3 );
            sc.geoms.Add( sp4 );
            sc.geoms.Add( pl );
            sc.geoms.Add( pl2 );
            sc.geoms.Add( sq );

            sc.lights.Add( ltBlue );
            sc.lights.Add( ltGreen );
            sc.lights.Add( ltRed );
            sc.lights.Add( ltWhite );

            rt.Scene = sc;
            rt.BackColor = Color.Black;

            rt.Raytrace( pictureBox1.Image, () => {
                Invoke( new Action( () => {
                    pictureBox1.Refresh();
                } ) );
            }, () => {
                Invoke( new Action( () => {
                    btnGo.Enabled = true;
                } ) );
            } );
        }
    }
}