using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Game;

namespace Arcanoid
{
    public partial class GameScene : Form, IGame
    {
        public event GameEvent OnStateChange;
        public event GameEvent OnCollide;
        public event GameEvent OnActivePlatformChange;

        public Rectangle rect { get; } = new Rectangle(0, 0, 700, 700);

        private Stopwatch watch = new Stopwatch();
        private Timer timer;
        private float prevTime;

        private Ball ball;
        private Block[] blocks;
        private Platform[] platforms;

        private int activePlatformIdx;
        private int health;

        private GameState state;

        private Bitmap healthIconBitmap;

        public GameScene()
        {
            InitializeComponent();

            healthIconBitmap = new Bitmap(Image.FromFile("./assets/heart.png"), new Size(25, 25));

            ball = new Ball(this);
            ball.OnLeave += Ball_OnLeave;

            platforms = new Platform[4];
            platforms[0] = new HPlatform(this, Side.Top);
            platforms[1] = new VPlatform(this, Side.Right);
            platforms[2] = new HPlatform(this, Side.Bottom);
            platforms[3] = new VPlatform(this, Side.Left);
        
            MouseClick += GameScene_MouseClick;

            ChangeState(GameState.Init);

            timer = new Timer();
            timer.Interval = (int)(10);
            timer.Tick += Update;
            timer.Start();

            watch.Start();
            prevTime = watch.ElapsedMilliseconds;   
        }

        private void ChangeState(GameState state_)
        {
            state = state_;

            if (state == GameState.Init)
            {
                health = 5;
                activePlatformIdx = 0;
                LoadLevel();
            }

            if (OnStateChange != null)
            {
                OnStateChange(this, new GameEventArgs(state, activePlatformIdx));
            }
        }

        private void ChangeActivePlatformIdx(int idx)
        {
            activePlatformIdx = idx;
            if (OnActivePlatformChange != null)
            {
                OnActivePlatformChange(this, new GameEventArgs(state, activePlatformIdx));
            }
        }

        private void Update(Object sender, EventArgs e)
        {
            float dt = watch.ElapsedMilliseconds - prevTime;

            ball.Update(dt);

            foreach (var platform in platforms)
            {
                platform.Update(dt);
            }

            if (state == GameState.Run)
            {
                foreach (var block in blocks)
                {
                    if (block.isAlive)
                    {
                        if (ball.IntersectsWithRect(block.rect) != null && OnCollide != null)
                        {
                            GameEventArgs collideEvent = new GameEventArgs(state, activePlatformIdx)
                            {
                                ball = ball,
                                block = block,
                            };
                            OnCollide(this, collideEvent);
                            break;
                        }
                    }
                }

                foreach (var platform in platforms)
                {
                    if (ball.IntersectsWithRect(platform.rect) != null)
                    {
                        GameEventArgs collideEvent = new GameEventArgs(state, activePlatformIdx)
                        {
                            ball = ball,
                            platform = platform,
                        };
                        OnCollide(this, collideEvent);
                        break;
                    }
                }
            }
            else if (state == GameState.Setup || state == GameState.Init)
            {
                // TODO: is that good?
                PointF newLocation = platforms[activePlatformIdx].rect.Location;
                if (activePlatformIdx % 2 == 0)
                {
                    newLocation.Y += platforms[activePlatformIdx].rect.Height * (activePlatformIdx >= 2 ? -1 : 1);
                    newLocation.X += platforms[activePlatformIdx].rect.Width / 2 - ball.rect.Width / 2;
                } else
                {
                    newLocation.X += platforms[activePlatformIdx].rect.Width * (activePlatformIdx >= 2 ? 1 : -1);
                    newLocation.Y += platforms[activePlatformIdx].rect.Height / 2 - ball.rect.Height / 2;

                }
                ball.rect.Location = newLocation;
            }

            // TODO: is that good?
            Invalidate();

            prevTime = watch.ElapsedMilliseconds;
        }

        // TODO: is that good to draw on paint event?
        // I do that because of double buffering
        // I can't to enable double buffering with CreateGraphics
        private void Game_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            ball.Show(e.Graphics);

            foreach (var block in blocks)
            {
                block.Show(e.Graphics);
            }

            foreach (var platform in platforms)
            {
                platform.Show(e.Graphics);
            }

            for (int i = 0; i < health; i++)
            {
                e.Graphics.DrawImage(healthIconBitmap, new Point(30 * i, -2));
            }
        }

        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            if ((state == GameState.Setup || state == GameState.Init) && e.KeyCode == Keys.Space)
            {
                ChangeState(GameState.Run);
            }
            else if (state == GameState.Run && e.KeyCode == Keys.Escape)
            {
                ChangeState(GameState.Setup);
            }
        }

        private void Ball_OnLeave(object sender, EventArgs ev)
        {
            health--;

            if (health <= 0)
            {
                ChangeState(GameState.Init);
            } else
            {
                ChangeState(GameState.Setup);
            }
        }

        private void GameScene_MouseClick(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                if (platforms[i].rect.Contains(e.Location))
                {
                    ChangeActivePlatformIdx(i);
                    break;
                }
            }
        }

        ///
        // TODO: improve

        private void LoadLevel()
        {
            const int CellsSize = 10;

            int[,] Cells = {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
		        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
		        { 0, 0, 0, 0, 3, 3, 0, 0, 0, 0},
		        { 0, 0, 0, 2, 1, 1, 2, 0, 0, 0},
		        { 0, 0, 3, 1, 3, 3, 1, 3, 0, 0},
		        { 0, 0, 3, 1, 3, 3, 1, 3, 0, 0},
		        { 0, 0, 0, 2, 1, 1, 2, 0, 0, 0},
		        { 0, 0, 0, 0, 3, 3, 0, 0, 0, 0},
		        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
		        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
	        };

            Color[,] CellColors = {
                { Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White},
		        { Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White},
		        { Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White},
		        { Color.White, Color.White, Color.White, Color.Pink , Color.Pink,  Color.Pink,  Color.Pink , Color.White, Color.White, Color.White},
		        { Color.White, Color.White, Color.White, Color.Pink , Color.White, Color.White, Color.Pink , Color.White, Color.White, Color.White},
		        { Color.White, Color.White, Color.White, Color.Blue , Color.White, Color.White, Color.Blue , Color.White, Color.White, Color.White},
		        { Color.White, Color.White, Color.White, Color.Blue , Color.Blue , Color.Blue , Color.Blue , Color.White, Color.White, Color.White},
		        { Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White},
		        { Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White},
		        { Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White},
	        };


            int BlocksCount = 0;

            for (int i = 0; i < CellsSize; i++)
            {
                for (int j = 0; j < CellsSize; j++)
                {
                    if (Cells[i,j] > 0)
                    {
                        BlocksCount++;
                    }
                }
            }

            blocks = new Block[BlocksCount];
            int LastBockIdx = -1;

            for (int i = 0; i < CellsSize; i++)
            {
                for (int j = 0; j < CellsSize; j++)
                {
                    if (Cells[i,j] == 1)
                    {
                        blocks[++LastBockIdx] = new Block(this, new Point(j * 80 + ((rect.Width - 10 * 80) / 2), i * 50 + ((rect.Height - 10 * 50) / 2)), CellColors[i, j]);
                    }
                    else if (Cells[i, j] == 2)
                    {
                        blocks[++LastBockIdx] = new SolidBlock(this, new Point(j * 80 + ((rect.Width - 10 * 80) / 2), i * 50 + ((rect.Height - 10 * 50) / 2)), CellColors[i, j]);
                    }
                    else if (Cells[i, j] == 3)
                    {
                        blocks[++LastBockIdx] = new MultiBlock(this, new Point(j * 80 + ((rect.Width - 10 * 80) / 2), i * 50 + ((rect.Height - 10 * 50) / 2)));
                    }
                }
            }
        }
    }
}
