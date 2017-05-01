//Game executable hosted at: http://www-users.york.ac.uk/~jwa509/executable.exe

using UnityEngine;

public class mapTileScript : MonoBehaviour
{
    private int tileId;
    
    public int GetTileId()
    {
        return tileId;
    }

    public void SetTileId(int id)
    {
        tileId = id;
    }
}
