using System.Drawing;

namespace Game
{
    internal class VPlatform : Platform
    {
        public VPlatform(IGame game, Side side) : base(game)
        {
            isVertical = true;
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
    }
}
