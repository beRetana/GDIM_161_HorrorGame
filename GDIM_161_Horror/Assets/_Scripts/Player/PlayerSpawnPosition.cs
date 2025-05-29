using Mirror;
using UnityEngine;

#if MIRROR
public class PlayerSpawnPosition : NetworkBehaviour
#else
public class PlayerSpawnPosition : MonoBehaviour
#endif
{
#if MIRROR
    [SyncVar] private bool _isOccupied;
#else
    private bool _isOccupied;
#endif

    public bool IsOccupied => _isOccupied;

    public Vector3 UseSpawner()
    {
        if (_isOccupied) return Vector3.zero;
        _isOccupied = true;
#if MIRROR
        CmdSetOccupied(true);
#endif
        gameObject.SetActive(false);

        return transform.position;
    }

#if MIRROR
    [Command(requiresAuthority = false)]
    private void CmdSetOccupied(bool isOccupied)
    {
        this._isOccupied = isOccupied;
    }
#endif
}
