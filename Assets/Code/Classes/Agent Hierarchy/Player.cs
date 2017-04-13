//Game executable hosted by JBT at: http://robins.tech/jbt/documents/assthree/GameExecutable.zip

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A player object, used to represent a player in the game, which can either be a Human or an AI
/// </summary>
public abstract class Player : Agent
{
    public static int PlayerCount;

    protected string name;
    protected int score;
    protected List<Roboticon> ownedRoboticons = new List<Roboticon>();
    protected List<Tile> ownedTiles = new List<Tile>();
    protected Color playerTileColor;

    /// <summary>
    /// Returns the tile color assigned to this player
    /// </summary>
    public Color GetTileColor()
    {
        return playerTileColor;
    }

    public void SetTileColor(Color color)
    {
        playerTileColor = color;
    }

    /// <summary>
    /// Calculates the players score based off of the amount of tiles and resources that they own and upgraded roboticons they have
    /// </summary>
    /// <returns>The score of the player</returns>
    public int CalculateScore()
    {
        int scoreFromTiles = 0;
        foreach(Tile tile in ownedTiles)
        {
            ResourceGroup tileResources = tile.GetBaseResourcesGenerated();
            scoreFromTiles += tileResources.energy + tileResources.food + tileResources.ore;
        }

        int scoreFromRoboticons = 0;
        foreach (Roboticon roboticon in ownedRoboticons)
        {
            scoreFromRoboticons += roboticon.GetPrice();
        }

        int scoreFromResources = 0;
        scoreFromResources += resources.Sum() * 5;
        scoreFromResources += money * 5;

        return scoreFromRoboticons + scoreFromTiles + scoreFromResources;
    }

    /// <summary>
    /// Adds the total resources for all tiles owned by the player to the player's resources.
    /// </summary>
    public void Produce()
    {
        resources += CalculateTotalResourcesGenerated();
    }

    /// <summary>
    /// Returns the sum of all tile-generated resources.
    /// </summary>
    /// <returns></returns>
    public ResourceGroup CalculateTotalResourcesGenerated()
    {
        ResourceGroup totalResources = new ResourceGroup();

        foreach (Tile tile in ownedTiles)
        {
            totalResources += tile.GetTotalResourcesGenerated();
        }
        return totalResources;
    }

    //JBT moved the logic for buying tiles to this method from the HumanGUI class, so AI's can use this method also
    /// <summary>
    /// Makes the player the owner of the provided tile, providing they have enough money to purchase it and they don't already own it
    /// </summary>
    /// <param name="tile">The tile to acquire</param>
    public void AcquireTile(Tile tile)
    {
        if (!ownedTiles.Contains(tile))
        {
            if (tile.GetPrice() < money)
            {
                SetMoney(GetMoney() - tile.GetPrice());
            }
            else
            {
                throw new System.InvalidOperationException("Not enough money to buy tile");
            }

            ownedTiles.Add(tile);
            tile.SetOwner(this);
        }
        else
        {
            throw new System.InvalidOperationException("Tried to acquire a tile which is already owned by this player.");
        }
    }

    /// <summary>
    /// Gets a list of owned tiles
    /// </summary>
    /// <returns>The list of owned tiles</returns>
    public List<Tile> GetOwnedTiles()
    {
        return ownedTiles;
    }

    /// <summary>
    /// Gets a list roboticons
    /// </summary>
    /// <returns>The list of owned tiles</returns>
    public List<Roboticon> GetRoboticons()
    {
        return ownedRoboticons;
    }

    /// <summary>
    /// Add a roboticon to a list of roboticons owned by the player
    /// </summary>
    /// <param name="roboticon">The roboticon to acquire</param>
    public void AcquireRoboticon(Roboticon roboticon)
    {
        ownedRoboticons.Add(roboticon);
    }

    /// <summary>
    /// Upgrade a roboticon in the players inventory
    /// </summary>
    /// <param name="roboticon">The roboticon to upgrade</param>
    /// <param name="upgrade">The upgrade to apply</param>
    public void UpgradeRoboticon(Roboticon roboticon, ResourceGroup upgrade)
    {
        roboticon.Upgrade(upgrade);
    }

    /// <summary>
    /// Installs a roboticon to a tile owned by the player
    /// </summary>
    /// <param name="roboticon">The roboticon to install</param>
    /// <param name="tile">The tile to install the roboticon to</param>
    public void InstallRoboticon(Roboticon roboticon, Tile tile)
    {
        tile.InstallRoboticon(roboticon);
        roboticon.InstallRoboticonToTile(tile);
    }

    //Added by JBT to support the uninstallation of Roboticons from tiles
    /// <summary>
    /// Uninstall a roboticon from a tile owned by the player
    /// </summary>
    /// <param name="roboticon">The roboticon to uninstall</param>
    /// <param name="tile">The tile to uninstall the roboticon from</param>
    public void UninstallRoboticon(Roboticon roboticon, Tile tile)
    {
        tile.UninstallRoboticon(roboticon);
        roboticon.UninstallRoboticonFromTile();
    }

    /// <summary>
    /// Returns a value indicating if this player is a human or AI player
    /// </summary>
    /// <returns>True if this player is human, false if AI</returns>
    public bool IsHuman()
    {
        return this.GetType().ToString() == "Human";
    }

    /// <summary>
    /// Getter for the name of this player
    /// </summary>
    /// <returns>The name of this player</returns>
    public string GetName()
    {
        return name;
    }

    /// <summary>
    /// Performs turn logic for the player, is used differently by human and AI players
    /// </summary>
    /// <param name="state">The current state of the game</param>
    public abstract void Act(GameManager.States state);
}
