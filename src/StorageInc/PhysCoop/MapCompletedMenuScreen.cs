using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace PhysCoop;

internal class MapCompletedMenuScreen : MenuScreen
{
	private ContentManager content;

	public MapCompletedMenuScreen()
		: base("Map Completed")
	{
		IsPopup = true;
		inputDelay = 90;
		MenuEntry menuEntry = new MenuEntry("", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		menuEntry.Selected += OkMenuEntrySelected;
		MenuEntries.Add(menuEntry);
		infoEntryList = new List<InfoEntry>();
		infoEntryList.Add(new InfoEntry("\u0096 Next Map", removeOnFullversion: false));
		infoEntryList.Add(new InfoEntry("\u0097 Game Setup", removeOnFullversion: false));
	}

	private void OkMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
		bool flag = false;
		ScreenManager.MapsContainer.SortByDifficulty();
		int currentMapId = ScreenManager.GameSaveInfo.CurrentMapId;
		int num = ScreenManager.FindMapListIndex(currentMapId);
		if (num >= ScreenManager.MapsContainer.Maps.Count - 1)
		{
			flag = true;
		}
		if (Guide.IsTrialMode && ScreenManager.MapsContainer.Maps[num + 1].LockedOnTrial)
		{
			flag = true;
		}
		if (flag)
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
		else
		{
			ScreenManager.NewMapSelected(num + 1);
			ScreenManager.PlayMenuMusic = false;
			LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen(ScreenManager.GameSaveInfo.CurrentMapId));
		}
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
		if (content == null)
		{
			content = new ContentManager((IServiceProvider)((GameComponent)ScreenManager).Game.Services, "Content");
		}
		infoTexture = content.Load<Texture2D>("ScreenManagerResources\\MapCompletedImage");
	}

	public override void UnloadContent()
	{
		content.Unload();
	}
}
