using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics;
using BEPUutilities;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.CollisionRuleManagement;

namespace RallysportGame
{
    static class Camera
    {
        static public Vector3 position { get; set; }

        static public BEPUphysics.Entities.Entity chasedEntity { get; set; }

        static public Vector3 offsetFromEntity {get;set;}

        static public float distanceToTarget { get; set; }

        static public float chaseCameraMargin { get; set; }

        static public void initCamera(BEPUphysics.Entities.Entity entity)
        {
            chasedEntity = entity;

            offsetFromEntity = new Vector3(0, 5, 15);

            distanceToTarget = 10.7f;

            chaseCameraMargin = 1;

            rayCastFilter = RayCastFilter;
        }

        static Func<BroadPhaseEntry, bool> rayCastFilter;
        static bool RayCastFilter(BroadPhaseEntry entry)
        {
            return entry != chasedEntity.CollisionInformation && (entry.CollisionRules.Personal <= CollisionRule.Normal);
        }

        static private Vector3 viewDirection = Vector3.Forward;
        static public Vector3 ViewDirection
        {
            get { return viewDirection; }
            set
            {
                float lengthSquared = value.LengthSquared();
                if (lengthSquared > Toolbox.Epsilon)
                {
                    Vector3 temp = Vector3.Up;
                    Vector3.Divide(ref value, (float)Math.Sqrt(lengthSquared), out value);
                    //Validate the input. A temporary violation of the maximum pitch is permitted as it will be fixed as the user looks around.
                    //However, we cannot allow a view direction parallel to the locked up direction.
                    float dot;
                    Vector3.Dot(ref value, ref temp, out dot);
                    if (Math.Abs(dot) > 1 - Toolbox.BigEpsilon)
                    {
                        //The view direction must not be aligned with the locked up direction.
                        //Silently fail without changing the view direction.
                        return;
                    }
                    viewDirection = value;
                }
            }
        }

        static public void Update()
        {
            bool transformOffset = false;

            Vector3 offset = transformOffset ? Matrix3x3.Transform(offsetFromEntity, chasedEntity.BufferedStates.InterpolatedStates.OrientationMatrix) : offsetFromEntity;//.BufferedStates.InterpolatedStates.OrientationMatrix
            OpenTK.Vector3 temp = offsetFromEntity;
            OpenTK.Vector3 behindcar;
            OpenTK.Quaternion rot2 = chasedEntity.Orientation;
            rot2.Z = 0;
            //rot2.X = rot2.X * 0.5f;
            OpenTK.Vector3.Transform(ref temp, ref rot2, out behindcar);

            OpenTK.Vector3 backRay;
            OpenTK.Vector3 back =Vector3.Forward;
            OpenTK.Quaternion rot3 = chasedEntity.Orientation;
            OpenTK.Vector3.Transform(ref back, ref rot3, out backRay);

            Vector3 downray = Vector3.Down;

            Vector3 lookAt = chasedEntity.BufferedStates.InterpolatedStates.WorldTransform.Translation + Utilities.ConvertToBepu(behindcar);
            Vector3 backwards = -backRay;

            //Find the earliest ray hit that isn't the chase target to position the camera appropriately.
            RayCastResult result;
            float cameraDistance = chasedEntity.Space.RayCast(new Ray(lookAt, backwards), distanceToTarget, rayCastFilter, out result) ? result.HitData.T : distanceToTarget;

            RayCastResult downRes;
            float cameraDownDistance = chasedEntity.Space.RayCast(new Ray(lookAt, downray), distanceToTarget, rayCastFilter, out downRes) ? downRes.HitData.T : distanceToTarget;



            Camera.position = lookAt + (Math.Max(cameraDistance - chaseCameraMargin, 0)) * backwards + (Math.Max(cameraDownDistance - chaseCameraMargin*5, 0)) * -downray; //Put the camera just before any hit spot.


        }
    }
}
