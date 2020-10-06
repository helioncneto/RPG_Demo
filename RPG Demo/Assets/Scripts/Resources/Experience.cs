﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.Resources
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = 0;

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
        }

        public object GetStates()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }
    }
}
