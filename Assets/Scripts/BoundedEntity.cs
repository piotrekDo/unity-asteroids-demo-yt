using UnityEngine;

public class BoundedEntity : MonoBehaviour {

    protected Rigidbody2D m_rigidbody;

    [SerializeField] protected Rect m_bounds;
    [SerializeField] protected int m_health;

    private void Awake() {
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
}
