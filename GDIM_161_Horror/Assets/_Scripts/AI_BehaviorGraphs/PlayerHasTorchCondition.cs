using System;
using Unity.Behavior;
using UnityEngine;
using Interactions;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Player has Torch", story: "[Player] has fired [object] within [range]", category: "Conditions", id: "683fd9b0cc881bbb8d97050c8afe7e96")]
public partial class PlayerHasTorchCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<float> Range;

    public override bool IsTrue()
    {
        Collider[] collisions = Physics.OverlapSphere(Player.Value.transform.position, Range.Value);     
        foreach (Collider collider in collisions)
        {
            if (collider.gameObject.TryGetComponent<IFireable>(out IFireable component))
            {
                if (!component.IsLit) continue;
                return true;
            }
        }
        return false;
    }
}
