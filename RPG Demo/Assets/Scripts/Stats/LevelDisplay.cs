using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
            BaseStats baseStats;
            Text levelText;
            private void Awake()
            {
                baseStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
                levelText = GetComponent<Text>();
            }

            private void Update()
            {
                levelText.text = String.Format("{0:0}", baseStats.GetLevel());
            }
    }
}
