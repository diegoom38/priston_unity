using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

public class OutlineManager : MonoBehaviourPun
{
    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;

    private Camera playerCamera; // Referência para a câmera do jogador local

    private void Start()
    {
        // Configura a câmera do jogador local
        SetPlayerCamera();
    }

    private void SetPlayerCamera()
    {
        if (photonView != null && photonView.IsMine)
        {
            // Encontra a câmera do jogador local
            if(transform.Find("CameraPlayer").TryGetComponent<Camera>(out Camera camera))
            {
                playerCamera = camera;
            }
        }
    }

    void Update()
    {
        if (playerCamera == null) SetPlayerCamera();

        // Limpar highlight anterior
        if (highlight != null)
        {
            if (highlight.TryGetComponent<Outline>(out var outline)) outline.enabled = false;
            highlight = null;
        }

        // Raycast para detectar objeto sob o mouse
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
        {
            Transform hitTransform = raycastHit.transform;
            if ((hitTransform.CompareTag("Selectable") || hitTransform.CompareTag("Enemy")) && hitTransform != selection)
            {
                highlight = hitTransform;
                if (!highlight.TryGetComponent<Outline>(out var outline))
                {
                    try
                    {
                        outline = highlight.gameObject.AddComponent<Outline>();
                        outline.OutlineWidth = 7.0f;

                        // Definir a cor com base na tag
                        if (hitTransform.CompareTag("Enemy"))
                        {
                            outline.OutlineColor = Color.red; // Cor vermelha para "Enemy"
                        }
                        else if (hitTransform.CompareTag("Selectable"))
                        {
                            outline.OutlineColor = Color.gray; // Cor cinza para "Selectable"
                        }
                    }
                    catch { }
                }
                outline.enabled = true;
            }
        }
    }
}