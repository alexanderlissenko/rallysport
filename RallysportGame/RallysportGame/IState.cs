using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RallysportGame
{
    /// <summary>
    /// Interface representing a state. A state describes the logic and graphics of the current state of the application
    /// Examples of things that are states : The menu, the game itself, and so on..
    /// </summary>
    public interface IState
    {
        void Load(GameWindow window);
        void Render(GameWindow window);
        void Update(GameWindow window);
        void HandleKeyDown(object sender, KeyboardKeyEventArgs e);
        void HandleKeyUp(object sender, KeyboardKeyEventArgs e);
        //void prepareSwap(); implement this to fix funky bugs when swapping between menu and game
    }
}
