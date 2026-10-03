using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PhysCoop;

internal class ControllerLayoutSelectMenuScreen : MenuScreen
{
	private int oldLayout = 0;

	public ControllerLayoutSelectMenuScreen(int layout)
		: base("Controller Layout")
	{
		IsPopup = true;
		instantAppear = false;
		oldLayout = layout;
		drawControllerLayout = true;
		MenuEntry menuEntry = new MenuEntry("", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		menuEntry.SelectRight += HiddenMenuEntrySelectRight;
		menuEntry.SelectLeft += HiddenMenuEntrySelectLeft;
		menuEntry.Selected += HiddenMenuEntrySelected;
		MenuEntries.Add(menuEntry);
		infoEntryList = new List<InfoEntry>();
		infoEntryList.Add(new InfoEntry("\u0096 Select", removeOnFullversion: false));
		infoEntryList.Add(new InfoEntry("£ Change Layout", removeOnFullversion: false));
		infoEntryList.Add(new InfoEntry("\u0097 Back", removeOnFullversion: false));
	}

	private void HiddenMenuEntrySelectRight(object sender, PlayerIndexEventArgs e)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		ScreenManager.PersonalSettings[e.PlayerIndex].ControllerLayout++;
		if (ScreenManager.PersonalSettings[e.PlayerIndex].ControllerLayout > ScreenManager.ControllerLayouts.Layouts.Count - 1)
		{
			ScreenManager.PersonalSettings[e.PlayerIndex].ControllerLayout = ScreenManager.ControllerLayouts.Layouts.Count - 1;
			ScreenManager.MenuSoundBank.PlayCue("Menu_Deny");
		}
	}

	private void HiddenMenuEntrySelectLeft(object sender, PlayerIndexEventArgs e)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		ScreenManager.PersonalSettings[e.PlayerIndex].ControllerLayout--;
		if (ScreenManager.PersonalSettings[e.PlayerIndex].ControllerLayout < 0)
		{
			ScreenManager.PersonalSettings[e.PlayerIndex].ControllerLayout = 0;
			ScreenManager.MenuSoundBank.PlayCue("Menu_Deny");
		}
	}

	private void HiddenMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
		ExitScreen();
	}

	protected override void OnCancel(PlayerIndex playerIndex)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		ScreenManager.PersonalSettings[playerIndex].ControllerLayout = oldLayout;
		ExitScreen();
	}
}
