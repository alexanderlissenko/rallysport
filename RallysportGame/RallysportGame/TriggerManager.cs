using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using OpenTK;

using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;

namespace RallysportGame
{
    static class TriggerManager
    {
        static ArrayList powerUps;
        static ArrayList checkpoints;

        static Environment world;

        static Space space;


        public static void initTriggers(Space sp, Environment wo)
        {
            checkpoints = new ArrayList();
            powerUps = new ArrayList();
            space = sp;
            world = wo;
        }

        public static void addPowerUp(BEPUutilities.Vector3 pos)
        {
            Trigger powerUp = new Trigger("Cube\\testCube", pos, "powerUp", space, world.bepu_mesh);

            powerUps.Add(powerUp);
        }

        public static void addGoal(BEPUutilities.Vector3[] pos)
        {
            for (int i = 0; i < pos.Length; i++)
            {
                if (i == pos.Length - 1)
                {
                    Trigger goal = new Trigger(pos[i], "goal", space, world.bepu_mesh);
                }
                else
                {
                    Trigger goal = new Trigger(pos[i], "checkpoint " + (pos.Length-1), space, world.bepu_mesh);
                }
            }
        }

        public static void renderPowerUps(int program, Matrix4 projectionMatrix,Matrix4 viewMatrix)
        {
            foreach(Trigger t in powerUps)
                t.firstPass(program, projectionMatrix, viewMatrix);
        }
        public static void updatePowerUps()
        {
            foreach (Trigger t in powerUps)
                t.update();
        }

    }
}
