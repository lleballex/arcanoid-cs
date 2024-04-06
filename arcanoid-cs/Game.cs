using System;
using System.Drawing;
using System.Windows.Forms;

namespace Game
{
    public class GameEventArgs : EventArgs
    {
        public Ball ball = null;
        public Block block = null;
        public Platform platform = null;
        public GameState state;
        public int activePlatformIdx;

        public GameEventArgs(GameState state_, int activePlatformIdx_)
        {
            state = state_;
            activePlatformIdx = activePlatformIdx_;
        }
    }

    public enum GameState
    {
        Init, Setup, Run, Inactive
    }

    public delegate void GameEvent(object sender, GameEventArgs e);

    public interface IGame
    {
        event GameEvent OnStateChange;
        event GameEvent OnCollide;
        event GameEvent OnActivePlatformChange;

        event KeyEventHandler KeyDown;
        event KeyEventHandler KeyUp;
        event MouseEventHandler MouseClick;

        Rectangle rect { get; }
    }

    public enum Side
    {
        Left, Right, Top, Bottom,
    }

    public class GameLevel
    {
        public int[][] map { get; set; }
    }
}
