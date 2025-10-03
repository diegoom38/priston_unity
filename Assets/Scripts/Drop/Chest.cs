using Assets.Constants;
using Assets.Scripts.Core.Services.Inventory;
using Assets.Sockets;
using Assets.Utils.Inventory;
using Assets.ViewModels;
using Assets.ViewModels.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            var text = newItem.transform.Find("name_item").GetComponent<TMPro.TextMeshProUGUI>();
            text.text = drop.nome;

            if (newItem.TryGetComponent<Button>(out var button))
            {
                GameObject currentItem = newItem;
                Item capturedDrop = drop;

                button.onClick.RemoveAllListeners(); // ?? limpa listeners antigos

                button.onClick.AddListener(() =>
                {
                    dropItems.Remove(capturedDrop);
                    Destroy(currentItem);

                    if (InventoryUtils.Inventario.itensInventario == null)
                        InventoryUtils.Inventario.itensInventario = new List<InventarioItemViewModel>();

                    int nextIndex;

                    if (InventoryUtils.Inventario.itensInventario.Count > 0)
                    {
                        var indices = InventoryUtils.Inventario.itensInventario
                            .Select(i => i.indice)
                            .OrderBy(i => i)
                            .ToList();

                        int min = indices.First();
                        int max = indices.Last();

                        var faltando = Enumerable.Range(min, max - min + 1)
                            .Except(indices)
                            .FirstOrDefault();

                        nextIndex = faltando != 0
                            ? faltando
                            : max + 1;
                    }
                    else
                    {
                        nextIndex = 0;
                    }

                    var novoItem = new InventarioItemViewModel
                    {
                        itemId = capturedDrop.id,
                        quantidade = 1,
                        itemDetalhes = capturedDrop,
                        indice = nextIndex,
                        id = Guid.NewGuid().ToString()
                    };

                    try
                    {
                        InventoryUtils.Inventario.itensInventario.Add(novoItem);
                        Task.Run(() => SharedWebSocketClient.ConnectAndSend(
                            InventoryUtils.Inventario.ToJson(),
                            VariablesContants.WS_INVENTORY)
                        )
                        .ContinueWith(task =>
                        {
                            if (task.IsFaulted)
                            {
                                Debug.LogWarning($"Erro ao salvar o inventário: {task.Exception?.GetBaseException().Message}");
                            }
                            else
                            {
                                InventoryUtils.NotifyInventoryChanged();
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Erro ao salvar o inventário: {ex.Message}");
                    }
                });
            }
        }

        panelDrops.SetActive(true);
    }
}