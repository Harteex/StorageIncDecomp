using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PhysCoop;

internal class PlayerSettingsMenuScreen : MenuScreen
{
	private MenuEntry reverseRotationMenuEntry;

	private MenuEntry switchThumbStickMenuEntry;

	private MenuEntry vibrationMenuEntry;

	private MenuEntry sensitivityMenuEntry;

	private MenuEntry layoutMenuEntry;

	private PlayerIndex playerIndex;

	public PlayerSettingsMenuScreen(ScreenManager sm, PlayerIndex e)
		: base("Player Settings")
	{
		IsPopup = true;
		playerIndex = e;
		ScreenManager = sm;
		reverseRotationMenuEntry = new MenuEntry(string.Empty, lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		switchThumbStickMenuEntry = new MenuEntry(string.Empty, lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		vibrationMenuEntry = new MenuEntry(string.Empty, lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		sensitivityMenuEntry = new MenuEntry(string.Empty, lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		layoutMenuEntry = new MenuEntry(string.Empty, lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		MenuEntry menuEntry = new MenuEntry("Back", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		reverseRotationMenuEntry.SelectRight += ReverseRotationMenuEntrySelectRight;
		reverseRotationMenuEntry.SelectLeft += ReverseRotationMenuEntrySelectLeft;
		switchThumbStickMenuEntry.SelectRight += SwitchThumbStickMenuEntrySelectRight;
		switchThumbStickMenuEntry.SelectLeft += SwitchThumbStickMenuEntrySelectLeft;
		vibrationMenuEntry.SelectRight += VibrationMenuEntrySelectRight;
		vibrationMenuEntry.SelectLeft += VibrationMenuEntrySelectLeft;
		sensitivityMenuEntry.SelectRight += SensitivityMenuEntrySelectRight;
		sensitivityMenuEntry.SelectLeft += SensitivityMenuEntrySelectLeft;
		layoutMenuEntry.Selected += LayoutMenuEntrySelected;
		menuEntry.Selected += OnCancel;
		MenuEntries.Add(reverseRotationMenuEntry);
		if (sm.UseOptionSensitivity)
		{
			MenuEntries.Add(sensitivityMenuEntry);
		}
		if (sm.UseOptionControllerLayouts)
		{
			MenuEntries.Add(layoutMenuEntry);
		}
		if (sm.UseOptionSwitchThumbSticks)
		{
			MenuEntries.Add(switchThumbStickMenuEntry);
		}
		if (sm.UseOptionVibration)
		{
			MenuEntries.Add(vibrationMenuEntry);
		}
		MenuEntries.Add(menuEntry);
		infoEntryList = new List<InfoEntry>();
		infoEntryList.Add(new InfoEntry("£ Modify Option", removeOnFullversion: false));
		infoEntryList.Add(new InfoEntry("\u0099 Defaults", removeOnFullversion: false));
		infoEntryList.Add(new InfoEntry("\u0097 Back", removeOnFullversion: false));
		UpdateVisuals();
	}

	private void UpdateVisuals()
	{
		reverseRotationMenuEntry.Text = "Arcade Steering: " + (ScreenManager.PersonalSettings[(int)playerIndex].ArcadeReverseSteering ? " On" : "Off");
		switchThumbStickMenuEntry.Text = "Switch Thumbsticks: " + (ScreenManager.PersonalSettings[(int)playerIndex].SwitchThumbSticks ? " On" : "Off");
		vibrationMenuEntry.Text = "Gamepad Vibration: " + (ScreenManager.PersonalSettings[(int)playerIndex].GamePadVibration ? " On" : "Off");
		sensitivityMenuEntry.Text = "Look Sensitivity: " + ScreenManager.PersonalSettings[(int)playerIndex].Sensitivity;
		layoutMenuEntry.Text = "Controller Layout";
	}

	protected override void OnCancel(PlayerIndex playerIndex)
	{
		ScreenManager.NeedToUseStorage = true;
		ScreenManager.RequestSaveProfileSettings[(int)playerIndex] = true;
		ExitScreen();
		ScreenManager.MenuSoundBank.PlayCue("Menu_Back");
	}

	protected override void OnSelectY(PlayerIndex playerIndex)
	{
		ScreenManager.PersonalSettings[(int)playerIndex] = new ProfileSettings();
		UpdateVisuals();
		ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
	}

	private void ReverseRotationMenuEntrySelectRight(object sender, PlayerIndexEventArgs e)
	{
		if (ScreenManager.PersonalSettings[(int)playerIndex].ArcadeReverseSteering)
		{
			ScreenManager.PersonalSettings[(int)playerIndex].ArcadeReverseSteering = false;
		}
		else
		{
			ScreenManager.PersonalSettings[(int)playerIndex].ArcadeReverseSteering = true;
		}
		ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		UpdateVisuals();
	}

	private void ReverseRotationMenuEntrySelectLeft(object sender, PlayerIndexEventArgs e)
	{
		if (ScreenManager.PersonalSettings[(int)playerIndex].ArcadeReverseSteering)
		{
			ScreenManager.PersonalSettings[(int)playerIndex].ArcadeReverseSteering = false;
		}
		else
		{
			ScreenManager.PersonalSettings[(int)playerIndex].ArcadeReverseSteering = true;
		}
		ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		UpdateVisuals();
	}

	private void SwitchThumbStickMenuEntrySelectRight(object sender, PlayerIndexEventArgs e)
	{
		if (ScreenManager.PersonalSettings[(int)playerIndex].SwitchThumbSticks)
		{
			ScreenManager.PersonalSettings[(int)playerIndex].SwitchThumbSticks = false;
		}
		else
		{
			ScreenManager.PersonalSettings[(int)playerIndex].SwitchThumbSticks = true;
		}
		ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		UpdateVisuals();
	}

	private void SwitchThumbStickMenuEntrySelectLeft(object sender, PlayerIndexEventArgs e)
	{
		if (ScreenManager.PersonalSettings[(int)playerIndex].SwitchThumbSticks)
		{
			ScreenManager.PersonalSettings[(int)playerIndex].SwitchThumbSticks = false;
		}
		else
		{
			ScreenManager.PersonalSettings[(int)playerIndex].SwitchThumbSticks = true;
		}
		ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		UpdateVisuals();
	}

	private void VibrationMenuEntrySelectRight(object sender, PlayerIndexEventArgs e)
	{
		if (ScreenManager.PersonalSettings[(int)playerIndex].GamePadVibration)
		{
			ScreenManager.PersonalSettings[(int)playerIndex].GamePadVibration = false;
		}
		else
		{
			ScreenManager.PersonalSettings[(int)playerIndex].GamePadVibration = true;
		}
		ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		UpdateVisuals();
	}

	private void VibrationMenuEntrySelectLeft(object sender, PlayerIndexEventArgs e)
	{
		if (ScreenManager.PersonalSettings[(int)playerIndex].GamePadVibration)
		{
			ScreenManager.PersonalSettings[(int)playerIndex].GamePadVibration = false;
		}
		else
		{
			ScreenManager.PersonalSettings[(int)playerIndex].GamePadVibration = true;
		}
		ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		UpdateVisuals();
	}

	private void SensitivityMenuEntrySelectRight(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.PersonalSettings[(int)playerIndex].Sensitivity++;
		if (ScreenManager.PersonalSettings[(int)playerIndex].Sensitivity > 9)
		{
			ScreenManager.PersonalSettings[(int)playerIndex].Sensitivity = 9;
			ScreenManager.MenuSoundBank.PlayCue("Menu_Deny");
		}
		else
		{
			ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		}
		UpdateVisuals();
	}

	private void SensitivityMenuEntrySelectLeft(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.PersonalSettings[(int)playerIndex].Sensitivity--;
		if (ScreenManager.PersonalSettings[(int)playerIndex].Sensitivity < 1)
		{
			ScreenManager.PersonalSettings[(int)playerIndex].Sensitivity = 1;
			ScreenManager.MenuSoundBank.PlayCue("Menu_Deny");
		}
		else
		{
			ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		}
		UpdateVisuals();
	}

	private void LayoutMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.AddScreen(new ControllerLayoutSelectMenuScreen(ScreenManager.PersonalSettings[(int)e.PlayerIndex].ControllerLayout), e.PlayerIndex);
	}
}
