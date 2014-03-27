using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RallysportGame
{
    /// <summary>
    /// Singleton class responsible for gathering keyboard events and keeping an eye on what keys are currently pressed down.
    /// </summary>
    public sealed class InputHandler
    {
        //dictionary containing all keys mapped to a boolean indicating wheter or not is is currently pressed down
        private static Dictionary<Key, bool> dict= new Dictionary<Key, bool>();

        private static readonly InputHandler instance = new InputHandler();

        private InputHandler() {}

        public static void Initialize(GameWindow gameWindow)
        {
            gameWindow.KeyDown += handleKeyDown;
            gameWindow.KeyUp += handleKeyUp;
        }

        public static InputHandler Instance
        {
            get
            {
                return instance;
            }
        }

        public bool isKeyPressed(Key key)
        {
            return dict[key];
        }
        /// <summary>
        /// Returns a dictionary containing all keys mapped to a boolean indicating wheter or not is is currently pressed down
        /// </summary>
        public Dictionary<Key, bool> inputDictionary()
        {
            return dict;  //dangerous method, might have strange effects if called while new things are added? : /
        }

        private static void handleKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            dict[e.Key] = true;
        }

        private static void handleKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            dict[e.Key] = false;
        }
    }
}
