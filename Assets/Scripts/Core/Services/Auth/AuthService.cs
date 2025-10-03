using Assets.Constants;
using Assets.Models;
using Assets.Scripts.Manager;
using Assets.Sockets;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthService : MonoBehaviour
{
    [SerializeField, Tooltip("Username to auth")] TMP_InputField username;
    [SerializeField, Tooltip("Password to auth")] TMP_InputField password;
    private Button ButtonSubmit;

    private GameObject loadingIcon;
    private bool isLoading = false;

    private void Start()
    {
        InstantiateButtonSubmit();

        password.inputType = TMP_InputField.InputType.Password;

        username.text = "diegoom38@hotmail.com";
        password.text = "55241461a@";
        //AuthAsync();
    }

    private void InstantiateButtonSubmit()
    {
        if (transform.Find("btn_login").gameObject.TryGetComponent<Button>(out Button buttonSubmit))
        {
            ButtonSubmit = buttonSubmit;
            ButtonSubmit.onClick.AddListener(AuthAsync);

            // Pega o Loading dentro do botão
            var loadingTransform = ButtonSubmit.transform.Find("Loading");
            if (loadingTransform != null)
            {
                loadingIcon = loadingTransform.gameObject;
                loadingIcon.SetActive(false);
            }
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
            // desabilita botão e ativa loading
            ButtonSubmit.interactable = false;
            if (loadingIcon != null) loadingIcon.SetActive(true);
            isLoading = true;

            AcessoRequisicao acesso = new()
            {
                email = username.text,
                password = password.text
            };

            var response = await SharedWebSocketClient.ConnectAndSend(
                JsonUtility.ToJson(acesso),
                VariablesContants.WS_AUTH
            );

            // encerra loading e reabilita botão
            isLoading = false;
            if (loadingIcon != null) loadingIcon.SetActive(false);
            ButtonSubmit.interactable = true;

            if (response != null)
            {
                var retornoAcesso = JsonUtility.FromJson<RetornoAcao<AcessoResposta>>(response);
                if (!retornoAcesso.isFailed)
                {
                    Acesso.LoggedUser = retornoAcesso.result;

                    Thread.Sleep(1000);

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
        }
        else
        {
            WarningUIManager.ExibirAviso("Por favor, preencha o formulário corretamente!", "Erro ao autenticar");
        }
    }
}
