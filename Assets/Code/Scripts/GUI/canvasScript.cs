//Game executable hosted by JBT at: http://robins.tech/jbt/documents/assthree/GameExecutable.zip

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// Handles interaction between UI elements within the PlayerGUI prefab
/// </summary>
public class canvasScript : MonoBehaviour
{
    public helpBoxScript helpBox;
    public GameObject optionsMenu;
    public GameObject gambleButton; //JBT
    public GameObject marketButton; //JBT
    public GameObject roboticonButton;  //JBT
    public GameObject auctionButton;    //JBT
    public roboticonWindowScript roboticonList;
    public GameObject gamblingWindow;
    public marketScript marketScript;
    public GameObject endPhaseButton;
    public GameObject auctionSellWindow; //JBT
    public GameObject auctionBuyWindow; //JBT
    public tileInfoWindowScript tileWindow;
    public Text currentPlayerText;
    public Text currentPhaseText;
    public Text timeoutText;
    public GameObject aiTurnBox;
    public Text aiTurnText;
    public roboticonUpgradesWindowScript roboticonUpgradesWindow;
    public Text NewEventTitle; //JBT Title of event displayed to user
    public Text NewEventDescription; //JBT Description of event displayed to user
    public GameObject NewEventMessage; //JBT UI element displayed when a new event is started.
    private Timeout CurrentPhaseTimeout; //JBT used to limit phase durations
    private Timeout EventMessageTimeout; //JBT used to display the new event message for a few seconds
    private GameObject eventSystem; //JBT used to create and track the eventsystem in the scene

    /// <summary>
    /// Tracks all of the player's resources within the UI
    /// </summary>
    #region Resource Labels
    public Text foodLabel;
    public Text foodChangeLabel;
    public Text energyLabel;
    public Text energyChangeLabel;
    public Text oreLabel;
    public Text oreChangeLabel;
    public Text moneyLabel;
    #endregion

    private HumanGui humanGui;
     
    // JBT created this method
    void Update()
    {
        if (CurrentPhaseTimeout != null)
        {
            // We are in a timed phase, update the display timer
            ShowTimeout(CurrentPhaseTimeout);

            /// If the timeout is finished then...
            if (CurrentPhaseTimeout.Finished)
            {
                //Set the phase time to null
                CurrentPhaseTimeout = null;

                //Move the game onto the next phase
                if (GameHandler.gameManager.GetCurrentPlayer() is Human)
                {
                    EndPhase();
                }
                else if(GameHandler.gameManager.GetCurrentPlayer() is AI)
                {
                    GameHandler.gameManager.GetCurrentPlayer().Act(GameHandler.gameManager.GetCurrentState());
                }
            }
        }
        //Else if the phase is not timed then...
        else
        {
            //Hide the phase timer
            HidePhaseTimeout();
        }

        //If there is an event in effect then...
        if (EventMessageTimeout != null)
        {
            //Check if the event's message should still be displayed
            if (EventMessageTimeout.Finished)
            {
                NewEventMessage.SetActive(false);
                EventMessageTimeout = null;
            }
        }
    }

    // JBT Created this method
    /// <summary>
    /// Dispaly information about a new event that has started for a few seconds
    /// </summary>
    /// <param name="newEvent">The event that has started</param>
    public void DisplayNewEventMessage(RandomEvent newEvent)
    {
        NewEventTitle.text = newEvent.getTitle();
        NewEventDescription.text = newEvent.getDescription();
        NewEventMessage.SetActive(true);
        EventMessageTimeout = new Timeout(8);
    }

    // JBT created this method
    /// <summary>
    /// Set a timout for the current phase
    /// </summary>
    public void SetPhaseTimeout(Timeout timeout)
    {
        if (timeout != null && timeout.Finished)
        {
            throw new ArgumentException("Need a fresh timeout");
        }
        CurrentPhaseTimeout = timeout;
    }

    /// <summary>
    /// Ends the current phase
    /// </summary>
    public void EndPhase()
    {
        CurrentPhaseTimeout = null;
        humanGui.EndPhase();
    }

    /// <summary>
    /// Disables the end of phase button
    /// </summary>
    public void DisableEndPhaseButton()
    {
        endPhaseButton.SetActive(false);
    }

    /// <summary>
    /// Enables the end of phase button
    /// </summary>
    public void EnableEndPhaseButton()
    {
        endPhaseButton.SetActive(true);
    }

    /// <summary>
    /// Sets the current phase text label to the specified text string
    /// </summary>
    /// <param name="text">The string that the current phase text should be set to</param>
    public void SetCurrentPhaseText(string text)
    {
        currentPhaseText.text = text;
    }

    /// <summary>
    /// Attempts to buy a resource group from the market. 
    /// </summary>
    /// <param name="resources">The resources the player is attempting to buy</param>
    /// <param name="roboticonsToBuy">The number of roboticons the player is attempting to buy</param>
    /// <param name="price">The price for all of these resources</param>
    public void BuyFromMarket(ResourceGroup resources, int roboticonsToBuy, int price)
    {
        humanGui.BuyFromMarket(resources, roboticonsToBuy, price);
        RefreshRoboticonList(); //Added by JBT to fix a bug when buying roboticons with the roboticon list open was not creating the GUI elements correctly
    }

    /// <summary>
    /// Attempts to sell a resource group to the market
    /// </summary>
    /// <param name="resources">The resource group being sold</param>
    /// <param name="price">The price for the resource group</param>
    public void SellToMarket(ResourceGroup resources, int price)
    {
        humanGui.SellToMarket(resources, price);
    }

    //Added by JBT 
    /// <summary>
    /// Shows or hides the gambling window depending on the state of the window. 
    /// Window open -> close window
    /// Window close -> open window
    /// </summary>
    public void GamblingButtonPressed()
    {
        if(gamblingWindow.activeSelf)
        {
            HideGamblingWindow();
        }
        else
        {
            ShowGamblingWindow();
        }
    }
    //Added by JBT 
    /// <summary>
    /// Shows or hides the market window depending on the state of the window. 
    /// Window open -> close window
    /// Window close -> open window
    /// </summary>
    public void MarketButtonPressed()
    {
        if (marketScript.gameObject.activeSelf)
        {
            HideMarketWindow();
        }
        else
        {
            ShowMarketWindow();
            marketScript.SetShownMarketPrices();
        }
    }

    //Added by JBT 
    /// <summary>
    /// Shows or hides the roboticon window depending on the state of the window. 
    /// Window open -> close window
    /// Window close -> open window
    /// </summary>
    public void RoboticonButtonPressed()
    {
        if(roboticonList.gameObject.activeSelf)
        {
            HideRoboticonWindow();
        }
        else
        {
            ShowRoboticonWindow();
        }
    }

    //Added by JBT 
    /// <summary>
    /// Shows or hides the option window depending on the state of the window. 
    /// Window open -> close window
    /// Window close -> open window
    /// </summary>
    public void OptionsButtonPressed()
    {
        if (optionsMenu.activeSelf)
        {
            HideOptionsMenu();
        }
        else
        {
            ShowOptionsMenu();
        }
    }

    //Added by JBT - Show or hide the auction window depending on the state the window is in when the button is pressed
    /// <summary>
    /// Shows the auction menu if it is closed and hides it if it is open
    /// </summary>
    public void AuctionButtonPressed()
    {
        if (GameHandler.GetGameManager().GetCurrentState() == GameManager.States.AUCTIONBID)
        {
            if (auctionBuyWindow.activeSelf)
            {
                HideAuctionBuyWindow();
            }
            else
            {
                ShowAuctionBuyWindow();
            }
        }
        else if (GameHandler.GetGameManager().GetCurrentState() == GameManager.States.AUCTIONLIST)
        {
            if (auctionSellWindow.activeSelf)
            {
                HideAuctionSellWindow();
            }
            else
            {
                ShowAuctionSellWindow();
            }
        }        
    }

    /// <summary>
    /// Opens the market window if the game state is purchase
    /// </summary>
    public void ShowMarketWindow()
    {
        if (GameHandler.GetGameManager().GetCurrentState() == GameManager.States.PURCHASE)
        {
            marketScript.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Hides the market window
    /// </summary>
    public void HideMarketWindow()
    {
        marketScript.gameObject.SetActive(false);
    }

    //Added by JBT
    /// <summary>
    /// Show the gambling window if the current game state is purchase.
    /// </summary>
    public void ShowGamblingWindow()
    {
        if (GameHandler.GetGameManager().GetCurrentState() == GameManager.States.PURCHASE)
        {
            gamblingWindow.SetActive(true);
            gamblingWindow.GetComponent<gamblingScript>().OpenGamblingWindow();
        }
    }

    //Added by JBT
    /// <summary>
    /// Hides the gambling window
    /// </summary>
    public void HideGamblingWindow()
    {
        gamblingWindow.SetActive(false);
    }

    /// <summary>
    /// Shows the roboticon window
    /// </summary>
    public void ShowRoboticonWindow()
    {
        roboticonList.gameObject.SetActive(true);
        ShowRoboticonList();
    }
    /// <summary>
    /// Hides the roboticon window
    /// </summary>
    public void HideRoboticonWindow()
    {
        roboticonList.gameObject.SetActive(false);
    }

    //JBT
    public void ShowAuctionSellWindow()
    {
        auctionSellWindow.GetComponent<auctionSellWindowScript>().LoadWindow();
        auctionSellWindow.SetActive(true);
    }

    //JBT
    public void HideAuctionSellWindow()
    {
        auctionSellWindow.SetActive(false);
    }

    //JBT
    public void ShowAuctionBuyWindow()
    {
        auctionBuyWindow.GetComponent<AuctionBuyWindowScript>().LoadAuction();
        auctionBuyWindow.SetActive(true);
    }

    //JBT
    public void HideAuctionBuyWindow()
    {
        auctionBuyWindow.SetActive(false);
    }

    //JBT
    /// <summary>
    /// Shows the market button
    /// </summary>
    public void ShowMarketButton()
    {
        marketButton.SetActive(true);
    }

    //JBT
    /// <summary>
    /// Hides the market button
    /// </summary>
    public void HideMarketButton()
    {
        marketButton.SetActive(false);
    }

    //JBT
    /// <summary>
    /// Shows the gamble button
    /// </summary>
    public void ShowGambleButton()
    {
        gambleButton.SetActive(true);
    }

    //JBT
    /// <summary>
    /// Hides the gamble button
    /// </summary>
    public void HideGambleButton()
    {
        gambleButton.SetActive(false);
    }

    //Added by JBT
    /// <summary>
    /// Shows the current player the amount of seconds they have left in the current turn
    /// </summary>
    /// <param name="t">The current timeout</param>
    public void ShowTimeout(Timeout t)
    {
        timeoutText.gameObject.SetActive(true);
        timeoutText.text = t.SecondsRemaining.ToString("00");
        if(t.SecondsRemaining < 5)
        {
            timeoutText.color = Color.red;
        }
        else
        {
            timeoutText.color = Color.white;
        }
    }

    //Added by JBT 
    /// <summary>
    /// Hides the timeout
    /// </summary>
    public void HidePhaseTimeout()
    {
        timeoutText.gameObject.SetActive(false);
    }

    //JBT
    /// <summary>
    /// Shows the Roboticon button
    /// </summary>
    public void ShowRoboticonButton()
    {
        roboticonButton.SetActive(true);
    }

    //JBT
    /// <summary>
    /// Hides the roboticon button
    /// </summary>
    public void HideRoboticonButton()
    {
        roboticonButton.SetActive(false);
    }

    //JBT
    /// <summary>
    /// Shows the auction button
    /// </summary>
    public void ShowAuctionButton()
    {
        auctionButton.SetActive(true);
    }

    //JBT
    /// <summary>
    /// Hide the auction button
    /// </summary>
    public void HideAuctionButton()
    {
        auctionButton.SetActive(false);
    }

    /// <summary>
    /// Shows the option menu
    /// </summary>
    public void ShowOptionsMenu()
    {
        optionsMenu.SetActive(true);
    }

    /// <summary>
    /// Hides the option menu
    /// </summary>
    public void HideOptionsMenu()
    {
        optionsMenu.SetActive(false);
    }

    /// <summary>
    /// Purchases the selected tile and refreshes the tile window
    /// </summary>
    /// <param name="tile">The selected tile</param>
    public void PurchaseTile(Tile tile)
    {
        humanGui.PurchaseTile(tile);
        tileWindow.Refresh();
    }

    /// <summary>
    /// Shows the selected tile info window
    /// </summary>
    /// <param name="tile">The selected tile</param>
    public void ShowTileInfoWindow(Tile tile)
    {
        tileWindow.Show(tile);
    }

    /// <summary>
    /// Refreshes the tile info window with up-to date information
    /// </summary>
    public void RefreshTileInfoWindow()
    {
        tileWindow.Refresh();
    }

    /// <summary>
    /// Hides the selected tile info window
    /// </summary>
    public void HideTileInfoWindow()
    {
        tileWindow.Hide();
    }

    /// <summary>
    /// Refreshes the Roboticon list window with accurate information.
    /// </summary>
    public void RefreshRoboticonList()
    {
        if (roboticonList.isActiveAndEnabled)
        {
            ShowRoboticonList();
        }
    }

    /// <summary>
    /// Shows the roboticon list window and initialises it's information
    /// </summary>
    public void ShowRoboticonList()
    {
        List<Roboticon> roboticonsToDisplay = new List<Roboticon>();

        foreach(Roboticon roboticon in humanGui.GetCurrentHumanRoboticonList())
        {
            roboticonsToDisplay.Add(roboticon);
        }

        roboticonList.DisplayRoboticonList(roboticonsToDisplay);
    }

    /// <summary>
    /// Adds a roboticon to the roboticon display list if it is currently
    /// being displayed.
    /// </summary>
    public void AddRoboticonToList(Roboticon roboticon)
    {
        if (roboticonList.isActiveAndEnabled)
        {
            roboticonList.AddRoboticon(roboticon);
        }
    }

    /// <summary>
    /// Hides the roboticon list window
    /// </summary>
    public void HideRoboticonList()
    {
        roboticonList.HideRoboticonList();
    }

    /// <summary>
    /// Shows the roboticon upgrades window
    /// </summary>
    /// <param name="roboticon"></param>
    public void ShowRoboticonUpgradesWindow(Roboticon roboticon)
    {
        roboticonUpgradesWindow.Show(roboticon);
    }

    /// <summary>
    /// Hides the roboticon upgrades window
    /// </summary>
    public void HideRoboticonUpgradesWindow()
    {
        roboticonUpgradesWindow.Hide();
    }
    
    /// <summary>
    /// Upgrades the selected roboticon with the selected upgrades
    /// </summary>
    /// <param name="roboticon">The selected roboticon</param>
    /// <param name="upgrades">The selected upgrades</param>
    public void UpgradeRoboticon(Roboticon roboticon, ResourceGroup upgrades)
    {
        humanGui.UpgradeRoboticon(roboticon, upgrades);
    }

    /// <summary>
    /// Installs the selected robotcion onto the selected tile
    /// </summary>
    /// <param name="roboticon">The selected robotcion</param>
    public void InstallRoboticon(Roboticon roboticon)
    {
        humanGui.InstallRoboticon(roboticon);
    }

    //Added by JBT 
    /// <summary>
    /// Uninstalls the selected roboticon from it's assigned tile
    /// </summary>
    /// <param name="roboticon">The selected roboticon</param>
    public void UninstallRoboticon(Roboticon roboticon)
    {
        humanGui.UninstallRoboticon(roboticon);
    }

    public void SetCurrentPlayerName(string name)
    {
        currentPlayerText.text = name;
    }

    public void ShowHelpBox(string helpBoxText)
    {
        helpBox.ShowHelpBox(helpBoxText);
    }

    public void HideHelpBox()
    {
        helpBox.HideHelpBox();
    }

    public void SetResourceLabels(ResourceGroup resources, int money)
    {
        foodLabel.text = resources.food.ToString();
        energyLabel.text = resources.energy.ToString();
        oreLabel.text = resources.ore.ToString();
        moneyLabel.text = money.ToString();
    }

    public void SetResourceChangeLabels(ResourceGroup resources)
    {
        foodChangeLabel.text = FormatResourceChangeLabel(resources.food);
        energyChangeLabel.text = FormatResourceChangeLabel(resources.energy);
        oreChangeLabel.text = FormatResourceChangeLabel(resources.ore);
    }

    //Created by JBT 
    /// <summary>
    /// Sets the resource labels for the AI's turn
    /// </summary>
    public void SetUnknownResourceLabels()
    {
        foodLabel.text = "??";
        energyLabel.text = "??";
        oreLabel.text = "??";
        moneyLabel.text = "??";
    }

    //Created by JBT 
    /// <summary>
    /// Sets the resource change labels for the AI's turn
    /// </summary>
    public void SetUnknownChangeLabels()
    {
        foodChangeLabel.text = "+??";
        energyChangeLabel.text = "+??";
        oreChangeLabel.text = "+??";
    }

    //Created by JBT 
    /// <summary>
    /// Shows the AI turn box window and sets the AI turn text to the specified string
    /// </summary>
    /// <param name="t">The specified string</param>
    public void SetAITurnText(string t)
    {
        aiTurnBox.SetActive(true);
        aiTurnText.text = t;
    }

    //Created by JBT 
    /// <summary>
    /// Hides the AI turn box window
    /// </summary>
    public void HideAITurnText()
    {
        aiTurnBox.gameObject.SetActive(false);
    }

    public void SetHumanGui(HumanGui gui)
    {
        humanGui = gui;
    }

    public HumanGui GetHumanGui()
    {
        return humanGui;
    }

    private string FormatResourceChangeLabel(int changeAmount)
    {
        string sign = (changeAmount >= 0) ? "+" : "";
        return "(" + sign + changeAmount.ToString() + ")";
    }

    //Added by JBT 
    /// <summary>
    /// Quits the game and returns to the main menu
    /// Loses all player progress
    /// </summary> 
    public void QuitToMenu()
    {
        //Remove gameobjects that would not get destroyed on load
        Destroy(GameObject.Find("GameManager"));
        Destroy(GameObject.Find("Tile Holder"));
        Destroy(GameObject.Find("Map Manager"));

        //Go to the main menu
        SceneManager.LoadScene(0);
    }
}
