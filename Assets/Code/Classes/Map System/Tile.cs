//Game executable hosted at: http://www-users.york.ac.uk/~jwa509/executable.exe

using UnityEngine;
using System.Collections.Generic;
using System;

public class Tile
{
    public const float TILE_SIZE = 0.41f;
    public const int ROBOTICON_UPGRADE_WEIGHT = 1;  //Currently each roboticon upgrade adds this amount to the production of its resource
    public List<RandomEventEffect> currentEvents = new List<RandomEventEffect>();   //Assessment 4:- Changed from a single RandomEventEffect to a list to allow for multiple events per tile.

    private int tileId;
    private ResourceGroup BaseResources; // JBT Renamed this from total resources as total resources better decribes resources inlcuding affects of events and roboticons
    private Player owner;
    private Roboticon installedRoboticon;   //Assessment 4:- changed from list to single roboticon per tile
    private TileObject tileObject;
    private bool tileIsSelected = false;

    public Tile(ResourceGroup resources, Vector2 mapDimensions, int tileId, Player owner = null)
    {
        this.BaseResources = resources;
        this.owner = owner;
        this.tileId = tileId;
        
        Vector2 tilePosition = new Vector2(tileId % mapDimensions.x, (int)(tileId / mapDimensions.y));
        this.tileObject = new TileObject(tileId, tilePosition, new Vector2(TILE_SIZE, TILE_SIZE));
    }

    /// <summary>
    /// Call when this tile is to be selected.
    /// </summary>
    public void TileSelected()
    {
        GameHandler.GetGameManager().GetHumanGui().DisplayTileInfo(this);
        tileObject.OnTileSelected();
    }

    public void TileHovered()
    {
        tileObject.OnTileHover();
    }

    /// <summary>
    /// Call to refresh a tile to its default colour based on ownership.
    /// </summary>
    public void TileNormal()
    {
        if (!tileIsSelected)
        {
            if (owner == null)
            {
                tileObject.OnTileNormal(TileObject.TILE_DEFAULT_COLOUR, false);
            }
            else
            {
                tileObject.OnTileNormal(owner.GetTileColor(), true);
            }
        }
    }

    //Edited by JBT to support the display on installed roboticons on tiles
    //Assessment 4:- changed to allow only one roboticon per tile
    /// <summary>
    /// Throws System.Exception if the roboticon already exists on this tile.
    /// </summary>
    /// <param name="roboticon">The roboticon to install</param>
    public void InstallRoboticon(Roboticon roboticon)
    {
        if (installedRoboticon != null)
        {
            throw new System.InvalidOperationException("Roboticon already exists on this tile");
        }

        installedRoboticon = roboticon;
        tileObject.ShowInstalledRoboticon();
    }

    /// <summary>
    /// Throws System.Exception if the roboticon does not exist on this tile.
    /// </summary>
    /// <param name="roboticon">The roboticon to uninstall</param>
    public void UninstallRoboticon()
    {
        if (installedRoboticon == null)
        {
            throw new System.InvalidOperationException("Roboticon doesn't exist on this tile");
        }

        installedRoboticon = null;

        tileObject.HideInstalledRoboticon();
    }

    /// <summary>
    /// Assessment 4:- changed to single roboticon per tile
    /// </summary>
    /// <returns></returns>
    public Roboticon GetInstalledRoboticon()
    {
        return this.installedRoboticon;
    }

    public int GetId()
    {
        return this.tileId;
    }

    public int GetPrice()
    {
        return (this.BaseResources*(new ResourceGroup(10, 10, 10))).Sum();
    }

    /// <summary>
    /// Returns the total resources given by the tile
    /// takes into account roboticons and random events
    /// </summary>
    /// <returns>the total resources produced this turn</returns>
    public ResourceGroup GetTotalResourcesGenerated()
    {
        /* JBT Changes to this method:
         * once the resources have been calculated for the tile apply random events buffs/debuffs if there is a random event
         * fixed an error where the calulation modified the base resources
         */
        ResourceGroup totalResources = new ResourceGroup(BaseResources.food, BaseResources.energy, BaseResources.ore);

        if (installedRoboticon != null)
        {
            totalResources += installedRoboticon.GetUpgrades() * ROBOTICON_UPGRADE_WEIGHT;
        }

        for (int i = currentEvents.Count - 1; i >= 0; i --)
        {
            RandomEventEffect effect = currentEvents[i];
            totalResources += effect.GetEffects();
        }

        totalResources.RemoveNegativeResources();
        return totalResources;
    }

    #region Install/Uninstall test functions
    //Added by JBT. The normal version will not work with tests as it references a tileobject, which NUnit cannot test
    /// <summary>
    /// Throws System.Exception if the roboticon already exists on this tile.
    /// </summary>
    /// <param name="roboticon">The roboticon to install</param>
    public void InstallRoboticonTest(Roboticon roboticon)
    {
        if (installedRoboticon != null)
        {
            throw new System.InvalidOperationException("Roboticon already exists on this tile\n");
        }
        installedRoboticon = roboticon;
    }

    //Added by JBT. The normal version will not work with tests as it references a tileobject, which NUnit cannot test
    /// <summary>
    /// Throws System.Exception if the roboticon does not exist on this tile.
    /// </summary>
    /// <param name="roboticon">The roboticon to uninstall</param>
    public void UninstallRoboticonTest()
    {
        if (installedRoboticon == null)
        {
            throw new System.InvalidOperationException("Roboticon doesn't exist on this tile\n");
        }

        installedRoboticon = null;
    }
    #endregion

    /// <summary>
    /// Assessment 4:- To be called once per turn to tick the random events attached to this tile.
    /// </summary>
    public void RandomEventEffectTick()
    {
        for (int i = currentEvents.Count - 1; i >= 0; i--)
        {
            RandomEventEffect effect = currentEvents[i];
            effect.Tick();

            if (effect.IsFinished())
            {
                currentEvents.Remove(effect);
            }
        }
    }

    /// <summary>
    /// Returns the base resources of this tile, not including roboticon yield.
    /// </summary>
    /// <returns></returns>
    public ResourceGroup GetBaseResourcesGenerated()
    {
        return BaseResources;
    }

    /// <summary>
    /// Assessment 4:- Use with care. Sets the base resources generated by this tile permanently.
    /// </summary>
    public void SetBaseResources(ResourceGroup newResources)
    {
        BaseResources = newResources;

        BaseResources.RemoveNegativeResources();
    }

    /// <summary>
    /// Instantiate the tile in the current scene.
    /// </summary>
    public void Instantiate(Vector3 mapCenterPosition)
    {
        tileObject.Instantiate(mapCenterPosition);
    }
    
    public void SetOwner(Player player)
    {
        this.owner = player;
        TileNormal();
    }

    public Player GetOwner()
    {
        return owner;
    }

    public TileObject GetTileObject()
    {
        return tileObject;
    }

    // Added by JBT
    public override int GetHashCode()
    {
        return tileId;
    }

    // Added by JBT to test tile equality
    public override bool Equals(object obj)
    {
        return ((Tile)obj).tileId == tileId;
    }
    
    /// <summary>
    /// Assessment 4:- Apply a new random event to the tile. Set the turns in newEffect to 0 for a permanent effect.
    /// Changed from a single-event-per-tile system to an multiple-event-per-tile system
    /// </summary>
    /// <param name="newEvent">the event</param>
    public void ApplyEventEffect(RandomEventEffect newEffect)
    {
        if (newEffect.IsFinished())     //Permanent tile effects are encoded as having 0 turns remaining at the start.
        {
            SetBaseResources(BaseResources + newEffect.GetEffects());      ///Permanently damage this tile.
        }
        else
        {
            newEffect.InstantiateVisualEffect(tileObject.GetTileWorldPosition());   //Create the visual effect at this tile's world position.
            currentEvents.Add(newEffect);   //Add the effect to the list of current effects for a non-permanent resource effect.
        }

        if(newEffect.DoDestroyRoboticons())
        {
            DestroyAllRoboticons();
        }

        GameHandler.GetGameManager().GetHumanGui().RefreshTileInfoWindow(); //Refresh the tile info window in case this tile is currently the one displayed.
    }

    /// <summary>
    /// Assessment 4:- Destroys roboticon on this tile, removing it from their its roboticon list and the tile.
    /// </summary>
    private void DestroyAllRoboticons()
    {
        try
        {
            owner.RemoveRoboticon(installedRoboticon);       //Remove last reference to Roboticon.
        }
        catch (System.ArgumentException)
        {
            throw new System.ArgumentException("Roboticon not owned by tile owner was installed to tile. Tried to remove but failed.");
        }

        UninstallRoboticon();
    }
}
