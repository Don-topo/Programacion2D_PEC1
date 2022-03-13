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

    public static Insult[] loadInsults()
    {
        string filePath = Application.persistentDataPath + "/insults";
        if (File.Exists(filePath))
        {
            Insult[] insults;
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            insults = binaryFormatter.Deserialize(fileStream) as Insult[];
            fileStream.Close();
            return insults;
        }
        else
        {
            return null;
        }
    }
}
