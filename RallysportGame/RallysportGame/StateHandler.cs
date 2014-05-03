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
    public sealed class StateHandler
    {
        private IState gameState;
        private IState menuState;
        private IState currentState;
        GameWindow game;

        private static readonly StateHandler instance = new StateHandler();

        private StateHandler()
        {
            SettingsParser.Init(@"..\\..\\..\\..\\RallysportGame\\RallysportGame\\ini\\default.ini");
        }

        public static StateHandler Instance
        {
            get
            {
                return instance;
            }
        }

        public void startGame()
        {
            gameState = new GameState();
            menuState = new MenuState();
            this.changeStateToGame();
            //this.changeStateToMenu();
            //this.enterGame();

            using (game = new GameWindow(SettingsParser.GetInt(Settings.WINDOW_WIDTH), SettingsParser.GetInt(Settings.WINDOW_HEIGHT), GraphicsMode.Default, "Speed Junkies"))
            {

                game.Load += (sender, e) =>
                {

                    //menuState.Load(game);

                    gameState.Load(game);
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

        public void restartGame()
        {
            game.Exit();
            startGame();
        }
        public void changeStateToMenu()
        {
            //TODO
            currentState = menuState;
        }
        public void changeStateToGame()
        {
            //TODO
            currentState = gameState;
        }

        public bool isInMenu()
        {
            return currentState == menuState;
        }

    }
}
