using Cinemachine;
using UnityEngine;
using Photon.Pun;

public class CameraManager : MonoBehaviourPun
{
    private CinemachineFreeLook freeLookCamera;

    private void Start()
    {
        // Configura a câmera para o jogador local e desativa as câmeras dos outros jogadores
        SetCamera();
    }

    private void SetCamera()
    {
        // Busca todos os objetos com a tag "Player"
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            // Verifica se o jogador é o local
            PhotonView photonView = player.GetComponent<PhotonView>();
            if (photonView != null)
            {
                // Encontra a câmera do jogador
                CinemachineFreeLook _freeCam = player.GetComponentInChildren<CinemachineFreeLook>();
                Camera _camera = player.GetComponentInChildren<Camera>();

                if (_freeCam != null)
                {
                    if (photonView.IsMine)
                    {
                        // Configura a câmera para o jogador local
                        freeLookCamera = _freeCam;
                    }
                    else
                    {
                        // Desativa a câmera dos outros jogadores
                        _freeCam.gameObject.SetActive(false);
                        _camera.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    private void Update()
    {
        SetCamera();

        if (freeLookCamera != null)
        {

            // Habilita ou desabilita o controle da câmera com base no botão do mouse
            if (Input.GetMouseButton(1)) // Botão direito do mouse
            {
                // Habilita o controle da câmera
                freeLookCamera.enabled = true;
                Cursor.lockState = CursorLockMode.Locked; // Tranca o cursor
                Cursor.visible = false; // Torna o cursor invisível
            }
            else
            {
                // Desabilita o controle da câmera
                freeLookCamera.enabled = false;
                Cursor.lockState = CursorLockMode.None; // Libera o cursor
                Cursor.visible = true; // Torna o cursor visível
            }
        }
    }
}