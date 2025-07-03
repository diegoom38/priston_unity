using Assets.Utils.Inventory;
using Assets.ViewModels;
using Assets.ViewModels.Inventory;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    private GameObject panelDrops;
    [HideInInspector] public List<Item> dropItems = new List<Item>();

    // Detecta clique com o botão esquerdo do mouse sobre o collider do GameObject
    private void OnMouseDown()
    {
        OpenChest();
    }

    private void Update()
    {
        if (!dropItems.Any())
        {
            panelDrops.gameObject.SetActive(false);
            Destroy(gameObject);            
        }
    }

    private void OpenChest()
    {
        panelDrops = GameObject.Find("HandleScene/Canvas/panel_right/panel_drops");

        Transform gridItems = panelDrops.transform.Find("grid_items/item_grid_scroll");
        Transform itemExample = gridItems.Find("item_example");

        if (itemExample == null) return;

        foreach (var drop in dropItems)
        {
            GameObject newItem = Instantiate(itemExample.gameObject, gridItems);
            newItem.SetActive(true);

            var rawImage = newItem.transform.Find("image_item").GetComponent<RawImage>();
            Texture2D texture = Resources.Load<Texture2D>($"ItemsIcons/{drop.recursoNomeItem}");
            if (texture != null)
                rawImage.texture = texture;
            else
                Debug.LogWarning($"Ícone não encontrado: ItemsIcons/{drop.recursoNomeItem}");

            var text = newItem.transform.Find("name_item").GetComponent<TMPro.TextMeshProUGUI>();
            text.text = drop.nome;

            if (newItem.TryGetComponent<Button>(out var button))
            {
                GameObject currentItem = newItem;
                Item capturedDrop = drop;

                button.onClick.AddListener(() =>
                {
                    // Adiciona ao inventário
                    //InventoryUtils.items.Add(new Item
                    //{
                    //    indice = capturedDrop.indice,
                    //    equipavel = capturedDrop.equipavel,
                    //    recursoNomeItem = capturedDrop.ResourceNameItem,
                    //    ResourceNamePrefab = capturedDrop.ResourceNamePrefab,
                    //    EquipmentItemType = capturedDrop.EquipmentItemType
                    //});

                    // Remove da lista de drops
                    dropItems.Remove(capturedDrop);

                    // Exclui da interface
                    Destroy(currentItem);
                });
            }
        }

        panelDrops.SetActive(true);
    }
}