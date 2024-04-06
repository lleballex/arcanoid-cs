using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.Json;
using Game;

namespace Arcanoid
{
    public partial class GameScene : Form, IGame
    {
        public event GameEvent OnStateChange;
        public event GameEvent OnCollide;
        public event GameEvent OnActivePlatformChange;

        public Rectangle rect { get; } = new Rectangle(0, 24, 700, 700);

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

        private int lastLevelIdx = 4;
        private int currentLevelIdx = 0;

        public GameScene()
        {
            InitializeComponent();

            Width = rect.Right + 16;
            Height = rect.Bottom + 39;

            InitToolStrip();

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

        private void InitToolStrip()
        {
            for (int i = 0; i < lastLevelIdx; i++)
            {
                ToolStripMenuItem item = new ToolStripMenuItem((i + 1).ToString());
                item.Click += SelectLevel;
                SelectLevelToolStripMenuItem.DropDown.Items.Add(item);
            }
        }

        private void ChangeState(GameState state_)
        {
            state = state_;

            if (state == GameState.Init)
            {
                health = 5;
                activePlatformIdx = 2;
                LoadLevel(currentLevelIdx);
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
                            CheckWinning();
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

        private void CheckWinning()
        {
            bool hasWon = true;

            foreach (var block in blocks) {
                if (block.isAlive && !block.isSolid)
                {
                    hasWon = false;
                    break;
                }
            }
            
            if (hasWon)
            {
                ChangeState(GameState.Inactive);

                if (currentLevelIdx < lastLevelIdx)
                {
                    currentLevelIdx++;
                    MessageBox.Show("You've won, but you're still a stupid donkey. How about next level?",
                                    "ARCANOID by lleballex",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                } else
                {
                    MessageBox.Show("You've won the whole game. There're no more levels. You're a real donkey",
                                    "ARCANOID by lleballex",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
                ChangeState(GameState.Init);
            }
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
                e.Graphics.DrawImage(healthIconBitmap, new Point(30 * i, rect.Top - 2));
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
                ChangeState(GameState.Inactive);
                MessageBox.Show("You've lost, stupid donkey", "ARCANOID by lleballex", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        
        private void RestartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeState(GameState.Inactive);
            DialogResult res = MessageBox.Show("Are you shure you want to restart? Current progress is not going to be saved",
                                               "ARCANOID by lleballex",
                                               MessageBoxButtons.YesNo,
                                               MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                ChangeState(GameState.Init);
            } else
            {
                ChangeState(GameState.Run);
            }
        }

        private void SelectLevel(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            currentLevelIdx = Int32.Parse(item.Text) - 1;
            ChangeState(GameState.Init);
        }

        ///
        // TODO: improve
        private void LoadLevel(int idx)
        {
            // 0 - nothing
            // 1 - regular
            // 2 - solid
            // 3 - multi
            // ---
            // 0 - white
            // 1 - pink
            // 2 - blue

            string jsonData = File.ReadAllText("./assets/levels.json");
            List<GameLevel> levels = JsonSerializer.Deserialize<List<GameLevel>>(jsonData);

            const int CellsSize = 10;
            int[][] Cells = levels[idx].map;
            Color[] colors = { Color.Pink, Color.Blue };

            int BlocksCount = 0;

            for (int i = 0; i < CellsSize; i++)
            {
                for (int j = 0; j < CellsSize; j++)
                {
                    if (Cells[i][j] > 0)
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
                    Point point = new Point(j * 80 + ((rect.Width - 10 * 80) / 2), i * 50 + ((rect.Height - 10 * 50) / 2));
                    if (Cells[i][j] / 10 == 1)
                    {
                        blocks[++LastBockIdx] = new Block(this, point, colors[Cells[i][j] % 10]);
                    }
                    else if (Cells[i][j] / 10 == 2)
                    {
                        blocks[++LastBockIdx] = new SolidBlock(this, point, colors[Cells[i][j] % 20]);
                    }
                    else if (Cells[i][j] == 3)
                    {
                        blocks[++LastBockIdx] = new MultiBlock(this, point);
                    }
                }
            }
        }
    }
}
