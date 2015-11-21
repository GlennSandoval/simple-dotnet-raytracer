using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

namespace RayTracer
{

    public partial class Form1 : Form
    {
        private Raytracer rt;
        private Scene sc;
        private DateTime start;

        public Form1()
        {
            InitializeComponent();
            rt = new Raytracer();
            rt.OnProgress += new Raytracer.ProgressHandler(rt_OnProgress);
            numericUpDown1.Value = rt.RayDepth = 3;
            numericUpDown1.ValueChanged += new EventHandler(numericUpDown1_ValueChanged);
        }

        void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            rt.RayDepth = (int)numericUpDown1.Value;
        }

        void rt_OnProgress(double percent)
        {
            Invoke(new Action(() =>
            {
                toolStripProgressBar1.Value = (int)(Math.Ceiling(100 * percent));
                var elapsed = DateTime.Now - start;
                toolStripStatusLabel4.Text = string.Format("{0}m {1}s {2}ms", elapsed.Minutes, elapsed.Seconds, elapsed.Milliseconds);
            }));
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            start = DateTime.Now;
            RayTrace();
        }

        private void RayTrace()
        {
            pnlControls.Enabled = false;

            pictureBox1.Image = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            pictureBox1.Update();

            rt.Size = pictureBox1.Image.Size;

            int centerX = pictureBox1.Width / 2;
            int centerY = pictureBox1.Height / 2;

            rt.Scene = sc;
            rt.BackColor = Color.Black;

            rt.Raytrace(pictureBox1.Image, () =>
            {
                Invoke(new Action(() =>
                {
                    pictureBox1.Refresh();
                }));
            }, () =>
            {
                Invoke(new Action(() =>
                {
                    pnlControls.Enabled = true;
                }));
            });
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JSON Scene|*.json";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    sc = Scene.Load(ofd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error loading scene", MessageBoxButtons.OK);
                    return;
                }

                tsslblScene.Text = Path.GetFileName(ofd.FileName);
                pnlControls.Enabled = true;

            }

        }

    }
}