using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPaths : MonoBehaviour
    {
        const float wayPointRadius = .3f;
        private void OnDrawGizmos()
        {
            for (int i = 0; i < GetTotalWaypoints(); i++)
            {
                int j = GetJ(i);

                Vector3 waypoint = GetWaypoint(i);
                Vector3 nextWaypoint = GetWaypoint(j);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(waypoint, wayPointRadius);
                Gizmos.DrawLine(waypoint, nextWaypoint);
            }
        }

        public int GetTotalWaypoints()
        {
            return transform.childCount;
        }

        private int GetJ(int i)
        {
            if (i + 1 == GetTotalWaypoints()) return 0;
            return i + 1;
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}
