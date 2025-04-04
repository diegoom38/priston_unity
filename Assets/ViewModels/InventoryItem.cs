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


    public static List<InventoryItem> GetInventory()
    {
        return new List<InventoryItem>()
        {
            new InventoryItem()
            {
                Index = 0,
                IsEquipable = true,
                ResourceNameItem = "Axe",
                EquipmentItemType = InventorySlotType.PrimaryWeapon,
                ResourceNamePrefab = "Axe_01"
            },
            new InventoryItem()
            {
                Index = 1,
                IsEquipable = true,
                ResourceNameItem = "Sword",
                EquipmentItemType = InventorySlotType.PrimaryWeapon,
                ResourceNamePrefab = "Long_Sword_01"
            },
            new InventoryItem()
            {
                Index = 32,
                IsEquipable = true,
                ResourceNameItem = "Sword",
                EquipmentItemType = InventorySlotType.PrimaryWeapon,
                ResourceNamePrefab = "Long_Sword_02"
            },
            new InventoryItem()
            {
                Index = 38,
                IsEquipable = true,
                ResourceNameItem = "shield",
                EquipmentItemType = InventorySlotType.SecondaryWeapon,
                ResourceNamePrefab = "Shield_02"
            }
        };
    }
}

public enum InventorySlotType
{
    Bag,
    PrimaryWeapon,
    SecondaryWeapon
}