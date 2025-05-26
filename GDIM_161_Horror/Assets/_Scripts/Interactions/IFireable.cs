using UnityEngine;


public interface IFireable
{
    public bool IsLit { get; }
    public void IgniteFire();

    public void ExtinguishFire();
}
