using System.Drawing;
using UnityEngine;

public class BoundedEntity : MonoBehaviour {

    protected SpriteRenderer m_spriteRenderer;
    protected Collider2D m_collider;
    protected Rigidbody2D m_rigidbody;

    float left, right, top, bottom;

    [SerializeField] protected int m_health;
    [SerializeField] protected int m_maxHealth = 1;

    public int MaxHealth => m_maxHealth;


    private void Awake() {
        m_spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        m_collider = gameObject.GetComponent<Collider2D>();
        m_rigidbody = GetComponent<Rigidbody2D>();

        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_collider = GetComponent<Collider2D>();
        m_rigidbody = GetComponent<Rigidbody2D>();

        float height = Camera.main.orthographicSize;
        float width = height * Camera.main.aspect;

        top = height;
        bottom = -height;
        right = width;
        left = -width;
    }

    protected virtual void LateUpdate() {
        float xRange = right - left;
        float yRange = top - bottom;

        Vector3 pos = transform.position;

        pos.x = Mathf.Repeat(pos.x - left, xRange) + left;
        pos.y = Mathf.Repeat(pos.y - bottom, yRange) + bottom;

        transform.position = pos;
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

    protected void SpawnVFX(GameObject vfxPrefarb) {
        GameObject vfx = GameObject.Instantiate(vfxPrefarb);
        vfx.transform.position = transform.position;
    }

    protected virtual void OnDie() {
    }
}
