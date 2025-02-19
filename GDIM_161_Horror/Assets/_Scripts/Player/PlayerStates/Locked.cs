using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Locked : PlayerState
{
    public Locked(GameObject _player) : base(_player)
    {
        stateName = STATE.LOCKED;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit() 
    { 
        base.Exit();
    }
}
