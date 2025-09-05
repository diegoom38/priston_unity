using Cinemachine;
using UnityEngine;
using Photon.Pun;

public class CameraManager : MonoBehaviourPun
{
    private CinemachineFreeLook freeLookCamera;
    private CinemachineCollider cameraCollider;

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

            if (freeLookCamera != null)
            {
                // Garante que o CinemachineCollider esteja presente
                cameraCollider = freeLookCamera.gameObject.GetComponent<CinemachineCollider>();
                if (cameraCollider == null)
                {
                    cameraCollider = freeLookCamera.gameObject.AddComponent<CinemachineCollider>();
                }

                // Configurações básicas para evitar atravessar paredes
                cameraCollider.m_AvoidObstacles = true;
                cameraCollider.m_Damping = 0.5f;
                cameraCollider.m_MinimumDistanceFromTarget = 2f;
                cameraCollider.m_Strategy = CinemachineCollider.ResolutionStrategy.PullCameraForward;
            }
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
