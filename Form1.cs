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
            sp1.Material = new SolidColor( 255, 0, 0 ) {
                Phong = .5
            };

            Sphere sp2 = new Sphere( new Vector3( centerX - 300, centerY + 200, 1000 ), 200 );
            sp2.Material = new SolidColor( 0, 0, 255 ) {
                Phong = .5
            };

            Sphere sp3 = new Sphere( new Vector3( centerX, centerY, 3000 ), 500 );
            sp3.Material = new SolidColor( 0, 255, 0 ) {
                Phong = .25
            };

            Sphere sp4 = new Sphere( new Vector3( centerX + 200, centerY, -6000 ), 750 );
            sp4.Material = new SolidColor( 255, 255, 0 ) {
                Phong = .25
            };

            Square sq = new Square( new Vector3( 0, 0, -1 ), new Vector3( centerX - 75, centerY - 200, 1500 ), 100, 100 );
            sq.Material = new SolidColor( 0, 255, 0 ) {
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
            sc.Camera = cam;

            Light ltRed = new Light();
            ltRed.Location = new Vector3( centerX, centerY - 200, 1000 );
            ltRed.Color = Color.Red;

            Light ltGreen = new Light();
            ltGreen.Location = new Vector3( centerX, centerY + 200, 2000 );
            ltGreen.Color = Color.Green;

            Light ltWhite = new Light();
            ltWhite.Location = new Vector3( centerX, centerY - 400, 1000 );
            ltWhite.Color = Color.White;

            Light ltBlue = new Light();
            ltBlue.Location = new Vector3( centerX, centerY - 200, 1000 );
            ltBlue.Color = Color.Blue;

            sc.Geoms.Add( sp1 );
            sc.Geoms.Add( sp2 );
            sc.Geoms.Add( sp3 );
            sc.Geoms.Add( sp4 );
            sc.Geoms.Add( pl );
            sc.Geoms.Add( pl2 );
            sc.Geoms.Add( sq );

            sc.Lights.Add( ltBlue );
            sc.Lights.Add( ltGreen );
            sc.Lights.Add( ltRed );
            sc.Lights.Add( ltWhite );

            sc.Serialize();

            rt.Scene = Scene.Load("Scene.json");
            //rt.Scene = sc;
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