//Game executable hosted at: http://www-users.york.ac.uk/~jwa509/executable.exe

using UnityEngine;
using UnityEngine.UI;

//JBT
public class auctionSellWindowScript : MonoBehaviour
{
    public canvasScript AuctionCanvas;

    private AuctionManager auction;
    private Player currentPlayer;

    #region Resource amount labels
    public InputField foodAuctionAmount;
    public InputField energyAuctionAmount;
    public InputField oreAuctionAmount;
    #endregion

    public InputField AuctionPrice;
    public GameObject AuctionListedText;
    public GameObject auctionInfo;

    #region errorMessage labels
    public GameObject NotEnoughResourcesMessage;
    public GameObject NoResourcesMessage;
    public GameObject NoPriceMessage;
    #endregion

    void Start()
    {
        auction = GameHandler.GetGameManager().auctionManager;

        foodAuctionAmount.onValidateInput += ValidatePositiveInput;
        energyAuctionAmount.onValidateInput += ValidatePositiveInput;
        oreAuctionAmount.onValidateInput += ValidatePositiveInput;
        AuctionPrice.onValidateInput += ValidatePositiveInput;
    }

    /// <summary>
    /// Only allows for integer characters in the input fields
    /// </summary>
    /// <param name="text"></param>
    /// <param name="charIndex"></param>
    /// <param name="addedChar"></param>
    /// <returns>The entered character if it is an integer</returns>
    public char ValidatePositiveInput(string text, int charIndex, char addedChar)
    {
        NotEnoughResourcesMessage.SetActive(false);
        NoResourcesMessage.SetActive(false);
        NoPriceMessage.SetActive(false);
        int tryParseResult;

        if (int.TryParse(addedChar.ToString(), out tryParseResult)) //Only accept characters which are integers (no '-')
        {
            return addedChar;
        }
        else
        {
            return '\0';    //Empty string character
        }
    }


    /// <summary>
    /// Gathers the entered resources and price when the list auction button is pressed.
    /// Displays error messages if invalid values are entered or calls PutUpForAuction if valid
    /// </summary>
    public void OnListAuctionButtonPress()
    {
        ResourceGroup resourcesToAuction = new ResourceGroup();
        currentPlayer = GameHandler.gameManager.GetCurrentPlayer();

        resourcesToAuction.food = int.Parse(foodAuctionAmount.text);
        resourcesToAuction.energy = int.Parse(energyAuctionAmount.text);
        resourcesToAuction.ore = int.Parse(oreAuctionAmount.text);
        int auctionPrice = int.Parse(AuctionPrice.text);

        //Checks that the player has enough resources, that a positive number of resources has been entered and then that a positive price has been entered
        if ((currentPlayer.GetResources().food < resourcesToAuction.food) || (currentPlayer.GetResources().energy < resourcesToAuction.energy) || (currentPlayer.GetResources().ore < resourcesToAuction.ore))
        {
            NotEnoughResourcesMessage.SetActive(true);
        }
        else if (resourcesToAuction.Sum() == 0)
        {
            NoResourcesMessage.SetActive(true);
        }
        else if (auctionPrice == 0)
        {
            NoPriceMessage.SetActive(true);
        }
        else
        {
            GameHandler.gameManager.auctionManager.PutUpForAuction(resourcesToAuction, currentPlayer, auctionPrice);
            ClearWindow();
        }
    }

    /// <summary>
    /// Loads a fresh Auction Sell Window
    /// Sets certain parts of the window to active depending on if the player already owns an auction
    /// </summary>
    public void LoadWindow()
    {
        currentPlayer = GameHandler.gameManager.GetCurrentPlayer();
        foodAuctionAmount.text = "0";
        energyAuctionAmount.text = "0";
        oreAuctionAmount.text = "0";
        AuctionPrice.text = "0";
        AuctionListedText.SetActive(false);
        auctionInfo.SetActive(true);
        NotEnoughResourcesMessage.SetActive(false);
        NoResourcesMessage.SetActive(false);
        NoPriceMessage.SetActive(false);

        //Checks if the player already owns an auction and displays the correct information if they do.
        foreach (AuctionManager.Auction curAuction in GameHandler.GetGameManager().auctionManager.auctionListings)
        {
            if (currentPlayer == curAuction.Owner)
            {
                auctionInfo.SetActive(false);
                AuctionListedText.SetActive(true);
            }
        }
    }
    
    /// <summary>
    /// Clears the window when the player lists their auction and displays the correct information (removes the option to list another auction and displays text
    /// </summary>
    public void ClearWindow()
    {
        AuctionListedText.SetActive(true);
        auctionInfo.SetActive(false);
        NotEnoughResourcesMessage.SetActive(false);
        NoResourcesMessage.SetActive(false);
        NoPriceMessage.SetActive(false);
    }
}