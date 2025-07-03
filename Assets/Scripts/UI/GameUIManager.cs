using Assets.Models;
using Assets.Scripts.Manager;
using Assets.Utils.Inventory;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviourPunCallbacks
{
    private Dictionary<int, float> ExpPerLevel = PersonagemUtils.ExpPerLevel();

    private GameObject panelItems;
    private GameObject panelMission;
    private GameObject panelBag;
    private GameObject panelDeath;
    private GameObject panelSettings;

    // Start is called before the first frame update
    public void Start()
    {
        SetButtons();
        SetSkills();
        SetPanels();
        SetInventoryItems();
    }

    private void Update()
    {
        HandleInputPanels();
        SetExpSlider();
        SetCharacterInfo();
    }

    private void SetCharacterInfo()
    {
        if (PersonagemUtils.LoggedChar is not null && PhotonNetwork.IsConnectedAndReady)
        {
            if (transform.Find("Canvas/panel_character/panel_image_char/panel_level/level").TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI levelText))
            {
                levelText.text = PersonagemUtils.LoggedChar.configuracao.level.ToString();
            }

            var sliderStatus = transform.Find($"Canvas/panel_character/SlidersStatus_{PhotonNetwork.LocalPlayer.ActorNumber}/name");

            if (sliderStatus != null)
                if (sliderStatus.TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI nameText))
                {
                    nameText.text = PersonagemUtils.LoggedChar.nome.ToString();
                }
        }
    }

    private void SetExpSlider()
    {
        if (transform.Find("Canvas/slider_exp").TryGetComponent<Slider>(out Slider sliderExp))
        {
            if (PersonagemUtils.LoggedChar is not null && PhotonNetwork.IsConnectedAndReady)
            {
                PersonagemConfiguracao configuracao = PersonagemUtils.LoggedChar.configuracao;

                sliderExp.maxValue = ExpPerLevel[configuracao.level];
                sliderExp.value = Mathf.Clamp(configuracao.percentage, 0, sliderExp.maxValue);
            }
        }
    }

    private void HandleInputPanels()
    {
        if (Input.GetKeyDown(KeyCode.C))
            panelItems.SetActive(!panelItems.activeSelf);

        if (Input.GetKeyDown(KeyCode.B))
        {
            panelBag.SetActive(!panelBag.activeSelf);
            SetInventoryItems();
        }
    }

    private void SetSkills()
    {
        GameObject exampleCharButton = transform.Find("Canvas/panel_bottom/wrapper_skills/skill").gameObject;
        GameObject g;

        string[] buttons = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "=" };

        foreach (string button in buttons)
        {
            g = Instantiate(exampleCharButton, exampleCharButton.transform.parent);
            g.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = button;
        }

        Destroy(exampleCharButton);
    }

    private void SetButtons()
    {
        SetButton("Canvas/panel_bottom/panel_menu/close_button", DisconnectAndQuit);
        SetButton("Canvas/panel_bottom/panel_menu/character_selection", GoToSelectionCharacterScene);
        SetButton("Canvas/panel_bottom/panel_menu/settings_handle", OpenSettingsPanel);
        SetButton("Canvas/panel_death/respawn", RespawnCharacter);

    }

    private void SetButton(string path, UnityAction action)
    {
        if (transform.Find(path).TryGetComponent<Button>(out var button))
            button.onClick.AddListener(action);
    }

    private void DisconnectAndQuit()
    {
        PhotonNetwork.Disconnect();
        Application.Quit();

    }

    private void GoToSelectionCharacterScene()
    {
        PhotonNetwork.Disconnect();
        LoadingManager
            .GetSceneLoader()
            .LoadSceneWithLoadingScreen(
                "CharacterSelection"
            );
    }

    private void SetPanels()
    {
        panelItems = transform.Find("Canvas/panel_right/panel_items").gameObject;
        panelMission = transform.Find("Canvas/panel_right/panel_mission").gameObject;
        panelBag = transform.Find("Canvas/panel_inventory").gameObject;
        panelDeath = transform.Find("Canvas/panel_death").gameObject;
        panelSettings = transform.Find("Canvas/panel_settings").gameObject;
    }

    private void SetInventoryItems()
    {
        Transform gridTransform = transform.Find("Canvas/panel_inventory/grid_items");
        if (gridTransform == null) return;

        foreach (Transform child in gridTransform)
        {
            if (child.name != "item_example")
                Destroy(child.gameObject);
        }

        GameObject exampleInventoryPanel = gridTransform.Find("item_example")?.gameObject;
        if (exampleInventoryPanel == null) return;

        var inventoryItems = InventoryUtils.Inventario.itensInventario;

        for (int i = 0; i < 88; i++)
        {
            GameObject newPanel = Instantiate(exampleInventoryPanel, gridTransform);
            newPanel.name = $"slot_{i}";
            newPanel.SetActive(true); // Garante que o item esteja visível

            var inventoryItem = inventoryItems?.FirstOrDefault(_ => _.indice == i);

            if (inventoryItem != null)
            {
                GameObject inventoryItemPrefab = Resources.Load<GameObject>("InventoryItem");
                if (inventoryItemPrefab != null)
                {
                    GameObject inventoryItemInstance = Instantiate(inventoryItemPrefab, newPanel.transform);
                    inventoryItemInstance.GetComponent<DraggableItem>().item = inventoryItem;

                    RawImage rawImage = inventoryItemInstance.GetComponent<RawImage>();
                    if (rawImage != null)
                    {
                        Texture2D itemTexture = Resources.Load<Texture2D>($"ItemsIcons/{inventoryItem.itemDetalhes.recursoNomeItem}");
                        if (itemTexture != null)
                        {
                            rawImage.texture = itemTexture;
                        }
                    }
                }
            }
        }

        // Opcional: esconder o item exemplo para evitar ele ficar visível
        exampleInventoryPanel.SetActive(false);
    }

    private void RespawnCharacter()
    {
        var player = PhotonNetwork.LocalPlayer.TagObject as GameObject;
        panelDeath.SetActive(false);

        if (player.TryGetComponent<Animator>(out Animator animator))
        {
            animator.ResetTrigger("dying");
            animator.SetTrigger("resurrecting");
            animator.applyRootMotion = true;
        }

        if (player.TryGetComponent<CharacterController>(out CharacterController controller))
            controller.height = 1.9f;

        if (player.TryGetComponent<Movement>(out Movement movement))
            movement.enabled = true; // Desabilita o movement

        if (player.TryGetComponent<Combat>(out Combat combat))
            combat.enabled = true; // Desabilita o Combat

        if (player.TryGetComponent<Mob_NPC_CharacterInfoManager>(out Mob_NPC_CharacterInfoManager infoManager))
            infoManager.currentLife = infoManager.maxLife;

        StartCoroutine(DisableApplyRootMotion(animator));
    }

    private IEnumerator DisableApplyRootMotion(Animator animator)
    {
        yield return new WaitForSeconds(1f);

        if (animator != null)
        {
            animator.applyRootMotion = false;
        }
    }

    public void ShowDeathPanel()
    {
        panelItems.SetActive(false);
        panelMission.SetActive(false);
        panelBag.SetActive(false);
        panelSettings.SetActive(false);
        panelDeath.SetActive(true);
    }

    private void OpenSettingsPanel()
    {
        panelSettings.SetActive(!panelSettings.activeSelf);
    }
}
