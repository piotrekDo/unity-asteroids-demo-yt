using UnityEngine;

public class Collectable : MonoBehaviour {

   [SerializeField] private int m_pickupScore;

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.TryGetComponent(out ShipController ship)) {
            GameEvents.Instance.AddToScore(m_pickupScore);
            Destroy(gameObject);
        }
    }
}
