using UnityEngine;
using UnityEngine.EventSystems;

public class OutlineManager : MonoBehaviour
{
    private Transform highlight;
    private readonly Transform selection;
    private RaycastHit raycastHit;

    void Update()
    {
        // Limpar highlight anterior
        if (highlight != null)
        {
            if (highlight.TryGetComponent<Outline>(out var outline)) outline.enabled = false;
            highlight = null;
        }

        // Raycast para detectar objeto sob o mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
