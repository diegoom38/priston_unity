
using Assets.ViewModels.Inventory;
using System;

namespace Assets.ViewModels.Personagem
{
    [Serializable]
    public class PersonagemInventarioViewModel:Models.Personagem
    {
        public Inventario inventario;
    }
}
