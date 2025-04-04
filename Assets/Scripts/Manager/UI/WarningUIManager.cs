using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Manager
{
    public class WarningUIManager : MonoBehaviour
    {
        public static void ExibirAviso(string mensagem, string titulo)
        {
            // Carrega o prefab do painel de mensagem
            GameObject messagePrefab = Resources.Load<GameObject>("Message");

            if (messagePrefab == null)
            {
                Debug.LogError("Prefab 'Message' não encontrado em Resources.");
                return;
            }

            // Encontra o Canvas ativo na cena
            Canvas canvas = FindObjectOfType<Canvas>();

            // Instancia o painel como filho do Canvas
            GameObject messagePanel = Instantiate(messagePrefab, canvas.transform, false);

            // Define os textos no painel instanciado
            messagePanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = titulo;
            messagePanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = mensagem;

            // Ativa o painel
            messagePanel.SetActive(true);

            // Inicia a corrotina para destruir o painel após 3 segundos
            messagePanel.AddComponent<WarningUIManager>().StartCoroutine(DestroyAfterDelay(messagePanel, 3f));
        }

        private static IEnumerator DestroyAfterDelay(GameObject panel, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(panel); // Remove o painel da cena
        }
    }
}
