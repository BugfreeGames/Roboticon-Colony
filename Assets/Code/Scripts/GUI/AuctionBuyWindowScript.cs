//Game executable hosted at: http://www-users.york.ac.uk/~jwa509/executable.exe

using UnityEngine;
using UnityEngine.UI;

//JBT
public class AuctionBuyWindowScript : MonoBehaviour
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
    public GameObject NoAuctionMessage;
    public GameObject NotEnoughMoneyMessage;

    void Start()
    {
        auctionManager = GameHandler.GetGameManager().auctionManager;
    }

    /// <summary>
    /// Applies AuctionBuy to the auction, clears the auction from the window and updates the resource bar
    /// </summary>
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

    /// <summary>
    /// Loads a valid auction to the Auction Buy Window
    /// Displays the auction belonging to the other player if one is present, otherwise displays the NoAuctionMessage
    /// </summary>
    public void LoadAuction()
    {
        currentPlayer = GameHandler.gameManager.GetCurrentPlayer();        
        auction = GameHandler.GetGameManager().auctionManager.RetrieveAuction(currentPlayer);
        NotEnoughMoneyMessage.SetActive(false);

        if (auction != null)            //If there is an auction that the other player owns
        {
            AuctionResources.SetActive(true);
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

    /// <summary>
    /// Removes auction information and buy button from the window and displays a message instead
    /// </summary>
    public void ClearWindow()
    {
        AuctionResources.SetActive(false);
        NoAuctionMessage.SetActive(true);
    }
}