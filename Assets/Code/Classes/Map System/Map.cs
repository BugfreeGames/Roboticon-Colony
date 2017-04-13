//Game executable hosted by JBT at: http://robins.tech/jbt/documents/assthree/GameExecutable.zip

using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;

[assembly: InternalsVisibleToAttribute("MapTests")]

public class Map
{
    public static Vector2 MAP_DIMENSIONS = new Vector2(10,10);
    public static Vector3 MAP_POSITION = new Vector3(-12, 1, -12);

    private List<Tile> tiles = new List<Tile>();
    private const int MAX_TILE_RESOURCE_PRODUCTION = 10;
    private mapManagerScript mapManager;         //Added by JBT to fix a bug when the last selected tile on a turn would not be selectable on the next

    public Map()
    {
        int numTiles = (int)(MAP_DIMENSIONS.x * MAP_DIMENSIONS.y);

        for(int i = 0; i < numTiles; i ++)
        {
            Tile tile = new Tile(GetRandomTileResources(), MAP_DIMENSIONS, i);
            tiles.Add(tile);
        }
    }

    public Tile GetTile(int tileId)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].GetId() == tileId)
            {
                return tiles[i];
            }
        }

        throw new System.IndexOutOfRangeException("Tile with index " + tileId.ToString() + " does not exist in the map.");
    }

    public int GetNumUnownedTilesRemaining()
    {
        int numTiles = 0;

        foreach(Tile tile in tiles)
        {
            if(tile.GetOwner() == null)
            {
                numTiles++;
            }
        }

        return numTiles;
    }

    public void TickAllTileEventEffects()
    {
        foreach (Tile tile in tiles)
        {
            tile.RandomEventEffectTick();
        }
    }

    /// <summary>
    /// Instantiate the map into the current scene.
    /// </summary>
    public void Instantiate()
    {
        foreach(Tile tile in tiles)
        {
            tile.Instantiate(MAP_POSITION);
        }

        mapManager = new GameObject("Map Manager").AddComponent<mapManagerScript>();
        MonoBehaviour.DontDestroyOnLoad(mapManager);
        mapManager.SetMap(this);
    }

    /// <summary>
    /// Refreshes all tiles in the map.
    /// </summary>
    public void UpdateMap()
    {
        foreach(Tile tile in tiles)
        {
            tile.TileNormal();
        }
        //Added by JBT to fix a bug when the last selected tile on a turn would not be selectable on the next
        mapManager.RefreshSelectedTile();
    }

    public List<Tile> GetTiles()
    {
        return tiles;
    }

    /// <summary>
    /// Returns a random set of resources for a tile to produce.
    /// </summary>
    /// <returns></returns>
    private ResourceGroup GetRandomTileResources()
    {
        //TODO - Varied resource distribution for map features such as lakes and landmarks.
        return new ResourceGroup(GetRandomResourceAmount(), 
                                 GetRandomResourceAmount(), 
                                 GetRandomResourceAmount());
    }

    private int GetRandomResourceAmount()
    {
        return UnityEngine.Random.Range(0, MAX_TILE_RESOURCE_PRODUCTION + 1);
    }

    // Created by JBT
    /// <summary>
    /// Get a list of a tiles neighbours
    /// North East South West (no diaganols)
    /// </summary>
    /// <param name="tile">The tile whoms neighbours are being requested</param>
    /// <returns>A list of the neighbours</returns>
    public List<Tile> GetTileNeighbours(Tile tile)
    {
        List<Tile> Neighbours = new List<Tile>();
        var TilePosition = tile.GetTileObject().GetTilePosition();
        if (TilePosition.x > 0)
        { 
           Neighbours.Add(GetTileAtPosition(new Vector2(TilePosition.x - 1, TilePosition.y)));
        }
        if (TilePosition.x < 9)
        {
            Neighbours.Add(GetTileAtPosition(new Vector2(TilePosition.x + 1, TilePosition.y)));
        }
        if (TilePosition.y > 0)
        {
            Neighbours.Add(GetTileAtPosition(new Vector2(TilePosition.x, TilePosition.y - 1)));
        }
        if (TilePosition.y < 9)
        {
            Neighbours.Add(GetTileAtPosition(new Vector2(TilePosition.x, TilePosition.y + 1)));
        }

        return Neighbours;
    }

    // created by JBT
    /// <summary>
    /// Get the tile at a given map position
    /// </summary>
    /// <param name="position">the position of the tile</param>
    /// <returns>the tile at that position</returns>
    public Tile GetTileAtPosition(Vector2 position)
    {
        foreach (Tile currentTile in tiles)
        {
            Vector2 currentTilePosition = currentTile.GetTileObject().GetTilePosition();
            if (currentTilePosition == position)
            {
                return currentTile;
            }
        }
        throw new ArgumentException("No tile at that position");
    }
}
