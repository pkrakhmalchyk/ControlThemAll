using ControllThemAll.Runtime.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public static class TrajectoryPredictor
    {
        private static LayerMask aimStopLayerMask = LayerMask.GetMask(RuntimeConstants.PhysicLayers.AimStopLayers);
        private static StraightStrategy straightStrategy = new StraightStrategy();
        private static BallisticStrategy ballisticStrategy = new BallisticStrategy();


        public static List<Vector3> PredictStraightTrajectory(Vector3 origin, Vector3 force, float maxRange)
        {
            return PredictTrajectory(straightStrategy, origin, force, maxRange);
        }

        public static List<Vector3> PredictBallisticTrajectory(Vector3 origin, Vector3 force, float maxRange)
        {
            return PredictTrajectory(ballisticStrategy, origin, force, maxRange);
        }

        public static Vector3 PredictBallisticForceToTarget(Vector3 origin, Vector3 target, float angle)
        {
            float gravity = Mathf.Abs(Physics.gravity.y);
            float radianAngle = angle * Mathf.Deg2Rad;
            float horizontalDistance = Vector3.Distance(new Vector3(origin.x, 0, origin.z), new Vector3(target.x, 0, target.z));
            float verticalDifference = origin.y - target.y;

            float squaredAttackForce = (gravity * Mathf.Pow(horizontalDistance, 2))
                / (2 * Mathf.Pow(Mathf.Cos(radianAngle), 2)
                * (verticalDifference + horizontalDistance * Mathf.Tan(radianAngle)));

            if (squaredAttackForce < 0)
            {
                return Vector3.zero;
            }

            float attackForce = Mathf.Sqrt(squaredAttackForce);
            Vector3 direction = (target - origin).normalized;
            Vector3 rotateAround = Vector3.Cross(direction, Vector3.up);
            Vector3 directionWithAngle = Quaternion.AngleAxis(angle, rotateAround) * direction;

            return directionWithAngle * attackForce;
        }


        private static List<Vector3> PredictTrajectory(ITrajectoryPointStrategy strategy, Vector3 startPosition, Vector3 force, float maxRange)
        {
            List<Vector3> trajectoryPoints = new List<Vector3>();

            if (force == Vector3.zero)
            {
                return trajectoryPoints;
            }

            float currentRange = 0f;
            int i = 1;
            trajectoryPoints.Add(startPosition);

            while (currentRange < maxRange)
            {
                Vector3 nextPoint = strategy.GetTrajectoryPoint(startPosition, force, RuntimeConstants.Settings.DistanceBetweenTrajectoryPoints * i);

                if (nextPoint.y < RuntimeConstants.Settings.MinTrajectoryHeight)
                {
                    return trajectoryPoints;
                }

                Vector3 previousPoint = trajectoryPoints[i - 1];

                float distance = (nextPoint - previousPoint).magnitude;
                Vector3 direction = (nextPoint - previousPoint).normalized;

                if (Physics.Raycast(previousPoint, direction, out RaycastHit hitInfo, distance, aimStopLayerMask))
                {
                    trajectoryPoints.Add(hitInfo.point);

                    return trajectoryPoints;
                }

                if (currentRange + distance > maxRange)
                {
                    trajectoryPoints.Add(previousPoint + direction * (maxRange - currentRange));

                    return trajectoryPoints;
                }

                trajectoryPoints.Add(nextPoint);
                currentRange += distance;
                i++;
            }

            return trajectoryPoints;
        }

        private interface ITrajectoryPointStrategy
        {
            public Vector3 GetTrajectoryPoint(Vector3 startPosition, Vector3 force, float distance);
        }

        private class StraightStrategy : ITrajectoryPointStrategy
        {
            public Vector3 GetTrajectoryPoint(Vector3 startPosition, Vector3 force, float distance)
            {
                return startPosition + force * distance;
            }
        }

        private class BallisticStrategy : ITrajectoryPointStrategy
        {
            public Vector3 GetTrajectoryPoint(Vector3 startPosition, Vector3 force, float distance)
            {
                Vector3 point = startPosition + force * distance;
                point.y = startPosition.y + force.y * distance + (Physics.gravity.y * distance * distance / 2f);

                return point;
            }
        }
    }
}