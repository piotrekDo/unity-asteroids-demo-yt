using System.Drawing;
using UnityEngine;

public class BoundedEntity : MonoBehaviour {

    protected SpriteRenderer m_spriteRenderer;
    protected Collider2D m_collider;
    protected Rigidbody2D m_rigidbody;

    [SerializeField] protected Rect m_bounds;
    [SerializeField] protected int m_health;
    [SerializeField] protected int m_maxHealth = 1;

    public int MaxHealth => m_maxHealth;


    private void Awake() {
        m_spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        m_collider = gameObject.GetComponent<Collider2D>();
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    protected virtual void LateUpdate() {
        if (!m_bounds.Contains(transform.position)) {
            Vector2 position = transform.position;
            if (position.x < m_bounds.xMin) {
                position.x = m_bounds.xMax - (m_bounds.xMin - position.x);
            }

            if (position.x > m_bounds.xMax) {
                position.x = m_bounds.xMin + (position.x - m_bounds.xMax);
            }

            if (position.y < m_bounds.yMin) {
                position.y = m_bounds.yMax - (m_bounds.yMin - position.y);
            }

            if (position.y > m_bounds.yMax) {
                position.y = m_bounds.yMin + (position.y - m_bounds.yMax);
            }

            m_rigidbody.position = position;
        }
    }

    protected virtual void OnEnable() {
        ResetHealth();
    }

    protected virtual void OnDisable() {
    }

    protected void ResetHealth() {
        m_health = m_maxHealth;
    }

    protected void LoseHealth(int value) {
        m_health -= value;
        if (m_health <= 0) {
            OnDie();
        }
    }

    protected virtual void OnDie() {
    }
}
