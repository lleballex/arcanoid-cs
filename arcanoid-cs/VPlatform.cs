using System.Drawing;
using System.Windows.Forms;

namespace arcanoid_cs
{
    internal class VPlatform : Platform
    {
        public VPlatform(Game game, Side side, Form form) : base(game, form)
        {
            rect.Width = breadth;
            rect.Height = length;

            float x;

            if (side == Side.Left)
            {
                x = 0;
            }
            else if (side == Side.Right)
            {
                x = game.rect.Width - rect.Width;
            }
            else
            {
                throw new System.Exception("Wrong platform side");
            }

            defaultLocation = new PointF(x, (game.rect.Height - rect.Height) / 2);
        }

        protected override void CheckOutOfBounds()
        {
            if (rect.Y < game.rect.Top + breadth)
            {
                rect.Y = game.rect.Top + breadth;
            }
            else if (rect.Bottom > game.rect.Bottom - breadth)
            {
                rect.Y = game.rect.Bottom - breadth - length;
            }
        }

        protected override void Game_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    movementDirections.Up = true;
                    break;
                case Keys.S:
                    movementDirections.Down = true;
                    break;
            }
        }

        protected override void Game_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
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
