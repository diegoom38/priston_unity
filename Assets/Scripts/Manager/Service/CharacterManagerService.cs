using Assets.Models;
using Scripts.Manager;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace Assets.Scripts.Manager
{
    public class CharacterCreationRandomize
    {
        public ColorManager HairColor { get; set; }
        public ColorManager SkinColor { get; set; }
        public ColorManager EyeColor { get; set; }
    }

    public static class AccountCharacters
    {
        //public const string URL_API = "https://pristontalewebapi.onrender.com/api/v1";
        public const string URL_API = "https://pristontalewebapi.onrender.com/api/v1";
        public async static Task<List<Personagem>> Characters()
        {
            var retornoAcesso = await HttpService.SendRequestAsync<RetornoAcao<List<Personagem>>>(
                method: HttpMethod.Get,
                url: $"{URL_API}/personagens/account/{Acesso.LoggedUser.user.id}"
            );

            if(!retornoAcesso.isFailed)
            {
                return retornoAcesso.result;
            }

            return null;
        }

        public async static Task<Personagem> EditCharacter(Personagem personagem)
        {
            var retornoAcesso = await HttpService.SendRequestAsync<RetornoAcao<Personagem>>(
                method: HttpMethod.Put,
                url: $"{URL_API}/personagens/{personagem.id}",
                personagem
            );

            if(!retornoAcesso.isFailed)
                PersonagemUtils.LoggedChar = retornoAcesso.result;

            return retornoAcesso.result;
        }

        public async static Task<bool> CreateCharacter(Personagem personagem)
        {
            personagem.contaId = Acesso.LoggedUser.user.id;
            var retornoAcesso = await HttpService.SendRequestAsync<RetornoAcao<Personagem>>(
                method: HttpMethod.Post,
                url: $"{URL_API}/personagens",
                personagem
            );

            return !retornoAcesso.isFailed;
        }
    }
}
