using Assets.Enums;
using Assets.Scripts.Manager;
using Photon.Pun;
using System;
using UnityEngine;

public class Movement : MonoBehaviourPun
{
    private CharacterController controller;
    private Animator animator;

    public float speed;
    public float gravity;
    public float rotSpeed;
    public float jumpForce; // Adiciona uma variável pública para a força de pulo.

    private float rotation;
    private Vector3 moveDirection;
    private bool isJumping; // Adiciona uma variável para verificar se o personagem está pulando.
    private bool swimming;

    private readonly float swimThresholdY = -1.3f;

    [SerializeField] private AudioSource stepAudioSource;
    [SerializeField] private AudioClip[] stepAudioClips;

    void Start()
    {
        InitializeComponents();

        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;

        HandleMovementProperties();
        HandleMovement();
    }

    private void HandleMovementProperties()
    {
        speed = speed == 0 ? 8 : speed;
        gravity = gravity == 0 ? 50 : gravity;
        rotSpeed = rotSpeed == 0 ? 200 : rotSpeed;
        jumpForce = jumpForce == 0 ? 10 : jumpForce; // Define um valor padrão para a força de pulo.
    }

    private void InitializeComponents()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void HandleMovement()
    {
        if (controller.isGrounded || swimming)
        {
            ResetMoveDirection();

            if (!IsAnyMovementKeyPressed())
            {
                animator.SetInteger("transitionMovement", swimming ? (int)MovementList.SwimmingIdle : (int)MovementList.Idle);
            }

            HandleInput();
        }

        UpdateRotation();
        ApplyGravity();
        MoveCharacter();
    }

    private void ResetMoveDirection()
    {
        moveDirection = Vector3.zero;
        isJumping = false;
    }

    private void HandleInput()
    {
        if (swimming)
        {
            if (Input.GetKey(KeyCode.W))
            {
                moveDirection = Vector3.forward * speed;
                animator.SetInteger("transitionMovement", (int)MovementList.Swimming);
            }

            if (Input.GetKey(KeyCode.Space))
                moveDirection.y = (speed / 4); // Move para cima na velocidade de nado            
            else if (Input.GetKey(KeyCode.LeftControl)) // Permite descer com Ctrl
                moveDirection.y = -(speed / 4); // Move para baixo na velocidade de nado
            else
                moveDirection.y = 0; // Mantém altura se nenhuma tecla vertical for pressionada
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                moveDirection = Vector3.forward * speed;
                animator.SetInteger("transitionMovement", (int)MovementList.Running);
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveDirection = Vector3.forward * (speed * 1.25f);
                animator.SetInteger("transitionMovement", (int)MovementList.Running);
            }

            if (Input.GetKey(KeyCode.S))
            {
                moveDirection = -Vector3.forward * (speed * 0.75f);
                animator.SetInteger("transitionMovement", (int)MovementList.WalkBack);
            }

            if (Input.GetKeyDown(KeyCode.Space) && !isJumping) // Adiciona a lógica de pulo.
            {
                //animator.SetInteger("transitionMovement", (int)MovementList.Jumping);
                moveDirection.y = jumpForce; // Aplica a força de pulo na direção Y.
                isJumping = true; // Define a variável de pulo para evitar múltiplos pulos enquanto no ar.
            }
        }
    }

    private bool IsAnyMovementKeyPressed()
    {
        // Verifica se alguma tecla de movimento foi pressionada (não inclui andar)
        return
            Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.LeftShift) ||
            Input.GetKey(KeyCode.Space);
    }

    private void UpdateRotation()
    {
        rotation += Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, rotation, 0);
    }

    private void ApplyGravity()
    {
        if (!swimming)
            moveDirection.y -= gravity * Time.deltaTime;
    }

    private void MoveCharacter()
    {
        Vector3 globalMoveDirection = transform.TransformDirection(moveDirection);
        controller.Move(globalMoveDirection * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        swimming = other.tag == "Water" && (transform.position.y < swimThresholdY);
    }

    private void OnTriggerExit(Collider other)
    {
        swimming = !(other.tag == "Water") && (transform.position.y > swimThresholdY);
    }

    private void SoundSteps()
    {
        stepAudioSource.PlayOneShot(stepAudioClips[UnityEngine.Random.Range(0, stepAudioClips.Length)]);
    }
}
