using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using System.Collections;
using StarterAssets;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttackPlayer", story: "[Enemy] attacks [Player]", category: "Action", id: "e6b4af2c023fdb12db722fb12e2e7192")]
public partial class AttackPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    protected override Status OnStart()
    {
        PlayerBase player = Player.Value.GetComponent<PlayerBase>();
        MonsterAnimator monsterAnimator = Self.Value.GetComponent<MonsterAnimator>();
        
        //player.LockPlayer();
        
        int attack = UnityEngine.Random.Range(0, 2);

        if (attack == 0)
            monsterAnimator.TriggerAttackBasic(player);
        else 
            monsterAnimator.TriggerAttackZombie(player);
        
        return Status.Success;
    }
}

