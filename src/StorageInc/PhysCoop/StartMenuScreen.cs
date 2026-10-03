using Microsoft.Xna.Framework;

namespace PhysCoop;

internal class StartMenuScreen : MenuScreen
{
	public StartMenuScreen()
		: base("")
	{
		IsPopup = true;
		MenuEntry menuEntry = new MenuEntry("Press Start", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		menuEntry.Selected += StartGameMenuEntrySelected;
		MenuEntries.Add(menuEntry);
	}

	private void StartGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.selectedStorage = null;
		ScreenManager.MainPlayer = e.PlayerIndex;
		ScreenManager.AskForMainProfile = true;
		ExitScreen();
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}

	protected override void OnCancel(PlayerIndex playerIndex)
	{
	}
}
