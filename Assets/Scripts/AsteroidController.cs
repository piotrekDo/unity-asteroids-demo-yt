using System;
using UnityEngine;

public class AsteroidController : BoundedEntity {

    [SerializeField] private int m_size;
    [SerializeField] private float m_forcePower;
    [SerializeField] private float m_angularPower;

    public int Size => m_size;

    public event Action<AsteroidController> onAsteroidDie;

    private void Start() {
        this.m_rigidbody.AddForce(
            UnityEngine.Random.insideUnitCircle * this.m_rigidbody.mass * m_forcePower,
            ForceMode2D.Impulse
            );

        this.m_rigidbody.angularVelocity = UnityEngine.Random.Range(-m_angularPower, m_angularPower);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        m_health--;
        if (m_health < 1) {
            Debug.Log("die?");
            onAsteroidDie?.Invoke(this);
        }
    }
}
