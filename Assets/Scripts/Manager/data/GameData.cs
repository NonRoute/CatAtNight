using System;
using UnityEngine;

#nullable enable

[Serializable]
public class GameData
{
	// Game Progress
	// TODO Implement
	public int currentLevel;
	public int unlockedLevel;

	// TODO Implement
	public int score;

	// State
	public Vector3 position = new(-3.13f, -0.41f, 0f);
	public float health = 9f;
	public bool liquidMode;
}
