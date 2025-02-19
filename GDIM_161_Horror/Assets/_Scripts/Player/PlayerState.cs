using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Codice.CM.Common;
using UnityEngine.Playables;

public class PlayerState
{
    public enum STATE
    {
        LOCKED, UNLOCKED, DOWNED
    }
    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    }

    public STATE stateName;
    protected EVENT stage;
    protected GameObject player;
    //protected Animator anim;
    protected PlayerState nextState;

    public PlayerState(GameObject _player)
    {
        stage = EVENT.ENTER;
        player = _player;
        //anim = _anim;
    }

    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; }

    public PlayerState Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            Debug.Log($"Going to {nextState} now");
            return nextState;
        }

        return this;
    }

}
