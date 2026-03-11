using Assets.Constants;
using Assets.Models;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketSharp;

namespace Assets.Sockets
{
    public static class SharedWebSocketClient
    {
        private static WebSocket websocket;
        private static bool isConnecting = false;

        // Mapa: requestId -> Task awaiter
        private static ConcurrentDictionary<string, TaskCompletionSource<string>> pendingRequests
            = new ConcurrentDictionary<string, TaskCompletionSource<string>>();

        private static readonly object locker = new object();

        public static async Task<string> SendRequest(object payload, string url)
        {
            await EnsureConnected(VariablesContants.WS_SHARED);

            string json = JsonUtility.ToJson(payload);
            Debug.LogWarning("[WS] DATA: " + json);
            Debug.LogWarning("[WS] URL: " + url);

            // cria id único para resposta
            string requestId = Guid.NewGuid().ToString();

            // cria objeto com id + payload
            var wrapper = new WebSocketEnvelope()
            {
                path = url,
                requestId = requestId,
                data = json
            };

            string message = JsonUtility.ToJson(wrapper);

            var tcs = new TaskCompletionSource<string>();
            pendingRequests[requestId] = tcs;

            Debug.Log($"[WS] SEND {url} / {requestId}");

            websocket.Send(message);

            return await tcs.Task;
        }


        private static async Task EnsureConnected(string url)
        {
            if (websocket != null && websocket.IsAlive)
                return;

            lock (locker)
            {
                if (isConnecting)
                    return;

                isConnecting = true;
            }

            websocket = new WebSocket(url);

            if (!string.IsNullOrEmpty(Acesso.LoggedUser?.token))
            {
                websocket.SetCookie(new WebSocketSharp.Net.Cookie("Authorization", "Bearer " + Acesso.LoggedUser.token));
            }

            websocket.OnOpen += (s, e) =>
            {
                Debug.LogWarning("WebSocket CONNECTED → " + url);
            };

            websocket.OnMessage += (s, e) =>
            {
                Debug.LogWarning("[WS] MESSAGE RECEIVED: " + e.Data);

                try
                {
                    var envelope = JsonUtility.FromJson<WebSocketEnvelope>(e.Data);

                    if (pendingRequests.TryRemove(envelope.requestId, out var tcs))
                    {
                        tcs.TrySetResult(envelope.data);
                    }
                    else
                    {
                        Debug.LogError("[WS] RequestId não encontrado: " + envelope.requestId);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Erro parseando resposta WS: " + ex);
                }
            };

            websocket.OnError += (s, e) =>
            {
                Debug.LogError("[WS] ERROR: " + e.Message);
            };

            websocket.OnClose += (s, e) =>
            {
                Debug.LogWarning("[WS] CLOSED");
            };

            websocket.ConnectAsync();

            // espera até conectar
            while (!websocket.IsAlive)
                await Task.Delay(10);

            lock (locker)
            {
                isConnecting = false;
            }
        }
    }

    [Serializable]
    public class WebSocketEnvelope
    {
        public string requestId;
        public string data;
        public string path;
    }
}
