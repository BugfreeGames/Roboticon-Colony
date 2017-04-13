//Game executable hosted by JBT at: http://robins.tech/jbt/documents/assthree/GameExecutable.zip

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A player that is controlled by the computer
/// </summary>
public class AI : Player
{
    /// <summary>
    /// Creates an AI player instance
    /// </summary>
    /// <param name="resources">The resources this AI player starts with</param>
    /// <param name="name">The name of the AI player</param>
    /// <param name="money">The money this AI player starts with</param>
    public AI(ResourceGroup resources, string name, int money)
    {
        this.name = name;
        this.resources = resources;
        this.money = money;
    }

    /// <summary>
    /// Performs the turn for the current AI player depending on the state
    /// </summary>
    /// <param name="state">The current state of the game</param>
    public override void Act(GameManager.States state)
    {
        switch(state)
        {
            case GameManager.States.ACQUISITION:
                Tile tileToAcquire = ChooseTileToAcquire();

                if (tileToAcquire != null)
                {
                    AcquireTile(tileToAcquire);
                }
                break;
        }

        GameHandler.GetGameManager().CurrentPlayerEndTurn();     //This must be done to signify the end of the AI turn.
    }

    //JBT changed this method to actually select a buyable tile
    /// <summary>
    /// Allows the AI to choose a tile to acquire
    /// </summary>
    /// <returns>A random tile that the AI can afford</returns>
    private Tile ChooseTileToAcquire()
    {
        List<Tile> tiles = GameHandler.GetGameManager().GetMap().GetTiles();
        List<Tile> possibleTiles = new List<Tile>();

        foreach (Tile t in tiles)
        {
            if (t.GetOwner() == null && t.GetPrice() < money)
            {
                possibleTiles.Add(t);
            }
        }

        if (possibleTiles.Count == 0)
        {
            return null;
        }
        else
        {
            return possibleTiles[UnityEngine.Random.Range(0, possibleTiles.Count - 1)];
        }
    }
}
