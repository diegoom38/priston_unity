using Cinemachine;
using UnityEngine;

namespace Assets.Scripts.Manager
{
    public class CameraManager : MonoBehaviour
    {
        private CinemachineFreeLook freeLookCamera;
        private void Start()
        {
            SetCamera();
        }

        private void SetCamera()
        {
            GameObject player = GameObject.FindWithTag("Player");

            if (player != null)
                freeLookCamera = player.GetComponentInChildren<CinemachineFreeLook>();
        }

        private void Update()
        {
            // Alterna o cursor ao pressionar Alt (removido, pois não é mais necessário)

            // Habilita ou desabilita o controle da câmera com base no botão do mouse
            if (freeLookCamera != null)
            {
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
            else
                SetCamera();

        }
    }
}
