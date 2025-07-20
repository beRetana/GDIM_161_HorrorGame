using Mirror;
using UnityEngine;

public class PlayerSpawnPosition : NetworkBehaviour
{
    [SyncVar] private bool _isOccupied;

    public bool IsOccupied => _isOccupied;

    public Vector3 UseSpawner()
    {
        if (isServer) _isOccupied = true;
        else CmdSetOccupied(true);
        return transform.position;
    }

    [Command(requiresAuthority = false)]
    private void CmdSetOccupied(bool isOccupied)
    {
        this._isOccupied = isOccupied;
        gameObject.SetActive(!isOccupied);
    }
}
