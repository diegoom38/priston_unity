using Assets.ViewModels.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Classe que implementa comportamento de arrastar e soltar (drag and drop) para elementos UI
/// </summary>
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Tooltip("Referência ao componente RawImage que será arrastado")]
    public RawImage image;

    [Tooltip("Armazena o parente original do objeto antes do arrasto")]
    [HideInInspector]
    public Transform parentAfterDrag;

    [Tooltip("Item armazenado no inventario")]
    public InventarioItemViewModel item;

    [Tooltip("Item equipado")]
    public CommonItemViewModel itemEquipado;

    /// <summary>
    /// Método chamado quando o arrasto é iniciado
    /// </summary>
    /// <param name="eventData">Dados do evento de ponteiro</param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Armazena o parente atual
        parentAfterDrag = transform.parent;

        // Move o objeto para o canvas principal para garantir renderização acima de outros elementos
        transform.SetParent(GameObject.Find("HandleScene/Canvas").transform);

        // Garante que o objeto seja renderizado por cima de outros
        transform.SetAsLastSibling();

        // Desativa a detecção de raycast para evitar conflitos durante o arrasto
        image.raycastTarget = false;
    }

    /// <summary>
    /// Método chamado continuamente durante o arrasto
    /// </summary>
    /// <param name="eventData">Dados do evento de ponteiro</param>
    public void OnDrag(PointerEventData eventData)
    {
        // Atualiza a posição do objeto para seguir o cursor do mouse
        transform.position = Input.mousePosition;
    }

    /// <summary>
    /// Método chamado quando o arrasto é finalizado
    /// </summary>
    /// <param name="eventData">Dados do evento de ponteiro</param>
    public void OnEndDrag(PointerEventData eventData)
    {
        // Restaura o parente original
        transform.SetParent(parentAfterDrag);

        // Reativa a detecção de raycast para permitir interações futuras
        image.raycastTarget = true;
    }
}