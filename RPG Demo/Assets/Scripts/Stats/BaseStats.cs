﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass _characterClass;
        [SerializeField] Progression progression;
        [SerializeField] GameObject LevelUpEffectPrefab;
        [SerializeField] bool shouldUseModifiers;
        Experience experience;
        int currentLevel = 0;

        public event Action onLevelUp;

        private void Awake()
        {
            experience = GetComponent<Experience>();
        }

        private void Start()
        {
            currentLevel = CalculateLevel();
            if (experience != null)
            {
                experience.onGainExperience += UpdateLevel;
            }
        }

        public int GetLevel()
        {
            if(currentLevel < 1)
            {
                currentLevel = CalculateLevel();
            }
            return currentLevel;
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAditiveModifier(stat)) * (1 + GetPercentageModifiers(stat) / 100);
        }


        private float GetBaseStat(Stat stat)
        {
            return progression.GetClassStat(stat, _characterClass, GetLevel());
        }

        private int CalculateLevel()
        {
            if (experience == null) return startingLevel;

            float currentXP = experience.GetExperiencePoints();
            int lastLevel = progression.GetLastLevel(Stat.ExperienceToLevelUp, _characterClass);
            for (int level = 1; level < lastLevel; level++)
            {
                float xpLevel = progression.GetClassStat(Stat.ExperienceToLevelUp, _characterClass, level);
                if (xpLevel > currentXP)
                {
                    return level;
                }
            }
            return lastLevel;// + 1;
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel)
            {
                currentLevel = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            if (LevelUpEffectPrefab == null) return;
            Instantiate(LevelUpEffectPrefab, transform);
        }

        private float GetAditiveModifier(Stat stat)
        {
            if (shouldUseModifiers)
            {
                float total = 0;
                foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
                {
                    foreach (float modifier in provider.GetAdditiveModifier(stat))
                    {
                        total += modifier;
                    }
                }
                return total;
            }
            else
            {
                return 0;
            }
            
        }

        private float GetPercentageModifiers(Stat stat)
        {
            if (shouldUseModifiers)
            {
                float total = 0;
                foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
                {
                    foreach (float modifier in provider.GetPercentageModifiers(stat))
                    {
                        total += modifier;
                    }
                }
                return total;
            }
            else
            {
                return 0;
            }
            
        }
    }
}
