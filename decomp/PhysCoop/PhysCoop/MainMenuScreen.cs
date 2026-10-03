using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace PhysCoop;

internal class MainMenuScreen : MenuScreen
{
	public MainMenuScreen(ScreenManager sm)
		: base("")
	{
		IsPopup = true;
		sm.AllSignedInIsInGame();
		MenuEntry menuEntry;
		if (sm.AllowMenuLoadGame)
		{
			menuEntry = new MenuEntry("Start New Game", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
			MenuEntry menuEntry2 = new MenuEntry("Load Game", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
			menuEntry2.Selected += LoadGameMenuEntrySelected;
			MenuEntries.Add(menuEntry2);
		}
		else
		{
			menuEntry = new MenuEntry("Start Game", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		}
		MenuEntry menuEntry3 = new MenuEntry("How to Play", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		MenuEntry menuEntry4 = new MenuEntry("Unlock Full Game", lockedOnTrial: false, removeOnFullversion: true, reqPrivAllowPurchaseContent: true);
		MenuEntry menuEntry5 = new MenuEntry("Game Options", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		MenuEntry menuEntry6 = new MenuEntry("Credits", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		MenuEntry menuEntry7 = new MenuEntry("Exit Game", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		menuEntry.Selected += PlayGameMenuEntrySelected;
		menuEntry3.Selected += HowToPlayMenuEntrySelected;
		menuEntry4.Selected += UnlockMenuEntrySelected;
		menuEntry5.Selected += OptionsMenuEntrySelected;
		menuEntry6.Selected += CreditsMenuEntrySelected;
		menuEntry7.Selected += OnCancel;
		MenuEntries.Add(menuEntry);
		MenuEntries.Add(menuEntry3);
		MenuEntries.Add(menuEntry4);
		MenuEntries.Add(menuEntry5);
		MenuEntries.Add(menuEntry6);
		MenuEntries.Add(menuEntry7);
		infoEntryList = new List<InfoEntry>();
		infoEntryList.Add(new InfoEntry("\u0096 Select", removeOnFullversion: false));
		infoEntryList.Add(new InfoEntry("© Stolpskott Studios 2009", removeOnFullversion: false));
	}

	private void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		ScreenManager.NeedToUseStorage = true;
		ScreenManager.RequestCreateNewGame = true;
		ScreenManager.AllSignedInIsInGame();
		if (ScreenManager.UseGameSetup || ScreenManager.AutoLoadSingleGlobalSave)
		{
			ScreenManager.AddScreen(new GameSetupMenuScreen(ScreenManager), null);
		}
		else if (ScreenManager.UseDifficultyMenu)
		{
			ScreenManager.AddScreen(new DifficultyMenuScreen(), null);
		}
		else
		{
			ScreenManager.PlayMenuMusic = false;
			LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen(1));
		}
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}

	private void LoadGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}

	private void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		ScreenManager.AddScreen(new GameOptionsMenuScreen(ScreenManager), e.PlayerIndex);
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}

	private void HowToPlayMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		ScreenManager.AddScreen(new HowToPlayMenuScreen(0), e.PlayerIndex);
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}

	private void UnlockMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		ScreenManager.UserWantsToBuy = true;
		ScreenManager.BuyerIndex = e.PlayerIndex;
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}

	private void CreditsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.AddScreen(new CreditsMenuScreen(), null);
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}

	protected override void OnCancel(PlayerIndex playerIndex)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		MessageBoxScreen messageBoxScreen = new MessageBoxScreen("Are you sure you want to exit?", p_allowCancel: true);
		messageBoxScreen.Accepted += ConfirmExitMessageBoxAccepted;
		ScreenManager.AddScreen(messageBoxScreen, playerIndex);
		ScreenManager.MenuSoundBank.PlayCue("Menu_Back");
	}

	private void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
	{
		if (Guide.IsTrialMode && ScreenManager.UseNagScreens)
		{
			ScreenManager.AddScreen(new NagMenuScreen(1), null);
		}
		else
		{
			((GameComponent)ScreenManager).Game.Exit();
		}
	}
}
