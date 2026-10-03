using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PhysCoop;

internal class SelectMapMenuScreen : MenuScreen
{
	private MenuEntry tempMenuEntry;

	private int oldMapId;

	public SelectMapMenuScreen(ScreenManager sm)
		: base("Select Map")
	{
		IsPopup = true;
		menuUpDownDelay = 10;
		oldMapId = sm.GameSaveInfo.CurrentMapId;
		sm.MapsContainer.SortByDifficulty();
		sm.MapsContainer.ListMapIndex = sm.FindMapListIndex(oldMapId);
		SelectedEntry = sm.MapsContainer.ListMapIndex;
		foreach (MapContainer.MapItem map in sm.MapsContainer.Maps)
		{
			tempMenuEntry = new MenuEntry(map.Name, map.LockedOnTrial, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
			tempMenuEntry.Selected += MapMenuEntrySelected;
			MenuEntries.Add(tempMenuEntry);
		}
		infoEntryList = new List<InfoEntry>();
		infoEntryList.Add(new InfoEntry("\u0096 Select", removeOnFullversion: false));
		infoEntryList.Add(new InfoEntry("\u0097 Back", removeOnFullversion: false));
		infoEntryList.Add(new InfoEntry("", removeOnFullversion: false));
	}

	protected override void OnCancel(PlayerIndex playerIndex)
	{
		ScreenManager.NewMapSelected(ScreenManager.FindMapListIndex(oldMapId));
		ExitScreen();
		ScreenManager.MenuSoundBank.PlayCue("Menu_Back");
	}

	protected override void OnSelectY(PlayerIndex playerIndex)
	{
	}

	private void MapMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.NeedToUseStorage = true;
		ScreenManager.RequestSaveGame = true;
		ExitScreen();
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
	}
}
