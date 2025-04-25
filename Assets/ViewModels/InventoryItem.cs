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

    //public static List<InventoryItem> GetInventory()
    //{
    //    return new List<InventoryItem>()
    //    {
    //        new InventoryItem()
    //        {
    //            Index = 0,
    //            IsEquipable = true,
    //            ResourceNameItem = "Axe_01",
    //            EquipmentItemType = InventorySlotType.PrimaryWeapon,
    //            ResourceNamePrefab = "Axe_01"
    //        },
    //        new InventoryItem()
    //        {
    //            Index = 1,
    //            IsEquipable = true,
    //            ResourceNameItem = "Mace_01",
    //            EquipmentItemType = InventorySlotType.PrimaryWeapon,
    //            ResourceNamePrefab = "Mace_01"
    //        },
    //        new InventoryItem()
    //        {
    //            Index = 2,
    //            IsEquipable = true,
    //            ResourceNameItem = "Pike_01",
    //            EquipmentItemType = InventorySlotType.PrimaryWeapon,
    //            ResourceNamePrefab = "Pike_01"
    //        },
    //        new InventoryItem()
    //        {
    //            Index = 3,
    //            IsEquipable = true,
    //            ResourceNameItem = "Shield_01",
    //            EquipmentItemType = InventorySlotType.SecondaryWeapon,
    //            ResourceNamePrefab = "Shield_01"
    //        },
    //        new InventoryItem()
    //        {
    //            Index = 4,
    //            IsEquipable = true,
    //            ResourceNameItem = "Shield_02",
    //            EquipmentItemType = InventorySlotType.SecondaryWeapon,
    //            ResourceNamePrefab = "Shield_02"
    //        },
    //        new InventoryItem()
    //        {
    //            Index = 5,
    //            IsEquipable = true,
    //            ResourceNameItem = "Shield_03",
    //            EquipmentItemType = InventorySlotType.SecondaryWeapon,
    //            ResourceNamePrefab = "Shield_03"
    //        },
    //        new InventoryItem()
    //        {
    //            Index = 6,
    //            IsEquipable = true,
    //            ResourceNameItem = "Staff_01",
    //            EquipmentItemType = InventorySlotType.PrimaryWeapon,
    //            ResourceNamePrefab = "Staff_01"
    //        },



    //        new InventoryItem()
    //        {
    //            Index = 8,
    //            IsEquipable = true,
    //            ResourceNameItem = "Helmet_01",
    //            EquipmentItemType = InventorySlotType.Head,
    //            ResourceNamePrefab = $"Helmet/Helmet_01"
    //        },

    //        new InventoryItem()
    //        {
    //            Index = 9,
    //            IsEquipable = true,
    //            ResourceNameItem = "Helmet_02",
    //            EquipmentItemType = InventorySlotType.Head,
    //            ResourceNamePrefab = "Helmet/Helmet_02"
    //        },
    //        new InventoryItem()
    //        {
    //            Index = 16,
    //            IsEquipable = true,
    //            ResourceNameItem = "Cape_01",
    //            EquipmentItemType = InventorySlotType.Cape,
    //            ResourceNamePrefab = "Cape/Cape_01"
    //        },
    //        new InventoryItem()
    //        {
    //            Index = 17,
    //            IsEquipable = true,
    //            ResourceNameItem = "Cape_02",
    //            EquipmentItemType = InventorySlotType.Cape,
    //            ResourceNamePrefab = "Cape/Cape_02"
    //        },
    //        new InventoryItem()
    //        {
    //            Index = 18,
    //            IsEquipable = true,
    //            ResourceNameItem = "Body_02",
    //            EquipmentItemType = InventorySlotType.Body,
    //            ResourceNamePrefab = "Body/Body_02"
    //        },
    //        new InventoryItem()
    //        {
    //            Index = 19,
    //            IsEquipable = true,
    //            ResourceNameItem = "Body_03",
    //            EquipmentItemType = InventorySlotType.Body,
    //            ResourceNamePrefab = "Body/Body_03"
    //        },
    //        new InventoryItem()
    //        {
    //            Index = 20,
    //            IsEquipable = true,
    //            ResourceNameItem = "Boot_02",
    //            EquipmentItemType = InventorySlotType.Boot,
    //            ResourceNamePrefab = "Boot/Boot_02"
    //        },
    //        new InventoryItem()
    //        {
    //            Index = 21,
    //            IsEquipable = true,
    //            ResourceNameItem = "Boot_03",
    //            EquipmentItemType = InventorySlotType.Boot,
    //            ResourceNamePrefab = "Boot/Boot_03"
    //        }
    //    };
    //}
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