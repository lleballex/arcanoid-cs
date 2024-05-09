using System.Drawing;

namespace arcanoid_cs
{
    public class Block
    {
        public RectangleF rect = new Rectangle(0, 0, 75, 45);

        public bool isAlive
        {
            get { return health > 0; }
        }
        public bool changesBallColor { get; protected set; } = false;
        public virtual bool allowsWinning { get { return !isAlive; } }

        protected int health = 1;

        protected SolidBrush brush = new SolidBrush(Color.White);
        protected Pen pen = new Pen(Color.White, 5);
        public Color color
        {
            get { return brush.Color; }
            set { brush.Color = value; pen.Color = value; }
        }

        public Block(Game game, Point location, Color color_)
        {
            rect.Location = location;
            color = color_;

            game.OnCollide += Game_OnCollide;
        }

        public virtual void Show(Graphics graphics)
        {
            if (isAlive)
            {
                graphics.FillRectangle(brush, rect);
            }
        }

        protected virtual void Game_OnCollide(object sender, GameEventArgs e)
        {
            if (e.block == this && e.ball != null && color == e.ball.color)
            {
                health--;
            }
        }
    }
}
