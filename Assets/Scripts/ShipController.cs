using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody2D))]
public class ShipController : BoundedEntity {

    private float m_turnInput;
    private float m_forwardInput;

    [Header("Sound Effects")]
    [SerializeField] private SoundEffectHandler m_thrustSoundFX;
    [SerializeField] private SoundEffectHandler m_boostSoundFX;
    [SerializeField] private SoundEffectHandler m_fireSoundFX;
    [SerializeField] private SoundEffectHandler m_deathSoundFX;
    [SerializeField] private SoundEffectHandler m_hitSoundFX;

    [Header("Move")]
    [SerializeField] private float m_moveSpeed;
    [SerializeField] private float m_maxSpeed;
    [SerializeField] private float m_turnSpeed;
    [SerializeField] private float m_maxBoostingSpeed;
    [SerializeField] private float m_boostSpeed;
    [SerializeField] private float m_stoppingPower;
    [SerializeField] private bool m_isBoosting;
    [SerializeField] private VisualEffect m_leftThruster;
    [SerializeField] private VisualEffect m_rightThruster;

    [Header("Firing")]
    [SerializeField] private float m_fireDeley;
    [SerializeField] private GameObject m_bulletPrefarb;

    [Header("Other Referecnces")]
    [SerializeField] private float m_invulnerableTime;
    [SerializeField] private GameObject m_deathVFXPrefarb;
    [SerializeField] private AnimationCurve m_fullscreenEase;
    [SerializeField] private Material m_fullscreenEffectMat;
    [SerializeField] private GameObject m_shield;
    [SerializeField] private bool m_isDead;

    Coroutine dmgRoutine;
    private bool m_isInvulnerable;

    private bool m_isFiring;
    private float m_fireTimer = 0f;

    protected override void OnEnable() {
        GameEvents.Instance.OnRetry += OnRetry;
        GameEvents.Instance.OnGameOver += OnGameOver;
        base.OnEnable();
    }

    protected override void OnDisable() {
        GameEvents.Instance.OnRetry -= OnRetry;
        GameEvents.Instance.OnGameOver -= OnGameOver;
        base.OnDisable();
    }

    private void OnGameOver() {
        StopAllCoroutines();
        m_shield.SetActive(false);
        m_isInvulnerable = false;
    }

    private void OnRetry() {
        SetPlayerAsDead();
        StartCoroutine(RespawnPlayer(false));
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (m_isDead || m_isInvulnerable)
            return;
        if (collision.gameObject.TryGetComponent(out AsteroidController asteroid)) {
            m_hitSoundFX.Play();
            LoseHealth(asteroid.MaxHealth);
            if (dmgRoutine != null)
                StopCoroutine(dmgRoutine);

            dmgRoutine = StartCoroutine(HitRoutine());
        }
    }

    protected override void OnDie() {
        if (dmgRoutine != null) {
            StopCoroutine(dmgRoutine);
            dmgRoutine = null;
        }
        EnableThrusters(false, false);
        m_thrustSoundFX.StopImmediate();
        m_boostSoundFX.StopImmediate();
        m_turnInput = 0f;
        m_forwardInput = 0f;
        m_isBoosting = false;
        m_isFiring = false;
        m_fullscreenEffectMat.SetFloat("_Amount", 0f);
        m_deathSoundFX.Play();
        SpawnVFX(m_deathVFXPrefarb);
        SetPlayerAsDead();
        StartCoroutine(RespawnPlayer(true));
        GameEvents.Instance.PlayerDies();
    }

    protected void SetPlayerAsDead() {
        m_isDead = true;
        m_spriteRenderer.enabled = false;
        m_collider.enabled = false;
        m_rigidbody.simulated = false;
    }

    private IEnumerator RespawnPlayer(bool hasShields = true) {
        if (dmgRoutine != null) {
            StopCoroutine(dmgRoutine);
            dmgRoutine = null;
        }
        m_fullscreenEffectMat.SetFloat("_Amount", 0f);

        yield return new WaitForSeconds(.75f);

        m_rigidbody.position = Vector2.zero;
        m_rigidbody.rotation = 0f;

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        m_rigidbody.linearVelocity = Vector2.zero;
        m_rigidbody.angularVelocity = 0f;

        m_spriteRenderer.enabled = true;
        ResetHealth();
        if (hasShields) {
            m_shield.SetActive(true);
            m_isInvulnerable = true;
        }
        m_collider.enabled = true;
        m_rigidbody.simulated = true;
        m_isDead = false;

        yield return new WaitForSeconds(m_invulnerableTime);
        m_shield.SetActive(false);
        m_isInvulnerable = false;
    }

    void EnableThrusters(bool left, bool right) {
        m_leftThruster.enabled = left;
        m_rightThruster.enabled = right;
    }

    void UpdateThrusters() {
        bool anyThrust = m_forwardInput > 0 || m_isBoosting;
        EnableThrusters(anyThrust, anyThrust);

        if (anyThrust) {
            if (m_isBoosting)
                m_boostSoundFX.FadeIn();
            else
                m_thrustSoundFX.FadeIn();
        } else {
            m_boostSoundFX.FadeOut();
            m_thrustSoundFX.FadeOut();
        }
    }

    void OnBoost(InputValue value) {
        if (m_isDead)
            return;
        m_isBoosting = value.Get<float>() > 0f;
        m_leftThruster.SetBool("isBoosting", m_isBoosting);
        m_rightThruster.SetBool("isBoosting", m_isBoosting);
        UpdateThrusters();
    }

    void OnMove(InputValue value) {
        if (m_isDead)
            return;
        Vector2 dir = value.Get<Vector2>();
        m_turnInput = dir.x;
        m_forwardInput = dir.y;
        UpdateThrusters();
    }

    void OnAttack(InputValue value) {
        m_isFiring = value.isPressed;
    }

    private void TrySpawnBullet() {
        GameObject bullet = Instantiate(m_bulletPrefarb);
        bullet.transform.position = transform.position + transform.up * 3f;
        bullet.transform.up = transform.up;
        m_fireSoundFX.Play();
        // reset cooldown
        m_fireTimer = m_fireDeley;
    }

    private void FixedUpdate() {
        if (m_isDead)
            return;

        m_rigidbody.rotation -= (m_turnInput * (m_turnSpeed * 100f)) * Time.fixedDeltaTime;
        if (m_isBoosting) {
            m_rigidbody.AddRelativeForceY(m_boostSpeed * 100f * Time.fixedDeltaTime);
        }
        if (m_forwardInput > 0) {
            m_rigidbody.AddRelativeForceY((m_forwardInput * (m_moveSpeed * 100f)) * Time.fixedDeltaTime);
        } else if (m_forwardInput < 0) {
            m_rigidbody.linearVelocity = Vector2.Lerp(m_rigidbody.linearVelocity, Vector2.zero, m_stoppingPower * Time.fixedDeltaTime);
        }

        if (m_isBoosting && m_rigidbody.linearVelocity.magnitude > m_maxBoostingSpeed) {
            m_rigidbody.linearVelocity = m_rigidbody.linearVelocity.normalized * m_maxBoostingSpeed;
        } else if (!m_isBoosting && m_rigidbody.linearVelocity.magnitude > m_maxSpeed) {
            m_rigidbody.linearVelocity = m_rigidbody.linearVelocity.normalized * m_maxSpeed;
        }

    }

    private void Update() {
        if (m_fireTimer > 0f)
            m_fireTimer -= Time.deltaTime;

        if (m_isFiring && m_fireTimer <= 0f)
            TrySpawnBullet();
    }

    private IEnumerator HitRoutine() {
        float amount = 0f;
        while (amount < 1f) {
            yield return new WaitForSeconds(.05f);
            amount += .1f;
            m_fullscreenEffectMat.SetFloat("_Amount", m_fullscreenEase.Evaluate(amount));
        }

        while (amount > 0f) {
            yield return new WaitForSeconds(.05f);
            amount -= .1f;
            m_fullscreenEffectMat.SetFloat("_Amount", m_fullscreenEase.Evaluate(amount));
        }
    }
}