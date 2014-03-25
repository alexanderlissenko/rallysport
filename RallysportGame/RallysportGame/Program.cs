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


    class Program
    {
        static void Main(string[] args)
        {
            //Thread menuThread = new Thread(new ThreadStart(startMenu));
            //menuThread.Start();
            //startGame();
            new Window();
        }
    }
}
