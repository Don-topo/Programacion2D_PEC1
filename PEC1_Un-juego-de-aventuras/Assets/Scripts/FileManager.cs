using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class FileManager
{

    public static void saveGame(SaveInfo saveInfo)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string filePath = Application.persistentDataPath + "/savedGames";

        // TODO Check if we are overwritting a file or adding a new one
        // TODO Update file content
        // TODO Add new file
    }
}