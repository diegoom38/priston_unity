using Assets.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class HttpService
{
    private static readonly HttpClient _httpClient = new HttpClient();

    public static async Task<TResponse> SendRequestAsync<TResponse>(
        HttpMethod method,
        string url,
        object content = default
    )
    {
        using var requestMessage = new HttpRequestMessage(method, url);

        // If content is provided, serialize it to JSON and add it to the request
        if (content != null && (method == HttpMethod.Post || method == HttpMethod.Put))
        {
            var jsonContent = JsonUtility.ToJson(content);
            requestMessage.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        }

        if (Acesso.LoggedUser?.token is not null)
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer", 
                Acesso.LoggedUser.token
            );
        }

        try
        {
            using var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonUtility.FromJson<TResponse>(responseData);
            }

            else
            {
                var responseDataError = await response.Content.ReadAsStringAsync();
                return JsonUtility.FromJson<TResponse>(responseDataError);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error: {ex.Message}");
            return default;
        }
    }
}