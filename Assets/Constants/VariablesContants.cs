namespace Assets.Constants
{
    public static class VariablesContants
    {
        private const string HTTP = "https://";
        private const string WS = "wss://";
        private const string DOMAIN = "pristontalewebapi.onrender.com";

        public const string BASE = HTTP + DOMAIN;
        public const string BASE_URL = HTTP + DOMAIN + "/api/v1";
        public const string WS_INVENTORY = WS + DOMAIN + "/ws/inventario";
        public const string WS_AUTH = WS + DOMAIN + "/ws/autenticar";
        public const string WS_PERSONAGENS = WS + DOMAIN + "/ws/personagens";
    }
}
