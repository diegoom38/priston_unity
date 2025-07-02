using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class InventoryItem
{
    public int Index { get; set; }
    public bool IsEquipable { get; set; }
    public string ResourceNameItem { get; set; }
    public string ResourceNamePrefab { get; set; }
    public InventorySlotType EquipmentItemType { get; set; }
}

public enum InventorySlotType
{
    Bag,
    PrimaryWeapon,
    SecondaryWeapon,
    Head,
    Cape,
    Body,
    Boot
}

public static class InventoryUtils
{
    public static List<InventoryItem> items = new List<InventoryItem>();
}