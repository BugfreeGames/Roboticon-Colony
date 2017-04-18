//Game executable hosted by JBT at: http://robins.tech/jbt/documents/assthree/GameExecutable.zip

using UnityEngine;
using System.Collections;

/// <summary>
/// Class that holds instances of roboticons. Handles manipulation of Roboticons.
/// </summary>
public class Roboticon
{
    public static int TotalRoboticons = 0;
    public const int UPGRADEVALUE = 50;
    public Tile InstalledTile;  //Added by JBT

    private ResourceGroup upgrades;
    private string name;

    public Roboticon()
    {
        this.name = "RBN#" + (++TotalRoboticons).ToString("0000");
        this.upgrades = new ResourceGroup(0,0,0);
    }

    public Roboticon(ResourceGroup upgrades, string name = "")
    {
        this.name = name;
        this.upgrades = upgrades;
    }

    public string GetName()
    {
        if(name == null)
        {
            throw new System.ArgumentNullException("Name not set in roboticon.");
        }

        return name;
    }

    public void Upgrade(ResourceGroup upgrades)
    {
        this.upgrades += upgrades;
    }

    public void Downgrade(ResourceGroup downgrades)
    {
        this.upgrades -= downgrades;
    }

    public int GetPrice()
    {
        return (this.upgrades * UPGRADEVALUE).Sum();
    }

    public ResourceGroup GetUpgrades()
    {
        return upgrades;
    }

    //Added by JBT
    /// <summary>
    /// Installs the Roboticon onto the selected Tile
    /// </summary>
    /// <param name="t">The selected tile<param>
    public void InstallRoboticonToTile(Tile t)
    {
        InstalledTile = t;
    }

    //Added by JBT
    /// <summary>
    /// Uninstalled the Roboticon from its tile
    /// </summary>
    public void UninstallRoboticonFromTile()
    {
        InstalledTile = null;
    }
    
    //Changed by JBT to use the tile reference for this instance instead
    /// <summary>
    /// Checks if the Roboticon is installed to a tile
    /// </summary>
    /// <returns>Bool, Is roboticon installed on a tile?</returns>
    public bool IsInstalledToTile()
    {
        return InstalledTile != null;
    }
}
