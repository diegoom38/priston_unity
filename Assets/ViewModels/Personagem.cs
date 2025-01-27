using System;

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

        [Serializable]
        public class PersonagemConfiguracaoCor
        {
            public int r;
            public int g;
            public int b;
        }
    }

    public static class PersonagemUtils
    {
        public static Personagem LoggedChar { get; set; }
    }

}
