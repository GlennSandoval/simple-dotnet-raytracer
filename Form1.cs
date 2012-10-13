using System;
using System.Drawing;
using System.Windows.Forms;

namespace RayTracer {

	public partial class Form1 : Form {
		private Raytracer rt;

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

			rt.Scene = Scene.Load( "Scene.json" );
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