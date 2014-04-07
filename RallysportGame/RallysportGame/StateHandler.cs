using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK.Input;
using BEPUphysics.Vehicle;
using System.Threading;
using RallysportGame.GUI;





namespace RallysportGame
{

    /// <summary>
    /// Class representing the window containing the menu and the game.
    /// This class is responsible for creating, swapping and exiting these states.
    /// </summary>
    public class StateHandler
    {
        private IState gameState;
        private IState menuState;
        private IState currentState;


        public StateHandler()
        {
            startGame();
        }
        public void startGame()
        {
            gameState = new GameState(this);
            menuState = new MenuState(this);
            this.enterMenu();
            //this.enterGame();

            SettingsParser.Init(@"..\\..\\..\\..\\RallysportGame\\RallysportGame\\ini\\default.ini");

            using (var game = new GameWindow(SettingsParser.GetInt(Settings.WINDOW_WIDTH), SettingsParser.GetInt(Settings.WINDOW_HEIGHT),GraphicsMode.Default, "Speed Junkies"))
            {
                //gameWindow.KeyDown += handleKeyDown;
                //gameWindow.KeyUp += handleKeyUp;
                game.Load += (sender, e) =>
                {
                    menuState.Load(game);

                    //gameState.Load(game);
                };

                game.Resize += (sender, e) =>
                {
                    GL.Viewport(0, 0, game.Width, game.Height);
                };

                game.UpdateFrame += (sender, e) =>
                {
                    currentState.Update(game);
                };

                game.RenderFrame += (sender, e) =>
                {
                    currentState.Render(game);
                };
                game.KeyDown += currentState.HandleKeyDown;
                game.KeyUp += currentState.HandleKeyUp;
                game.Mouse.ButtonDown += currentState.MouseButtonDown;
                game.Mouse.ButtonUp += currentState.MouseButtonUp;
                // Run the game at 60 updates per second
                game.Run(60.0);
            }
        }
        public void enterMenu()
        {
            //TODO
            currentState = menuState;
        }
        public void enterGame()
        {
            //TODO
            currentState = gameState;
        }

        public bool isInMenu()
        {
            return currentState == menuState;
        }
        public static void restartGame() {

        }
    }
}
