using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public InventorySlotType inventorySlot;

    private const string LEFT_HAND_SHIELD_PATH = "PT_Hips/PT_Spine/PT_Spine2/PT_Spine3/PT_LeftShoulder/PT_LeftArm/PT_LeftForeArm/PT_LeftHand/PT_Left_Hand_Shield_slot";
    private const string RIGHT_HAND_WEAPON_PATH = "PT_Hips/PT_Spine/PT_Spine2/PT_Spine3/PT_RightShoulder/PT_RightArm/PT_RightForeArm/PT_RightHand/PT_Right_Hand_Weapon_slot";

    // Armazena os objetos desabilitados e seus estados originais
    private Dictionary<string, bool> disabledObjectsState = new Dictionary<string, bool>();

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        if (transform.childCount > 0) return;

        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        if (draggableItem == null || draggableItem.item == null) return;

        if (!CanPlaceItem(draggableItem.item)) return;

        HandleItemTransfer(draggableItem);
    }

    private bool CanPlaceItem(InventoryItem item)
    {
        return inventorySlot == InventorySlotType.Bag ||
               item.EquipmentItemType == inventorySlot;
    }

    private void HandleItemTransfer(DraggableItem draggableItem)
    {
        InventorySlot previousSlot = draggableItem.parentAfterDrag?.GetComponent<InventorySlot>();
        if (inventorySlot == InventorySlotType.Bag && (previousSlot != null && previousSlot.inventorySlot != InventorySlotType.Bag))
        {
            previousSlot.UnequipItem(draggableItem.item);
        }

        draggableItem.parentAfterDrag = transform;

        if (inventorySlot != InventorySlotType.Bag)
        {
            EquipItem(draggableItem.item);
        }
    }

    public void EquipItem(InventoryItem item)
    {
        if (item == null) return;

        GameObject playerObject = PhotonNetwork.LocalPlayer?.TagObject as GameObject;
        if (playerObject == null) return;

        Transform targetSlot = null;

        switch (inventorySlot)
        {
            case InventorySlotType.SecondaryWeapon when item.EquipmentItemType == InventorySlotType.SecondaryWeapon:
                targetSlot = playerObject.transform.Find(LEFT_HAND_SHIELD_PATH);
                break;

            case InventorySlotType.PrimaryWeapon when item.EquipmentItemType == InventorySlotType.PrimaryWeapon:
                targetSlot = playerObject.transform.Find(RIGHT_HAND_WEAPON_PATH);
                break;

            case InventorySlotType.Head when item.EquipmentItemType == InventorySlotType.Head:
                // Armazena o estado dos objetos que serão desabilitados (cabelo)
                StoreDisabledObjectsState(playerObject, "Hair");

                // Desativa os objetos com tag Hair
                ToggleObjectsWithTag(playerObject, "Hair", false);

                // Ativa o capacete
                ToggleEquipmentObject(playerObject, item.ResourceNamePrefab, true);
                break;

            case InventorySlotType.Cape when item.EquipmentItemType == InventorySlotType.Cape:
                // Armazena o estado dos objetos que serão desabilitados (capa padrão)
                StoreDisabledObjectsState(playerObject, "Cape");

                // Desativa a capa padrão se existir
                ToggleObjectsWithTag(playerObject, "Cape", false);

                // Ativa a nova capa
                ToggleEquipmentObject(playerObject, item.ResourceNamePrefab, true);
                break;
        }

        if (targetSlot != null)
        {
            ClearSlotChildren(targetSlot);
            InstantiateEquipment(item, targetSlot);
        }
    }

    public void UnequipItem(InventoryItem item)
    {
        if (item == null) return;

        GameObject playerObject = PhotonNetwork.LocalPlayer?.TagObject as GameObject;
        if (playerObject == null) return;

        Transform slot = null;

        switch (item.EquipmentItemType)
        {
            case InventorySlotType.SecondaryWeapon:
                slot = playerObject.transform.Find(LEFT_HAND_SHIELD_PATH);
                break;

            case InventorySlotType.PrimaryWeapon:
                slot = playerObject.transform.Find(RIGHT_HAND_WEAPON_PATH);
                break;

            case InventorySlotType.Head:
                // Restaura os objetos desabilitados (cabelo)
                RestoreDisabledObjectsState(playerObject);

                // Desativa o capacete
                ToggleEquipmentObject(playerObject, item.ResourceNamePrefab, false);
                break;

            case InventorySlotType.Cape:
                // Restaura os objetos desabilitados (capa padrão)
                RestoreDisabledObjectsState(playerObject);

                // Desativa a capa equipada
                ToggleEquipmentObject(playerObject, item.ResourceNamePrefab, false);
                break;
        }

        if (slot != null) ClearSlotChildren(slot);
    }

    // Armazena o estado dos objetos com uma tag específica
    private void StoreDisabledObjectsState(GameObject playerObject, string tag)
    {
        foreach (Transform child in playerObject.GetComponentsInChildren<Transform>(false))
        {
            if (child.CompareTag(tag))
            {
                if (!disabledObjectsState.ContainsKey(child.name))
                {
                    disabledObjectsState[child.name] = child.gameObject.activeSelf;
                }
            }
        }
    }

    // Restaura o estado dos objetos que foram desabilitados
    private void RestoreDisabledObjectsState(GameObject playerObject)
    {
        foreach (Transform child in playerObject.GetComponentsInChildren<Transform>(true)) // Include inactive
        {
            if (disabledObjectsState.TryGetValue(child.name, out bool originalState))
            {
                child.gameObject.SetActive(originalState);
            }
        }

        disabledObjectsState.Clear();
    }

    // Ativa/desativa objetos com uma tag específica
    private void ToggleObjectsWithTag(GameObject playerObject, string tag, bool state)
    {
        foreach (Transform child in playerObject.GetComponentsInChildren<Transform>(false))
        {
            if (child.CompareTag(tag))
            {
                child.gameObject.SetActive(state);
            }
        }
    }

    // Ativa/desativa um equipamento pelo nome do prefab
    private void ToggleEquipmentObject(GameObject playerObject, string prefabName, bool state)
    {
        var equipment = playerObject.transform.Find(prefabName)?.gameObject;
        if (equipment != null)
        {
            equipment.SetActive(state);
        }
    }

    private void InstantiateEquipment(InventoryItem item, Transform parent)
    {
        if (string.IsNullOrEmpty(item.ResourceNamePrefab)) return;

        var prefabPath = $"ItemsPrefabs/{item.ResourceNamePrefab}";
        var prefab = Resources.Load(prefabPath);

        if (prefab != null)
        {
            Instantiate(prefab, parent);
        }
    }

    private void ClearSlotChildren(Transform slot)
    {
        if (slot == null) return;

        foreach (Transform child in slot)
        {
            Destroy(child.gameObject);
        }
    }
}