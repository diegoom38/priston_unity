using Assets.Constants;
using Assets.Models;
using Assets.Sockets;
using Assets.ViewModels.Personagem;
using Scripts.Manager;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

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
        public async static Task<List<PersonagemInventarioViewModel>> GetCharacters()
        {
            var response = await SharedWebSocketClient.ConnectAndSend(
                JsonUtility.ToJson(new PersonagemViewModel()
                {
                    accountId = Acesso.LoggedUser.user.id
                }),
                VariablesContants.WS_PERSONAGENS
            );

            var responsePersonagens = JsonUtility.FromJson<RetornoAcao<List<PersonagemInventarioViewModel>>>(response);

            if(!responsePersonagens.isFailed)
            {
                return responsePersonagens.result;
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
