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
        SetPlayerCamera();
    }

    private void SetPlayerCamera()
    {
        if (photonView != null && photonView.IsMine)
        {
            if (transform.Find("CameraPlayer").TryGetComponent(out Camera camera))
            {
                playerCamera = camera;
            }
        }
    }

    void Update()
    {
        if (playerCamera == null) SetPlayerCamera();

        if (playerCamera == null) return;

        // Raycast da posição do mouse
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out raycastHit))
        {
            Transform target = raycastHit.transform;

            // Verifica se o objeto tem as tags desejadas
            if (target.CompareTag("Enemy") || target.CompareTag("Selectable"))
            {
                if (highlight != target)
                {
                    ClearHighlight();

                    highlight = target;

                    var outline = highlight.GetComponent<Outline>();
                    if (outline == null)
                    {
                        outline = highlight.gameObject.AddComponent<Outline>();
                        outline.OutlineMode = Outline.Mode.OutlineVisible;
                        outline.OutlineColor = Color.cyan;
                        outline.OutlineWidth = 2f;
                    }
                    else
                    {
                        outline.enabled = true;
                    }
                }
            }
            else
            {
                ClearHighlight();
            }
        }
        else
        {
            ClearHighlight();
        }
    }

    private void ClearHighlight()
    {
        if (highlight != null)
        {
            Outline outline = highlight.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
            }
            highlight = null;
        }
    }
}
