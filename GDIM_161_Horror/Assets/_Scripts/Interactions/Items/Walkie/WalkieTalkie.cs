using Dissonance;
using Interactions;
using UnityEngine;

public class WalkieTalkie : NetworkPickableItem
{
   
    protected  override void PickItem(int playerid)
    { 
        base.PickItem(playerid);

        var local = FindObjectOfType<DissonanceComms>();
        local.AddToken("AddWalkie");
    }

    public override void UnPossessItem(Vector3 throwDir, int playerID)
    { 
        base.UnPossessItem(throwDir, playerID);

        var local = FindObjectOfType<DissonanceComms>();
        local.RemoveToken("AddWalkie");
    }




}
