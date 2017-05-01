//Game executable hosted at: http://www-users.york.ac.uk/~jwa509/executable.exe

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

//Made by JBT
/// <summary>
/// Handles UI interaction for the Main Menu scene
/// </summary>
public class mainMenuScript : MonoBehaviour
{
    public const int GAME_SCENE_INDEX = 1;

    public mainMenuManager menuManager;
    public Toggle player2aiToggle;
    public Toggle player3aiToggle;
    public Toggle player4aiToggle;
    public InputField player1Name;
    public InputField player2Name;
    public InputField player3Name;
    public InputField player4Name;

    public GameObject eventSystem;
    public GameObject addPlayerButton;
    public GameObject removePlayerButton;
    public GameObject guiAssets;

    public string gameName = "game";
    private int numPlayersShown = 2;
    private List<setupPlayer> setupPlayers;
    private List<Player> players;

    /// <summary>
    /// Assessment 4:- Added setupPlayer to simplify creation of multiple players for the 4 player requirement.
    /// </summary>
    private struct setupPlayer
    {
        InputField nameField;
        Toggle isAi;
        int playerNum; //Used to set the default name

        public setupPlayer(InputField field, Toggle toggle, int playerNum)
        {
            nameField = field;
            isAi = toggle;
            this.playerNum = playerNum;
        }

        public bool isAI()
        {
            if(isAi == null)
            {
                return false;
            }

            return isAi.isOn;
        }

        public string getName()
        {
            return nameField.text;
        }

        public void setNameToDefaultIfNoNameSet()
        {
            if(nameField.text == "" || nameField.text == null)
            {
                nameField.text = "Player" + playerNum.ToString();
            }
        } 

        public bool isDisabled()
        {
            return !nameField.IsActive();
        }

        public Color getColor()
        {
            switch(playerNum)
            {
                case 1:
                    return new Color(1, 0, 0);

                case 2:
                    return new Color(0, 1, 1);

                case 3:
                    return new Color(0, 0, 1);

                case 4:
                    return new Color(1, 0, 1);

                default:
                    throw new System.ArgumentException("SetupPlayer has no assigned player number. Player number is necessary for generating default name and color.");
            }
        }

        public void Show()
        {
            nameField.transform.parent.gameObject.SetActive(true);
        }

        public void Hide()
        {
            nameField.transform.parent.gameObject.SetActive(false);
        }
    }

    public void Start()
    {
        //If a player is quitting to the menu from the game itself, or the end of game screen, then remove the GUI canvas that was not destroyed on load
        if (GameObject.Find("Player GUI Canvas(Clone)") != null)
        {
            Destroy(GameObject.Find("Player GUI Canvas(Clone)"));
        }

        setupPlayers = new List<setupPlayer>();
        setupPlayer player1 = new setupPlayer(player1Name, null, 1);
        setupPlayer player2 = new setupPlayer(player2Name, player2aiToggle, 2);
        setupPlayer player3 = new setupPlayer(player3Name, player3aiToggle, 3);
        setupPlayer player4 = new setupPlayer(player4Name, player4aiToggle, 4);

        setupPlayers.Add(player1);
        setupPlayers.Add(player2);
        setupPlayers.Add(player3);
        setupPlayers.Add(player4);
    }

    /// <summary>
    /// Assessment 4:- Called when the play button is clicked
    /// Changed to go through this function to allow for an animation prior to starting the game
    /// </summary>
    public void PlayButtonClicked()
    {
        CreatePlayerList();
        StartCoroutine(DelayStartGame());
        menuManager.PlayShipLandAnimation();
    }

    /// <summary>
    /// Assessment 4:- Added this to hide the menu until the play button has been clicked in the 3D menu.
    /// </summary>
    public void HideMenu()
    {
        guiAssets.SetActive(false);
    }

    /// <summary>
    /// Assessment 4:- Added this to show the menu when the play button has been clicked in the 3D menu.
    /// </summary>
    public void ShowMenu()
    {
        guiAssets.SetActive(true);
    }

    /// <summary>
    /// Assessment 4:- Moved code from StartGame into this function so that player list may be 
    /// generated before the pre-game animation plays 
    /// </summary>
    public void CreatePlayerList()
    {
        players = new List<Player>();

        foreach (setupPlayer player in setupPlayers)
        {
            //If no player name entered, set default player names
            player.setNameToDefaultIfNoNameSet();

            //If the current player is disabled, don't add it to the players list and break as all future players must also be disabled.
            if (player.isDisabled())
            {
                break;
            }

            //If AI Is on, then add an AI player, else add a human player
            if (player.isAI())
            {
                AI ai = new AI(new ResourceGroup(10, 10, 10), player.getName(), 500);
                ai.SetTileColor(player.getColor());
                players.Add(ai);
            }
            else
            {
                Human human = new Human(new ResourceGroup(10, 10, 10), player.getName(), 500);
                human.SetTileColor(player.getColor());
                players.Add(human);
            }
        }
    }

    /// <summary>
    /// Starts the game by creating a new gamehandler
    /// </summary>
    public void StartGame()
    {
        Destroy(eventSystem);

        GameHandler.CreateNew(gameName, players);
        GameHandler.GetGameManager().StartGame();
            
        SceneManager.LoadScene(GAME_SCENE_INDEX);   //LoadScene is asynchronous   
    }

    public void AddPlayerClicked()
    {
        setupPlayers[numPlayersShown].Show();

        numPlayersShown++;
        Vector3 oldAddPos = addPlayerButton.transform.localPosition;
        Vector3 newAddPlayerPos = new Vector3(oldAddPos.x, oldAddPos.y - 30, oldAddPos.z);
        addPlayerButton.transform.localPosition = newAddPlayerPos;

        if (numPlayersShown != 4)
        {
            Vector3 oldRemPos = removePlayerButton.transform.localPosition;
            Vector3 newRemPlayerPos = new Vector3(oldRemPos.x, oldRemPos.y - 30, oldRemPos.z);
            removePlayerButton.transform.localPosition = newRemPlayerPos;
        }

        removePlayerButton.SetActive(true);

        if (numPlayersShown == 4)
        {
            addPlayerButton.SetActive(false);
        }
    }

    public void RemovePlayerClicked()
    {
        numPlayersShown--;
        setupPlayers[numPlayersShown].Hide();

        Vector3 oldAddPos = addPlayerButton.transform.localPosition;
        Vector3 newAddPlayerPos = new Vector3(oldAddPos.x, oldAddPos.y + 30, oldAddPos.z);
        addPlayerButton.transform.localPosition = newAddPlayerPos;

        if (numPlayersShown != 3)
        {
            Vector3 oldRemPos = removePlayerButton.transform.localPosition;
            Vector3 newRemPlayerPos = new Vector3(oldRemPos.x, oldRemPos.y + 30, oldRemPos.z);
            removePlayerButton.transform.localPosition = newRemPlayerPos;
        }

        addPlayerButton.SetActive(true);

        if (numPlayersShown == 2)
        {
            removePlayerButton.SetActive(false);
        }
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Assessment 4:- Delays the start of the game to allow the pre-game animation to play
    /// </summary>
    private IEnumerator DelayStartGame()
    {
        yield return new WaitForSeconds(mainMenuManager.SHIP_ANIM_LENGTH + mainMenuManager.CAMERA_ANIM_LENGTH);
        StartGame();
    }
}