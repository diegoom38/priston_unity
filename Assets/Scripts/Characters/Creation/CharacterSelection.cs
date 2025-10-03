using Assets.Models;
using Assets.Scripts.Manager;
using Assets.ViewModels.Personagem;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static CharacterAppearanceHandler;

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

    private void InstantiateCharactersAndDestroyDefault(List<PersonagemInventarioViewModel> characters)
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
        LoadingManager.GetSceneLoader().LoadSceneWithLoadingScreen("CharacterSelection");
    }

    public void StartGame()
    {
        LoadingManager.GetSceneLoader().LoadSceneWithLoadingScreen("GameScene");
    }

    public void Exit()
    {
        LoadingManager.GetSceneLoader().LoadSceneWithLoadingScreen("CharacterCreation");
    }

    private async Task<List<PersonagemInventarioViewModel>> GetCharacters()
    {
        return await AccountCharacters.GetCharacters();
    }

    public void SetCharacterSelected(Personagem character)
    {
        GameObject respawnPoint = GameObject.Find("Respawn");
        if (respawnPoint == null)
        {
            Debug.LogWarning("Respawn point not found in the scene.");
            return;
        }

        if (currentCharacterInstance != null)
        {
            Destroy(currentCharacterInstance);
        }

        playerPrefab = Resources.Load<GameObject>(character?.configuracao?.prefab);
        currentCharacterInstance = Instantiate(playerPrefab, respawnPoint.transform.position, respawnPoint.transform.rotation);

        currentCharacterInstance.transform.Find("CameraPlayer").gameObject.SetActive(false);

        if (currentCharacterInstance.TryGetComponent<Movement>(out var movementScript))
            movementScript.enabled = false;

        if (currentCharacterInstance.TryGetComponent<Combat>(out var combatScript))
            combatScript.enabled = false;

        if (currentCharacterInstance.TryGetComponent<OutlineManager>(out var outlineManager))
            outlineManager.enabled = false;

        Transform meshes = currentCharacterInstance.transform;
        foreach (var apply in GetAppearanceHandlers())
        {
            apply(meshes, character);
        }

        PersonagemUtils.LoggedChar = character;

        StatusComponentsHandle(true);
    }
}
