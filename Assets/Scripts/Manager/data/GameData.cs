using System;
using UnityEngine;

#nullable enable

[Serializable]
public class GameData
{
	// Game Progress
	public int currentLevel;
	public int unlockedLevel;

	// State
	public Vector3 position;
	public float health = 9f;
	public bool liquidMode;
}
