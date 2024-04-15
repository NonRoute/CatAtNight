using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

[Serializable]
public class GameData
{
    // Game Progress
    // TODO Implement
    public int skillProgression;
    public int unlockedLevel;

    // TODO Implement
    public int score;

    // Position
    public int currentScene;
    public string sceneName = "";
    public Vector3 position = new(-3.13f, -0.41f, 0f);

    // State
    public float health = 9f;
	public bool liquidMode;
    public Dictionary<string, int> inventory = new();

	// Quest
    public Dictionary<string, string> allQuestData = new();
}
