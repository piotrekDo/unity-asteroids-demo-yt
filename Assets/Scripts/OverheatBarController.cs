using UnityEngine;

public class OverheatBarController : MonoBehaviour {
    private Material m_material;

    private float m_displayFill;
    private float m_targetFill;

    private void Awake() {
        m_material = GetComponent<SpriteRenderer>().material;
    }

    private void Update() {
        m_displayFill = Mathf.Lerp(m_displayFill, m_targetFill, Time.deltaTime * 5f);
        m_material.SetFloat("_Fill", m_displayFill);
    }

    public void SetFill(float current, float max) {
        m_targetFill = Mathf.Clamp01((float) current / max);
    }
}