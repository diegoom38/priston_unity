using Assets.Enums;
using Assets.Scripts;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Combat : MonoBehaviourPun
{
    private Animator animator;
    private Slider sliderRes;
    private Slider sliderMana;
    private Slider sliderHp;
    private float resRegenRate = 0.05f; // Taxa de regeneração por segundo
    private float maxResValue = 1.0f;  // Valor máximo do sliderRes
    private GameObject currentTarget; // Armazena o alvo do ataque

    private void Start()
    {
        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }

        InitializeComponents();
    }

    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        InitializeSliders();
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        HandleInput();
        RegenerateRes();
    }

    void InitializeSliders()
    {
        sliderRes = FindSliderByTag("SliderRes");
        sliderMana = FindSliderByTag("SliderMana");
        sliderHp = FindSliderByTag("SliderHp");
    }

    Slider FindSliderByTag(string tag)
    {
        GameObject gameObject = GameObject.FindGameObjectWithTag(tag);
        if (gameObject != null && gameObject.TryGetComponent(out Slider slider))
            return slider;

        return null;
    }

    void HandleInput()
    {
        if ((sliderRes?.value ?? 1) > 0)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
                BasicAttack(TypeCombat.Melee, "transitionCombat");

            if (Input.GetKeyUp(KeyCode.Alpha1))
                BasicAttack(TypeCombat.WeaponMelee, "transitionWeaponCombat", 1);

            if (Input.GetKeyUp(KeyCode.Alpha2))
                BasicAttack(TypeCombat.WeaponMelee, "transitionWeaponCombat", 2);

            if (Input.GetKeyUp(KeyCode.Alpha3))
                BasicAttack(TypeCombat.WeaponMelee, "transitionWeaponCombat", 3);
        }
    }

    void BasicAttack(TypeCombat type, string integerCombat, int attack = 0)
    {
        if (animator.GetBool("inCombat") || animator.GetInteger("transitionMovement") > 0) return;

        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 direction = transform.forward;

        // Verifica se há um inimigo na linha de ataque
        if (Physics.Raycast(origin, direction, out RaycastHit hit, 2f))
        {
            GameObject target = hit.collider.gameObject;
            if (target.CompareTag("Enemy"))
            {
                currentTarget = target; // Armazena o inimigo atual
                int attackIndex = attack > 0 ? attack : Random.Range(1, 4);

                // Configura a animação de ataque
                animator.SetInteger(integerCombat, attackIndex);
                animator.SetInteger("typeCombat", (int)type);
                animator.SetBool("inCombat", true);
            }
        }
    }

    /// <summary>
    /// Método chamado via evento de animação para aplicar dano.
    /// </summary>
    public void ApplyDamage()
    {
        if (currentTarget != null)
        {
            sliderRes.value = Mathf.Clamp(sliderRes.value - 0.05f, 0, sliderRes.maxValue);
            currentTarget.GetComponent<Enemy>().TakeDamage(400f);
        }
    }

    public void ResetCombatState()
    {
        animator.SetInteger("transitionCombat", 0);
        animator.SetInteger("transitionWeaponCombat", 0);
        animator.SetInteger("typeCombat", 0);
        animator.SetBool("inCombat", false);
    }

    void RegenerateRes()
    {
        if (sliderRes != null && sliderRes.value < maxResValue)
        {
            sliderRes.value = Mathf.Min(sliderRes.value + resRegenRate * Time.deltaTime, maxResValue);
        }
    }
}
