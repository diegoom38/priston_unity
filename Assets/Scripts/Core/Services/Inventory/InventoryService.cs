using Assets.Constants;
using Assets.Models;
using Assets.ViewModels.Inventory;
using System.Net.Http;
using System.Threading.Tasks;
namespace Assets.Scripts.Core.Services.Inventory
{
    public static class InventoryService
    {
        public async static Task<InventarioViewModel> CreateInventory(Inventario inventario)
        {
            var retornoAcesso = await HttpService.SendRequestAsync<RetornoAcao<InventarioViewModel>>(
                method: HttpMethod.Post,
                url: $"{VariablesContants.BASE_URL}/inventario",
                inventario
            );

            return retornoAcesso?.result;
        }

        public async static Task<InventarioViewModel> GetInventoryByCharacterId(string id)
        {
            var retornoAcesso = await HttpService.SendRequestAsync<RetornoAcao<InventarioViewModel>>(
                method: HttpMethod.Get,
                url: $"{VariablesContants.BASE_URL}/inventario/by-personagem/{id}"
            );

            return retornoAcesso?.result;
        }
    }
}
