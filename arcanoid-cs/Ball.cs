using System;
using System.Drawing;

namespace Game
{
    public class Ball
    {
        public delegate void BallEvent(object sender, EventArgs e);
        public event BallEvent OnLeave;

        public RectangleF rect = new RectangleF(0, 0, 20, 20);

        private IGame game;

        private SolidBrush brush;
        public Color color
        {
            get { return brush.Color; }
            private set { brush.Color = value; }
        }

        private Speed speed = new Speed(0.3f, 0);
        private bool isMoving = false;

        public Ball(IGame game_)
        {
            game = game_;
            game.OnStateChange += Game_OnStateChange;
            game.OnCollide += Game_OnCollide;
            game.OnActivePlatformChange += Game_OnActivePlatformChange;

            brush = new SolidBrush(Color.White);
        }

        public void Show(Graphics graphics)
        {
            graphics.FillEllipse(brush, rect);
        }

        public void Update(float dt)
        {
            if (!isMoving) return;

            rect.X += speed.x * dt;
            rect.Y += speed.y * dt;

            if (!game.rect.IntersectsWith(Rectangle.Round(rect)))
            {
                if (OnLeave != null)
                {
                    OnLeave(this, new EventArgs());
                }
            }
        }

        public float[] IntersectsWithRect(RectangleF rect_)
        {
            float radius = rect.Width / 2;

            float closestX = Math.Max(Math.Min(rect.X + radius, rect_.Right), rect_.Left);
            float closestY = Math.Max(Math.Min(rect.Y + radius, rect_.Bottom), rect_.Top);

            float DistanceX = rect.X + radius - closestX;
            float DistanceY = rect.Y + radius - closestY;

            if (DistanceX * DistanceX + DistanceY * DistanceY > radius * radius)
            {
                return null;
            }

            return new float[2] { DistanceX, DistanceY };
        }

        private void Game_OnStateChange(object sender, GameEventArgs e)
        {
            switch (e.state)
            {
                case GameState.Init:
                    isMoving = false;
                    rect.Location = new Point(0, 0);
                    brush = new SolidBrush(Color.White);
                    Game_OnActivePlatformChange(this, new GameEventArgs(e.state, e.activePlatformIdx));
                    break;
                case GameState.Setup:
                    isMoving = false;
                    rect.Location = new Point(0, 0);
                    Game_OnActivePlatformChange(this, new GameEventArgs(e.state, e.activePlatformIdx));
                    break;
                case GameState.Run:
                    isMoving = true;
                    break;
            }
        }

        private void Game_OnCollide(object sender, GameEventArgs e)
        {
            if (e.block != null && e.ball == this)
            {
                float[] Collision = IntersectsWithRect(e.block != null ? e.block.rect : e.platform.rect);

                if (Math.Abs(Collision[1]) > Math.Abs(Collision[0]))
                {
                    rect.Y += Collision[1];
                    speed.angle = 360 - speed.angle;
                }
                else                {
                    rect.X += Collision[0];
                    speed.angle = 180 - speed.angle;
                }

                if (e.block != null && e.block.isSolid)
                {
                    color = e.block.color;
                }
            } else if (e.platform != null && e.ball == this)
            {
                float[] Collision = IntersectsWithRect(e.block != null ? e.block.rect : e.platform.rect);

                if (Math.Abs(Collision[1]) > Math.Abs(Collision[0]))
                {
                    rect.Y += Collision[1];
                    speed.angle = 360 - speed.angle;
                    speed.angle += new Random().Next(-45, 45);
                }
                else
                {
                    rect.X += Collision[0];
                    speed.angle = 180 - speed.angle;
                    speed.angle += new Random().Next(-45, 45);
                }
            }
        }

        private void Game_OnActivePlatformChange(object sender, GameEventArgs e)
        {
            speed.angle = e.activePlatformIdx * 90 + 90;
        }
    }
}
