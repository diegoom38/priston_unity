
using Assets.Models;
using Assets.ViewModels.Inventory;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Assets.Scripts.Core.Services.Inventory
{
    public static class ItemService
    {
        public const string URL_API = "https://pristontalewebapi.onrender.com/api/v1";
        public async static Task<List<Item>> GetItems(List<int> ids)
        {
            string query = string.Join("&", ids.Select(id => $"ids={id}"));
            string url = $"{URL_API}/Item/ids?{query}";

            var retornoAcesso = await HttpService.SendRequestAsync<RetornoAcao<List<Item>>>(
                method: HttpMethod.Get,
                url
            );

            return retornoAcesso?.result;
        }
    }
}
