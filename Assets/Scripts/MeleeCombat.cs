using Assets.Enums;
using Assets.Scripts;
using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class MeleeCombat : MonoBehaviour
{
    private Animator animator;
    private Slider sliderRes;
    private Slider sliderMana;
    private Slider sliderHp;
    private float resRegenRate = 0.05f; // Taxa de regeneração por segundo
    private float maxResValue = 1.0f;  // Valor máximo do sliderRes

    private void Start()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        InitializeSliders();
    }

    void Update()
    {
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
        //animator.SetInteger("transitionCombat", (int)CombatMeleeList.Idle);
        //animator.SetInteger("transitionWeaponCombat", (int)CombatMeleeWeaponList.Idle);

        if ((sliderRes?.value ?? 1) > 0)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
                BasicAttack(TypeCombat.Melee, "transitionCombat");

            if (Input.GetKeyUp(KeyCode.Alpha1))
                BasicAttack(TypeCombat.WeaponMelee, "transitionWeaponCombat", 1, 2);
        }
    }

    void BasicAttack(
        TypeCombat type, 
        string integerCombat, 
        int attack = 0,
        int layerIndex = 1
    )
    {
        if (animator.GetBool("inCombat") || animator.GetInteger("transitionMovement") > 0) return;

        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 direction = transform.forward;

        // Inicia a coroutine para lidar com a animação e a lógica de ataque
        StartCoroutine(HandleAttack(origin, direction, type, integerCombat, attack, layerIndex));
    }

    private IEnumerator HandleAttack(
        Vector3 origin, 
        Vector3 direction, 
        TypeCombat type, 
        string integerCombat, 
        int attack = 0,
        int layerIndex = 1)
    {
        // Realiza o ataque se houver um inimigo na linha de ataque
        if (Physics.Raycast(origin, direction, out RaycastHit hit, 2f))
        {
            GameObject target = hit.collider.gameObject;

            if (target.CompareTag("Enemy"))
            {
                System.Random randNum = new();
                int randomBasicAttack = attack > 0 ? attack : randNum.Next(1, 4);

                // Configura a animação de ataque
                animator.SetInteger(integerCombat, randomBasicAttack);
                animator.SetInteger("typeCombat", (int)type);
                animator.SetBool("inCombat", true);

                // Espera um frame para garantir que a animação foi atualizada
                yield return null;

                // Obtém o estado atual da Layer 1 (Ataque Melee)
                AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(layerIndex);


                // Define o momento em que o dano será aplicado (50% da animação)
                float damageTime = 0.5f;

                // Espera até que a animação atinja o momento do dano
                while (currentState.normalizedTime < damageTime)
                {
                    yield return null;
                    currentState = animator.GetCurrentAnimatorStateInfo(layerIndex);
                }

                // Aplica o dano se a animação não for "FightIdle" ou "SwordIdle"
                if (!currentState.IsTag("Idle") && !currentState.IsTag("Default"))
                {
                    sliderRes.value = Mathf.Clamp(sliderRes.value - 0.05f, 0, sliderRes.maxValue);
                    target.GetComponent<Enemy>().life -= 40;
                }

                // Espera o restante da animação, se necessário
                float timeRemaining = (1 - currentState.normalizedTime) * currentState.length;
                animator.SetInteger(integerCombat, 0);
                yield return new WaitForSeconds(timeRemaining);

            }
        }

        // Volta ao estado de idle após o ataque
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
