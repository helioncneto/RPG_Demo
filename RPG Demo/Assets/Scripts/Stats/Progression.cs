using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] CharacterLevels[] classesAvailble;
        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable;

        public float GetClassStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookupTable();
            float[] levels = lookupTable[characterClass][stat];
            if(levels.Length < level)
            {
                return 0;
            }
            else
            {
                return levels[level - 1];
            }
        }

        public int GetLastLevel(Stat stat, CharacterClass characterClass)
        {
            BuildLookupTable();
            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;

        }

        private void BuildLookupTable()
        {
            if(lookupTable == null)
            {
                lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();
                foreach (CharacterLevels characterLevels in classesAvailble)
                {
                    Dictionary<Stat, float[]> currentStat = new Dictionary<Stat, float[]>();
                    foreach (ProgressionStats progressionStat in characterLevels.stats)
                    {
                        currentStat[progressionStat.stat] = progressionStat.levels;
                    }
                    lookupTable[characterLevels.characterClass] = currentStat;
                }
            }
        }

        [System.Serializable]
        class CharacterLevels
        {
            public CharacterClass characterClass;
            //public float[] health;
            public ProgressionStats[] stats;
        }

        [System.Serializable]
        class ProgressionStats
        {
            public Stat stat;
            public float[] levels;
        }
    }
}


