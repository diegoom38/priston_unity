using Assets.Enums;
using Assets.Scripts.Manager;
using Photon.Pun;
using System.Linq;
using UnityEngine;

public class OutlineManager : MonoBehaviourPun
{
    private Transform selection;
    private Camera playerCamera;
    private int currentEnemyIndex = -1;
    private Transform[] nearbyEnemies;

    [SerializeField] private float searchRadius = 40f; // raio de detecção

    private void Start()
    {
        SetPlayerCamera();
    }

    private void SetPlayerCamera()
    {
        if (photonView != null && photonView.IsMine)
        {
            if (transform.Find("CameraPlayer").TryGetComponent(out Camera camera))
            {
                playerCamera = camera;
            }
        }
    }

    private void Update()
    {
        if (playerCamera == null)
            SetPlayerCamera();

        if (playerCamera == null)
            return;

        HandleMouseSelection();
        HandleKeyboardInput();
    }

    private void HandleMouseSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Transform target = hit.transform;

                if (target.CompareTag("Enemy") || target.CompareTag("Selectable"))
                {
                    SelectTarget(target);
                }
            }
        }
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CycleEnemiesInRange();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearSelection();
        }
    }

    private void CycleEnemiesInRange()
    {
        // Busca todos os inimigos dentro do raio definido
        nearbyEnemies = Physics.OverlapSphere(transform.position, searchRadius)
            .Where(c => c.CompareTag("Enemy"))
            .Select(c => c.transform)
            .OrderBy(c => Vector3.Distance(transform.position, c.position))
            .ToArray();

        if (nearbyEnemies.Length == 0)
        {
            ClearSelection();
            return;
        }

        // Avança o índice (loop circular)
        currentEnemyIndex++;
        if (currentEnemyIndex >= nearbyEnemies.Length)
            currentEnemyIndex = 0;

        SelectTarget(nearbyEnemies[currentEnemyIndex]);
    }

    private void SelectTarget(Transform target)
    {
        // Só considera inimigos dentro do raio
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > searchRadius)
        {
            ClearSelection();
            return;
        }

        // Remove destaque anterior
        if (selection != null && selection != target)
        {
            Outline oldOutline = selection.GetComponent<Outline>();
            if (oldOutline != null)
                oldOutline.enabled = false;

            // Esconde o painel anterior, se o anterior também era um mob
            if (selection.TryGetComponent<Mob_NPC_CharacterInfoManager>(out var oldMob))
            {
                oldMob.HideSliders();
            }
        }

        selection = target;

        // Aplica outline
        Outline outline = selection.GetComponent<Outline>();
        if (outline == null)
            outline = selection.gameObject.AddComponent<Outline>();

        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = Color.cyan;
        outline.OutlineWidth = 2f;
        outline.enabled = true;

        // Mostra o painel do mob selecionado
        if (selection.TryGetComponent<Mob_NPC_CharacterInfoManager>(out var mobInfo))
        {
            mobInfo.ToggleMobNpcSliders();
        }
    }

    private void ClearSelection()
    {
        if (selection != null)
        {
            // Remove o destaque
            if (selection.TryGetComponent<Outline>(out var outline))
                outline.enabled = false;

            // Esconde o painel do mob selecionado, se existir
            if (selection.TryGetComponent<Mob_NPC_CharacterInfoManager>(out var mobInfo))
                mobInfo.HideSliders();
        }

        selection = null;
        currentEnemyIndex = -1;
        nearbyEnemies = null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Visualiza o raio de busca no editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
#endif
}
