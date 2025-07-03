using System;
using System.Collections.Generic;


namespace Assets.ViewModels.Inventory
{
    [Serializable]
    public class Inventario
    {
        public string id;
        public string personagemId;
        public List<CommonItem> itensEquipados;
        public List<InventarioItem> itensInventario;
    }

    [Serializable]
    public class CommonItem
    {
        public int quantidade;
        public int itemId;
    }

    [Serializable]
    public class InventarioItem : CommonItem
    {
        public int indice;
    }
}
