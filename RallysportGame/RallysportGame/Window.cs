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





namespace RallysportGame
{

    /// <summary>
    /// Class representing the window containing the menu and the game.
    /// This class is responsible for creating, swapping and exiting these states.
    /// </summary>
    public class Window
    {
        private IState gameState;
        private IState menuState;
        private IState currentState;


        public Window()
        {
            startGame();
        }
        public void startGame()
        {
            gameState = new GameState(this);
            menuState = new MenuState(this);
            currentState = menuState;


            //using (var game = new GameWindow(SettingsParser.GetInt(Settings.WINDOW_WIDTH), SettingsParser.GetInt(Settings.WINDOW_HEIGHT),GraphicsMode.Default, "Speed Junkies"))
            using (var game = new GameWindow(800, 600, GraphicsMode.Default, "Speed Junkies"))
            {
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
        /// <summary>
        /// Will handle key events so multiple keys can be triggered at once
        /// 
        /// alla loopar kan säkert optimeras och borde kanske ses över detta e mest som ett snabb test 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
    }
}
