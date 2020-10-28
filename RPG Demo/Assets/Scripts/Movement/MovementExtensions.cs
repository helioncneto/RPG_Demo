using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public static class MovementExtensions
    {
        public static float CalculateDistante(this NavMeshPath path)
        {
            float total = 0f;
            if (path.corners.Length < 2) return total;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return total;
        }
    }
}
