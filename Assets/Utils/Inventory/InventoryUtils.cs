using Assets.ViewModels.Inventory;
using System;

namespace Assets.Utils.Inventory
{
    public static class InventoryUtils
    {
        public static InventarioViewModel Inventario { get; set; }

        public static event Action OnInventoryChanged;

        public static void NotifyInventoryChanged()
        {
            OnInventoryChanged?.Invoke();
        }

        public static void Clear()
        {
            Inventario = null;
            NotifyInventoryChanged();
        }
    }
}