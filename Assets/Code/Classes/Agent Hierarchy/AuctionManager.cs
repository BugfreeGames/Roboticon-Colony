//Game executable hosted at: http://www-users.york.ac.uk/~jwa509/executable.exe

//Created by JBT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AuctionManager
{
    public List<Auction> auctionListings { get; private set; }

    public AuctionManager()
    {
        auctionListings = new List<Auction>();
    }

    /// <summary>
    /// Put resources up for auction
    /// </summary>
    /// <param name="resources">The resources to be auctioned</param>
    /// <param name="setPrice">The price the player has set</param>
    /// <param name="player">The player setting the auction</param>
    public void PutUpForAuction(ResourceGroup resources, Player player, int price)
    {
        if ((player.GetResources().food < resources.food) || (player.GetResources().energy < resources.energy) || (player.GetResources().ore < resources.ore))
        {
            throw new ArgumentOutOfRangeException("Not enough resources");
        }
        else
        {
            auctionListings.Add(new Auction(resources, player, price));
        }
    }

    /// <summary>
    /// Returns an auction that the current player can bid on. For example, if player 1 is passed in, player 2's auction will be returned
    /// </summary>
    /// <param name="player">The player bidding</param>
    /// <returns>The auction the player can bid on</returns>
    public Auction RetrieveAuction(Player player)
    {
        foreach (Auction curAuction in auctionListings)
        {
            if (curAuction.Owner != player)      //finds an auction that belongs to the other player
            {
                return curAuction;
            }
        }
        return null;
    }

    /// <summary>
    /// Transfers the auction's resources to the current player and the money to the owner of the auction
    /// </summary>
    /// <param name="player">The player buying the resources</param>
    public void AuctionBuy(Player player)
    {
        Auction auction = RetrieveAuction(player);
        int auctionMoney = auction.AuctionPrice;
        if (player.GetMoney() < auction.AuctionPrice)
        {
            throw new ArgumentOutOfRangeException("Not enough money");
        }
        else
        {
            player.SetResources(player.GetResources() + auction.AuctionResources);
            player.SetMoney(player.GetMoney() - auction.AuctionPrice);
            auction.Owner.SetResources(auction.Owner.GetResources() - auction.AuctionResources);
            auction.Owner.SetMoney(auction.Owner.GetMoney() + auction.AuctionPrice);
            auctionListings.Remove(auction);            //removes the auction from auctionListings when it has been bought
        }
    }

    /// <summary>
    /// Clears the list of auctions
    /// </summary>
    public void ClearAuctions()
    {
        auctionListings = new List<Auction>();
    }

    /// <summary>
    /// Contains a list of resources, the price set and the owner of the auction
    /// </summary>
    public class Auction
    {
        public int AuctionPrice { get; private set; }
        public ResourceGroup AuctionResources { get; private set; }
        public Player Owner { get; private set; }

        /// <summary>
        /// Creates an auction listing with its price and resource type
        /// </summary>
        /// <param name="resourceQuantity">amount of resources being auctioned</param>
        /// <param name="player">Player creating the auction</param>
        /// <param name="price">Price of auction</param>
        public Auction(ResourceGroup resourceQuantity, Player player, int price)
        {
            AuctionResources = resourceQuantity;
            Owner = player;
            AuctionPrice = price;
        }
    }
}