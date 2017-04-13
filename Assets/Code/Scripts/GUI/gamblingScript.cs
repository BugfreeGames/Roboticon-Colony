//Game executable hosted by JBT at: http://robins.tech/jbt/documents/assthree/GameExecutable.zip

using UnityEngine;
using UnityEngine.UI;

//Added by JBT for gambling support in purchase phase
/// <summary>
/// Handles UI related to the gambling window
/// </summary>
public class gamblingScript : MonoBehaviour {

    public Text marketBalance;
    public Text resultText;

    private Market market;

    void Awake()
    {
        market = GameHandler.GetGameManager().market;
    }
	
    /// <summary>
    /// Called when the play button is clicked on the gambling UI. Rolls a number between 1-100. Gives the current player double the amount back if they win.
    /// </summary>
    /// <param name="amount">The amount to gamble with</param>
	public void PlayDoubleOrNothingButtonClicked(int amount)
    {
        Human currentPlayer = GameHandler.gameManager.GetHumanGui().GetCurrentHuman();

        //If the player doesn't have the requried amount of money then...
        if(currentPlayer.GetMoney() <= amount)
        {
            SetResultText("You need at least £10 to play!", false);
            return;
        }

        //Take the money from the player
        currentPlayer.SetMoney(currentPlayer.GetMoney() - amount);

        try
        {
            int valueRolled;
            //Check if the player has won.
            bool won = market.DoubleOrNothing(amount, 0, 100, out valueRolled);

            //Display the result to the player
            SetResultText("You rolled: " + valueRolled.ToString() + "\n" + (won ? "You won £" + (amount * 2).ToString() + "!" : "You Lose!"), won);

            //If the player has won, add the won money to the player.
            if (won)
            {
                currentPlayer.SetMoney(currentPlayer.GetMoney() + (amount * 3));
            }
        }
        catch(System.ArgumentException e)
        {
            SetResultText(e.Message,false);
            currentPlayer.SetMoney(currentPlayer.GetMoney() + amount);
        }

        ResetGamblingWindow();
        GameHandler.gameManager.GetHumanGui().UpdateResourceBar(false);
    }

    /// <summary>
    /// Refreshes the Gambling and market windows with up-to date information. 
    /// </summary>
    public void ResetGamblingWindow()
    {
        RefreshMarketBalance();

        GameObject marketMenu = GameObject.Find("Market Menu");

        //Refresh the balance on the market menu if it is open
        if (marketMenu != null)
        {
            marketMenu.GetComponent<marketScript>().RefreshMarketBalance();
        }
    }

    /// <summary>
    /// Refreshes the Market window's balance
    /// </summary>
    public void RefreshMarketBalance()
    {
        marketBalance.text = "Market has £" + market.GetMoney();
    }

    /// <summary>
    /// Opens the gambling window. 
    /// </summary>
    public void OpenGamblingWindow()
    {
        resultText.text = "";
        RefreshMarketBalance();
    }

    /// <summary>
    /// Sets the text of the result textbox
    /// </summary>
    /// <param name="text">The text to display</param>
    /// <param name="green">Displays the text in green if true, red otherwise</param>
    private void SetResultText(string text, bool green)
    {
        string output = "";

        if(green)
        {
            output += "<color=green>";
        }
        else
        {
            output += "<color=red>";
        }

        output += text;
        output += "</color>";
        resultText.text = output;
    }
}
