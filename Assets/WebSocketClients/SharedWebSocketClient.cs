using Assets.ViewModels.Inventory;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketSharp;

namespace Assets.Sockets
{
    public static class SharedWebSocketClient
    {
        public static Task<string> ConnectAndSend(string json, string url)
        {
            var tcs = new TaskCompletionSource<string>();
            var websocket = new WebSocket(url);
            var messageBuilder = new StringBuilder();

            // Conexão aberta
            websocket.OnOpen += (sender, e) =>
            {
                Debug.Log("Conectado ao WebSocket");
                websocket.Send(json);
                Debug.Log("Mensagem enviada: " + json);
            };

            // Mensagem recebida
            websocket.OnMessage += (sender, e) =>
            {
                messageBuilder.Append(e.Data);

                if (e.IsText) // só finaliza quando o frame for de texto
                {
                    string fullMessage = messageBuilder.ToString();
                    Debug.Log("Mensagem completa recebida: " + fullMessage);

                    // Fecha de forma segura
                    websocket.CloseAsync();
                    tcs.TrySetResult(fullMessage);
                }
            };

            // Erro
            websocket.OnError += (sender, e) =>
            {
                Debug.LogWarning($"Erro no WebSocket: {e.Message}\nException: {e.Exception}");
                tcs.TrySetException(e.Exception ?? new Exception(e.Message));
            };

            // Fechamento
            websocket.OnClose += (sender, e) =>
            {
                Debug.Log($"WebSocket fechado. Código: {e.Code}, Motivo: {e.Reason}");
            };

            // Conecta de forma assíncrona
            websocket.ConnectAsync();
            return tcs.Task;
        }
    }
}
