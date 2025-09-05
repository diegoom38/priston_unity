using Assets.Constants;
using Assets.Enums;
using Assets.Models;
using Assets.Scripts.Core.Services.Inventory;
using Assets.Sockets;
using Assets.Utils.Inventory;
using Assets.ViewModels.Inventory;
using Photon.Pun;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public InventorySlotType inventorySlot;

    private const string LEFT_HAND_SHIELD_PATH = "PT_Hips/PT_Spine/PT_Spine2/PT_Spine3/PT_LeftShoulder/PT_LeftArm/PT_LeftForeArm/PT_LeftHand/PT_Left_Hand_Shield_slot";
    private const string RIGHT_HAND_WEAPON_PATH = "PT_Hips/PT_Spine/PT_Spine2/PT_Spine3/PT_RightShoulder/PT_RightArm/PT_RightForeArm/PT_RightHand/PT_Right_Hand_Weapon_slot";

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        if (transform.childCount > 0) return;

        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        if (draggableItem == null || (draggableItem.item == null && draggableItem.itemEquipado == null)) return;

        if (!CanPlaceItem(
            draggableItem?.item?.itemDetalhes?.slotTipo ??
            draggableItem?.itemEquipado?.itemDetalhes?.slotTipo)
        ) return;

        HandleItemTransfer(draggableItem);
    }

    private bool CanPlaceItem(InventorySlotType? type)
    {
        return inventorySlot == InventorySlotType.Bag ||
               type == inventorySlot;
    }

    private void HandleItemTransfer(DraggableItem draggableItem)
    {
        InventorySlot previousSlot = draggableItem.parentAfterDrag?.GetComponent<InventorySlot>();
        if (inventorySlot == InventorySlotType.Bag && (previousSlot != null && previousSlot.inventorySlot != InventorySlotType.Bag))
        {
            previousSlot.UnequipItem(draggableItem);
        }

        draggableItem.parentAfterDrag = transform;

        if (inventorySlot != InventorySlotType.Bag)
        {
            EquipItem(draggableItem);
        }
    }

    public void EquipItem(DraggableItem item)
    {
        GameObject playerObject = PhotonNetwork.LocalPlayer?.TagObject as GameObject;
        if (playerObject == null) return;

        Transform targetSlot = null;

        switch (inventorySlot)
        {
            case InventorySlotType.SecondaryWeapon:
            case InventorySlotType.PrimaryWeapon:
                targetSlot = playerObject.transform.Find(inventorySlot == InventorySlotType.PrimaryWeapon ? RIGHT_HAND_WEAPON_PATH : LEFT_HAND_SHIELD_PATH);
                break;
            case InventorySlotType.Head:
                ToggleHairMesh(playerObject, false);
                ToggleEquipmentObject(playerObject, item.item.itemDetalhes.recursoNomePrefab, "Helmet/Helmet");
                break;
            case InventorySlotType.Cape:
                ToggleEquipmentObject(playerObject, item.item.itemDetalhes.recursoNomePrefab, "Cape/Cape");
                break;
            case InventorySlotType.Body:
                ToggleEquipmentObject(playerObject, item.item.itemDetalhes.recursoNomePrefab, "Body/Body");
                break;
            case InventorySlotType.Boot:
                ToggleEquipmentObject(playerObject, item.item.itemDetalhes.recursoNomePrefab, "Boot/Boot");
                break;
        }

        if (targetSlot != null)
        {
            ClearSlotChildren(targetSlot);
            InstantiateEquipment(item.item, targetSlot);
        }

        InventoryUtils.Inventario.itensInventario = InventoryUtils.Inventario.itensInventario.Where(_item => _item.id != item.item.id).ToList();

        InventoryUtils.Inventario.itensEquipados.Add(new CommonItemViewModel()
        {
            itemDetalhes = item.item.itemDetalhes,
            itemId = item.item.itemId,
            quantidade = item.item.quantidade,
            id = item.item.id
        });


        //InventoryService.EditInventory(InventoryUtils.Inventario)
        Task.Run(() => SharedWebSocketClient.ConnectAndSend(
            InventoryUtils.Inventario.ToJson(),
            VariablesContants.WS_INVENTORY)
        )
        .ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogWarning($"Erro ao equipar item: {task.Exception?.GetBaseException().Message}");
            }
            else
            {
                item.itemEquipado = new CommonItemViewModel()
                {
                    itemId = item.item.itemId,
                    itemDetalhes = item.item.itemDetalhes,
                    quantidade = item.item.quantidade,
                    id = item.item.id
                };

                item.item = null;

                InventoryUtils.NotifyInventoryChanged();
            }
        });
    }

    public void UnequipItem(DraggableItem item)
    {
        if (item.itemEquipado.itemId == 0) return;

        GameObject playerObject = PhotonNetwork.LocalPlayer?.TagObject as GameObject;
        if (playerObject == null) return;

        Transform slot = null;

        switch (item.itemEquipado.itemDetalhes.slotTipo)
        {
            case InventorySlotType.SecondaryWeapon:
                slot = playerObject.transform.Find(LEFT_HAND_SHIELD_PATH);
                break;
            case InventorySlotType.PrimaryWeapon:
                slot = playerObject.transform.Find(RIGHT_HAND_WEAPON_PATH);
                break;
            case InventorySlotType.Head:
                ToggleHairMesh(playerObject, true);
                ToggleEquipmentObject(playerObject, "Helmet/Helmet", "Helmet/Helmet");
                break;
            case InventorySlotType.Cape:
                ToggleEquipmentObject(playerObject, "Cape/Cape", "Cape/Cape");
                break;
            case InventorySlotType.Body:
                ToggleEquipmentObject(playerObject, "Body/Body", "Body/Body");
                break;
            case InventorySlotType.Boot:
                ToggleEquipmentObject(playerObject, "Boot/Boot", "Boot/Boot");
                break;
        }

        if (slot != null)
            ClearSlotChildren(slot);

        InventoryUtils.Inventario.itensEquipados = InventoryUtils.Inventario.itensEquipados.Where(_item => _item.id != item.itemEquipado.id).ToList();

        int nextIndex;

        if (InventoryUtils.Inventario.itensInventario.Count > 0)
        {
            var indices = InventoryUtils.Inventario.itensInventario
                .Select(i => i.indice)
                .OrderBy(i => i)
                .ToList();

            int min = indices.First();
            int max = indices.Last();

            var faltando = Enumerable.Range(min, max - min + 1)
                .Except(indices)
                .FirstOrDefault();

            nextIndex = faltando != 0
                ? faltando
                : max + 1;
        }
        else
        {
            nextIndex = 0;
        }

        InventoryUtils.Inventario.itensInventario.Add(new InventarioItemViewModel()
        {
            indice = nextIndex,
            itemId = item.itemEquipado.itemId,
            itemDetalhes = item.itemEquipado.itemDetalhes,
            quantidade = item.itemEquipado.quantidade,
            id = item.itemEquipado.id
        });

        Task.Run(() => SharedWebSocketClient.ConnectAndSend(
            InventoryUtils.Inventario.ToJson(),
            VariablesContants.WS_INVENTORY)
        )
        .ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogWarning($"Erro ao desequipar item: {task.Exception?.GetBaseException().Message}");
            }
            else
            {
                item.item = new InventarioItemViewModel()
                {
                    itemId = item.itemEquipado.itemId,
                    itemDetalhes = item.itemEquipado.itemDetalhes,
                    quantidade = item.itemEquipado.quantidade,
                    indice = nextIndex,
                    id = item.itemEquipado.id,
                };

                item.itemEquipado = null;

                InventoryUtils.NotifyItemUnequipped(item.itemEquipado);
                InventoryUtils.NotifyInventoryChanged();
            }
        });
    }

    // Ativa/desativa objetos com uma tag específica
    private void ToggleHairMesh(GameObject playerObject, bool state)
    {
        GameObject hairObj = playerObject.transform.Find("Hair/Hair")?.gameObject;
        hairObj?.SetActive(state);
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

        var prefabPath = $"WeaponsPrefabs/{item.itemDetalhes.recursoNomePrefab}";
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