﻿using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator _animator;
    private CharacterController _characterController;
    public float Speed = 5.0f;
    public float bendSpeed = 2.5f;
    public float RotationSpeed = 240.0f;

    public float JumpForce = 10.0f;

    private readonly float gravity = 20.0f;

    private float _height, _bendHeight;

    private Vector3 _moveDir = Vector3.zero;

    public event EventHandler<PlayerControllerEventArgs> Moving; 
    public event EventHandler Moved;

    // Use this for initialization
    void Start()
    {
        _animator = GetComponent<Animator>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _characterController = GetComponent<CharacterController>();
        _height = _characterController.height;
        _bendHeight = _characterController.height * 0.8f;
    }

    // Update is called once per frame
    void Update()
    {
        // Avisa de que se va a mover
        PlayerControllerEventArgs e = OnMoving();

        // Si alguien le ha dicho que cancele el movimiento, para
        if (e.Cancel) return;

        // Get Input for axis
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Calculate the forward vector
        Vector3 camForward_Dir = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 move = v * camForward_Dir + h * Camera.main.transform.right;

        if (move.magnitude > 1f) move.Normalize();

        // Calculate the rotation for the player
        move = transform.InverseTransformDirection(move);

        // Get Euler angles
        float turnAmount = Mathf.Atan2(move.x, move.z);

        transform.Rotate(0, turnAmount *  RotationSpeed * Time.deltaTime, 0);

        if (_characterController.isGrounded)
        {
            _animator.SetBool("isMoving", move.magnitude> 0);
            _moveDir = transform.forward * move.magnitude;

            if (Input.GetButton("Fire1")) { //left control - va lento
                _moveDir *= bendSpeed;
                _animator.SetTrigger("isSneakingIn");
                _characterController.height = _bendHeight;
            }
            else {
                _moveDir *= Speed;
                _animator.SetTrigger("isSneakingOut");
                _characterController.height = _height;
            }

            _moveDir.y = 0;
            if (Input.GetButtonDown("Jump"))
            {
                _moveDir.y = JumpForce;
                _animator.SetTrigger("isJumping");
            }
        }

        _moveDir.y -= gravity * Time.deltaTime;

        _characterController.Move(_moveDir * Time.deltaTime);

        OnMoved();
    }

    protected virtual PlayerControllerEventArgs OnMoving()
    {
        PlayerControllerEventArgs e = new PlayerControllerEventArgs();
        Moving?.Invoke(this, e);
        return e;
    }

    protected virtual void OnMoved()
    {
        Moved?.Invoke(this, EventArgs.Empty);
    }
}

public class PlayerControllerEventArgs : EventArgs
{
    public bool Cancel { get; set; }
}