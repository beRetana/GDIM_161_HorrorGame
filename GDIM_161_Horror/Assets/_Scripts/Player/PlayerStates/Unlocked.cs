using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unlocked : PlayerState
{
    public enum PlayerActionEnum
    {
        None = 0,           // 00000
        Idle = 1 << 0,      // 00001
        Running = 1 << 1,   // 00010
        Jumping = 1 << 2,   // 00100
        Downed = 1 << 3,    // 01000
        Flamed = 1 << 4     // 10000
    }

    public Unlocked(GameObject _player) : base(_player)
    {
        stateName = STATE.UNLOCKED;
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
