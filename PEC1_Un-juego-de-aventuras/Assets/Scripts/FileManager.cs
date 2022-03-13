using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class FileManager
{

    public static Insults LoadInsults(TextAsset jsonText)
    {
        Insults jsonInsults = JsonUtility.FromJson<Insults>(jsonText.text);
        return jsonInsults;
    }
}