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

            Sphere sp1 = new Sphere( new Vector3( centerX + 330, centerY - 230, 2000 ), 200 );
            sp1.Material = new SolidColor( 255, 0, 0 ) {
                Phong = .5
            };

            Sphere sp2 = new Sphere( new Vector3( centerX - 330, centerY + 230, 2000 ), 200 );
            sp2.Material = new SolidColor( 0, 0, 255 ) {
                Phong = .5
            };

            Sphere sp3 = new Sphere( new Vector3( centerX, centerY, 2000 ), 200 );
            sp3.Material = new SolidColor( 255, 255, 255 ) {
                Phong = .25
            };

            Vector3 plnm = new Vector3( 0, 0, -1 );
            Vector3 point = new Vector3( 0, -500, 6000 );
            plnm.Normalize();
            Plane pl = new Plane( plnm, point );

            Camera cam = new Camera();
            cam.Location = new Vector3( centerX, centerY, -2500 );
            sc.Camera = cam;

            Light ltRed = new Light();
            ltRed.Location = new Vector3( centerX, centerY + 300, -500 );
            ltRed.Color = Color.Red;

            Light ltGreen = new Light();
            ltGreen.Location = new Vector3( centerX - 300, centerY - 300, -500 );
            ltGreen.Color = Color.Green;

            Light ltBlue = new Light();
            ltBlue.Location = new Vector3( centerX + 300, centerY - 300, -500 );
            ltBlue.Color = Color.Blue;

            sc.Geoms.Add( sp1 );
            sc.Geoms.Add( sp2 );
            sc.Geoms.Add( sp3 );            
            sc.Geoms.Add( pl );

            sc.Lights.Add( ltBlue );
            sc.Lights.Add( ltGreen );
            sc.Lights.Add( ltRed );

            sc.Serialize();

            rt.Scene = Scene.Load("Scene.json");
            rt.BackColor = Color.Black;
            rt.RayDepth = 1;

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