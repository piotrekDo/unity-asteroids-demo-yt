using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class ShipController : BoundedEntity {
    private float m_turnInput;
    private float m_forwardInput;

    [SerializeField] private float m_maxSpeed;
    [SerializeField] private float m_turnSpeed;
    [SerializeField] private float m_moveSpeed;
    [SerializeField] private float m_stoppingPower;

    [SerializeField] private GameObject m_bulletPrefarb;
    [SerializeField] private float m_fireDeley;
    [SerializeField] private float m_fireCount;

    private bool m_isFiring;
    private float m_fireTimer = 0f;

    void OnMove(InputValue value) {
        Vector2 moveInputDirection = value.Get<Vector2>();
        m_turnInput = moveInputDirection.x;
        m_forwardInput = moveInputDirection.y;
    }

    void OnAttack(InputValue value) {
        Debug.Log(value.isPressed);
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
        m_rigidbody.rotation -= (m_turnInput * (m_turnSpeed * 100f)) * Time.deltaTime;

        if (m_forwardInput > 0) {
            m_rigidbody.AddRelativeForceY((m_forwardInput * (m_moveSpeed * 100f)) * Time.deltaTime);
        } else if (m_forwardInput < 0) {
            m_rigidbody.linearVelocity = Vector2.Lerp(m_rigidbody.linearVelocity, Vector2.zero, m_stoppingPower * Time.deltaTime);
        }

        if (m_rigidbody.linearVelocity.magnitude > m_maxSpeed) {
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