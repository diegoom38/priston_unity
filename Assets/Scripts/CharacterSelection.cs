using Assets.Models;
using Assets.Scripts.Manager;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
        ChangeSkinColor(
            new Color(
                r: character.configuracao.configuracaoCorPele.r / 255f,
                g: character.configuracao.configuracaoCorPele.g / 255f,
                b: character.configuracao.configuracaoCorPele.b / 255f
            ),
            meshes,
            character.configuracao.gender,
            character.configuracao.age
        );
        ChangeHairColor(
            new Color(
                r: character.configuracao.configuracaoCorCabelo.r / 255f,
                g: character.configuracao.configuracaoCorCabelo.g / 255f,
                b: character.configuracao.configuracaoCorCabelo.b / 255f
            ),
            meshes,
            character.configuracao.gender,
            character.configuracao.age
        );
        ChangeEyeColor(
            new Color(
                r: character.configuracao.configuracaoCorOlhos.r / 255f,
                g: character.configuracao.configuracaoCorOlhos.g / 255f,
                b: character.configuracao.configuracaoCorOlhos.b / 255f
            ),
            meshes,
            character.configuracao.gender,
            character.configuracao.age
        );

        PersonagemUtils.LoggedChar = character;
        StatusComponentsHandle(true);
    }

    private void ChangeSkinColor(Color color, Transform characterMeshes, string gender, string age)
    {
        foreach (KeyValuePair<string, string> mesh in MaterialManager.MeshSkinsList(gender, age))
            MaterialManager.ChangeMaterialColor(characterMeshes.Find(mesh.Key), color, mesh.Value);
    }

    private void ChangeHairColor(Color color, Transform characterMeshes, string gender, string age)
    {
        foreach (KeyValuePair<string, string> mesh in MaterialManager.MeshHairList(gender, age))
            MaterialManager.ChangeMaterialColor(characterMeshes.Find(mesh.Key), color, mesh.Value);
    }

    private void ChangeEyeColor(Color color, Transform characterMeshes, string gender, string age)
    {
        foreach (KeyValuePair<string, string> mesh in MaterialManager.MeshEyeList(gender, age))
            MaterialManager.ChangeMaterialColor(characterMeshes.Find(mesh.Key), color, mesh.Value);
    }
}

