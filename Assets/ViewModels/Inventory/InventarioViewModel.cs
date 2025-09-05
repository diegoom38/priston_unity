using System;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.ViewModels.Inventory
{
    [Serializable]
    public class InventarioViewModel
    {
        public string id;
        public string personagemId;

        public List<CommonItemViewModel> itensEquipados;
        public List<InventarioItemViewModel> itensInventario;

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public class CommonItemViewModel : CommonItem
    {
        public Item itemDetalhes;

        public InventarioItemViewModel ToBag()
        {
            return new InventarioItemViewModel
            {
                itemDetalhes = itemDetalhes,
                itemId = itemId,
                quantidade = quantidade,
                indice = 0,
                id = id
            };
        }
    }

    [Serializable]
    public class InventarioItemViewModel : CommonItemViewModel
    {
        public int indice;
    }
}
