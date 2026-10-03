using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace PhysCoop;

internal class GameSetupMenuScreen : MenuScreen
{
	private MenuEntry playGameMenuEntry;

	private MenuEntry selectMapMenuEntry;

	private MenuEntry difficultyMenuEntry;

	private MenuEntry optionsMenuEntry;

	private MenuEntry howToPlayMenuEntry;

	public GameSetupMenuScreen(ScreenManager sm)
		: base("Game Setup")
	{
		IsPopup = true;
		sm.AllSignedInIsInGame();
		playGameMenuEntry = new MenuEntry("Start Game", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		howToPlayMenuEntry = new MenuEntry("How To Play", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		difficultyMenuEntry = new MenuEntry(string.Empty, lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		selectMapMenuEntry = new MenuEntry("Select Map", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		optionsMenuEntry = new MenuEntry("Game Options", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		MenuEntry menuEntry = new MenuEntry("Back", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
		difficultyMenuEntry.SelectLeft += DifficultyMenuEntrySelectLeft;
		difficultyMenuEntry.SelectRight += DifficultyMenuEntrySelectRight;
		difficultyMenuEntry.Selected += DifficultyMenuEntrySelectRight;
		selectMapMenuEntry.Selected += SelectMapMenuEntrySelected;
		howToPlayMenuEntry.Selected += HowToPlayMenuEntrySelected;
		optionsMenuEntry.Selected += OptionsMenuEntrySelected;
		menuEntry.Selected += OnCancel;
		MenuEntries.Add(playGameMenuEntry);
		MenuEntries.Add(selectMapMenuEntry);
		MenuEntries.Add(difficultyMenuEntry);
		MenuEntries.Add(howToPlayMenuEntry);
		MenuEntries.Add(optionsMenuEntry);
		MenuEntries.Add(menuEntry);
		switch (sm.GameSaveInfo.GameDifficulty)
		{
		case 0:
			difficultyMenuEntry.Text = "Difficulty: Easy   ";
			break;
		case 1:
			difficultyMenuEntry.Text = "Difficulty: Normal";
			break;
		case 2:
			difficultyMenuEntry.Text = "Difficulty: Hard   ";
			break;
		case 3:
			difficultyMenuEntry.Text = "Difficulty: Master";
			break;
		}
		infoEntryList = new List<InfoEntry>();
		infoEntryList.Add(new InfoEntry("\u0096 Select", removeOnFullversion: false));
		infoEntryList.Add(new InfoEntry("\u0097 Back", removeOnFullversion: false));
	}

	public void UpdateText()
	{
		playGameMenuEntry.Text = "Start Game";
		selectMapMenuEntry.Text = "Select Map";
		switch (ScreenManager.GameSaveInfo.GameDifficulty)
		{
		case 0:
			difficultyMenuEntry.Text = "Difficulty: Easy   ";
			break;
		case 1:
			difficultyMenuEntry.Text = "Difficulty: Normal";
			break;
		case 2:
			difficultyMenuEntry.Text = "Difficulty: Hard   ";
			break;
		case 3:
			difficultyMenuEntry.Text = "Difficulty: Master";
			break;
		}
		howToPlayMenuEntry.Text = "How To Play";
		optionsMenuEntry.Text = "Game Options";
	}

	protected override void OnCancel(PlayerIndex playerIndex)
	{
		ScreenManager.NeedToUseStorage = true;
		ScreenManager.RequestSaveGame = true;
		ExitScreen();
		ScreenManager.GoToMainMenu();
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}

	protected override void OnSelectY(PlayerIndex playerIndex)
	{
		if (ScreenManager.UsePersonalSettings && ScreenManager.PlayerIsSignedIn[(int)playerIndex])
		{
			ScreenManager.AddScreen(new PlayerSettingsMenuScreen(ScreenManager, playerIndex), playerIndex);
		}
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}

	protected override void OnSelectX(PlayerIndex playerIndex)
	{
		if (!ScreenManager.PlayerIsInGame[(int)playerIndex] && !Guide.IsVisible)
		{
			Guide.ShowSignIn(4, false);
		}
	}

	private void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen(ScreenManager.GameSaveInfo.CurrentMapId));
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
		ScreenManager.PlayMenuMusic = false;
	}

	private void SelectMapMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.AddScreen(new SelectMapMenuScreen(ScreenManager), null);
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}

	private void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.AddScreen(new GameOptionsMenuScreen(ScreenManager), e.PlayerIndex);
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}

	private void HowToPlayMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.AddScreen(new HowToPlayMenuScreen(0), null);
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}

	private void DifficultyMenuEntrySelectRight(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		ScreenManager.GameSaveInfo.GameDifficulty++;
		if (ScreenManager.GameSaveInfo.GameDifficulty > 3)
		{
			ScreenManager.GameSaveInfo.GameDifficulty = 0;
		}
		UpdateText();
		ScreenManager.NeedToUseStorage = true;
		ScreenManager.RequestSaveGame = true;
	}

	private void DifficultyMenuEntrySelectLeft(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		ScreenManager.GameSaveInfo.GameDifficulty--;
		if (ScreenManager.GameSaveInfo.GameDifficulty < 0)
		{
			ScreenManager.GameSaveInfo.GameDifficulty = 3;
		}
		UpdateText();
		ScreenManager.NeedToUseStorage = true;
		ScreenManager.RequestSaveGame = true;
	}
}
