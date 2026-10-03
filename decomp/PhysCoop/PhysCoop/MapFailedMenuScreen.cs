using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PhysCoop;

internal class MapFailedMenuScreen : MenuScreen
{
	private ContentManager content;

	public MapFailedMenuScreen()
		: base("Map Failed")
	{
		IsPopup = true;
		inputDelay = 90;
		MenuEntry menuEntry = new MenuEntry("", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		menuEntry.Selected += OkMenuEntrySelected;
		MenuEntries.Add(menuEntry);
		infoEntryList = new List<InfoEntry>();
		infoEntryList.Add(new InfoEntry("\u0096 Restart Map", removeOnFullversion: false));
		infoEntryList.Add(new InfoEntry("\u0097 Game Setup", removeOnFullversion: false));
	}

	private void OkMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
		LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen(ScreenManager.GameSaveInfo.CurrentMapId));
	}

	protected override void OnCancel(PlayerIndex playerIndex)
	{
		if (ScreenManager.UseGameSetup)
		{
			ScreenManager.GoToGameSetup();
		}
		else
		{
			ScreenManager.GoToMainMenu();
		}
	}

	public override void LoadContent()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected Obj, but got Unknown
		if (content == null)
		{
			content = new ContentManager((IServiceProvider)((GameComponent)ScreenManager).Game.Services, "Content");
		}
		infoTexture = content.Load<Texture2D>("ScreenManagerResources\\MapFailedImage");
	}

	public override void UnloadContent()
	{
		content.Unload();
	}
}
