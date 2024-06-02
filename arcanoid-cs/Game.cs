using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.Json;

namespace arcanoid_cs
{
    public class GameEventArgs : EventArgs
    {
        public Ball ball = null;
        public Block block = null;
        public Platform platform = null;
        public GameState state;
        public int activePlatformIdx;

        public GameEventArgs(GameState state_, int activePlatformIdx_)
        {
            state = state_;
            activePlatformIdx = activePlatformIdx_;
        }
    }

    public enum GameState
    {
        Init, Setup, Run, Inactive
    }

    public enum Side
    {
        Left, Right, Top, Bottom,
    }

    public class GameLevel
    {
        public int[][] map { get; set; }
    }

    public partial class Game
    {
        public delegate void GameEvent(object sender, GameEventArgs e);

        public event GameEvent OnStateChange;
        public event GameEvent OnCollide;
        public event GameEvent OnActivePlatformChange;

        //public Rectangle rect { get; private set; } = new Rectangle();
        public Rectangle rect;

        public Bitmap bmp;

        public GameState state { private set; get; } = GameState.Init;

        private Ball ball;
        private Block[] blocks;
        private Platform[] platforms;

        private int activePlatformIdx;
        private int health;

        private Bitmap healthIconBitmap;
        //private Bitmap bgBitmap;

        private const int lastLevelIdx = 4;
        private int currentLevelIdx = 0;

        private Graphics graphics;

        public Game(Form form)
        {
            rect = new Rectangle(0, 30, 700, 700);

            bmp = new Bitmap(rect.Right, rect.Bottom);
            graphics = Graphics.FromImage(bmp);

            healthIconBitmap = new Bitmap(Image.FromFile("./assets/heart.png"), new Size(25, 25));
            //bgBitmap = new Bitmap(Image.FromFile("./assets/background.png"));

            ball = new Ball(this);
            ball.OnLeave += Ball_OnLeave;

            platforms = new Platform[4];
            platforms[0] = new HPlatform(this, Side.Top, form);
            platforms[1] = new VPlatform(this, Side.Right, form);
            platforms[2] = new HPlatform(this, Side.Bottom, form);
            platforms[3] = new VPlatform(this, Side.Left, form);

            form.MouseClick += Form_MouseClick;
            form.KeyDown += Form_KeyDown;
            form.Resize += Form_Resize;

            ChangeState(GameState.Init);
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            Form form = sender as Form;
            rect.Location = new Point(0, (int)Math.Ceiling(rect.Height / (float)form.Height * 30));
        }

        public void Show()
        {
            //graphics.DrawImage(bgBitmap, 0, 0, rect.Right, rect.Bottom);
            graphics.Clear(Color.Black);

            ball.Show(graphics);

            foreach (var block in blocks)
            {
                block.Show(graphics);
            }

            foreach (var platform in platforms)
            {
                platform.Show(graphics);
            }

            for (int i = 0; i < health; i++)
            {
                graphics.DrawImage(healthIconBitmap, new Point(30 * i, rect.Top - 2));
            }
        }

        public void Update(float dt)
        {
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
                PointF newLocation = platforms[activePlatformIdx].rect.Location;
                if (activePlatformIdx % 2 == 0)
                {
                    newLocation.Y += platforms[activePlatformIdx].rect.Height * (activePlatformIdx >= 2 ? -1 : 1);
                    newLocation.X += platforms[activePlatformIdx].rect.Width / 2 - ball.rect.Width / 2;
                }
                else
                {
                    newLocation.X += platforms[activePlatformIdx].rect.Width * (activePlatformIdx >= 2 ? 1 : -1);
                    newLocation.Y += platforms[activePlatformIdx].rect.Height / 2 - ball.rect.Height / 2;

                }
                ball.rect.Location = newLocation;
            }
        }

        public void ChangeLevel(int idx)
        {
            currentLevelIdx = idx;
            ChangeState(GameState.Init);
        }

        public void Restart()
        {
            ChangeState(GameState.Inactive);
            DialogResult res = MessageBox.Show("Are you shure you want to restart? Current progress is not going to be saved",
                                               "ARCANOID by lleballex",
                                               MessageBoxButtons.YesNo,
                                               MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                ChangeState(GameState.Init);
            }
            else
            {
                ChangeState(GameState.Run);
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

        private void CheckWinning()
        {
            bool hasWon = true;

            foreach (var block in blocks)
            {
                if (!block.allowsWinning)
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
                }
                else
                {
                    MessageBox.Show("You've won the whole game. There're no more levels. You're a real donkey",
                                    "ARCANOID by lleballex",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                ChangeState(GameState.Init);
            }
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if ((state == GameState.Setup || state == GameState.Init) && e.KeyCode == Keys.Space)
            {
                ChangeState(GameState.Run);
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
            }
            else
            {
                ChangeState(GameState.Setup);
            }
        }

        private void Form_MouseClick(object sender, MouseEventArgs e)
        {
            if (state == GameState.Init || state == GameState.Setup)
            {
                Form form = sender as Form;
                float ratioX = (float)rect.Width / form.Width * 1.03f;
                float ratioY = (float)rect.Height / form.Height * 1.11f;
                Point eLocation = new Point((int)(e.Location.X * ratioX), (int)(e.Location.Y * ratioY));

                for (int i = 0; i < 4; i++)
                {
                    if (platforms[i].rect.Contains(eLocation))
                    {
                        ChangeActivePlatformIdx(i);
                        break;
                    }
                }
            }
        }

        private void LoadLevel(int idx) 
        {
            // ------------|
            // 0 - nothing |
            // 1 - regular |
            // 2 - solid   |
            // 3 - multi   |
            // ------------|
            // 0 - white   |
            // 1 - pink    |
            // 2 - blue    |
            // ------------|

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
