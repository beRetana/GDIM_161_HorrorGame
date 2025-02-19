using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Downed : PlayerState
{
    public Downed(GameObject _player) : base(_player)
    {
        stateName = STATE.DOWNED;
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
