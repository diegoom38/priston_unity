using Assets.Models;
using Assets.Scripts.Manager;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Dictionary<int, float> ExpPerLevel = PersonagemUtils.ExpPerLevel();

    // Start is called before the first frame update
    void Start()
    {
        SetButtons();
        SetSkills();
        DeactivatePanels();
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
        if (PersonagemUtils.LoggedChar is not null)
        {
            if (transform.Find("Canvas/panel_character/panel_image_char/panel_level/level").TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI levelText))
            {
                levelText.text = PersonagemUtils.LoggedChar.configuracao.level.ToString();
            }

            if (transform.Find("Canvas/panel_character/panel_status/name").TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI nameText))
            {
                nameText.text = PersonagemUtils.LoggedChar.nome.ToString();
            }
        }
    }

    private void SetExpSlider()
    {
        if (transform.Find("Canvas/slider_exp").TryGetComponent<Slider>(out Slider sliderExp))
        {
            if (PersonagemUtils.LoggedChar is not null)
            {
                PersonagemConfiguracao configuracao = PersonagemUtils.LoggedChar.configuracao;

                sliderExp.maxValue = ExpPerLevel[configuracao.level];
                sliderExp.value = Mathf.Clamp(configuracao.percentage, 0, sliderExp.maxValue);
            }
        }
    }

    private void HandleInputPanels()
    {
        GameObject panelItems = transform.Find("Canvas/panel_items").gameObject;

        if (Input.GetKeyDown(KeyCode.C))
            panelItems.SetActive(!panelItems.activeSelf);

        GameObject panelBag = transform.Find("Canvas/panel_inventory").gameObject;

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
        SetButton("Canvas/panel_bottom/panel_menu/close_button", Close);
        SetButton("Canvas/panel_bottom/panel_menu/character_selection", GoToSelectionCharacterScene);

    }

    private void SetButton(string path, UnityAction action)
    {
        if (transform.Find(path).TryGetComponent<Button>(out var button))
            button.onClick.AddListener(action);
    }

    private void Close()
    {
        Application.Quit();
    }

    private void GoToSelectionCharacterScene()
    {
        LoadingManager
            .GetSceneLoader()
            .LoadSceneWithLoadingScreen(
                "CharacterSelection"
            );
    }

    private void DeactivatePanels()
    {
        GameObject panelItems = transform.Find("Canvas/panel_items").gameObject;
        GameObject panelBag = transform.Find("Canvas/panel_inventory").gameObject;
        panelItems.SetActive(false);
        panelBag.SetActive(false);
    }

    private void SetInventoryItems()
    {
        GameObject exampleInventoryPanel = transform.Find("Canvas/panel_inventory/grid_items/item").gameObject;
        GameObject g;

        for (int i = 0; i < 88; i++)
        {
            g = Instantiate(exampleInventoryPanel, exampleInventoryPanel.transform.parent);
            //g.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = button;
        }

        Destroy(exampleInventoryPanel);
    }
}
