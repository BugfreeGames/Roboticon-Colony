//Game executable hosted at: http://www-users.york.ac.uk/~jwa509/executable.exe

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Contains information used to allow human players to see details about the current game state.
/// Contains logic used to allow human players to interact with the game.
/// </summary>
public class HumanGui
{
    public static GameObject humanGuiGameObject;

    private Human currentHuman;
    private GameManager.States currentPhase;
    private GameManager gameManager;

    private canvasScript canvas;
    private Tile currentSelectedTile;

    public const string ANIM_TRIGGER_FLASH_RED = "Flash Red";
    private const string humanGuiGameObjectPath = "Prefabs/GUI/Player GUI Canvas";

    /// <summary>
    /// Creates a new humanGUI from a prefab in the resources folder
    /// </summary>
    public HumanGui()
    {
        humanGuiGameObject = (GameObject)Resources.Load(humanGuiGameObjectPath);

        if(humanGuiGameObject == null)
        {
            throw new System.ArgumentException("Could not find human GUI GameObject at the specified path.");
        }
    }

    /// <summary>
    /// Gets the canvas attached to this humanGUI instance
    /// </summary>
    /// <returns>The canvas attached to this humanGUI instance</returns>
    public canvasScript GetCanvas()
    {
        return canvas;
    }

    /// <summary>
    /// Displays the GUI to the screen, using the current humans details
    /// </summary>
    /// <param name="human">The current human using the GUI</param>
    /// <param name="phase">The current phase</param>
    public void DisplayGui(Human human, GameManager.States phase)
    {
        currentHuman = human;
        currentPhase = phase;

        canvas.ShowMarketButton();
        canvas.ShowRoboticonButton();
        
        ShowHelpBox();

        UpdateResourceBar(false);
        canvas.RefreshRoboticonList();
        canvas.RefreshTileInfoWindow();
        canvas.HideGamblingWindow();
        canvas.HideMarketWindow();
        canvas.HideRoboticonWindow();
        canvas.HideAuctionBuyWindow();
        canvas.HideAuctionSellWindow();
        canvas.HideAITurnText();

        if(phase == GameManager.States.PRODUCTION)
        {
            canvas.DisableEndPhaseButton();
        }
        else
        {
            canvas.EnableEndPhaseButton();
        }

        //Added by JBT - enables or disables gambling, auction, market and roboticon buttons depending on the phase, as the buttons are only used in the production and installation phase
        if (phase == GameManager.States.PURCHASE)
        {
            canvas.HideAuctionButton();
            canvas.ShowGambleButton();
            canvas.ShowMarketButton();
            canvas.ShowRoboticonButton();
        }
        else if(phase == GameManager.States.INSTALLATION)
        {
            canvas.HideAuctionButton();
            canvas.HideGambleButton();
            canvas.HideMarketButton();
            canvas.ShowRoboticonButton();
        }
        else if(phase == GameManager.States.AUCTIONLIST || phase == GameManager.States.AUCTIONBID)
        {
            canvas.ShowAuctionButton();
            canvas.HideGambleButton();
            canvas.HideMarketButton();
            canvas.HideRoboticonButton();
        }
        else
        {
            canvas.HideAuctionButton();
            canvas.HideGambleButton();
            canvas.HideMarketButton();
            canvas.HideRoboticonButton();
        }

        canvas.SetCurrentPhaseText(GameManager.StateToPhaseName(phase) + " Phase");
    }

    public void DisplayRandomEventInfo(RandomEvent randomEvent)
    {
        if(!randomEvent.isNullEvent)
        {
            canvas.DisplayNewEventMessage(randomEvent);
        }
    }

    //Added by JBT to display information about the current AI
    /// <summary>
    /// Displays information about the AI for a few seconds, to simulate the AI taking a turn
    /// </summary>
    /// <param name="ai">The AI player to display details fo</param>
    /// <param name="phase">The current phase</param>
    public void DisplayAIInfo(AI ai, GameManager.States phase)
    {
        currentHuman = null;
        currentPhase = phase;
        canvas.HideGamblingWindow();
        canvas.HideMarketWindow();
        canvas.HideGambleButton();
        canvas.HideRoboticonWindow();
        canvas.HideAuctionBuyWindow();
        canvas.HideAuctionSellWindow();
        canvas.HideMarketButton();
        canvas.HideRoboticonButton();
        canvas.HideAuctionButton();
        canvas.HideTileInfoWindow();
        canvas.SetAITurnText(ai.GetName() + " is thinking...");
        UpdateResourceBar(true);
    }

    /// <summary>
    /// Sets the name for the current player to the string that is provided
    /// </summary>
    /// <param name="name">The new name to give the player</param>
    public void SetCurrentPlayerName(string name)
    {
        canvas.SetCurrentPlayerName(name);
    }

    /// <summary>
    /// Ends the phase for the current player
    /// </summary>
    public void EndPhase()
    {
        gameManager.CurrentPlayerEndTurn();
    }

    /// <summary>
    /// Disables the GUI for the current player
    /// </summary>
    public void DisableGui()
    {
        currentHuman = new Human(new ResourceGroup(), "", 0);
        UpdateResourceBar(false);    //This will reset all resource values to 0.
        canvas.HideRoboticonUpgradesWindow();

        canvas.DisableEndPhaseButton();
    }

    /// <summary>
    /// Attemps to purchase the given tile for the current player
    /// </summary>
    /// <param name="tile"></param>
    public void PurchaseTile(Tile tile)
    {
        try
        {
            currentHuman.AcquireTile(tile);
            UpdateResourceBar(false);
        }
        catch(System.InvalidOperationException)
        {
            canvas.tileWindow.PlayPurchaseDeclinedAnimation();
        }
    }

    /// <summary>
    /// Enables the current player to buy resources and roboticons from the market for a given price
    /// </summary>
    /// <param name="resourcesToBuy">The amount of resources being bought</param>
    /// <param name="roboticonsToBuy">The amount of roboticons being bought</param>
    /// <param name="buyPrice">The buy price for the resources</param>
    public void BuyFromMarket(ResourceGroup resourcesToBuy, int roboticonsToBuy, int buyPrice)
    {        
        if (currentHuman.GetMoney() >= buyPrice)
        {
            try
            {
                gameManager.market.BuyFrom(resourcesToBuy, roboticonsToBuy);
            }
            catch (System.ArgumentException e)
            {
                canvas.marketScript.PlayPurchaseDeclinedAnimation();
                return;
            }

            currentHuman.SetMoney(currentHuman.GetMoney() - buyPrice);

            for(int i = 0; i < roboticonsToBuy; i ++)
            {
                Roboticon newRoboticon = new Roboticon();
                currentHuman.AcquireRoboticon(newRoboticon);
                canvas.AddRoboticonToList(newRoboticon);
            }

            ResourceGroup currentResources = currentHuman.GetResources();
            currentHuman.SetResources(currentResources + resourcesToBuy);

            UpdateResourceBar(false);
        }
        else
        {
            canvas.marketScript.PlayPurchaseDeclinedAnimation();
        }
    }

    /// <summary>
    /// Enables the current player to sell resources to the market for a given price
    /// </summary>
    /// <param name="resourcesToSell">The resources to sell</param>
    /// <param name="sellPrice">The prices that the resources are being sold for</param>
    public void SellToMarket(ResourceGroup resourcesToSell, int sellPrice)
    {
        ResourceGroup humanResources = currentHuman.GetResources();
        bool humanHasEnoughResources =
            humanResources.food   >= resourcesToSell.food &&
            humanResources.energy >= resourcesToSell.energy &&
            humanResources.ore    >= resourcesToSell.ore;

        if(humanHasEnoughResources)
        {
            try
            {
                gameManager.market.SellTo(resourcesToSell);
            }
            catch (System.ArgumentException)
            {
                canvas.marketScript.PlaySaleDeclinedAnimation();
                return;
            }

            currentHuman.SetMoney(currentHuman.GetMoney() + sellPrice);

            ResourceGroup currentResources = currentHuman.GetResources();
            currentHuman.SetResources(currentResources - resourcesToSell);

            UpdateResourceBar(false);
        }
        else
        {
            canvas.marketScript.PlaySaleDeclinedAnimation();
        }
    }

    /// <summary>
    /// Gets the current players list of owned roboticons
    /// </summary>
    /// <returns>The current players list of owned roboticons</returns>
    public List<Roboticon> GetCurrentHumanRoboticonList()
    {
        return currentHuman.GetRoboticons();
    }

    /// <summary>
    /// Returns the Human instance of the current human player
    /// </summary>
    /// <returns>The current human player</returns>
    public Human GetCurrentHuman()
    {
        return currentHuman;
    }

    /// <summary>
    /// Sets the gameManager of this instance to the reference provided
    /// </summary>
    /// <param name="gameManager">The gamemanager to set</param>
    public void SetGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    /// <summary>
    /// Sets the canvasScript of this instance to the reference provided
    /// </summary>
    /// <param name="canvas">The canvas to set</param>
    public void SetCanvasScript(canvasScript canvas)
    {
        this.canvas = canvas;
    }

    /// <summary>
    /// Populates the tile information window with details about the provided tile
    /// </summary>
    /// <param name="tile">The tile to populate the information window with</param>
    public void DisplayTileInfo(Tile tile)
    {
        currentSelectedTile = tile;     //Selection of a tile always passes through here
        canvas.ShowTileInfoWindow(tile);
    }

    public void RefreshTileInfoWindow()
    {
        canvas.RefreshTileInfoWindow();
    }

    /// <summary>
    /// Gets the reference to the current selected tile in the UI
    /// </summary>
    /// <returns>The current selected tile</returns>
    public Tile GetCurrentSelectedTile()
    {
        return currentSelectedTile;
    }

    /// <summary>
    /// Upgrades a roboticon that the current player owns with a provided upgrade
    /// </summary>
    /// <param name="roboticon">The roboticon to upgrade</param>
    /// <param name="upgrades">The upgrade to apply</param>
    public void UpgradeRoboticon(Roboticon roboticon, ResourceGroup upgrades)
    {
        Player currentPlayer = GameHandler.GetGameManager().GetCurrentPlayer();
        int upgradeCost = (upgrades * Roboticon.UPGRADEVALUE).Sum();

        if (currentPlayer.GetMoney() >= upgradeCost)
        {
            currentPlayer.SetMoney(currentPlayer.GetMoney() - upgradeCost);
            roboticon.Upgrade(upgrades);
            UpdateResourceBar(false);
            canvas.ShowRoboticonUpgradesWindow(roboticon);
            canvas.RefreshTileInfoWindow();
        }
    }

    /// <summary>
    /// Installs a roboticon to a tile that the current human owns
    /// </summary>
    /// <param name="roboticon">The roboticon to install</param>
    public void InstallRoboticon(Roboticon roboticon)
    {
        if (currentSelectedTile.GetOwner() == currentHuman)
        {
            //Added by JBT - only install a roboticon to a tile if the roboticon is not already installed to a tile
            if (!roboticon.IsInstalledToTile())
            {
                try
                {
                    currentHuman.InstallRoboticon(roboticon, currentSelectedTile);
                }
                catch (System.InvalidOperationException)
                {
                    throw new System.InvalidOperationException("Tried to install a roboticon to a tile which already has a roboticon");
                }

                canvas.RefreshTileInfoWindow();
            }
            else
            {
                throw new System.InvalidOperationException("Tried to install a roboticon which is already installed");
            }
        }
        else
        {
            throw new System.InvalidOperationException("Tried to install roboticon to tile which is not owned by the current player. This should not happen");
        }
    }

    //Added by JBT to support the uninstallation of roboticons from tiles
    /// <summary>
    /// Uninstalls a roboticon from a tile that the current human owns
    /// </summary>
    /// <param name="roboticon">The roboticon to uninstall</param>
    public void UninstallRoboticon(Roboticon roboticon)
    {
        if(roboticon.IsInstalledToTile())
        {
            try
            {
                currentHuman.UninstallRoboticon(roboticon.InstalledTile);
            }
            catch (System.InvalidOperationException)
            {
                throw new System.InvalidOperationException("Tried to uninstall a roboticon from a tile which has no roboticon");
            }

            canvas.RefreshTileInfoWindow();
        }
        else
        {
            throw new System.InvalidOperationException("This roboticon is not installed on a tile. This should not happen");
        }
    }

    //Changed by JBT to show different values depending on if the current player is an AI
    /// <summary>
    /// Updates the resource bar, populating it with values to reflect the current players resources
    /// </summary>
    /// <param name="aiTurn">True if the current player is AI, false otherwise</param>
    public void UpdateResourceBar(bool aiTurn)
    {
        if (aiTurn)
        {
            canvas.SetUnknownResourceLabels();
            canvas.SetUnknownChangeLabels();
        }
        else
        {
            canvas.SetResourceLabels(currentHuman.GetResources(), currentHuman.GetMoney());
            canvas.SetResourceChangeLabels(currentHuman.CalculateTotalResourcesGenerated());
        }
    }

    /// <summary>
    /// Shows the help box in the GUI
    /// </summary>
    private void ShowHelpBox()
    {
        canvas.ShowHelpBox(GuiTextStore.GetHelpBoxText(currentPhase));
    }

    /// <summary>
    /// Hides the help box shown in the GUI
    /// </summary>
    private void HideHelpBox()
    {
        canvas.HideHelpBox();
    }
}
