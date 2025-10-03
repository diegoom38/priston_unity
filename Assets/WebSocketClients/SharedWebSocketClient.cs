using Assets.Models;
using Assets.ViewModels.Inventory;
using System;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using WebSocketSharp;

namespace Assets.Sockets
{
    public static class SharedWebSocketClient
    {
        private static WebSocket websocket;

        // Reusable and public property for other scripts to use
        public static WebSocket Instance => websocket;

        /// <summary>
        /// Connects to a WebSocket, sends a message, and handles authorization with a token.
        /// </summary>
        public static Task<string> ConnectAndSend(string json, string url)
        {
            var tcs = new TaskCompletionSource<string>();

            // Remove o if (websocket == null || !websocket.IsAlive)
            // para garantir uma nova conexão a cada chamada.
            var websocket = new WebSocket(url);

            if (!string.IsNullOrEmpty(Acesso.LoggedUser?.token))
            {
                websocket.SetCookie(new WebSocketSharp.Net.Cookie("Authorization", "Bearer " + Acesso.LoggedUser.token));
            }

            var messageBuilder = new StringBuilder();

            websocket.OnMessage += (sender, e) =>
            {
                messageBuilder.Append(e.Data);
                string fullMessage = messageBuilder.ToString();
                tcs.TrySetResult(fullMessage);
                // Fecha a conexão após a resposta para evitar bugs de reuso
                ((WebSocket)sender).CloseAsync();
            };

            websocket.OnError += (sender, e) =>
            {
                tcs.TrySetException(e.Exception ?? new Exception(e.Message));
                ((WebSocket)sender).CloseAsync();
            };

            websocket.OnOpen += (sender, e) =>
            {
                websocket.Send(json);
            };

            websocket.ConnectAsync();

            return tcs.Task;
        }

        /// <summary>
        /// Closes the WebSocket connection manually.
        /// </summary>
        public static void Close()
        {
            if (websocket != null && websocket.IsAlive)
            {
                websocket.CloseAsync();
                Debug.Log("WebSocket fechado manualmente");
            }
        }
    }
}