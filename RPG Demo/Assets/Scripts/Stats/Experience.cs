using System;
using UnityEngine;
using RPG.Saving;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = 0;
       // public delegate void ExperienceGainedDelegate();
       // Um delegate que nao possui retorno, ou seja void.
        public event Action onGainExperience;

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            onGainExperience();
        }

        public object GetStates()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }

        public float GetExperiencePoints()
        {
            return experiencePoints;
        }
    }
}
