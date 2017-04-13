//Game executable hosted by JBT at: http://robins.tech/jbt/documents/assthree/GameExecutable.zip

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles UI interaction for the market window
/// </summary>
public class marketScript : MonoBehaviour
{
    public canvasScript uiCanvas;

    public Text marketBalance;  //Added by JBT

    #region Resource price labels
    public Text foodBuyPrice;
    public Text foodSellPrice;
    public Text energyBuyPrice;
    public Text energySellPrice;
    public Text oreBuyPrice;
    public Text oreSellPrice;
    public Text roboticonBuyPrice;

    public Text totalBuyPrice;
    public Text totalSellPrice;

    public Text foodAmount;
    public Text energyAmount;
    public Text oreAmount;
    public Text roboticonAmount;
    #endregion

    #region Resource amount labels
    public InputField foodBuyAmount;
    public InputField foodSellAmount;
    public InputField energyBuyAmount;
    public InputField energySellAmount;
    public InputField oreBuyAmount;
    public InputField oreSellAmount;
    public InputField roboticonBuyAmount;
    #endregion

    private Market market;

    public char ValidatePositiveInput(string text, int charIndex, char addedChar)
    {
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

    private void Awake()
    {
        market = GameHandler.GetGameManager().market;
    }

    // Use this for initialization
    void Start ()
    {
        market = GameHandler.GetGameManager().market;
        SetShownMarketPrices();

        foodBuyAmount.onValidateInput   += ValidatePositiveInput;       //Add the ValidatePositiveInput function to
        energyBuyAmount.onValidateInput += ValidatePositiveInput;       //each GUI Text.
        oreBuyAmount.onValidateInput    += ValidatePositiveInput;

        foodSellAmount.onValidateInput  += ValidatePositiveInput;
        energySellAmount.onValidateInput += ValidatePositiveInput;
        oreSellAmount.onValidateInput   += ValidatePositiveInput;
    }

    public void OnBuyButtonPress()
    {
        ResourceGroup resourcesToBuy = new ResourceGroup();
        resourcesToBuy.food = int.Parse(foodBuyAmount.text);
        resourcesToBuy.energy = int.Parse(energyBuyAmount.text);
        resourcesToBuy.ore = int.Parse(oreBuyAmount.text);
        int roboticonsToBuy = int.Parse(roboticonBuyAmount.text);

        int buyPrice = int.Parse(totalBuyPrice.text.Substring(1));

        uiCanvas.BuyFromMarket(resourcesToBuy, roboticonsToBuy, buyPrice);
        SetShownMarketPrices();
    }

    public void OnSellButtonPress()
    {
        ResourceGroup resourcesToSell = new ResourceGroup();
        resourcesToSell.food = int.Parse(foodSellAmount.text);
        resourcesToSell.energy = int.Parse(energySellAmount.text);
        resourcesToSell.ore = int.Parse(oreSellAmount.text);

        int sellPrice = int.Parse(totalSellPrice.text.Substring(1));

        uiCanvas.SellToMarket(resourcesToSell, sellPrice);
        SetShownMarketPrices();
    }

    public void PlayPurchaseDeclinedAnimation()
    {
        totalBuyPrice.GetComponent<Animator>().SetTrigger(HumanGui.ANIM_TRIGGER_FLASH_RED);
    }

    public void PlaySaleDeclinedAnimation()
    {
        totalSellPrice.GetComponent<Animator>().SetTrigger(HumanGui.ANIM_TRIGGER_FLASH_RED);
    }

    public void SetShownMarketPrices()
    {
        UpdateShownMarketPrices();
        UpdateTotalBuyPrice();
        UpdateTotalSellPrice();
        RefreshMarketBalance();
    }

    //JBT
    /// <summary>
    /// Refreshes the balance on the gambling window, if the gambling window is open.
    /// </summary>
    public void RefreshMarketBalance()
    {
        UpdateMarketBalance();

        GameObject gamblingMenu = GameObject.Find("Gambling Menu");

        //Refresh the balance on the gambling menu if it is open
        if (gamblingMenu != null)
        {
            gamblingMenu.GetComponent<gamblingScript>().RefreshMarketBalance();
        }
    }

    public void UpdateTotalBuyPrice()
    {
        ResourceGroup buyingPrices = market.GetResourceBuyingPrices();

        int foodPrice = int.Parse(foodBuyAmount.text) * buyingPrices.food;
        int energyPrice = int.Parse(energyBuyAmount.text) * buyingPrices.energy;
        int orePrice = int.Parse(oreBuyAmount.text) * buyingPrices.ore;
        int roboticonPrice = int.Parse(roboticonBuyAmount.text) * market.GetRoboticonSellingPrice();

        totalBuyPrice.text = "£" + (foodPrice + energyPrice + orePrice + roboticonPrice).ToString();
    }

    public void UpdateTotalSellPrice()
    {
        ResourceGroup sellingPrices = market.GetResourceSellingPrices();

        int foodPrice = int.Parse(foodSellAmount.text) * sellingPrices.food;
        int energyPrice = int.Parse(energySellAmount.text) * sellingPrices.energy;
        int orePrice = int.Parse(oreSellAmount.text) * sellingPrices.ore;

        totalSellPrice.text = "£" + (foodPrice + energyPrice + orePrice).ToString();
    }

    private void UpdateMarketBalance()
    {
        marketBalance.text = "Market has £" + market.GetMoney().ToString();
    }

    //Amended by JBT to show market stock in ui
    /// <summary>
    /// Update shown market stock, sell price and buy price of market items in the market window
    /// </summary>
    private void UpdateShownMarketPrices()
    {
        ResourceGroup sellingPrices = market.GetResourceSellingPrices();
        ResourceGroup buyingPrices = market.GetResourceBuyingPrices();

        foodBuyPrice.text      = "£" + buyingPrices.food.ToString();
        energyBuyPrice.text    = "£" + buyingPrices.energy.ToString();
        oreBuyPrice.text       = "£" + buyingPrices.ore.ToString();
        roboticonBuyPrice.text = "£" + market.GetRoboticonSellingPrice().ToString();
        
        foodSellPrice.text     = "£" + sellingPrices.food.ToString();
        energySellPrice.text   = "£" + sellingPrices.energy.ToString();
        oreSellPrice.text      = "£" + sellingPrices.ore.ToString();

        foodAmount.text = "/" + market.GetResources().food.ToString();
        energyAmount.text = "/" + market.GetResources().energy.ToString();
        oreAmount.text = "/" + market.GetResources().ore.ToString();
        roboticonAmount.text = "/" + market.GetNumRoboticonsForSale().ToString();
    }
}
