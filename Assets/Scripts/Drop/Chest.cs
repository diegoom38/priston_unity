using Assets.ViewModels;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    private GameObject panelDrops;
    [HideInInspector] public List<DropItem> dropItems;

    // Detecta clique com o botão esquerdo do mouse sobre o collider do GameObject
    private void OnMouseDown()
    {
        OpenChest();        
    }

    private void OpenChest()
    {
        panelDrops = GameObject.Find("HandleScene/Canvas/panel_right/panel_drops");

        Transform gridItems = panelDrops.transform.Find("grid_items/item_grid_scroll");
        Transform itemExample = gridItems.Find("item_example");

        if (itemExample == null) return;

        foreach (var drop in dropItems)
        {
            // Instancia o item novo
            GameObject newItem = Instantiate(itemExample.gameObject, gridItems);
            newItem.SetActive(true);

            // Atualiza os dados (imagem/texto)
            var image = newItem.transform.Find("image_item").GetComponent<RawImage>();
            var text = newItem.transform.Find("name_item").GetComponent<TMPro.TextMeshProUGUI>();

            // Atualiza ícone (RawImage usa Texture)
            Texture2D itemTexture = Resources.Load<Texture2D>($"ItemsIcons/{drop.ResourceNameItem}");
            if (itemTexture != null)
            {
                image.texture = itemTexture;
            }
            else
                Debug.LogWarning($"Ícone não encontrado: itemsIcons/{drop.ResourceNameItem}");

            text.text = drop.ResourceNameItem;
        }

        panelDrops.SetActive(true);
    }
}