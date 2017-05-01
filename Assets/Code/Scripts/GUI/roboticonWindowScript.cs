//Game executable hosted at: http://www-users.york.ac.uk/~jwa509/executable.exe

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles UI interaction for the Roboticons List window
/// </summary>
public class roboticonWindowScript : MonoBehaviour
{
    public canvasScript canvas;
    public GameObject roboticonIconsList;   //Roboticon gui elements are added to this GUI content

    private GameObject roboticonTemplate;
    private List<GameObject> currentlyDisplayedRoboticonObjects = new List<GameObject>();
    private List<Roboticon> currentlyDisplayedRoboticons = new List<Roboticon>();
    private const string ROBOTICON_TEMPLATE_PATH = "Prefabs/GUI/TemplateRoboticon";

    // JBT fixed an error in this method where having the roboticons list open and no tile selected could cause the gui to stop responding / be drawn
    // JBT also added a list of roboticons to fix a bug where the install button would still appear when a roboticon was already installed
    /// <summary>
    /// Display a new set of roboticons to the GUI. Overwrite any previously displayed
    /// roboticons.
    /// </summary>
    /// <param name="roboticonsToDisplay"></param>
    public void DisplayRoboticonList(List<Roboticon> roboticonsToDisplay)
    {
        //Remove all roboticon from list and add new roboticons to currently displayed roboticons list
        currentlyDisplayedRoboticons = roboticonsToDisplay; // JBT
        ClearRoboticonList();

        gameObject.SetActive(true);

        //Display all the roboticons
        foreach (Roboticon roboticon in roboticonsToDisplay)
        {
            AddRoboticon(roboticon);
        }

        //If the current state = Purchase state then, display the Roboticon Upgrade buttons
        GameManager.States currentState = GameHandler.GetGameManager().GetCurrentState();
        if (currentState == GameManager.States.PURCHASE)
        {
            ShowRoboticonUpgradeButtons();
        }
        //Else if the current state = Installation state, and the tile selected is owned by the current player then show the roboticon install button
        else if(currentState == GameManager.States.INSTALLATION)
        {
            HumanGui humanGui = canvas.GetHumanGui();
            Tile CurrentSelectedTile = humanGui.GetCurrentSelectedTile(); // JBT
            if (CurrentSelectedTile != null) // JBT
            {
                if (CurrentSelectedTile.GetOwner() == humanGui.GetCurrentHuman())
                {
                    ShowRoboticonInstallButtons();
                }
            }
        }
        // else if the current state is neither instll or purchase then hide both the install and upgrade buttons
        else
        {
            HideInstallAndUpgradeButtons();
        }
    }

    public void HideRoboticonList()
    {
        ClearRoboticonList();
        currentlyDisplayedRoboticonObjects = new List<GameObject>();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Add a roboticon to the displayed list of roboticons in the UI.
    /// Returns the gameobject representing the roboticon in the scene.
    /// </summary>
    /// <param name="roboticon"></param>
    public void AddRoboticon(Roboticon roboticon)
    {
        LoadRoboticonTemplate();

        GameObject roboticonGuiObject = (GameObject)GameObject.Instantiate(roboticonTemplate);
        roboticonGuiObject.transform.SetParent(roboticonIconsList.transform, true);
        RectTransform guiObjectTransform = roboticonGuiObject.GetComponent<RectTransform>();

        guiObjectTransform.localScale = new Vector3(1, 1, 1);               //Undo Unity's instantiation meddling

        roboticonGuiElementScript roboticonElementScript = guiObjectTransform.GetComponent<roboticonGuiElementScript>();
        roboticonElementScript.SetRoboticon(roboticon);
        roboticonElementScript.SetButtonEventListeners(this);

        currentlyDisplayedRoboticonObjects.Add(roboticonGuiObject);
    }

    /// <summary>
    /// Show the Upgrade button for each roboticon in the window.
    /// </summary>
    public void ShowRoboticonUpgradeButtons()
    {
        foreach (GameObject roboticonElement in currentlyDisplayedRoboticonObjects)
        {
            roboticonElement.GetComponent<roboticonGuiElementScript>().ShowUpgradeButton();
        } 
    }

    //Edited by JBT to show an already installed text box, if a roboticon is already installed
    /// <summary>
    /// Show the install button for each roboticon in the window, or a text box explaining that the roboticon has already been installed
    /// </summary>
    public void ShowRoboticonInstallButtons()
    {
        for(int i = 0; i < currentlyDisplayedRoboticonObjects.Count; i++)
        {
            if(currentlyDisplayedRoboticons[i].IsInstalledToTile())
            {
                currentlyDisplayedRoboticonObjects[i].GetComponent<roboticonGuiElementScript>().ShowRemoveButton();
            }
            else
            {
                currentlyDisplayedRoboticonObjects[i].GetComponent<roboticonGuiElementScript>().ShowInstallButton();
            }        
        }
    }

    public void HideInstallAndUpgradeButtons()
    {
        foreach (GameObject roboticonElement in currentlyDisplayedRoboticonObjects)
        {
            roboticonElement.GetComponent<roboticonGuiElementScript>().HideButtons();
        }
    }

    public void UpgradeRoboticon(Roboticon roboticon)
    {
        canvas.ShowRoboticonUpgradesWindow(roboticon);
    }

    /// <summary>
    /// Install the given roboticon to the current selected tile.
    /// </summary>
    /// <param name="roboticon">The roboticon to install</param>
    public void InstallRoboticon(Roboticon roboticon)
    {
        canvas.InstallRoboticon(roboticon);
    }

    /// <summary>
    /// Uninstall the given roboticon from its tile
    /// </summary>
    /// <param name="roboticon">The roboticon to uninstall</param>
    public void UninstallRoboticon(Roboticon roboticon)
    {
        canvas.UninstallRoboticon(roboticon);
    }

    private void ClearRoboticonList()
    {
        if (currentlyDisplayedRoboticonObjects.Count > 0)
        {
            for (int i = currentlyDisplayedRoboticonObjects.Count - 1; i >= 0; i--)
            {
                Destroy(currentlyDisplayedRoboticonObjects[i]);
            }

            currentlyDisplayedRoboticonObjects = new List<GameObject>();
        }
    }

    /// <summary>
    /// Loads the roboticon template if not already loaded.
    /// </summary>
    private void LoadRoboticonTemplate()
    {
        if (roboticonTemplate == null)
        {
            roboticonTemplate = (GameObject)Resources.Load(ROBOTICON_TEMPLATE_PATH);

            if (roboticonTemplate == null)
            {
                throw new System.ArgumentException("Cannot find roboticon template at the specified path.");
            }
        }
    }
}
 