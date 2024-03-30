using System.Drawing;

namespace Game
{
    public class Block
    {
        public RectangleF rect = new Rectangle(0, 0, 75, 45);

        public bool isAlive
        {
            get { return health > 0; }
        }
        public bool isMulti { get; protected set; } = false;
        public bool isSolid { get; protected set; } = false;

        protected int health = 1;

        protected SolidBrush brush = new SolidBrush(Color.White);
        protected Pen pen = new Pen(Color.White, 5);
        public Color color
        {
            get { return brush.Color; }
            set { brush.Color = value; pen.Color = value; }
        }

        public Block(IGame game, Point location, Color color_)
        {
            rect.Location = location;
            color = color_;

            game.OnCollide += Game_OnCollide;
        }

        public void Show(Graphics graphics)
        {
            if (isAlive)
            {
                if (isSolid)
                {
                    Rectangle newRect = Rectangle.Round(rect);
                    newRect.Width -= (int)pen.Width;
                    newRect.Height -= (int)pen.Width;
                    newRect.X += (int)pen.Width / 2;
                    newRect.Y += (int)pen.Width / 2;
                    graphics.DrawRectangle(pen, newRect);
                }
                else
                {
                    graphics.FillRectangle(brush, rect);
                }
            }
        }

        private void Game_OnCollide(object sender, GameEventArgs e)
        {
            if (e.block != this || e.ball == null) return;

            if (isMulti || (!isSolid && color == e.ball.color))
            {
                health--;
            }
        }
    }
}
