using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JonahController : MonoBehaviour
{
    [SerializeField]
    private float _walkSpeed = 5f;

    [SerializeField] 
    private float _jumpForce = 100f;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private Rigidbody2D _rigidbody;

    [SerializeField]
    private LayerMask _groundLayers;

    [SerializeField]
    private Transform _groundCheck;

    private const float _groundingRadius = .2f;

    private bool _isGrounded = false;
    private JonahControls _controls;

    private void Awake()
    {
        _controls = new JonahControls();

        _controls.Normal.Crouch.started += _ => this.Crouch(true);
        _controls.Normal.Crouch.canceled += _ => this.Crouch(false);

        _controls.Normal.Jump.performed += _ => this.Jump();
    }


    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void FixedUpdate()
    {
        this.Move(_controls.Normal.Move.ReadValue<float>());
        this.CheckForGrounding();
    }

    private void Move(float velocity)
    {
        var magnitude = Mathf.Abs(velocity);
        if (!_animator.GetBool("IsCrouching"))
        {
            _rigidbody.velocity = new Vector2(velocity * _walkSpeed, _rigidbody.velocity.y);
            _animator.SetFloat("Speed", magnitude);
            if (magnitude > 0f)
            {
                var xScale = velocity < 0f ? -1f : 1f;
                this.transform.localScale = new Vector3(xScale, 1f, 1f);
            }
        }
    }

    private void Crouch(bool crouch)
    {
        _animator.SetBool("IsCrouching", crouch);
    }

    private void Jump()
    {
        if (_isGrounded)
        {
            _rigidbody.AddForce(new Vector2(0f, _jumpForce));
        }
    }


    private void CheckForGrounding()
    {
        var collisons = Physics2D.OverlapCircleAll(_groundCheck.position, _groundingRadius, _groundLayers);
        if (collisons.Any(c => c.gameObject != this.gameObject))
        {
            if (!_isGrounded)
            {
                this.Ground(true);
            }
        }
        else
        {
            this.Ground(false);
        }
    }

    private void Ground(bool grounded)
    {
        _animator.SetBool("IsJumping", !grounded);
        _isGrounded = grounded;
    }
}
