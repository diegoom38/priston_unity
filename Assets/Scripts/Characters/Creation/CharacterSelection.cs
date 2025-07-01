using Assets.Models;
using Assets.Scripts.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;
using UnityEngine.UI;
using static Assets.Models.PersonagemConfiguracao;

public class CharacterSelection : MonoBehaviour
{
    private GameObject playerPrefab;
    private GameObject currentCharacterInstance;

    async void Start()
    {
        InstantiateCharactersAndDestroyDefault(await GetCharacters());
        StartButtonListeners();
        StatusComponentsHandle(false);
    }

    private void StatusComponentsHandle(bool active)
    {
        transform.Find("panel_actions_selected").gameObject.SetActive(active);
    }

    private void StartButtonListeners()
    {
        void SetupButton(string path, UnityAction action)
        {
            var button = transform.Find(path).GetComponent<Button>();
            button.onClick.AddListener(action);
        }

        SetupButton("panel_actions/btn_add", AddCharacter);
        SetupButton("panel_actions/btn_exit", Exit);
        SetupButton("panel_actions_selected/btn_start", StartGame);
    }

    private void InstantiateCharactersAndDestroyDefault(List<Personagem> characters)
    {
        GameObject exampleCharButton = transform.Find("panel_scroll_character/panel_character_list/btn_example_char").gameObject;
        GameObject g;

        for (int i = 0; i < characters.Count; i++)
        {
            int index = i;
            g = Instantiate(exampleCharButton, exampleCharButton.transform.parent);
            g.SetActive(true);
            g.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = characters[i].nome;
            g.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"Nível {characters[i]?.configuracao?.level}";

            g.GetComponent<Button>().onClick.AddListener(() => SetCharacterSelected(characters[index]));
        }

        Destroy(exampleCharButton);
    }

    public void AddCharacter()
    {
        LoadingManager.GetSceneLoader().LoadSceneWithLoadingScreen("CharacterCreation");
    }

    public void StartGame()
    {
        LoadingManager.GetSceneLoader().LoadSceneWithLoadingScreen("GameScene");
    }

    public void Exit()
    {
        LoadingManager.GetSceneLoader().LoadSceneWithLoadingScreen("LoginScene");
    }

    private async Task<List<Personagem>> GetCharacters()
    {
        return await AccountCharacters.Characters();
    }

    public void SetCharacterSelected(Personagem character)
    {
        // Encontra o GameObject "Respawn" na cena
        GameObject respawnPoint = GameObject.Find("Respawn");
        if (respawnPoint == null)
        {
            Debug.LogError("Respawn point not found in the scene.");
            return;
        }

        // Destroi a instância do personagem atual, se existir
        if (currentCharacterInstance != null)
        {
            Destroy(currentCharacterInstance);
        }

        // Carrega o prefab do personagem
        playerPrefab = Resources.Load<GameObject>(character?.configuracao?.prefab);

        // Instancia o novo personagem na posição e rotação do "Respawn"
        currentCharacterInstance = Instantiate(playerPrefab, respawnPoint.transform.position, respawnPoint.transform.rotation);

        // Desativa a câmera do jogador no prefab, se existir
        currentCharacterInstance.transform.Find("CameraPlayer").gameObject.SetActive(false);

        if (currentCharacterInstance.TryGetComponent<Movement>(out var movementScript))
            movementScript.enabled = false;

        if (currentCharacterInstance.TryGetComponent<Combat>(out var combatScript))
            combatScript.enabled = false;

        if (currentCharacterInstance.TryGetComponent<OutlineManager>(out var outlineManager))
            outlineManager.enabled = false;

        // Configurações de aparência
        Transform meshes = currentCharacterInstance.transform;

        ChangeSkinColor(CharacterAppearance.GetColor(character.configuracao.configuracaoCorPele), meshes, character.configuracao.gender);
        ChangeHairColor(CharacterAppearance.GetColor(character.configuracao.configuracaoCorCabelo), meshes, character.configuracao.gender);
        ChangeEyeColor(CharacterAppearance.GetColor(character.configuracao.configuracaoCorOlhos), meshes, character.configuracao.gender);
        ChangeLipColor(CharacterAppearance.GetColor(character.configuracao.configuracaoCorLabios), meshes, character.configuracao.gender);

        CharacterAppearance.DropdownValueChangedDefault(character.configuracao.head, "Head", Scripts.Manager.SpecsManager.GetHeadOptions(), meshes);
        CharacterAppearance.DropdownValueChangedDefault(character.configuracao.hair, "Hair", Scripts.Manager.SpecsManager.GetHairOptions(), meshes);
        CharacterAppearance.UpdateScale(character.configuracao.scale.x, character.configuracao.scale.y, character.configuracao.scale.z, meshes);
        PersonagemUtils.LoggedChar = character;
        StatusComponentsHandle(true);
    }

    private void ChangeSkinColor(Color color, Transform characterMeshes, string gender) =>
        CharacterAppearance.ChangeSkinColor(color, characterMeshes, gender);

    private void ChangeHairColor(Color color, Transform characterMeshes, string gender) =>
        CharacterAppearance.ChangeHairColor(color, characterMeshes, gender);


    private void ChangeEyeColor(Color color, Transform characterMeshes, string gender) =>
        CharacterAppearance.ChangeEyeColor(color, characterMeshes, gender);


    private void ChangeLipColor(Color color, Transform characterMeshes, string gender) =>
        CharacterAppearance.ChangeLipColor(color, characterMeshes, gender);

}

