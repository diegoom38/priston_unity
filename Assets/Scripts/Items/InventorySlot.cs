using Assets.Enums;
using Assets.Models;
using Assets.ViewModels.Inventory;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public InventorySlotType inventorySlot;

    private const string LEFT_HAND_SHIELD_PATH = "PT_Hips/PT_Spine/PT_Spine2/PT_Spine3/PT_LeftShoulder/PT_LeftArm/PT_LeftForeArm/PT_LeftHand/PT_Left_Hand_Shield_slot";
    private const string RIGHT_HAND_WEAPON_PATH = "PT_Hips/PT_Spine/PT_Spine2/PT_Spine3/PT_RightShoulder/PT_RightArm/PT_RightForeArm/PT_RightHand/PT_Right_Hand_Weapon_slot";

    // Armazena os objetos desabilitados e seus estados originais
    private Dictionary<string, bool> disabledObjectsState = new();

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

    private bool CanPlaceItem(InventarioItemViewModel item)
    {
        return inventorySlot == InventorySlotType.Bag ||
               item.itemDetalhes.slotTipo == inventorySlot;
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

    public void EquipItem(InventarioItemViewModel item)
    {
        if (item == null) return;

        GameObject playerObject = PhotonNetwork.LocalPlayer?.TagObject as GameObject;
        if (playerObject == null) return;

        Transform targetSlot = null;

        switch (inventorySlot)
        {
            case InventorySlotType.SecondaryWeapon when item.itemDetalhes.slotTipo == InventorySlotType.SecondaryWeapon:
                targetSlot = playerObject.transform.Find(LEFT_HAND_SHIELD_PATH);
                break;

            case InventorySlotType.PrimaryWeapon when item.itemDetalhes.slotTipo == InventorySlotType.PrimaryWeapon:
                targetSlot = playerObject.transform.Find(RIGHT_HAND_WEAPON_PATH);
                break;

            case InventorySlotType.Head when item.itemDetalhes.slotTipo == InventorySlotType.Head:
                // Armazena o estado dos objetos que serão desabilitados (cabelo)
                //StoreDisabledObjectsState(playerObject, "Hair");

                // Desativa os objetos com tag Hair
                ToggleObjectsWithTag(playerObject, "Hair", false);

                // Ativa o capacete
                ToggleEquipmentObject(playerObject, item.itemDetalhes.recursoNomePrefab, "");
                break;

            case InventorySlotType.Cape when item.itemDetalhes.slotTipo == InventorySlotType.Cape:
                //Seta o Mesh do item
                ToggleEquipmentObject(playerObject, item.itemDetalhes.recursoNomePrefab, "Cape/Cape");
                break;

            case InventorySlotType.Body when item.itemDetalhes.slotTipo == InventorySlotType.Body:
                //Seta o Mesh do item
                ToggleEquipmentObject(playerObject, item.itemDetalhes.recursoNomePrefab, "Body/Body");
                break;
            case InventorySlotType.Boot when item.itemDetalhes.slotTipo == InventorySlotType.Boot:
                //Seta o Mesh do item
                ToggleEquipmentObject(playerObject, item.itemDetalhes.recursoNomePrefab, "Boot/Boot");
                break;
        }

        if (targetSlot != null)
        {
            ClearSlotChildren(targetSlot);
            InstantiateEquipment(item, targetSlot);
        }
    }

    public void UnequipItem(InventarioItemViewModel item)
    {
        if (item == null) return;

        GameObject playerObject = PhotonNetwork.LocalPlayer?.TagObject as GameObject;
        if (playerObject == null) return;

        Transform slot = null;

        switch (item.itemDetalhes.slotTipo)
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
                ToggleEquipmentObject(playerObject, item.itemDetalhes.recursoNomePrefab, "");
                break;

            case InventorySlotType.Cape:
                //Seta o Mesh padrão
                ToggleEquipmentObject(playerObject, "Cape/Cape", "Cape/Cape");
                break;
            case InventorySlotType.Body:
                //Seta o Mesh padrão
                ToggleEquipmentObject(playerObject, "Body/Body", "Body/Body");
                break;
            case InventorySlotType.Boot:
                //Seta o Mesh padrão
                ToggleEquipmentObject(playerObject, "Boot/Boot", "Boot/Boot");
                break;
        }

        if (slot != null) ClearSlotChildren(slot);
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
    private void ToggleEquipmentObject(GameObject playerObject, string resourceMesh, string child)
    {
        Mesh mesh = Resources.Load(@$"ItemMeshes/{PersonagemUtils.LoggedChar.configuracao.gender}/{resourceMesh}") as Mesh;

        GameObject equipment = playerObject.transform.Find(child)?.gameObject;

        SkinnedMeshRenderer smr = equipment?.GetComponent<SkinnedMeshRenderer>();
        smr.sharedMesh = mesh;
    }

    private void InstantiateEquipment(InventarioItemViewModel item, Transform parent)
    {
        if (string.IsNullOrEmpty(item.itemDetalhes.recursoNomePrefab)) return;

        var prefabPath = $"ItemsPrefabs/{item.itemDetalhes.recursoNomePrefab}";
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