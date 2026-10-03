using System;

namespace PhysCoop;

[Serializable]
public class SavedGame
{
	public int GameDifficulty;

	public int CurrentMapId;

	public bool[] StageCompleted;

	public SavedGame()
	{
		GameDifficulty = 1;
		CurrentMapId = 1;
		StageCompleted = new bool[256];
	}
}
