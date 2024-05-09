using System.Drawing;

namespace arcanoid_cs
{
    internal class MultiBlock : Block
    {
        public MultiBlock(Game Game, Point Location) : base(Game, Location, Color.White)
        {
        }

        protected override void Game_OnCollide(object sender, GameEventArgs e)
        {
            if (e.block == this && e.ball != null)
            {
                health--;
            }
        }
    }
}
