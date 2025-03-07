using Cinemachine;
using UnityEngine;
using Photon.Pun;

public class CameraManager : MonoBehaviourPun
{
    private CinemachineFreeLook freeLookCamera;

    private void Start()
    {
        SetCamera();
    }

    private void SetCamera()
    {
        // Verifica se o jogador local é este objeto (o CameraManager está no prefab do jogador)
        if (photonView.IsMine)
        {
            // Busca a câmera do jogador local, que é um filho do prefab
            freeLookCamera = GetComponentInChildren<CinemachineFreeLook>();
        }
    }

    private void Update()
    {
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
