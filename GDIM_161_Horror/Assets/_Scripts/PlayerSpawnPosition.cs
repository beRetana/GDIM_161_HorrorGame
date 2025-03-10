using Mirror;
using UnityEngine;

public class PlayerSpawnPosition : NetworkBehaviour
{
    [SyncVar] private bool _isOccupied;

    public bool IsOccupied => _isOccupied;

    public void UseSpawner()
    {
        if (_isOccupied) return;
        _isOccupied = true;
        CmdSetOccupied(true);
    }

    [Command(requiresAuthority = false)]
    private void CmdSetOccupied(bool isOccupied)
    {
        this._isOccupied = isOccupied;
    }
}
