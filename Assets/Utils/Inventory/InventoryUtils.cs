using Assets.ViewModels.Inventory;
using System;
using System.Diagnostics;

namespace Assets.Utils.Inventory
{
    public static class InventoryUtils
    {
        public static InventarioViewModel Inventario { get; set; }

        public static event Action OnInventoryChanged;
        public static event Action<CommonItemViewModel> OnItemUnequipped;

        public static void NotifyItemUnequipped(CommonItemViewModel item)
        {
            OnItemUnequipped?.Invoke(item);
        }

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