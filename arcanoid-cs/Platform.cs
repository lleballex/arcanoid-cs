using System;
using System.Drawing;
using System.Windows.Forms;

namespace Game
{
    public abstract class Platform
    {
        protected struct Directions
        {
            public bool Up, Right, Down, Left;
        }

        protected IGame game;

        public RectangleF rect;

        protected float length = 160, breadth = 20;
        protected float speed = 1.1f;
        protected bool isVertical = false, isHorizontal = false;

        protected Directions movementDirections = new Directions();

        protected PointF defaultLocation = new PointF(0, 0);

        protected SolidBrush brush = new SolidBrush(Color.White);

        public Platform(IGame game_)
        {
            game = game_;

            game.OnStateChange += Game_OnStateChange;
            game.KeyDown += Game_KeyDown;
            game.KeyUp += Game_KeyUp;
        }

        public void Update(float dt)
        {
            if (movementDirections.Up)
            {
                rect.Y -= speed * dt;
            }
            if (movementDirections.Right)
            {
                rect.X += speed * dt;
            }
            if (movementDirections.Down)
            {
                rect.Y += speed * dt;
            }
            if (movementDirections.Left)
            {
                rect.X -= speed * dt;
            }

            if (isHorizontal)
            {
                if (rect.X < breadth)
                {
                    rect.X = breadth;
                } else if (rect.Right > game.rect.Width - breadth)
                {
                    rect.X = game.rect.Width - breadth - length;
                }
            }
            if (isVertical)
            {
                if (rect.Y < breadth)
                {
                    rect.Y = breadth;
                }
                else if (rect.Bottom > game.rect.Height - breadth)
                {
                    rect.Y = game.rect.Height - breadth - length;
                }
            }
        }

        public void Show(Graphics Graphics)
        {
            Graphics.FillRectangle(brush, rect);
        }

        private void Game_OnStateChange(object sender, GameEventArgs e)
        {
            if (e.state == GameState.Init)
            {
                rect.Location = defaultLocation;
                movementDirections.Up = false;
                movementDirections.Right = false;
                movementDirections.Down = false;
                movementDirections.Left = false;
            }
            else if (e.state == GameState.Setup)
            {
                rect.Location = defaultLocation;
                movementDirections.Up = false;
                movementDirections.Right = false;
                movementDirections.Down = false;
                movementDirections.Left = false;
            }
        }

        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A when isHorizontal:
                    movementDirections.Left = true;
                    break;
                case Keys.D when isHorizontal:
                    movementDirections.Right = true;
                    break;
                case Keys.W when isVertical:
                    movementDirections.Up = true;
                    break;
                case Keys.S when isVertical:
                    movementDirections.Down = true;
                    break;
            }
        }

        private void Game_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    movementDirections.Left = false;
                    break;
                case Keys.D:
                    movementDirections.Right = false;
                    break;
                case Keys.W:
                    movementDirections.Up = false;
                    break;
                case Keys.S:
                    movementDirections.Down = false;
                    break;
            }
        }
    }
}
