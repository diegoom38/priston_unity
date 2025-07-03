using System;
using System.Collections.Generic;


namespace Assets.ViewModels.Inventory
{
    [Serializable]
    public class InventarioViewModel
    {
        public string id;
        public string personagemId;

        public List<CommonItemViewModel> itensEquipados;
        public List<InventarioItemViewModel> itensInventario;
    }

    [Serializable]
    public class CommonItemViewModel : CommonItem
    {
        public Item itemDetalhes;
    }

    [Serializable]
    public class InventarioItemViewModel : CommonItemViewModel
    {
        public int indice;
    }
}
