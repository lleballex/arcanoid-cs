using System.Drawing;
using System.Windows.Forms;

namespace arcanoid_cs
{
    public abstract class Platform
    {
        protected struct Directions
        {
            public bool Up, Right, Down, Left;
        }

        protected Game game;

        public RectangleF rect;

        protected const float length = 160, breadth = 20;
        protected const float speed = 1.1f;

        protected Directions movementDirections = new Directions();

        protected PointF defaultLocation = new PointF(0, 0);

        protected SolidBrush brush = new SolidBrush(Color.White);

        public Platform(Game game_, Form form)
        {
            game = game_;

            game.OnStateChange += Game_OnStateChange;
            form.KeyDown += Game_KeyDown;
            form.KeyUp += Game_KeyUp;
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

            CheckOutOfBounds();
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

        protected abstract void CheckOutOfBounds();
        protected abstract void Game_KeyDown(object sender, KeyEventArgs e);
        protected abstract void Game_KeyUp(object sender, KeyEventArgs e);
    }
}
