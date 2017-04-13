//Game executable hosted by JBT at: http://robins.tech/jbt/documents/assthree/GameExecutable.zip

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Holds information relating to 3 resource types. 
/// Handles manipultation of these resources and interactions between resource types. 
/// </summary>
public class ResourceGroup
{
    public int food;
    public int energy;
    public int ore;

    public ResourceGroup(int food = 0, int energy = 0, int ore = 0)
    {
        this.food = food;
        this.energy = energy;
        this.ore = ore;
    }

    public static ResourceGroup operator +(ResourceGroup c1, ResourceGroup c2)
    {
        return new ResourceGroup(c1.getFood() + c2.getFood(),
                                 c1.getEnergy() + c2.getEnergy(),
                                 c1.getOre() + c2.getOre());
    }

    public static ResourceGroup operator -(ResourceGroup c1, ResourceGroup c2)
    {
        return new ResourceGroup(c1.getFood() - c2.getFood(),
                                 c1.getEnergy() - c2.getEnergy(),
                                 c1.getOre() - c2.getOre());
    }

    public static ResourceGroup operator *(ResourceGroup c1, ResourceGroup c2)
    {
        return new ResourceGroup(c1.getFood() * c2.getFood(),
                                 c1.getEnergy() * c2.getEnergy(),
                                 c1.getOre() * c2.getOre());
    }

    public static ResourceGroup operator *(ResourceGroup r, int s)
    {
        return new ResourceGroup(r.getFood() * s,
                                 r.getEnergy() * s,
                                 r.getOre() * s);
    }
    public static ResourceGroup operator *(ResourceGroup r, double s)
    {
        return new ResourceGroup(Convert.ToInt16(r.getFood() * s),
                                 Convert.ToInt16(r.getEnergy() * s),
                                 Convert.ToInt16(r.getOre() * s));
    }

    public static ResourceGroup operator *(int s, ResourceGroup r)
    {
        return r * s;
    }

    public override string ToString()
    {
        return "ResourceGroup(" + food + ", " + energy + ", " + ore + ")";
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        ResourceGroup resourcesToCompare = (ResourceGroup)obj;

        return (food == resourcesToCompare.food && 
                energy == resourcesToCompare.energy &&
                ore == resourcesToCompare.ore);
    }

    public override int GetHashCode()
    {
        return food.GetHashCode() ^ energy.GetHashCode() << 2 ^ ore.GetHashCode() >> 2;
    }

    /// <summary>
    /// If this resource group contains any negative resource values, sets them to 0.
    /// </summary>
    public void RemoveNegativeResources()
    {
        if (food < 0)
        {
            food = 0;
        }
        if (energy < 0)
        {
            energy = 0;
        }
        if (ore < 0)
        {
            ore = 0;
        }
    }

    public int Sum()
    {
        return this.food + this.energy + this.ore;
    }

    public int getFood()
    {
        return this.food;
    }

    public int getEnergy()
    {
        return this.energy;
    }

    public int getOre()
    {
        return this.ore;
    }

    /// <summary>
    /// Added by JBT for a shorthand method of creating empty ResourceGroups
    /// </summary>
    public static ResourceGroup Empty
    {
        get { return new ResourceGroup(0, 0, 0); }
    }
}


