using Assets.Models;
using Assets.Scripts.Manager;
using System.Net.Http;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthService : MonoBehaviour
{
    [SerializeField, Tooltip("Username to auth")] TMP_InputField username;
    [SerializeField, Tooltip("Password to auth")] TMP_InputField password;
    private Button ButtonSubmit;

    private void Start()
    {
        InstantiateButtonSubmit();

        password.inputType = TMP_InputField.InputType.Password;

        username.text = "diegoom38@hotmail.com";
        password.text = "55241461a@";
        AuthAsync();
    }

    private void InstantiateButtonSubmit()
    {
        if (transform.Find("btn_login").gameObject.TryGetComponent<Button>(out Button buttonSubmit))
        {
            ButtonSubmit = buttonSubmit;
            ButtonSubmit.onClick.AddListener(AuthAsync);
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (username.isFocused)
            {
                password.Select();
            }
            else if (password.isFocused)
            {
                username.Select();
            }
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            AuthAsync();
    }

    public async void AuthAsync()
    {
        bool formValid = !string.IsNullOrEmpty(username.text) && !string.IsNullOrEmpty(password.text);

        if (formValid)
        {
            AcessoRequisicao acesso = new()
            {
                email = username.text,
                password = password.text
            };

            var retornoAcesso = await HttpService.SendRequestAsync<RetornoAcao<AcessoResposta>>(
                method: HttpMethod.Post,
                url: "https://pristontalewebapi.onrender.com/api/v1/conta/autenticar",
                content: acesso
            );

            if (!retornoAcesso.isFailed)
            {
                Acesso.LoggedUser = retornoAcesso.result;
                LoadingManager
                    .GetSceneLoader()
                    .LoadSceneWithLoadingScreen(
                        "CharacterSelection"
                    );
            }
            else
            {
                foreach (var item in retornoAcesso.errors)
                {
                    WarningUIManager.ExibirAviso($"{item.message}", "Erro ao autenticar!");
                }
            }
        }
        else
        {
            WarningUIManager.ExibirAviso("Por favor, preencha o formulário corretamente!", "Erro ao autenticar");
        }
    }
}