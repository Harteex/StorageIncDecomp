using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PhysCoop;

internal class HowToPlayMenuScreen : MenuScreen
{
	private int page;

	private int maxPages = 5;

	private ContentManager content;

	public HowToPlayMenuScreen(int p_page)
		: base("INFO")
	{
		IsPopup = true;
		page = p_page;
		instantAppear = true;
		if (page == 0)
		{
			page = 1;
			instantAppear = false;
		}
		if (page == 1)
		{
			drawControllerLayout = true;
		}
		MenuEntry menuEntry = new MenuEntry("", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		menuEntry.SelectRight += HiddenMenuEntrySelectRight;
		menuEntry.SelectLeft += HiddenMenuEntrySelectLeft;
		MenuEntries.Add(menuEntry);
		infoEntryList = new List<InfoEntry>();
		infoEntryList.Add(new InfoEntry("£ Change Page", removeOnFullversion: false));
		infoEntryList.Add(new InfoEntry(page + " of " + maxPages, removeOnFullversion: false));
		infoEntryList.Add(new InfoEntry("\u0097 Back", removeOnFullversion: false));
	}

	public override void LoadContent()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected Obj, but got Unknown
		if (content == null)
		{
			content = new ContentManager((IServiceProvider)((GameComponent)ScreenManager).Game.Services, "Content");
		}
		infoBackground = content.Load<Texture2D>("ScreenManagerResources\\info_Background");
		switch (page)
		{
		case 1:
			infoTexture = ScreenManager.BlankTexture;
			break;
		case 2:
			infoTexture = content.Load<Texture2D>("ScreenManagerResources\\howtoimages\\Goal");
			break;
		case 3:
			infoTexture = content.Load<Texture2D>("ScreenManagerResources\\howtoimages\\Goal2");
			break;
		case 4:
			infoTexture = content.Load<Texture2D>("ScreenManagerResources\\howtoimages\\GoodsStatus");
			break;
		case 5:
			infoTexture = content.Load<Texture2D>("ScreenManagerResources\\howtoimages\\Tips");
			break;
		default:
			infoTexture = ScreenManager.BlankTexture;
			break;
		}
	}

	public override void UnloadContent()
	{
		content.Unload();
	}

	private void HiddenMenuEntrySelectRight(object sender, PlayerIndexEventArgs e)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		page++;
		if (page > maxPages)
		{
			page = maxPages;
			ScreenManager.MenuSoundBank.PlayCue("Menu_Deny");
		}
		else
		{
			ScreenManager.AddScreen(new HowToPlayMenuScreen(page), e.PlayerIndex);
			ExitScreen();
			ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		}
	}

	private void HiddenMenuEntrySelectLeft(object sender, PlayerIndexEventArgs e)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		page--;
		if (page < 1)
		{
			page = 1;
			ScreenManager.MenuSoundBank.PlayCue("Menu_Deny");
		}
		else
		{
			ScreenManager.AddScreen(new HowToPlayMenuScreen(page), e.PlayerIndex);
			ExitScreen();
			ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		}
	}
}
