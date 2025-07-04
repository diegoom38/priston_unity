using Assets.Models;
using Assets.Scripts.Manager;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static Assets.Models.PersonagemConfiguracao;

public class CharacterAppearance : MonoBehaviourPun
{
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
        DropdownValueChangedDefault(head, "Head", Scripts.Manager.SpecsManager.GetHeadOptions(), characterInstance);
        DropdownValueChangedDefault(hair, "Hair", Scripts.Manager.SpecsManager.GetHairOptions(), characterInstance);
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
        Dictionary<string, Action<string>> actions // Note o Action<string>
    )
    {
        string selectedOption = dropdown.options[dropdown.value].text;

        if (!options.Any(pair => pair.Value == selectedOption || pair.Key == selectedOption)) return;

        string selectedKey = options.FirstOrDefault(pair => pair.Value == selectedOption || pair.Key == selectedOption).Key;

        if (actions.TryGetValue(folderName, out var action))
            action(selectedKey);

        Transform folderTransform = characterInstance.Find(folderName);

        if (folderTransform == null) return;

        foreach (var key in options.Keys)
        {
            Transform part = folderTransform.Find(key);
            if (part != null)
                part.gameObject.SetActive(key == selectedKey);
        }
    }

    public static void DropdownValueChangedDefault(
        string specValue,
        string folderName, 
        Dictionary<string, string> options, 
        Transform characterInstance)
    {
        if (!options.Any(pair => pair.Value == specValue || pair.Key == specValue)) return;

        string selectedKey = options.FirstOrDefault(pair => pair.Value == specValue || pair.Key == specValue).Key;

        Transform folderTransform = characterInstance.Find(folderName);

        if (folderTransform == null) return;

        foreach (var key in options.Keys)
        {
            Transform head = folderTransform.Find(key);
            if (head != null)
                head.gameObject.SetActive(key == selectedKey);
        }
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
