using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RallysportGame
{
    /// <summary>
    /// abstract class representing a state. A state describes the logic and graphics of the current state of the application
    /// Examples of things that are states : The menu, the game itself, and so on..
    /// </summary>
    public abstract class IState
    {
        public abstract void Load(GameWindow window);
        public abstract void Render(GameWindow window);
        public abstract void Update(GameWindow window);
        public virtual void HandleKeyDown(object sender, KeyboardKeyEventArgs e) { }
        public virtual void HandleKeyUp(object sender, KeyboardKeyEventArgs e) { }
        public virtual void MouseButtonDown(object sender, MouseButtonEventArgs e) { }
        public virtual void MouseButtonUp(object sender, MouseButtonEventArgs e) { }

        //void prepareSwap(); implement this to fix funky bugs when swapping between menu and game
    }
}
