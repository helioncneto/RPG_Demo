using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("Inventory/Drops/New Drop Library"))]
    class DropLibrary : ScriptableObject
    {
        [SerializeField]
        DropConfig[] potentialDrops;
        [SerializeField] float[] dropChancePercentage;
        [SerializeField] int[] minDrops;
        [SerializeField] int[] maxDrops;

        [System.Serializable]
        class DropConfig
        {
            public InventoryItem item;
            public float[] relativeChance;
            public int[] minNumber;
            public int[] maxNumber;
            public int GetRandomNumber(int level)
            {
                if (!item.IsStackable())
                {
                    return 1;
                }
                int min = GetByLevel(minNumber, level);
                int max = GetByLevel(maxNumber, level);
                return UnityEngine.Random.Range(min, max + 1);
            }
        }

        public struct Dropped
        {
            public InventoryItem item;
            public int number;
        }

        public IEnumerable<Dropped> GetRandomDrops(int level)
        {
            if (!ShouldRandomDrop(level))
            {
                yield break;
            }
            for (int i = 0; i < GetRandomNumberOfDrops(level); i++)
            {
                yield return GetRandomDrop(level);
            }
        }

        bool ShouldRandomDrop(int level)
        {
            // Se eu escolher um numero de 0 a 100 ele vai ser menor que a porcentagem de drop
            return Random.Range(0, 100) < GetByLevel(dropChancePercentage, level);
        }

        int GetRandomNumberOfDrops(int level)
        {
            int min = GetByLevel(minDrops, level);
            int max = GetByLevel(maxDrops, level);
            return Random.Range(min, max);
        }

        Dropped GetRandomDrop(int level)
        {
            // Seleciona um item aleatório
            var drop = SelectRandomItem(level);
            // Cria a estrutura
            var result = new Dropped();
            // Adiciona o item na estrutura e retorna
            result.item = drop.item;
            result.number = drop.GetRandomNumber(level);
            return result;
        }

        DropConfig SelectRandomItem(int level)
        {
            // Verifica o numero total de chances que é o somatorio de todas as chances
            float totalChance = GetTotalChance(level);
            // Seleciona um num aleatório
            float randomRoll = Random.Range(0, totalChance);
            // inicia a variavel de chances
            float chanceTotal = 0;
            foreach (var drop in potentialDrops)
            {
                // Acrescenta a chance desse item
                chanceTotal += GetByLevel(drop.relativeChance, level);
                // Se a chance for maior que o valor aleatorio, selecione
                if (chanceTotal > randomRoll)
                {
                    return drop;
                }
            }
            // Caso nenhum item seja selecionado
            return null;
        }

        float GetTotalChance(int level)
        {
            float total = 0;
            foreach (var drop in potentialDrops)
            {
                total += GetByLevel(drop.relativeChance, level);
            }
            return total;
        }

        static T GetByLevel<T>(T[] values, int level)
        {
            // Se não houve valores na lista
            if (values.Length == 0)
            {
                return default;
            }
            // Se o level solicitado for maior que os valores da lista, selecione o ultimo level
            if (level > values.Length)
            {
                return values[values.Length - 1];
            }
            // Caso o level seja menor que zero
            if (level <= 0)
            {
                Debug.LogError("O Level selecionado é menor que zero");
                return default;
            }
            // Se tudo estiver certo, retorne o valor correspondente ao level
            return values[level - 1];
        }
    }
}
