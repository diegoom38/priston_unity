using Assets.Models;
using Assets.Scripts.Manager;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime; // Para acessar a lista de jogadores do Photon
using UnityEngine;
using static Assets.Models.PersonagemConfiguracao;

public class Game : MonoBehaviourPunCallbacks
{
    private GameObject playerPrefab;

    // Start é chamado antes do primeiro frame
    void Start()
    {
        // Carrega o prefab do personagem selecionado
        SetCharacterSelected(PersonagemUtils.LoggedChar);
    }

    public void SetCharacterSelected(Personagem character)
    {
        // Carrega o prefab do personagem
        playerPrefab = Resources.Load<GameObject>(character?.configuracao?.prefab);

        if (playerPrefab == null)
            return;

        // Apenas configura o prefab, sem instanciar ainda
        PersonagemUtils.LoggedChar = character;

        // Define o nome do personagem no Photon
        string characterName = character?.nome ?? "Personagem Desconhecido"; // Define um nome padrão se não houver nome
        SetCharacterName(characterName);

        // Armazenar os dados do personagem no Photon
        SetCharacterData(character);

        Connect();
    }

    public void SetCharacterName(string name)
    {
        // Define o nome do personagem no Photon
        PhotonNetwork.NickName = name;

        // Aqui você pode adicionar lógica para exibir o nome no jogo, caso necessário
        Debug.Log($"Nome do personagem: {name}");
    }

    public void SetCharacterData(Personagem character)
    {
        // Armazena os dados do personagem no PlayerCustomProperties do Photon
        ExitGames.Client.Photon.Hashtable playerProperties = new()
        {
            { "PersonagemId", character.id },
            { "PersonagemNome", character.nome },
            { "ContaId", character.contaId },
            { "CriadoEm", character.criadoEm },
            { "PersonagemConfig", character.configuracao.ToString() }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRoom("Game");
        base.OnConnectedToMaster();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom("Game");
        base.OnJoinRoomFailed(returnCode, message);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.LocalPlayer.IsLocal)
        {
            GameObject respawnPoint = GameObject.Find("Respawn");

            GameObject playerInstance = PhotonNetwork.Instantiate(playerPrefab.name, respawnPoint.transform.position, respawnPoint.transform.rotation);
            playerInstance.name = PersonagemUtils.LoggedChar.nome;
            PhotonNetwork.LocalPlayer.TagObject = playerInstance;
            playerInstance.GetComponent<Movement>().enabled = true;
            playerInstance.GetComponent<Combat>().enabled = true;
            playerInstance.GetComponent<OutlineManager>().enabled = true;

            string[] objectsToActive = new string[] { "CameraPlayer", "CameraFreeLook", "CameraDisplay", "HandleScene" };

            foreach (var objects in objectsToActive)
            {
                GameObject _object = playerInstance.transform.Find(objects)?.gameObject;
                _object?.SetActive(true);
            }

            PhotonView photonView = playerInstance.GetComponent<PhotonView>();
            CharacterAppearance characterAppearance = playerInstance.GetComponent<CharacterAppearance>();

            if (photonView.IsMine && characterAppearance != null)
            {
                static Color GetColor(PersonagemConfiguracaoCor configuracao)
                {
                    return new Color(
                        configuracao.r / 255f,
                        configuracao.g / 255f,
                        configuracao.b / 255f
                    );
                }

                Color skinColor = GetColor(PersonagemUtils.LoggedChar.configuracao.configuracaoCorPele);
                Color hairColor = GetColor(PersonagemUtils.LoggedChar.configuracao.configuracaoCorCabelo);
                Color eyeColor = GetColor(PersonagemUtils.LoggedChar.configuracao.configuracaoCorOlhos);
                Color lipColor = GetColor(PersonagemUtils.LoggedChar.configuracao.configuracaoCorLabios);

                photonView.RPC("UpdateCharacterAppearance", RpcTarget.AllBuffered,
                    skinColor.r, skinColor.g, skinColor.b,
                    hairColor.r, hairColor.g, hairColor.b,
                    eyeColor.r, eyeColor.g, eyeColor.b,
                    lipColor.r, lipColor.g, lipColor.b,
                    PersonagemUtils.LoggedChar.configuracao.gender);
            }
        }
    }
}
