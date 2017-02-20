﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;


//Made by JBT
public class MenuScript : MonoBehaviour
{
    public Toggle AIToggle;
    public InputField Player1Name;
    public InputField Player2Name;
    public const int GAME_SCENE_INDEX = 1;
    public const string AIPlayerName = "Bot";
    public string gameName = "game";

    /// <summary>
    /// Starts the game by creating a new gamehandler
    /// </summary>
    public void StartGame()
    {
        //If a player is quitting to the menu from the game itself, or the end of game screen, then remove the GUI canvas that was not destroyed on load
        if (GameObject.Find("Player GUI Canvas(Clone)") != null)
        {
            Destroy(GameObject.Find("Player GUI Canvas(Clone)"));
        }

        List<Player> players = new List<Player>();

        //If no player name entered, set default player names
        if (Player1Name.text == "" || Player1Name.text == null)
        {
            Player1Name.text = "Player1";
        }
        if (Player2Name.text == "" || Player2Name.text == null|| Player2Name.text == "Enter Name Here...")
        {
            Player2Name.text = "Player2";
        }

        //If AI Is on, then make an AI player and a human, else make 2 human players
        if (AIToggle.isOn)
        {
            players.Add(new Human(new ResourceGroup(10, 10, 10), Player1Name.text, 500));
            players.Add(new AI(new ResourceGroup(10, 10, 10), AIPlayerName, 500));
        }
        else
        {
            players.Add(new Human(new ResourceGroup(10, 10, 10), Player2Name.text, 500));
            players.Add(new Human(new ResourceGroup(10, 10, 10), Player1Name.text, 500));
        }


        GameHandler.CreateNew(gameName, players);
        GameHandler.GetGameManager().StartGame();
            
        SceneManager.LoadScene(GAME_SCENE_INDEX);   //LoadScene is asynchronous   

    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Toggles if AI is active
    /// </summary>
    public void ToggleAI()
    {
        if (AIToggle.isOn)
        {
            Player2Name.text = AIPlayerName;
            Player2Name.interactable = false;
        }
        else
        {
            Player2Name.text = "Enter Name Here...";
            Player2Name.interactable = true;
        }
    }
}