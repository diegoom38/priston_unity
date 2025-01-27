using Assets.Models;
using Assets.Scripts.Manager;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private GameObject playerPrefab;
    private GameObject currentCharacterInstance;
    // Start is called before the first frame update
    void Start()
    {
        SetCharacterSelected(PersonagemUtils.LoggedChar);
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
        if (playerPrefab != null)
        {
            currentCharacterInstance = Instantiate(playerPrefab, respawnPoint.transform.position, respawnPoint.transform.rotation);

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
        }
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
