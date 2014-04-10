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

        static Environment world;

        static Space space;

        public static void initTriggers(Space sp, Environment wo)
        {
            powerUps = new ArrayList();
            space = sp;
            world = wo;
        }

        public static void addPowerUp(BEPUutilities.Vector3 pos)
        {
            Trigger powerUp = new Trigger("Cube\\3ds-cube", pos, "powerUp", space, world.bepu_mesh);

            powerUps.Add(powerUp);
        }

        public static void addGoal( BEPUutilities.Vector3 pos)
        {
            Trigger goal = new Trigger(pos, "goal", space, world.bepu_mesh);
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
