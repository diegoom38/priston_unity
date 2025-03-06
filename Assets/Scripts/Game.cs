using Assets.Models;
using Assets.Scripts.Manager;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Game : MonoBehaviourPunCallbacks
{
    private GameObject playerPrefab;

    // Start é chamado antes do primeiro frame
    void Start()
    {
        // Carrega o prefab do personagem selecionado
        SetCharacterSelected(PersonagemUtils.LoggedChar);
        _Connect();
    }

    public void SetCharacterSelected(Personagem character)
    {
        // Carrega o prefab do personagem
        playerPrefab = Resources.Load<GameObject>(character?.configuracao?.prefab);

        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab not found.");
            return;
        }

        // Apenas configura o prefab, sem instanciar ainda
        PersonagemUtils.LoggedChar = character;
    }

    public void _Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRoom("teste");
        base.OnConnectedToMaster();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom("teste");
        base.OnJoinRoomFailed(returnCode, message);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // Verifica se o jogador é o local
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.LocalPlayer.IsLocal)
        {
            // Encontra o ponto de respawn
            GameObject respawnPoint = GameObject.Find("Respawn");
            if (respawnPoint == null)
            {
                Debug.LogError("Respawn point not found in the scene.");
                return;
            }

            // Instancia o personagem na rede
            GameObject playerInstance = PhotonNetwork.Instantiate(playerPrefab.name, respawnPoint.transform.position, respawnPoint.transform.rotation);
            playerInstance.GetComponent<Movement>().enabled = true;
            playerInstance.GetComponent<Combat>().enabled = true;

            // Configurações de aparência (aplicadas apenas ao jogador local)
            if (playerInstance.GetComponent<PhotonView>().IsMine)
            {
                Transform meshes = playerInstance.transform;
                ChangeSkinColor(
                    new Color(
                        r: PersonagemUtils.LoggedChar.configuracao.configuracaoCorPele.r / 255f,
                        g: PersonagemUtils.LoggedChar.configuracao.configuracaoCorPele.g / 255f,
                        b: PersonagemUtils.LoggedChar.configuracao.configuracaoCorPele.b / 255f
                    ),
                    meshes,
                    PersonagemUtils.LoggedChar.configuracao.gender,
                    PersonagemUtils.LoggedChar.configuracao.age
                );
                ChangeHairColor(
                    new Color(
                        r: PersonagemUtils.LoggedChar.configuracao.configuracaoCorCabelo.r / 255f,
                        g: PersonagemUtils.LoggedChar.configuracao.configuracaoCorCabelo.g / 255f,
                        b: PersonagemUtils.LoggedChar.configuracao.configuracaoCorCabelo.b / 255f
                    ),
                    meshes,
                    PersonagemUtils.LoggedChar.configuracao.gender,
                    PersonagemUtils.LoggedChar.configuracao.age
                );
                ChangeEyeColor(
                    new Color(
                        r: PersonagemUtils.LoggedChar.configuracao.configuracaoCorOlhos.r / 255f,
                        g: PersonagemUtils.LoggedChar.configuracao.configuracaoCorOlhos.g / 255f,
                        b: PersonagemUtils.LoggedChar.configuracao.configuracaoCorOlhos.b / 255f
                    ),
                    meshes,
                    PersonagemUtils.LoggedChar.configuracao.gender,
                    PersonagemUtils.LoggedChar.configuracao.age
                );
            }
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