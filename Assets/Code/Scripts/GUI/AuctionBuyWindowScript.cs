﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//JBT
public class auctionBuyWindowScript : MonoBehaviour
{
    public canvasScript AuctionCanvas;
    private AuctionManager auctionManager;
    private Player currentPlayer;
    private AuctionManager.Auction auction;

    #region Resource amount labels
    public Text foodAuctionAmount;
    public Text energyAuctionAmount;
    public Text oreAuctionAmount;
    #endregion

    public Text AuctionBuyPrice;
    public GameObject AuctionResources;
    public GameObject Price;
    public GameObject NoAuctionMessage;
    public GameObject NotEnoughMoneyMessage;

    void Start()
    {
        auctionManager = GameHandler.GetGameManager().auctionManager;
    }

    public void OnBuyAuctionButtonPress()
    {
        if (auction.AuctionPrice < currentPlayer.GetMoney())
        {
            GameHandler.gameManager.auctionManager.AuctionBuy(currentPlayer);
            ClearWindow();
            GameHandler.gameManager.GetHumanGui().UpdateResourceBar(false);
        }
        else
        {
            NotEnoughMoneyMessage.SetActive(true);
        }
    }

    public void LoadAuction()
    {
        currentPlayer = GameHandler.gameManager.GetCurrentPlayer();        
        auction = GameHandler.GetGameManager().auctionManager.RetrieveAuction(currentPlayer);
        NotEnoughMoneyMessage.SetActive(false);

        if (auction != null)
        {
            AuctionResources.SetActive(true);
            Price.SetActive(true);
            NoAuctionMessage.SetActive(false);
            foodAuctionAmount.text = auction.AuctionResources.food.ToString();
            energyAuctionAmount.text = auction.AuctionResources.energy.ToString();
            oreAuctionAmount.text = auction.AuctionResources.ore.ToString();
            AuctionBuyPrice.text = auction.AuctionPrice.ToString();
        }
        else
        {
            ClearWindow();
        }
    }

    public void ClearWindow()
    {
        AuctionResources.SetActive(false);
        Price.SetActive(false);
        NoAuctionMessage.SetActive(true);
    }
}