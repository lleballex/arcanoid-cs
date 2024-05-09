using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace arcanoid_cs
{
    public partial class GameForm : Form
    {
        private Game game;

        private Graphics graphics;
        private Stopwatch watch = new Stopwatch();
        private float prevTime;

        public GameForm()
        {
            InitializeComponent();
            InitToolStrip();

            game = new Game(this);

            Width = game.rect.Right + 16;
            Height = game.rect.Bottom + 39;

            graphics = CreateGraphics();

            Timer timer = new Timer();
            timer.Interval = 10;
            timer.Tick += Timer_Tick;
            timer.Start();

            watch.Start();
            prevTime = watch.ElapsedMilliseconds;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            float dt = watch.ElapsedMilliseconds - prevTime;

            game.Update(dt);
            game.Show();

            prevTime = watch.ElapsedMilliseconds;

            graphics.DrawImage(game.bmp, 0, 0);
        }

        private void InitToolStrip()
        {
            for (int i = 0; i < 4; i++)
            {
                ToolStripMenuItem item = new ToolStripMenuItem((i + 1).ToString());
                item.Click += SelectLevel;
                SelectLevelToolStripMenuItem.DropDown.Items.Add(item);
            }
        }

        private void RestartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.Restart();
        }

        private void SelectLevel(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            game.ChangeLevel(Int32.Parse(item.Text) - 1);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog(this);
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpForm helpForm = new HelpForm();
            helpForm.ShowDialog(this);
        }
    }
}
