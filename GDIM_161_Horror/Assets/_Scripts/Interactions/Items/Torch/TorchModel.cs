using UnityEngine;

public class TorchModel : MonoBehaviour
{
    [SerializeField] private Transform wood;
    [SerializeField] private Transform flameBase;
    [SerializeField] private LayerMask groundLayers;

    private float maxWoodScale;
    private float maxFlameHeight;
    private float smotherRadius;

    public void Initialize(TorchConfig config)
    {
        maxWoodScale = wood.localScale.y;
        maxFlameHeight = flameBase.localPosition.y;
        smotherRadius = config.smotherRadius;
    }

    public void UpdatePyrolysis(float burnRatio)
    {
        wood.localScale = new Vector3(wood.localScale.x, maxWoodScale * burnRatio, wood.localScale.z);
        flameBase.localPosition = new Vector3(flameBase.localPosition.x, maxFlameHeight * burnRatio, flameBase.localPosition.x);
    }

    public void UpdateOrientation() => flameBase.eulerAngles = Vector3.up;
    public bool CheckGrounded() => Physics.CheckSphere(transform.position, smotherRadius, groundLayers);

}
