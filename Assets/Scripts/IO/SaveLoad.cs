using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad{

    public static void Save(List<Move> movesToSave)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedMoves.mvs");
        bf.Serialize(file, movesToSave);
        file.Close();
    }

    public static void Load()
    {
        if(File.Exists(Application.persistentDataPath + "/savedMoves.mvs"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedMoves.mvs", FileMode.Open);
            AvailableMoves.SetMoves((List<Move>)bf.Deserialize(file));
            file.Close();
        }
	}

	public static void SaveButtons(List<string> playerButtons, string path)
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + path);
		bf.Serialize (file, playerButtons);
		file.Close ();
	}

	public static List<string> LoadButtons(string path)
	{
		List<string> buttons = null;
		if (File.Exists (Application.persistentDataPath + path))
		{
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + path, FileMode.Open);
			buttons = (List<string>)bf.Deserialize (file);
			file.Close ();
		}
		return buttons;
	}
}
