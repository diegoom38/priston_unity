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
                Index = 2,
                IsEquipable = true,
                ResourceNameItem = "Sword",
                EquipmentItemType = InventorySlotType.PrimaryWeapon,
                ResourceNamePrefab = "Long_Sword_02"
            },
            new InventoryItem()
            {
                Index = 3,
                IsEquipable = true,
                ResourceNameItem = "Shield",
                EquipmentItemType = InventorySlotType.SecondaryWeapon,
                ResourceNamePrefab = "Shield_02"
            },
            new InventoryItem()
            {
                Index = 8,
                IsEquipable = true,
                ResourceNameItem = "Helmet",
                EquipmentItemType = InventorySlotType.Head,
                ResourceNamePrefab = "PT_Armor_01_A_helmet"
            },
            
            new InventoryItem()
            {
                Index = 9,
                IsEquipable = true,
                ResourceNameItem = "Helmet",
                EquipmentItemType = InventorySlotType.Head,
                ResourceNamePrefab = "PT_Armor_05_C_helmet"
            },
            new InventoryItem()
            {
                Index = 16,
                IsEquipable = true,
                ResourceNameItem = "Cape",
                EquipmentItemType = InventorySlotType.Cape,
                ResourceNamePrefab = "PT_Armor_01_A_cape"
            },
            new InventoryItem()
            {
                Index = 17,
                IsEquipable = true,
                ResourceNameItem = "Cape",
                EquipmentItemType = InventorySlotType.Cape,
                ResourceNamePrefab = "PT_Armor_05_C_cape"
            }
        };
    }
}

public enum InventorySlotType
{
    Bag,
    PrimaryWeapon,
    SecondaryWeapon,
    Head,
    Cape
}