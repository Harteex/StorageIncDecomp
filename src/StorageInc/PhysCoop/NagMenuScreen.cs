using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PhysCoop;

internal class NagMenuScreen : MenuScreen
{
	private int page;

	private int maxPages = 1;

	private ContentManager content;

	public NagMenuScreen(int p_page)
		: base("INFO")
	{
		IsPopup = true;
		inputDelay = 60;
		page = p_page;
		MenuEntry menuEntry = new MenuEntry("", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		menuEntry.Selected += HiddenMenuEntrySelected;
		MenuEntries.Add(menuEntry);
		infoEntryList = new List<InfoEntry>();
		if (page < maxPages)
		{
			infoEntryList.Add(new InfoEntry("\u0096 Next", removeOnFullversion: false));
		}
		infoEntryList.Add(new InfoEntry("\u0098 Unlock Full Version", removeOnFullversion: false));
		infoEntryList.Add(new InfoEntry("\u0097 Exit Game", removeOnFullversion: false));
	}

	public override void LoadContent()
	{
		if (content == null)
		{
			content = new ContentManager((IServiceProvider)((GameComponent)ScreenManager).Game.Services, "Content");
		}
		infoBackground = content.Load<Texture2D>("ScreenManagerResources\\info_Background");
		infoTexture = content.Load<Texture2D>("ScreenManagerResources\\Nag");
	}

	public override void UnloadContent()
	{
		content.Unload();
	}

	protected override void OnSelectX(PlayerIndex playerIndex)
	{
		ScreenManager.UserWantsToBuy = true;
		ScreenManager.BuyerIndex = playerIndex;
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
		ScreenManager.GoToStartMenu();
	}

	private void HiddenMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
		if (page < maxPages)
		{
			page++;
			ScreenManager.AddScreen(new NagMenuScreen(page), null);
			ExitScreen();
		}
	}

	protected override void OnCancel(PlayerIndex playerIndex)
	{
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
		((GameComponent)ScreenManager).Game.Exit();
	}
}
