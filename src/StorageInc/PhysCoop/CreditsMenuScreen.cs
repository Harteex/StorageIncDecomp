using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PhysCoop;

internal class CreditsMenuScreen : MenuScreen
{
	private ContentManager content;

	public CreditsMenuScreen()
		: base("INFO")
	{
		IsPopup = true;
		MenuEntry menuEntry = new MenuEntry("", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		menuEntry.Selected += HiddenMenuEntrySelected;
		MenuEntries.Add(menuEntry);
		infoEntryList = new List<InfoEntry>();
		infoEntryList.Add(new InfoEntry("\u0097 Back", removeOnFullversion: false));
	}

	public override void LoadContent()
	{
		if (content == null)
		{
			content = new ContentManager((IServiceProvider)((GameComponent)ScreenManager).Game.Services, "Content");
		}
		infoBackground = content.Load<Texture2D>("ScreenManagerResources\\info_Background");
		infoTexture = content.Load<Texture2D>("ScreenManagerResources\\Credits");
	}

	public override void UnloadContent()
	{
		content.Unload();
	}

	private void HiddenMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
	}

	protected override void OnCancel(PlayerIndex playerIndex)
	{
		ScreenManager.MenuSoundBank.PlayCue("Menu_Back");
		ExitScreen();
	}
}
