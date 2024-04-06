using System.Drawing;

namespace Game
{
    internal class HPlatform : Platform
    {
        public HPlatform(IGame game, Side side) : base(game)
        {
            isHorizontal = true;
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
    }
}
