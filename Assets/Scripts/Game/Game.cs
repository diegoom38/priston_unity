using Assets.Models;
using Assets.Scripts.Core.Services.Inventory;
using Assets.Utils.Inventory;
using Photon.Pun;
using UnityEngine;
using static CharacterAppearanceHandler;

public class Game : MonoBehaviourPunCallbacks
{
    private GameObject playerPrefab;
    private readonly string[] objectsToActivate = { "CameraPlayer", "CameraFreeLook", "CameraDisplay", "HandleScene" };

    void Start()
    {
        SetCharacterSelected(PersonagemUtils.LoggedChar);
    }

    public void SetCharacterSelected(Personagem character)
    {
        playerPrefab = Resources.Load<GameObject>(character?.configuracao?.prefab);
        if (playerPrefab == null) return;

        PersonagemUtils.LoggedChar = character;

        SetCharacterName(character?.nome ?? "Personagem Desconhecido");
        SetCharacterData(character);
        Connect();
    }

    public void SetCharacterName(string name)
    {
        PhotonNetwork.NickName = name;
        Debug.Log($"Nome do personagem: {name}");
    }

    public void SetCharacterData(Personagem character)
    {
        var playerProperties = new ExitGames.Client.Photon.Hashtable
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

    public override async void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if (!PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.LocalPlayer.IsLocal) return;

        var inventory = await InventoryService.GetInventoryByCharacterId();
        if (inventory != null)
            InventoryUtils.Inventario = inventory;

        GameObject respawnPoint = GameObject.Find("Respawn");
        GameObject playerInstance = PhotonNetwork.Instantiate(playerPrefab.name, respawnPoint.transform.position, respawnPoint.transform.rotation);
        playerInstance.name = PersonagemUtils.LoggedChar.nome;
        PhotonNetwork.LocalPlayer.TagObject = playerInstance;

        EnablePlayerComponents(playerInstance);
        ActivateObjects(playerInstance);

        ApplyAppearanceToPlayer(playerInstance, PersonagemUtils.LoggedChar);
    }

    private void EnablePlayerComponents(GameObject player)
    {
        player.GetComponent<Movement>().enabled = true;
        player.GetComponent<Combat>().enabled = true;
        player.GetComponent<OutlineManager>().enabled = true;
    }

    private void ActivateObjects(GameObject player)
    {
        foreach (var objName in objectsToActivate)
        {
            GameObject obj = player.transform.Find(objName)?.gameObject;
            obj?.SetActive(true);
        }
    }

    private void ApplyAppearanceToPlayer(GameObject player, Personagem character)
    {
        var photonView = player.GetComponent<PhotonView>();
        var appearanceComponent = player.GetComponent<CharacterAppearance>();

        if (!photonView.IsMine || appearanceComponent == null) return;

        // Aplicar aparência com delegates
        Transform meshes = player.transform;
        foreach (var apply in GetAppearanceHandlers())
        {
            apply(meshes, character);
        }

        Color skin = CharacterAppearance.GetColor(character.configuracao.configuracaoCorPele);
        Color hair = CharacterAppearance.GetColor(character.configuracao.configuracaoCorCabelo);
        Color eye = CharacterAppearance.GetColor(character.configuracao.configuracaoCorOlhos);
        Color lip = CharacterAppearance.GetColor(character.configuracao.configuracaoCorLabios);

        photonView.RPC("UpdateCharacterAppearance", RpcTarget.AllBuffered,
            skin.r, skin.g, skin.b,
            hair.r, hair.g, hair.b,
            eye.r, eye.g, eye.b,
            lip.r, lip.g, lip.b,
            character.configuracao.gender,
            character.configuracao.head,
            character.configuracao.hair,
            character.configuracao.scale.x,
            character.configuracao.scale.y,
            character.configuracao.scale.z);
    }
}
