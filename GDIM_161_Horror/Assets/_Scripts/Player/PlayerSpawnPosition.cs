using Mirror;
using UnityEngine;

public class PlayerSpawnPosition : NetworkBehaviour
{
    [SyncVar] private bool _isOccupied;

    public bool IsOccupied => _isOccupied;

    private void Start()
    {
        Debug.Log("PlayerSpawnPosition Start");
    }

    public Vector3 UseSpawner()
    {
        if (_isOccupied) return Vector3.zero;
        _isOccupied = true;
        CmdSetOccupied(true);
        gameObject.SetActive(false);

        return transform.position;
    }

    [Command(requiresAuthority = false)]
    private void CmdSetOccupied(bool isOccupied)
    {
        this._isOccupied = isOccupied;
    }
}
