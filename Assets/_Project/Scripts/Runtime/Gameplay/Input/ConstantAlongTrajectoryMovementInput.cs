using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public class ConstantAlongTrajectoryMovementInput : IMovementInput
    {
        private IEnumerable<Vector3> trajectory;
        private readonly Transform owner;

        private int closestPointIndex = 0;


        public Vector3 Direction => GetTrajectoryDirection();


        public ConstantAlongTrajectoryMovementInput(IEnumerable<Vector3> trajectory, Transform owner)
        {
            this.trajectory = trajectory;
            this.owner = owner;
        }


        private Vector3 GetTrajectoryDirection()
        {
            Vector3? nextPoint = null;
            float closestDistance = float.MaxValue;

            if (trajectory == null)
            {
                return Vector3.zero;
            }

            if (closestPointIndex >= trajectory.Count())
            {
                trajectory = null;

                return Vector3.zero;
            }

            for (int i = closestPointIndex; i < trajectory.Count(); i++)
            {
                Vector3 point = trajectory.ElementAt(i);

                if (point == owner.position || IsPointBehindOwner(i))
                {
                    continue;
                }

                float distance = Vector3.Distance(owner.position, point);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPointIndex = i;
                    nextPoint = point;
                }
                else if (distance > closestDistance)
                {
                    break;
                }
            }

            return nextPoint.HasValue ? (nextPoint.Value - owner.position).normalized : Vector3.zero;
        }

        private bool IsPointBehindOwner(int pointIndex)
        {
            Vector3 point = trajectory.ElementAt(pointIndex);
            Vector3 previousPoint = pointIndex > 0 ? trajectory.ElementAt(pointIndex - 1) : owner.position;
            Vector3 trajectoryForward = (point - previousPoint).normalized;
            Vector3 directionToPoint = (point - owner.position).normalized;

            return Vector3.Dot(trajectoryForward, directionToPoint) <= 0;
        }
    }
}