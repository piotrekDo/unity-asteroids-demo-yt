using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody2D))]
public class ShipController : BoundedEntity {

    private float m_turnInput;
    private float m_forwardInput;


    [SerializeField] private float m_maxBoostingSpeed;
    [SerializeField] private float m_maxSpeed;
    [SerializeField] private float m_turnSpeed;
    [SerializeField] private float m_moveSpeed;
    [SerializeField] private float m_boostSpeed;
    [SerializeField] private float m_stoppingPower;

    [SerializeField] private GameObject m_bulletPrefarb;
    [SerializeField] private GameObject m_deathVFXPrefarb;
    [SerializeField] private float m_fireDeley;
    [SerializeField] private float m_fireCount;
    [SerializeField] private bool m_isBoosting;

    [SerializeField] private VisualEffect m_leftThruster;
    [SerializeField] private VisualEffect m_rightThruster;

    [SerializeField] private bool m_isDead;

    private bool m_isFiring;
    private float m_fireTimer = 0f;

    protected override void OnEnable() {
        GameEvents.Instance.OnRetry += OnRetry;
        base.OnEnable();
    }

    protected override void OnDisable() {
        GameEvents.Instance.OnRetry -= OnRetry;
        base.OnDisable();
    }


    private void OnRetry() {
        SetPlayerAsDead();
        StartCoroutine(RespawnPlayer());
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.TryGetComponent(out AsteroidController asteroid)) {
            LoseHealth(asteroid.MaxHealth);
        }
    }

    protected override void OnDie() {
        GameEvents.Instance.PlayerDies();
        SpawnVFX(m_deathVFXPrefarb);
        SetPlayerAsDead();
        StartCoroutine(RespawnPlayer());
    }

    protected void SetPlayerAsDead() {
        m_isDead = true;
        m_spriteRenderer.enabled = false;
        m_collider.enabled = false;
        m_rigidbody.simulated = false;
    }

    private IEnumerator RespawnPlayer() {
        yield return new WaitForSeconds(.5f);

        m_rigidbody.position = Vector2.zero;
        m_rigidbody.rotation = 0f;

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        m_rigidbody.linearVelocity = Vector2.zero;
        m_rigidbody.angularVelocity = 0f;

        m_spriteRenderer.enabled = true;
        ResetHealth();

        yield return new WaitForSeconds(.5f);

        m_isDead = false;
        m_rigidbody.simulated = true;

        yield return new WaitForSeconds(2f);
        m_collider.enabled = true;
    }

    void EnableThrusters(bool left, bool right) {
        m_leftThruster.enabled = left;
        m_rightThruster.enabled = right;
    }

    void OnBoost(InputValue value) {
        m_isBoosting = value.Get<float>() > 0f;

        if (m_isBoosting) {
            EnableThrusters(true, true);
        } else if (m_forwardInput == 0) {
            EnableThrusters(false, false);
        }

        m_leftThruster.SetBool("isBoosting", m_isBoosting);
        m_rightThruster.SetBool("isBoosting", m_isBoosting);
    }

    void OnMove(InputValue value) {
        Vector2 moveInputDirection = value.Get<Vector2>();
        m_turnInput = moveInputDirection.x;
        m_forwardInput = moveInputDirection.y;

        if (m_forwardInput > 0) {
            EnableThrusters(true, true);
        } else {
            EnableThrusters(false, false);
        }
    }

    void OnAttack(InputValue value) {
        m_isFiring = value.isPressed;
    }

    private void TrySpawnBullet() {
        GameObject bullet = Instantiate(m_bulletPrefarb);
        bullet.transform.position = transform.position + transform.up * 3f;
        bullet.transform.up = transform.up;

        // reset cooldown
        m_fireTimer = m_fireDeley;
    }

    protected override void LateUpdate() {
        if (m_isDead)
            return;

        m_rigidbody.rotation -= (m_turnInput * (m_turnSpeed * 100f)) * Time.deltaTime;
        if (m_isBoosting) {
            m_rigidbody.AddRelativeForceY(m_boostSpeed * 100f * Time.deltaTime);
        }
        if (m_forwardInput > 0) {
            m_rigidbody.AddRelativeForceY((m_forwardInput * (m_moveSpeed * 100f)) * Time.deltaTime);
        } else if (m_forwardInput < 0) {
            m_rigidbody.linearVelocity = Vector2.Lerp(m_rigidbody.linearVelocity, Vector2.zero, m_stoppingPower * Time.deltaTime);
        }

        if (m_isBoosting && m_rigidbody.linearVelocity.magnitude > m_maxBoostingSpeed) {
            m_rigidbody.linearVelocity = m_rigidbody.linearVelocity.normalized * m_maxBoostingSpeed;
        } else if (!m_isBoosting && m_rigidbody.linearVelocity.magnitude > m_maxSpeed) {
            m_rigidbody.linearVelocity = m_rigidbody.linearVelocity.normalized * m_maxSpeed;
        }

        base.LateUpdate();

        if (m_fireTimer > 0f) {
            m_fireTimer -= Time.deltaTime;
        }

        if (m_isFiring && m_fireTimer <= 0f) {
            TrySpawnBullet();
        }
    }
}