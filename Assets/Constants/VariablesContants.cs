namespace Assets.Constants
{
    public static class VariablesContants
    {
        private const string HTTP = "https://";
        private const string WS = "wss://";
        //private const string DOMAIN = "localhost:44325";
        private const string DOMAIN = "pristontalewebapi.onrender.com";

        public const string BASE = HTTP + DOMAIN;
        public const string BASE_URL = HTTP + DOMAIN + "/api/v1";
        public const string WS_INVENTORY = "inventario";
        public const string WS_AUTH = "autenticar";
        public const string WS_PERSONAGENS = "personagens";
        public const string WS_SHARED = WS + DOMAIN + "/ws";
    }
}
