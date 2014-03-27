using OpenTK;
using OpenTK.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RallysportGame
{
    public sealed class InputHandler
    {
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
