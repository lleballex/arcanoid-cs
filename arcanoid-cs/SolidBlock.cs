using System.Drawing;

namespace Game
{
    internal class SolidBlock : Block
    {
        public SolidBlock(IGame Game, Point Location, Color color) : base(Game, Location, color)
        {
            isSolid = true;        
        }
    }
}
