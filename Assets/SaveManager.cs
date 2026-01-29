using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEngine;

/// <summary>
/// A simple SaveManager designed to save the total number of coins the player has.
/// In later parts, this will be used to store all the player's save data, but we are
/// keeping it simple for now.
/// </summary>
public class SaveManager
{
    public class GameData
    {
        public float coins;
    }
    
    const string SAVE_FILE_NAME = "SaveData.json";

    static GameData lastLoadedGameData;
    public static GameData LastLoadedGameData
    {
        get 
        {
            if (lastLoadedGameData == null) Load();
            return lastLoadedGameData;
        }
    }

    public static string GetSavePath() {
        return string.Format("{0}/{1}", Application.persistentDataPath, SAVE_FILE_NAME);
    }

    // This function, when called without an argument, will save into the last loaded
    // game file (this is how you should be calling Save() 99% of the time.
    // But you can optionally also provide an argument to it to if you want to overwrite the save completely.
    public static void Save(GameData data = null)
    {
        // Ensures that the save always works.
        if (data == null)
        {
            // If there is no last loaded game, we load the game to populate
            // lastLoadedGameData first, then we save.
            if (lastLoadedGameData == null) Load();
            data = lastLoadedGameData;
        }
        File.WriteAllText(GetSavePath(), JsonUtility.ToJson(data));
    }

    public static GameData Load(bool usePreviousLoadIfAvailable = false)
    {
        // usePreviousLoadIfAvailable is meant to speed up load calls,
        // since we don't need to read the save file every time we want to access data.
        if (usePreviousLoadIfAvailable) return lastLoadedGameData;

        // Retrieve the load in the hard drive.
        string path = GetSavePath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            lastLoadedGameData = JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            lastLoadedGameData = new GameData();
        }
        return lastLoadedGameData;
    }
}
