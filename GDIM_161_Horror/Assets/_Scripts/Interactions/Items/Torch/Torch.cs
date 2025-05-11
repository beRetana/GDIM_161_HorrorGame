using UnityEngine;
using Mirror;
using Interactions;

public class Torch : NetworkPickableItem, IFireable
{
    [SerializeField] private TorchConfig config;
    [SerializeField] private TorchModel model;
    [SerializeField] private TorchFireController fireController;
    [SerializeField] private TorchEvents events;

    private TorchData data;

    private void Awake()
    {
        data = gameObject.AddComponent<TorchData>();
        data.Initialize(config);
    }

    protected override void Start()
    {
        base.Start();
        fireController.Initialize(config);
    }

    private void Update()
    {
        if (!fireController.IsLit) return;
        model.UpdateOrientation();
        if (model.CheckGrounded() && !IsHeld)
            fireController.Smother();
    }

    private void FixedUpdate()
    {
        if (!fireController.IsList) return;

        data.UpdateTimers(Time.fixedDeltaTime);

        if(data.ShouldUpdatePyrolysis())
        {
            model.UpdatePyrolysis(data.BurnRatio);
            data.ResetPyrolysisTimer();
        }

        if (data.BurnTimer <= 0)
            fireController.Extinguish();
    }

    public override void UseItem(int playerID)
    {
        //TK
    }

    public void Light() => fireController.Light();
    public void Extinguish() => fireController.Extinguish();
    public bool IsLit => fireController.IsLit;
}