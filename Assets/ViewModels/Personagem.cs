using Assets.ViewModels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Models
{
    [Serializable]
    public class Personagem
    {
        public string id;
        public string nome;
        public string contaId;
        public string criadoEm;
        public PersonagemConfiguracao configuracao;
    }

    [Serializable]
    public class PersonagemConfiguracao
    {
        public string gender;
        public string age;
        public int level;
        public float percentage;
        public string prefab;
        public PersonagemConfiguracaoCor configuracaoCorCabelo;
        public PersonagemConfiguracaoCor configuracaoCorPele;
        public PersonagemConfiguracaoCor configuracaoCorOlhos;
        public PersonagemConfiguracaoCor configuracaoCorLabios;
        public string hair;
        public string head;
        public PersonagemScale scale;

        [Serializable]
        public class PersonagemScale
        {
            public float x;

            public float y;

            public float z;
        }

        [Serializable]
        public class PersonagemConfiguracaoCor
        {
            public int r;
            public int g;
            public int b;
        }

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    public static class PersonagemUtils
    {
        public static Personagem LoggedChar { get; set; }
        public static Dictionary<int, float> ExpPerLevel()
        {
            var expPerLevel = new Dictionary<int, float>();

            static float CalcExpPerLevel(int nivel)
            {
                // Exemplo de fórmula para calcular XP necessário para cada nível
                // Você pode ajustar essa fórmula conforme necessário
                return (float)(100 * Math.Pow(1.1, nivel - 1)); // Aumenta a dificuldade a cada nível
            }

            for (int i = 1; i <= 60; i++)
            {
                expPerLevel[i] = CalcExpPerLevel(i);
            }

            return expPerLevel;
        }

        public static void IncreaseExp(float increaseAmount)
        {
            if (LoggedChar != null)
            {
                int currentLevel = LoggedChar.configuracao.level;
                float currentExpPercentage = LoggedChar.configuracao.percentage;
                Dictionary<int, float> expTable = ExpPerLevel();
                if (!expTable.ContainsKey(currentLevel)) return;

                float currentExp = expTable[currentLevel] * (currentExpPercentage / 100);
                float newExp = currentExp + increaseAmount;

                while (currentLevel < 60 && newExp >= expTable[currentLevel])
                {
                    newExp -= expTable[currentLevel];
                    currentLevel++;
                }

                float newExpPercentage = (newExp / expTable[currentLevel]) * 100;

                LoggedChar.configuracao.level = currentLevel;
                LoggedChar.configuracao.percentage = newExpPercentage;
            }
        }
    }
}
