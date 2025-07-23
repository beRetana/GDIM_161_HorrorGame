using Interactions;
using System.Collections;
using UnityEngine;

public class Brazier : FireCollision
{
    [SerializeField, Tooltip("Life Span in Minutes")]
    private float m_DurationMins;
    [SerializeField] private Transform m_VFX;

    private const byte SEC_PER_MIN = 60;

    protected override void Start()
    {
        StartCoroutine(LifeSpand());
    }

    private IEnumerator LifeSpand()
    {
        yield return new WaitForSeconds(m_DurationMins * SEC_PER_MIN);
        m_VFX.gameObject.SetActive(false);
    }

    protected override void OnTriggerEnter(Collider col)
    {
        
    }

    public override bool IsLit()
    {
        return true;
    }
}
