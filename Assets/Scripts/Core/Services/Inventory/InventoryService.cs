using Assets.Models;
using Assets.ViewModels.Inventory;
using System.Net.Http;
using System.Threading.Tasks;
namespace Assets.Scripts.Core.Services.Inventory
{
    public static class InventoryService
    {
        public const string URL_API = "https://pristontalewebapi.onrender.com/api/v1";
        public async static Task<InventarioViewModel> CreateInventory(Inventario inventario)
        {
            var retornoAcesso = await HttpService.SendRequestAsync<RetornoAcao<InventarioViewModel>>(
                method: HttpMethod.Post,
                url: $"{URL_API}/inventario",
                inventario
            );

            return retornoAcesso?.result;
        }

        public async static Task<InventarioViewModel> GetInventoryByCharacterId()
        {
            var retornoAcesso = await HttpService.SendRequestAsync<RetornoAcao<InventarioViewModel>>(
                method: HttpMethod.Get,
                url: $"{URL_API}/inventario/by-personagem/{PersonagemUtils.LoggedChar.id}"
            );

            return retornoAcesso?.result;
        }

        public async static Task<InventarioViewModel> EditInventory(Inventario inventario)
        {
            var retornoAcesso = await HttpService.SendRequestAsync<RetornoAcao<InventarioViewModel>>(
                method: HttpMethod.Put,
                url: $"{URL_API}/inventario/{inventario.id}",
                inventario
            );

            return retornoAcesso?.result;
        }
    }
}
