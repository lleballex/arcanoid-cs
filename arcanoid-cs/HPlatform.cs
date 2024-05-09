using System.Drawing;
using System.Windows.Forms;

namespace arcanoid_cs
{
    internal class HPlatform : Platform
    {
        public HPlatform(Game game, Side side, Form form) : base(game, form)
        {
            rect.Width = length;
            rect.Height = breadth;

            float y;

            if (side == Side.Top)
            {
                y = game.rect.Top;
            }
            else if (side == Side.Bottom)
            {
                y = game.rect.Bottom - rect.Height;
            }
            else
            {
                throw new System.Exception("Wrong platform side");
            }

            defaultLocation = new PointF((game.rect.Width - rect.Width) / 2, y);
        }

        protected override void CheckOutOfBounds()
        {
            if (rect.X < game.rect.Left + breadth)
            {
                rect.X = game.rect.Left + breadth;
            }
            else if (rect.Right > game.rect.Right - breadth)
            {
                rect.X = game.rect.Right - breadth - length;
            }
        }

        protected override void Game_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    movementDirections.Left = true;
                    break;
                case Keys.D:
                    movementDirections.Right = true;
                    break;
            }
        }

        protected override void Game_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    movementDirections.Left = false;
                    break;
                case Keys.D:
                    movementDirections.Right = false;
                    break;
            }
        }
    }
}
