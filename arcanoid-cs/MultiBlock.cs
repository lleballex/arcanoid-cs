using System.Drawing;

namespace Game
{
    internal class MultiBlock : Block
    {
        public MultiBlock(IGame Game, Point Location) : base(Game, Location, Color.White)
        {
            isMulti = true;
        }
    }
}
