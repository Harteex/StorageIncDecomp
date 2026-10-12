using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

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
		// Port: on Xbox the main player had to be signed in to a profile already (otherwise the
		// sign-in Guide opened). There are no profiles on PC, so pressing Start signs in.
		Gamer.SignedInGamers.SignIn(e.PlayerIndex);
		ScreenManager.AskForMainProfile = true;
		ExitScreen();
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}

	protected override void OnCancel(PlayerIndex playerIndex)
	{
	}
}
