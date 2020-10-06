using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] CharacterLevels[] classesAvailble;

        public float GetClassStat(Stat stat, CharacterClass characterClass, int level)
        {
            foreach (CharacterLevels characterLevels in classesAvailble)
            {
                if (characterLevels.characterClass == characterClass)
                {
                    foreach(ProgressionStats progressionStat in characterLevels.stats)
                    {
                        if (level > progressionStat.levels.Length) continue;
                        if(progressionStat.stat == stat)
                        {
                            return progressionStat.levels[level - 1];
                        }
                    }
                    //return characterLevels.health[level - 1]
                }
            }
            return 0;
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


