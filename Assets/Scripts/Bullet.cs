using UnityEngine;

public class Bullet : MonoBehaviour {
    [SerializeField] private float m_forwardSpeed;
    [SerializeField] private float m_maximumLifetime;

    private float m_currentLifetime;

    private void Update() {
        transform.position += (transform.up * m_forwardSpeed) * Time.deltaTime;
        m_currentLifetime += Time.deltaTime;

        if (m_currentLifetime > m_maximumLifetime) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Destroy(gameObject);
    }
}
