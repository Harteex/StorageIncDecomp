using System.Collections.Generic;

namespace PhysCoop;

internal class PauseMenuScreen : MenuScreen
{
	public PauseMenuScreen(ScreenManager sm)
		: base("Paused")
	{
		IsPopup = true;
		MenuEntry menuEntry = new MenuEntry("Resume Game", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		MenuEntry menuEntry2 = new MenuEntry("Unlock Full Game", lockedOnTrial: false, removeOnFullversion: true, reqPrivAllowPurchaseContent: true);
		MenuEntry menuEntry3 = new MenuEntry("How To Play", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		MenuEntry menuEntry4 = new MenuEntry("Game Options", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		MenuEntry menuEntry5 = new MenuEntry("Player Settings", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		MenuEntry menuEntry6 = new MenuEntry("Exit to Main Menu", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		if (sm.UseGameSetup)
		{
			menuEntry6.Text = "Exit Map";
		}
		menuEntry.Selected += OnCancel;
		menuEntry2.Selected += UnlockMenuEntrySelected;
		menuEntry3.Selected += HowToPlayMenuEntrySelected;
		menuEntry4.Selected += OptionsMenuEntrySelected;
		menuEntry5.Selected += SettingsMenuEntrySelected;
		menuEntry6.Selected += QuitGameMenuEntrySelected;
		MenuEntries.Add(menuEntry);
		MenuEntries.Add(menuEntry2);
		MenuEntries.Add(menuEntry3);
		MenuEntries.Add(menuEntry4);
		if (sm.UsePersonalSettings)
		{
			MenuEntries.Add(menuEntry5);
		}
		MenuEntries.Add(menuEntry6);
		infoEntryList = new List<InfoEntry>();
		infoEntryList.Add(new InfoEntry("\u0096 Select", removeOnFullversion: false));
		infoEntryList.Add(new InfoEntry("\u0097 Back", removeOnFullversion: false));
	}

	private void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		MessageBoxScreen messageBoxScreen = new MessageBoxScreen("Are you sure you want to quit this game?", p_allowCancel: true);
		messageBoxScreen.Accepted += ConfirmQuitMessageBoxAccepted;
		ScreenManager.AddScreen(messageBoxScreen, ControllingPlayer);
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}

	private void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
	{
		if (ScreenManager.UseGameSetup)
		{
			LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new GameSetupMenuScreen(ScreenManager));
			ScreenManager.PlayMenuMusic = true;
		}
		else
		{
			LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen(ScreenManager));
		}
	}

	private void UnlockMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		ScreenManager.UserWantsToBuy = true;
		ScreenManager.BuyerIndex = e.PlayerIndex;
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}

	private void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.AddScreen(new GameOptionsMenuScreen(ScreenManager), null);
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}

	private void SettingsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (ScreenManager.PlayerIsInGame[e.PlayerIndex])
		{
			ScreenManager.AddScreen(new PlayerSettingsMenuScreen(ScreenManager, e.PlayerIndex), e.PlayerIndex);
			ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
		}
	}

	private void HowToPlayMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		ScreenManager.AddScreen(new HowToPlayMenuScreen(1), e.PlayerIndex);
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}
}
