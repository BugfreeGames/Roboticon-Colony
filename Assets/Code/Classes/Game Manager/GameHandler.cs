//Game executable hosted at: http://www-users.york.ac.uk/~jwa509/executable.exe

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public static class GameHandler
{
    public static GameManager gameManager;

    /// <summary>
    /// Throws System.ArgumentException if given a list of players not containing any
    /// Human players.
    /// </summary>
    /// <param name="gameName"></param>
    /// <param name="players"></param>
    /// <returns></returns>
    public static void CreateNew(string gameName, List<Player> players)
    {
        gameManager = new GameManager(gameName, players);
    }

    public static GameManager GetGameManager()
    {
        return gameManager;
    }
}
