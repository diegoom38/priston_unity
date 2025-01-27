using System;

namespace Assets.Models
{
    [Serializable]
    public class AcessoRequisicao
    {
        public string email;
        public string password;
    }

    [Serializable]
    public class AcessoResposta
    {
        public UsuarioResposta user;
        public string token;
    }

    [Serializable]
    public class UsuarioResposta
    {
        public string id;
        public string email;
        public string password;
        public string createdAt;
    }

    public static class Acesso
    {
        public static AcessoResposta LoggedUser { get; set; } = null;
    }
}
