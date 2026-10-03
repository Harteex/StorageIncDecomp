using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PhysCoop;

internal class TutorialMessageMenuScreen : MenuScreen
{
	public string message;

	public Texture2D messageBackground;

	public TutorialMessageMenuScreen(string p_message, Vector2 p_arrowTarget2D)
		: base("INFO")
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		message = p_message;
		arrowTarget2D = p_arrowTarget2D;
		inputDelay = 60;
		IsPopup = true;
		instantAppear = true;
		instantDisappear = true;
		MenuEntry menuEntry = new MenuEntry("", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		menuEntry.Selected += HiddenMenuEntrySelected;
		MenuEntries.Add(menuEntry);
		infoEntryList = new List<InfoEntry>();
		infoEntryList.Add(new InfoEntry("\u0096 Ok", removeOnFullversion: false));
	}

	private void HiddenMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		ExitScreen();
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}

	protected override void OnCancel(PlayerIndex playerIndex)
	{
		ExitScreen();
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}

	public override void LoadContent()
	{
		ContentManager content = ((GameComponent)ScreenManager).Game.Content;
		messageBackground = content.Load<Texture2D>("ScreenManagerResources\\MessageBoxBg");
	}
}
