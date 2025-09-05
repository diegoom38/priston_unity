using Assets.Constants;
using Assets.Models;
using Scripts.Manager;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Assets.Scripts.Manager
{
    public class CharacterCreationRandomize
    {
        public ColorManager HairColor { get; set; }
        public ColorManager SkinColor { get; set; }
        public ColorManager EyeColor { get; set; }
        public ColorManager LipColor { get; set; }
    }

    public static class AccountCharacters
    {
        public async static Task<List<Personagem>> GetCharacters()
        {
            var retornoAcesso = await HttpService.SendRequestAsync<RetornoAcao<List<Personagem>>>(
                method: HttpMethod.Get,
                url: $"{VariablesContants.BASE_URL}/personagens/account/{Acesso.LoggedUser.user.id}"
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
                url: $"{VariablesContants.BASE_URL}/personagens/{personagem.id}",
                personagem
            );

            if(!retornoAcesso.isFailed)
                PersonagemUtils.LoggedChar = retornoAcesso.result;

            return retornoAcesso.result;
        }

        public async static Task<RetornoAcao<Personagem>> CreateCharacter(Personagem personagem)
        {
            personagem.contaId = Acesso.LoggedUser.user.id;
            var retornoAcesso = await HttpService.SendRequestAsync<RetornoAcao<Personagem>>(
                method: HttpMethod.Post,
                url: $"{VariablesContants.BASE_URL}/personagens",
                personagem
            );

            return retornoAcesso;
        }
    }
}
