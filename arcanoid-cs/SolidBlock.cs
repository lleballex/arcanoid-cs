using System.Drawing;

namespace arcanoid_cs
{
    internal class SolidBlock : Block
    {
        public override bool allowsWinning { get { return true; } }

        public SolidBlock(Game Game, Point Location, Color color) : base(Game, Location, color)
        {
            changesBallColor = true;
        }

        public override void Show(Graphics graphics)
        {
            if (isAlive)
            {
                Rectangle newRect = Rectangle.Round(rect);
                newRect.Width -= (int)pen.Width;
                newRect.Height -= (int)pen.Width;
                newRect.X += (int)pen.Width / 2;
                newRect.Y += (int)pen.Width / 2;
                graphics.DrawRectangle(pen, newRect);
            }
        }

        protected override void Game_OnCollide(object sender, GameEventArgs e)
        {
        }
    }
}
