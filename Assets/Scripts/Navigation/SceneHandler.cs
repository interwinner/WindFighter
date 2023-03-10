using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
//using UnityEditor;

/// <summary>
/// Class containing methods for changing and exiting the current scene.
/// </summary>
public class SceneHandler
{
	private static bool alreadyInitialized;
	private static Stack sceneStack;
	private static AssetBundle myLoadedAssetBundle;
	private static string[] scenePaths;
	private static ArrayList scenePathsList;

	public static void Init()
	{
		if (!alreadyInitialized)
		{
			scenePathsList = new ArrayList ();
			sceneStack = new Stack ();

			//String path of folder containing all scenes
		//	string sceneDirectoryPath = "Assets" + Path.DirectorySeparatorChar + "Scenes";
		//	scenePaths = Directory.GetFiles (sceneDirectoryPath);

			//Add all scene names to list scenePathsList
			// for (int i = 0; i < scenePaths.Length; i++) {
			// 	string path = scenePaths [i];
			// 	path = path.Split (Path.DirectorySeparatorChar) [2].ToString (); //Remove begining of path
			// 	path = path.Split ('.') [0].ToString (); //Remove file ending
			// 	if (!scenePathsList.Contains (path)) {
			// 		scenePathsList.Add (path);
			// 	}
			// }

			scenePathsList.Add("FightScene");
			scenePathsList.Add("MainMenuScene");
			scenePathsList.Add("MoveEditorScene");
			scenePathsList.Add("MoveSelectionScene");
			scenePathsList.Add("SettingsScene");
		}
	}

	/// <summary>
	/// Opens the scene if it exists and adds the current scene to the stack
	/// </summary>
	/// <param name="sceneName">The name of the scene to be changed to.</param>
	public static void SwitchScene(string sceneName)
	{
		if (scenePathsList.Contains (sceneName))
		{
			sceneStack.Push (SceneManager.GetActiveScene().name);
			SceneManager.LoadScene (sceneName);
		}
	}
	/// <summary>
	/// If the scene stack is non empty, pops the stack and loads the top scene.
	/// If the current scene is the MoveEditor, a "Are you sure"-dialog is opened to not discard the move.
	/// </summary>
	public static void GoBack()
	{
		if (sceneStack.Count > 0) 
		{
			float fadeTime = GameObject.Find("Canvas").GetComponent<SceneFade>().BeginFade(1);
			Wait(fadeTime);
            if (SceneManager.GetActiveScene().name == "MoveEditorScene")
            {
				SaveLoad.Save(AvailableMoves.GetMoves());
            }
            string previousSceneName = (string)sceneStack.Pop();
            if (scenePathsList.Contains(previousSceneName))
            {
                SceneManager.LoadScene(previousSceneName);
            }
		}
	}

	/// <summary>
	/// Goes the back to the specified scene, poping scene stack until it is reached or until the stack only contains one element.
	/// </summary>
	/// <param name="sceneName">Scene name.</param>
	public static void GoBackToScene(string sceneName)
	{
		//Pop stack until a matching scene is found.
		while (!sceneName.Equals ((string)sceneStack.Peek ()) && sceneStack.Count > 1)
		{
			sceneStack.Pop ();
		}
		//Fade between scenes.
		float fadeTime = GameObject.Find("Canvas").GetComponent<SceneFade>().BeginFade(1);
		Wait(fadeTime);
		string newSceneName = (string)sceneStack.Pop ();
		if (scenePathsList.Contains(newSceneName))
		{
			SceneManager.LoadScene(newSceneName);
		}
	}

	static IEnumerator Wait(float time)
	{
		yield return new WaitForSeconds(time);
	}
}
