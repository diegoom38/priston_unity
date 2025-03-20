using Assets.Models;
using Assets.Scripts.Manager;
using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviourPunCallbacks
{
    private Dictionary<int, float> ExpPerLevel = PersonagemUtils.ExpPerLevel();

    private GameObject panelItems;
    private GameObject panelMission;
    private GameObject panelBag;
    private GameObject panelDeath;

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
            panelBag.SetActive(!panelBag.activeSelf);
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
    }

    private void SetInventoryItems()
    {
        GameObject exampleInventoryPanel = transform.Find("Canvas/panel_inventory/grid_items/item").gameObject;

        for (int i = 0; i < 88; i++)
            Instantiate(exampleInventoryPanel, exampleInventoryPanel.transform.parent);

        Destroy(exampleInventoryPanel);
    }

    public void ShowDeathPanel()
    {
        panelItems.SetActive(false);
        panelMission.SetActive(false);
        panelBag.SetActive(false);
        panelDeath.SetActive(true);
    }
}
