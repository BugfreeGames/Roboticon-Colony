//Game executable hosted by JBT at: http://robins.tech/jbt/documents/assthree/GameExecutable.zip

using UnityEngine;

/// <summary>
/// Handles all interactions for the market
/// </summary>
public class Market : Agent
{
    private ResourceGroup resourceSellingPrices;
    private ResourceGroup resourceBuyingPrices;
    private int numRoboticonsForSale;
    private int roboticonBuyingPrice = 15;

    #region Market Starting Constants
    public static int STARTING_FOOD_AMOUNT = 16;
    public static int STARTING_ENERGY_AMOUNT = 16;
    public static int STARTING_ORE_AMOUNT = 0;
    public static int STARTING_ROBOTICON_AMOUNT = 12;

    public static int TARGET_FOOD_AMOUNT = 16;
    public static int TARGET_ENERGY_AMOUNT = 16;
    public static int TARGET_ORE_AMOUNT = 16;

    public static int STARTING_FOOD_BUY_PRICE = 10;
    public static int STARTING_ORE_BUY_PRICE = 10;
    public static int STARTING_ENERGY_BUY_PRICE = 10;
    public static int STARTING_FOOD_SELL_PRICE = 10;
    public static int STARTING_ORE_SELL_PRICE = 10;
    public static int STARTING_ENERGY_SELL_PRICE = 10;

    public static int STARTING_MONEY = 100;
    #endregion

    private const int ROBOTICON_PRODUCTION_COST = 12;

    public Market()
    {
        this.resourceSellingPrices = new ResourceGroup(STARTING_FOOD_BUY_PRICE, STARTING_ENERGY_BUY_PRICE, STARTING_ORE_BUY_PRICE);
        this.resourceBuyingPrices = new ResourceGroup(STARTING_FOOD_SELL_PRICE, STARTING_ENERGY_SELL_PRICE, STARTING_ORE_SELL_PRICE);
        this.resources = new ResourceGroup(STARTING_FOOD_AMOUNT, STARTING_ENERGY_AMOUNT, STARTING_ORE_AMOUNT);
        this.numRoboticonsForSale = STARTING_ROBOTICON_AMOUNT;
        this.money = STARTING_MONEY;
    }

    //Edited by JBT to stop the player from buying unlimited roboticons whilst the markets stock doesnt change
    /// <summary>
    /// Throws System.ArgumentException if the market does not have enough resources 
    /// to complete the transaction.
    /// </summary>
    /// <param name="resources"></param>
    /// <param name="price"></param>
    public void BuyFrom(ResourceGroup resourcesToBuy, int roboticonAmount)
    {
        if (resourcesToBuy.food < 0 || resourcesToBuy.energy < 0|| resourcesToBuy.ore < 0 || roboticonAmount < 0)
        {
            throw new System.ArgumentException("Cannot buy negative amounts of items");
        }
            

        bool hasEnoughResources = !(resourcesToBuy.food > this.resources.food
            || resourcesToBuy.energy > this.resources.energy
            || resourcesToBuy.ore > this.resources.ore
            || roboticonAmount > numRoboticonsForSale);

        if (hasEnoughResources)
        {
            this.resources -= resourcesToBuy; //Requires subtraction overload
            this.numRoboticonsForSale -= roboticonAmount;
            this.money = this.money + (resourcesToBuy * resourceSellingPrices).Sum() + (roboticonBuyingPrice * roboticonAmount); //Overloading * to perform element-wise product to get total gain 
        }
        else
        {
            throw new System.ArgumentException("Market does not have enough resources to perform this transaction.");
        }

    }

    /// <summary>
    /// Throws System.ArgumentException if the market does not have enough money
    /// to complete the transaction.
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="price"></param>
    public void SellTo(ResourceGroup resourcesToSell)
    {
        if (resourcesToSell.getFood() < 0)
        {
            throw new System.ArgumentException("Negative food values cannot be sold.");
        }

        if (resourcesToSell.getEnergy() < 0)
        {
            throw new System.ArgumentException("Negative energy values cannot be sold.");
        }

        if (resourcesToSell.getOre() < 0)
        {
            throw new System.ArgumentException("Negtaive ore values cannot be sold.");
        }

        int price = (resourcesToSell * resourceBuyingPrices).Sum();

        if (price <= money)
        {
            resources += resourcesToSell;
            money = money - price; //Overloading * to perform element-wise product to get total expenditure
        }
        else
        {
            throw new System.ArgumentException("Market does not have enough money to perform this transaction.");
        }

    }

    //Amended by JBT
    /// <summary>
    /// Will update the buy and sell prices in the market, based on the distance to the target stock.
    /// </summary>
    public void UpdatePrices()
    {
        //The lowest price the market will buy resources at
        int baseBuyPrice = 5;

        //The lowest price the market will sell resources at
        int baseSellPrice = baseBuyPrice + 3;
        
        
        //New buy prices = start price + distance to target
        int newFoodBuy = STARTING_FOOD_BUY_PRICE + (TARGET_FOOD_AMOUNT - resources.food);
        int newEnergyBuy = STARTING_ENERGY_BUY_PRICE + (TARGET_ENERGY_AMOUNT - resources.energy);
        int newOreBuy = STARTING_ORE_BUY_PRICE + (TARGET_ORE_AMOUNT - resources.ore);

        //Ensure that the lowest buy price is basePrice
        if (newFoodBuy < baseBuyPrice)
        {
            newFoodBuy = baseBuyPrice;
        }
        if (newEnergyBuy < baseBuyPrice)
        {
            newEnergyBuy = baseBuyPrice;
        }
        if (newOreBuy < baseBuyPrice)
        {
            newOreBuy = baseBuyPrice;
        }
        ResourceGroup newBuyPrice = new ResourceGroup(newFoodBuy,newEnergyBuy,newOreBuy);
        resourceBuyingPrices = newBuyPrice;

        //New sell price = start price - distance to target
        int newFoodSell = STARTING_FOOD_SELL_PRICE - (resources.food - TARGET_FOOD_AMOUNT);
        int newEnergySell = STARTING_ENERGY_SELL_PRICE - (resources.energy - TARGET_ENERGY_AMOUNT);
        int newOreSell = STARTING_ORE_SELL_PRICE - (resources.ore - TARGET_ORE_AMOUNT);

        //Ensure that the lowest sell price is baseSellPrice
        if (newFoodSell < baseSellPrice)
        {
            newFoodSell = baseSellPrice;
        }
        if (newEnergySell < baseSellPrice)
        {
            newEnergySell = baseSellPrice;
        }
        if (newOreSell < baseSellPrice)
        {
            newOreSell = baseSellPrice;
        }

        ResourceGroup newSellPrice = new ResourceGroup(newFoodSell, newEnergySell, newOreSell);
        resourceSellingPrices = newSellPrice;
    }

    /// <summary>
    /// Produces a Roboticon in the market if the market has enough ore.
    /// The market's ore will be decreased by the price or the Roboticon
    /// </summary>
    public void ProduceRoboticon()
    {
        if (resources.ore >= ROBOTICON_PRODUCTION_COST)
        {
            resources.ore -= ROBOTICON_PRODUCTION_COST;
            numRoboticonsForSale++;
        }
    }

    //Added by JBT - A double or nothing gambling game
    /// <summary>
    /// A double or nothing gambling game, takes a min and max roll, outputs the roll result, and returns whether the roll was greater than 50 or not
    /// </summary>
    /// <param name="amount">The amount of money being gambled</param>
    /// <param name="minRoll">The min value of the roll</param>
    /// <param name="maxRoll">The max value of the roll</param>
    /// <param name="roll">The actual roll result</param>
    /// <returns>True if the roll was greater than 50</returns>
    public bool DoubleOrNothing(int amount, int minRoll, int maxRoll, out int roll)
    {
        money += amount;
        if (money < amount * 2)
        {
            money -= amount;
            throw new System.ArgumentException("Market does not have enough money to play");
        }

        roll = Random.Range(minRoll, maxRoll);

        if(roll >= 50)
        {
            money -= amount * 2;
        }

        return roll >= 50;
    }

    public int GetNumRoboticonsForSale()
    {
        return numRoboticonsForSale;
    }

    public ResourceGroup GetResourceBuyingPrices()
    {
        return resourceBuyingPrices;
    }

    public ResourceGroup GetResourceSellingPrices()
    {
        return resourceSellingPrices;
    }

    public int GetRoboticonSellingPrice()
    {
        return roboticonBuyingPrice;
    }
}
