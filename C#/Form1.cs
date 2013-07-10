using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace DesktopViz
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
            notifyIcon1.ShowBalloonTip(3000);
        }
        int sx, sy;
        bool down = false;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            sx = e.X;
            sy = e.Y;
            down = true;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (down)
            {
                using (var g = CreateGraphics())
                {
                    var w = Math.Abs(e.X - sx);
                    var h = Math.Abs(e.Y - sy);
                    var x = Math.Min(e.X, sx);
                    var y = Math.Min(e.Y, sy);
                    g.DrawRectangle(Pens.Red, getRect(e));
                }
            }
        }
        Rectangle getRect(MouseEventArgs e)
        {
            var w = Math.Abs(e.X - sx);
            var h = Math.Abs(e.Y - sy);
            var x = Math.Min(e.X, sx);
            var y = Math.Min(e.Y, sy);
            return new Rectangle(x, y, w, h);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            var r = getRect(e);
            if (r.Width > 0 && r.Height > 0)
            {
                using (var img = new Bitmap(r.Width, r.Height))
                {
                    using (var g = Graphics.FromImage(img))
                        g.CopyFromScreen(r.X, r.Y, 0, 0, img.Size);
                    int i = 0;
                    while (i < 10000)
                    {
                        var name = System.IO.Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                            string.Format("screenshot{0}.png", i));
                        if (System.IO.File.Exists(name))
                            ++i;
                        else
                        {
                            img.Save(name, System.Drawing.Imaging.ImageFormat.Png);
                            Clipboard.SetImage(img);
                            break;
                        }
                    }
                }
                this.Opacity = 0.0;
            }
            down = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
            Application.Exit();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Opacity = 0.1;
            }
        }
    }
}
