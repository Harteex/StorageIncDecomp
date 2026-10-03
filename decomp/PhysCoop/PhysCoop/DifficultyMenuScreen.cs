using System.Collections.Generic;

namespace PhysCoop;

internal class DifficultyMenuScreen : MenuScreen
{
	public DifficultyMenuScreen()
		: base("Select Difficulty")
	{
		IsPopup = true;
		MenuEntry menuEntry = new MenuEntry("Easy", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		MenuEntry menuEntry2 = new MenuEntry("Normal", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		MenuEntry menuEntry3 = new MenuEntry("Hard", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		menuEntry.Selected += EasyMenuEntrySelected;
		menuEntry2.Selected += NormalMenuEntrySelected;
		menuEntry3.Selected += HardMenuEntrySelected;
		MenuEntries.Add(menuEntry);
		MenuEntries.Add(menuEntry2);
		MenuEntries.Add(menuEntry3);
		infoEntryList = new List<InfoEntry>();
		infoEntryList.Add(new InfoEntry("\u0096 Select", removeOnFullversion: false));
		infoEntryList.Add(new InfoEntry("\u0097 Back", removeOnFullversion: false));
	}

	private void EasyMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		ScreenManager.PlayMenuMusic = false;
		ScreenManager.GameSaveInfo.GameDifficulty = 0;
		LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen(1));
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}

	private void NormalMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		ScreenManager.PlayMenuMusic = false;
		ScreenManager.GameSaveInfo.GameDifficulty = 1;
		LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen(1));
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}

	private void HardMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		ScreenManager.PlayMenuMusic = false;
		ScreenManager.GameSaveInfo.GameDifficulty = 2;
		LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen(1));
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}
}
