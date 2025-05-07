using Assets.Models;
using Assets.Scripts.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Scripts.Manager.ColorManager;

public class CharacterCreation : MonoBehaviour
{
    private Transform customizationPanel;
    private string gender = "Male";

    private GameObject playerPrefab;
    private GameObject currentCharacterInstance;
    private TMP_InputField characterName;

    private readonly Dictionary<string, string> headOptions = Scripts.Manager.SpecsManager.GetHeadOptions();
    private readonly Dictionary<string, string> hairOptions = Scripts.Manager.SpecsManager.GetHairOptions();

    private Color colorSkin;
    private Color colorHair;
    private Color colorEye;
    private Color colorLips;
    private string head;
    private string hair;

    private void Start()
    {
        customizationPanel = gameObject.transform.Find("Canvas/panel_scroll/panel_customization");

        InstantiateCharacter();
        StartInputFields();
        StartButtonListeners();
        InitializeColorGrid("skin_wrapper/skin_color_grid/skin_color_option", GetSkinColors(), ChangeSkinColor);
        InitializeColorGrid("hair_wrapper/hair_color_grid/hair_color_option", GetHairColors(), ChangeHairColor);
        InitializeColorGrid("eye_wrapper/eye_color_grid/eye_color_option", GetEyeColors(), ChangeEyeColor);
        InitializeColorGrid("lips_wrapper/lips_color_grid/lips_color_option", GetLipsColors(), ChangeLipColor);
        InitializeOptions("head_wrapper/dropdown_head", headOptions, OnDropdownHeadValueChanged);
        InitializeOptions("hair_wrapper/dropdown_hair", hairOptions, OnDropdownHairValueChanged);
    }

    private void StartInputFields()
    {
        if (gameObject.transform.Find("Canvas/character_name").TryGetComponent<TMP_InputField>(out TMP_InputField field))
            characterName = field;
    }

    private void InstantiateCharacter()
    {
        // Destrói a instância atual do personagem, se houver
        Destroy(currentCharacterInstance);

        // Carrega o prefab do personagem com base no gênero e idade
        playerPrefab = Resources.Load<GameObject>($"Player{gender}CharacterSelector");

        GameObject tempPositionHolder = new GameObject("TempPositionHolder");
        tempPositionHolder.transform.parent = transform;
        tempPositionHolder.transform.localPosition = new Vector3(35.2f, -176.7f, 65.33f);
        tempPositionHolder.transform.localRotation = Quaternion.Euler(0, 180, 0);

        currentCharacterInstance = Instantiate(playerPrefab, tempPositionHolder.transform.position, tempPositionHolder.transform.rotation, transform);

        // Ajusta o estado da câmera do jogador
        currentCharacterInstance.transform.Find("CameraPlayer").gameObject.SetActive(false);

        if (currentCharacterInstance.TryGetComponent<Movement>(out var movementScript))
            movementScript.enabled = false;


        // Remove o GameObject temporário
        Destroy(tempPositionHolder);

        Randomize();
    }

    private void StartButtonListeners()
    {
        void SetupButton(string path, UnityAction action)
        {
            var button = gameObject.transform.Find(path).GetComponent<Button>();
            button.onClick.AddListener(action);
        }

        SetupButton("Canvas/panel_actions/back_button", Back);

        SetupButton("Canvas/panel_gender/btn_fem", () =>
        {
            gender = "Female";
            InstantiateCharacter();
        });

        SetupButton("Canvas/panel_gender/btn_masc", () =>
        {
            gender = "Male";
            InstantiateCharacter();
        });

        SetupButton("Canvas/panel_actions/randomize_button", Randomize);

        SetupButton("Canvas/save_button", async () =>
        {
            if (string.IsNullOrEmpty(characterName.text))
            {
                WarningUIManager.ExibirAviso("Nome precisa ser preenchido!", "Erro ao criar o seu personagem");
                return;
            }

            var personagem = new Personagem
            {
                nome = characterName.text,
                configuracao = new PersonagemConfiguracao
                {
                    level = 1,
                    percentage = 0.0f,
                    prefab = $"Player{gender}CharacterSelector",
                    gender = gender,
                    age = "Adult",
                    configuracaoCorPele = new PersonagemConfiguracao.PersonagemConfiguracaoCor
                    {
                        r = (int)(colorSkin.r * 255f),
                        g = (int)(colorSkin.g * 255f),
                        b = (int)(colorSkin.b * 255f)
                    },

                    configuracaoCorCabelo = new PersonagemConfiguracao.PersonagemConfiguracaoCor
                    {
                        r = (int)(colorHair.r * 255f),
                        g = (int)(colorHair.g * 255f),
                        b = (int)(colorHair.b * 255f)
                    },

                    configuracaoCorOlhos = new PersonagemConfiguracao.PersonagemConfiguracaoCor
                    {
                        r = (int)(colorEye.r * 255f),
                        g = (int)(colorEye.g * 255f),
                        b = (int)(colorEye.b * 255f)
                    },

                    configuracaoCorLabios = new PersonagemConfiguracao.PersonagemConfiguracaoCor
                    {
                        r = (int)(colorLips.r * 255f),
                        g = (int)(colorLips.g * 255f),
                        b = (int)(colorLips.b * 255f)
                    },

                    hair = head,
                    head = hair
                },
                contaId = Acesso.LoggedUser.user.id,
                id = Guid.NewGuid().ToString(),
                criadoEm = DateTime.UtcNow.ToString("o"),
            };

            var createCharacter = await AccountCharacters.CreateCharacter(personagem);

            Back();
        });
    }

    private void InitializeColorGrid(string gridPath, List<Scripts.Manager.ColorManager> colors, System.Action<Color> colorChangeAction)
    {
        GameObject colorOptionTemplate = customizationPanel.Find(gridPath).gameObject;

        foreach (Scripts.Manager.ColorManager colorInfo in colors)
        {
            GameObject colorOption = Instantiate(colorOptionTemplate, colorOptionTemplate.transform.parent);

            if (colorOption.TryGetComponent<Image>(out Image imageComponent))
            {
                Color color = colorInfo.ToColorEngine();
                imageComponent.color = color;

                if (!colorOption.TryGetComponent<Button>(out Button button))
                    button = colorOption.AddComponent<Button>();

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => colorChangeAction(color));
            }

            colorOption.transform.SetParent(colorOptionTemplate.transform.parent, false);
        }

        Destroy(colorOptionTemplate);
    }

    private void ChangeSkinColor(Color color)
    {
        foreach (KeyValuePair<string, string> mesh in MaterialManager.MeshSkinsList(gender))
            MaterialManager.ChangeMaterialColor(currentCharacterInstance.transform.Find(mesh.Key), color, mesh.Value);

        colorSkin = color;
    }
    private void ChangeHairColor(Color color)
    {
        foreach (KeyValuePair<string, string> mesh in MaterialManager.MeshHairList(gender))
            MaterialManager.ChangeMaterialColor(currentCharacterInstance.transform.Find(mesh.Key), color, mesh.Value);

        colorHair = color;
    }

    private void ChangeEyeColor(Color color)
    {
        foreach (KeyValuePair<string, string> mesh in MaterialManager.MeshEyeList(gender))
            MaterialManager.ChangeMaterialColor(currentCharacterInstance.transform.Find(mesh.Key), color, mesh.Value);

        colorEye = color;
    }

    private void ChangeLipColor(Color color)
    {
        foreach (KeyValuePair<string, string> mesh in MaterialManager.MeshLipList(gender))
            MaterialManager.ChangeMaterialColor(currentCharacterInstance.transform.Find(mesh.Key), color, mesh.Value);

        colorLips = color;
    }

    private void InitializeOptions(string path, Dictionary<string, string> options, System.Action<TMP_Dropdown> dropdownAction)
    {
        GameObject dropdownObject = customizationPanel.Find(path).gameObject;


        if (dropdownObject.TryGetComponent<TMP_Dropdown>(out var dropdown))
        {
            dropdown.ClearOptions();

            List<TMP_Dropdown.OptionData> hairOptions = new();

            foreach (KeyValuePair<string, string> option in options)
            {
                hairOptions.Add(new TMP_Dropdown.OptionData(option.Value));
            }

            dropdown.AddOptions(hairOptions);
            dropdown.value = 0;
            dropdown.onValueChanged.AddListener(delegate { dropdownAction(dropdown); });
        }
    }

    private void OnDropdownHeadValueChanged(TMP_Dropdown dropdown) => DropdownValueChangedDefault(dropdown, "Head", headOptions);

    private void OnDropdownHairValueChanged(TMP_Dropdown dropdown) => DropdownValueChangedDefault(dropdown, "Hair", hairOptions);

    private void DropdownValueChangedDefault(TMP_Dropdown dropdown, string folderName, Dictionary<string, string> options)
    {
        string selectedOption = dropdown.options[dropdown.value].text;

        if (!options.Any(pair => pair.Value == selectedOption)) return;

        string selectedKey = options.First(pair => pair.Value == selectedOption).Key;

        var actions = new Dictionary<string, Action>
        {
            { "Head", () => head = selectedKey },
            { "Hair", () => hair = selectedKey }
        };

        if (actions.ContainsKey(folderName))        
            actions[folderName]();

        Transform folderTransform = currentCharacterInstance.transform.Find(folderName);

        if (folderTransform == null) return;

        foreach (var key in options.Keys)
        {
            Transform head = folderTransform.Find(key);
            if (head != null)
                head.gameObject.SetActive(key == selectedKey);
        }
    }

    public void Back() => LoadingManager.GetSceneLoader().LoadSceneWithLoadingScreen("CharacterSelection");

    public void Randomize()
    {
        CharacterCreationRandomize randomSkinColor = Scripts.Manager.ColorManager.Randomize();
        ChangeSkinColor(randomSkinColor.SkinColor.ToColorEngine());
        ChangeHairColor(randomSkinColor.HairColor.ToColorEngine());
        ChangeEyeColor(randomSkinColor.EyeColor.ToColorEngine());
        ChangeLipColor(randomSkinColor.LipColor.ToColorEngine());

        void RandomizeSpecs(Dictionary<string, string> options, string folder)
        {
            int randomIndex = UnityEngine.Random.Range(0, options.Count);

            var headDropdown = customizationPanel.Find(folder).GetComponent<TMP_Dropdown>();
            headDropdown.value = randomIndex;
            OnDropdownHeadValueChanged(headDropdown);
        }

        RandomizeSpecs(headOptions, "head_wrapper/dropdown_head");
        RandomizeSpecs(hairOptions, "hair_wrapper/dropdown_hair");
    }
}