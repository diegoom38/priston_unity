using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public InventorySlotType inventorySlot;

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

            if (draggableItem.item.EquipmentItemType == inventorySlot || inventorySlot == InventorySlotType.Bag)
                draggableItem.parentAfterDrag = transform;

            EquipItem(draggableItem.item);
        }
    }

    public void EquipItem(InventoryItem item)
    {
        GameObject objectPlayer = PhotonNetwork.LocalPlayer.TagObject as GameObject;

        if (inventorySlot == InventorySlotType.SecondaryWeapon)
        {
            var gameObj = objectPlayer.transform.Find("PT_Hips/PT_Spine/PT_Spine2/PT_Spine3/PT_LeftShoulder/PT_LeftArm/PT_LeftForeArm/PT_LeftHand/PT_Left_Hand_Shield_slot");
            Instantiate(Resources.Load($"ItemsPrefabs/{item.ResourceNamePrefab}"), gameObj.transform);
        }

        if (inventorySlot == InventorySlotType.PrimaryWeapon)
        {
            var gameObj = objectPlayer.transform.Find("PT_Hips/PT_Spine/PT_Spine2/PT_Spine3/PT_RightShoulder/PT_RightArm/PT_RightForeArm/PT_RightHand/PT_Right_Hand_Weapon_slot");
            Instantiate(Resources.Load($"ItemsPrefabs/{item.ResourceNamePrefab}"), gameObj.transform);
        }
    }
}