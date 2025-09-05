using Assets.Enums;
using Assets.Models;
using Assets.Scripts.Core.Services.Inventory;
using Assets.Scripts.Manager;
using Assets.Utils.Inventory;
using Assets.ViewModels.Inventory;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static Assets.Models.PersonagemConfiguracao;

public class CharacterAppearance : MonoBehaviourPun
{
    private const string LEFT_HAND_SHIELD_PATH = "PT_Hips/PT_Spine/PT_Spine2/PT_Spine3/PT_LeftShoulder/PT_LeftArm/PT_LeftForeArm/PT_LeftHand/PT_Left_Hand_Shield_slot";
    private const string RIGHT_HAND_WEAPON_PATH = "PT_Hips/PT_Spine/PT_Spine2/PT_Spine3/PT_RightShoulder/PT_RightArm/PT_RightForeArm/PT_RightHand/PT_Right_Hand_Weapon_slot";

    [PunRPC]
    public void UpdateCharacterAppearance(float skinR, float skinG, float skinB,
                                          float hairR, float hairG, float hairB,
                                          float eyeR, float eyeG, float eyeB,
                                          float lipR, float lipG, float lipB,
                                          string gender, string head, string hair,
                                          float scaleX, float scaleY, float scaleZ)
    {
        Transform characterInstance = transform;

        UpdateScale(scaleX, scaleY, scaleZ, characterInstance);
        ChangeSkinColor(new Color(skinR, skinG, skinB), characterInstance, gender);
        ChangeHairColor(new Color(hairR, hairG, hairB), characterInstance, gender);
        ChangeEyeColor(new Color(eyeR, eyeG, eyeB), characterInstance, gender);
        ChangeLipColor(new Color(lipR, lipG, lipB), characterInstance, gender);
        DropdownValueChangedDefault(head, "Head", Scripts.Manager.SpecsManager.GetHeadOptions(), characterInstance, gender);
        DropdownValueChangedDefault(hair, "Hair", Scripts.Manager.SpecsManager.GetHairOptions(), characterInstance, gender);
        ChangeEquipedItems(characterInstance);
    }

    private async void ChangeEquipedItems(Transform characterInstance)
    {
        List<CommonItemViewModel> itensEquipados = InventoryUtils.Inventario.itensEquipados;
        List<int> itemIds = itensEquipados.Select(item => item.itemId).ToList();
        List<Item> items = await ItemService.GetItems(itemIds);

        foreach (Item itemEquipado in items)
        {
            if (itemEquipado.slotTipo == InventorySlotType.SecondaryWeapon || itemEquipado.slotTipo == InventorySlotType.PrimaryWeapon)
            {
                GameObject equipment = characterInstance.Find(
                    itemEquipado.slotTipo == InventorySlotType.PrimaryWeapon ? RIGHT_HAND_WEAPON_PATH : LEFT_HAND_SHIELD_PATH
                ).gameObject;

                UnityEngine.Object obj = Resources.Load(@$"WeaponsPrefabs/{itemEquipado.recursoNomePrefab}");

                if (obj != null)
                {
                    Instantiate(obj, equipment.transform);
                }
            }
            else
            {
                Mesh mesh = Resources.Load(@$"ItemMeshes/{PersonagemUtils.LoggedChar.configuracao.gender}/{itemEquipado.recursoNomePrefab}") as Mesh;

                GameObject equipment = itemEquipado.slotTipo switch
                {
                    InventorySlotType.PrimaryWeapon => characterInstance.Find("PrimaryWeapon/PrimaryWeapon").gameObject,
                    InventorySlotType.SecondaryWeapon => characterInstance.Find("SecondaryWeapon/SecondaryWeapon").gameObject,
                    InventorySlotType.Head => characterInstance.Find("Head/Head").gameObject,
                    InventorySlotType.Cape => characterInstance.Find("Cape/Cape").gameObject,
                    InventorySlotType.Body => characterInstance.Find("Body/Body").gameObject,
                    InventorySlotType.Boot => characterInstance.Find("Boot/Boot").gameObject,
                    _ => null
                };

                SkinnedMeshRenderer smr = equipment?.GetComponent<SkinnedMeshRenderer>();
                smr.sharedMesh = mesh;
            }
        }
    }

    public static void ChangeSkinColor(Color color, Transform characterMeshes, string gender)
    {
        foreach (KeyValuePair<string, string> mesh in MaterialManager.MeshSkinsList(gender))
            MaterialManager.ChangeMaterialColor(characterMeshes.Find(mesh.Key), color, mesh.Value);
    }

    public static void ChangeHairColor(Color color, Transform characterMeshes, string gender)
    {
        foreach (KeyValuePair<string, string> mesh in MaterialManager.MeshHairList(gender))
            MaterialManager.ChangeMaterialColor(characterMeshes.Find(mesh.Key), color, mesh.Value);
    }

    public static void ChangeEyeColor(Color color, Transform characterMeshes, string gender)
    {
        foreach (KeyValuePair<string, string> mesh in MaterialManager.MeshEyeList(gender))
            MaterialManager.ChangeMaterialColor(characterMeshes.Find(mesh.Key), color, mesh.Value);
    }

    public static void ChangeLipColor(Color color, Transform characterMeshes, string gender)
    {
        foreach (KeyValuePair<string, string> mesh in MaterialManager.MeshLipList(gender))
            MaterialManager.ChangeMaterialColor(characterMeshes.Find(mesh.Key), color, mesh.Value);
    }

    public static void DropdownValueChangedDefault(
        TMP_Dropdown dropdown,
        string folderName,
        Dictionary<string, string> options,
        Transform characterInstance,
        Dictionary<string, Action<string>> actions,
        string gender
    )
    {
        string selectedOption = dropdown.options[dropdown.value].text;

        if (!options.Any(pair => pair.Value == selectedOption || pair.Key == selectedOption)) return;

        string selectedKey = options.FirstOrDefault(pair => pair.Value == selectedOption || pair.Key == selectedOption).Key;

        if (actions.TryGetValue(folderName, out var action))
            action(selectedKey);

        GameObject folderTransform = characterInstance.Find($@"{folderName}/{folderName}").gameObject;

        if (folderTransform == null) return;

        Mesh mesh = Resources.Load(@$"ItemMeshes/{gender}/{folderName}/{selectedKey}") as Mesh;

        SkinnedMeshRenderer smr = folderTransform.GetComponent<SkinnedMeshRenderer>();
        smr.sharedMesh = mesh;
    }

    public static void DropdownValueChangedDefault(
        string specValue,
        string folderName,
        Dictionary<string, string> options,
        Transform characterInstance,
        string gender)
    {
        if (!options.Any(pair => pair.Value == specValue || pair.Key == specValue)) return;

        string selectedKey = options.FirstOrDefault(pair => pair.Value == specValue || pair.Key == specValue).Key;

        GameObject folderTransform = characterInstance.Find($@"{folderName}/{folderName}").gameObject;

        if (folderTransform == null) return;

        Mesh mesh = Resources.Load(@$"ItemMeshes/{gender}/{folderName}/{selectedKey}") as Mesh;

        SkinnedMeshRenderer smr = folderTransform.GetComponent<SkinnedMeshRenderer>();
        smr.sharedMesh = mesh;
    }

    public static Color GetColor(PersonagemConfiguracaoCor configuracao)
    {
        return new Color(
            configuracao.r / 255f,
            configuracao.g / 255f,
            configuracao.b / 255f
        );
    }

    public static void UpdateScale(float scaleX, float scaleY, float scaleZ, Transform characterInstance)
    {
        characterInstance.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
    }
}
