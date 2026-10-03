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
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		reverseRotationMenuEntry.Text = "Arcade Steering: " + (ScreenManager.PersonalSettings[playerIndex].ArcadeReverseSteering ? " On" : "Off");
		switchThumbStickMenuEntry.Text = "Switch Thumbsticks: " + (ScreenManager.PersonalSettings[playerIndex].SwitchThumbSticks ? " On" : "Off");
		vibrationMenuEntry.Text = "Gamepad Vibration: " + (ScreenManager.PersonalSettings[playerIndex].GamePadVibration ? " On" : "Off");
		sensitivityMenuEntry.Text = "Look Sensitivity: " + ScreenManager.PersonalSettings[playerIndex].Sensitivity;
		layoutMenuEntry.Text = "Controller Layout";
	}

	protected override void OnCancel(PlayerIndex playerIndex)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		ScreenManager.NeedToUseStorage = true;
		ScreenManager.RequestSaveProfileSettings[playerIndex] = true;
		ExitScreen();
		ScreenManager.MenuSoundBank.PlayCue("Menu_Back");
	}

	protected override void OnSelectY(PlayerIndex playerIndex)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		ScreenManager.PersonalSettings[playerIndex] = new ProfileSettings();
		UpdateVisuals();
		ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
	}

	private void ReverseRotationMenuEntrySelectRight(object sender, PlayerIndexEventArgs e)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (ScreenManager.PersonalSettings[playerIndex].ArcadeReverseSteering)
		{
			ScreenManager.PersonalSettings[playerIndex].ArcadeReverseSteering = false;
		}
		else
		{
			ScreenManager.PersonalSettings[playerIndex].ArcadeReverseSteering = true;
		}
		ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		UpdateVisuals();
	}

	private void ReverseRotationMenuEntrySelectLeft(object sender, PlayerIndexEventArgs e)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (ScreenManager.PersonalSettings[playerIndex].ArcadeReverseSteering)
		{
			ScreenManager.PersonalSettings[playerIndex].ArcadeReverseSteering = false;
		}
		else
		{
			ScreenManager.PersonalSettings[playerIndex].ArcadeReverseSteering = true;
		}
		ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		UpdateVisuals();
	}

	private void SwitchThumbStickMenuEntrySelectRight(object sender, PlayerIndexEventArgs e)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (ScreenManager.PersonalSettings[playerIndex].SwitchThumbSticks)
		{
			ScreenManager.PersonalSettings[playerIndex].SwitchThumbSticks = false;
		}
		else
		{
			ScreenManager.PersonalSettings[playerIndex].SwitchThumbSticks = true;
		}
		ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		UpdateVisuals();
	}

	private void SwitchThumbStickMenuEntrySelectLeft(object sender, PlayerIndexEventArgs e)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (ScreenManager.PersonalSettings[playerIndex].SwitchThumbSticks)
		{
			ScreenManager.PersonalSettings[playerIndex].SwitchThumbSticks = false;
		}
		else
		{
			ScreenManager.PersonalSettings[playerIndex].SwitchThumbSticks = true;
		}
		ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		UpdateVisuals();
	}

	private void VibrationMenuEntrySelectRight(object sender, PlayerIndexEventArgs e)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (ScreenManager.PersonalSettings[playerIndex].GamePadVibration)
		{
			ScreenManager.PersonalSettings[playerIndex].GamePadVibration = false;
		}
		else
		{
			ScreenManager.PersonalSettings[playerIndex].GamePadVibration = true;
		}
		ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		UpdateVisuals();
	}

	private void VibrationMenuEntrySelectLeft(object sender, PlayerIndexEventArgs e)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (ScreenManager.PersonalSettings[playerIndex].GamePadVibration)
		{
			ScreenManager.PersonalSettings[playerIndex].GamePadVibration = false;
		}
		else
		{
			ScreenManager.PersonalSettings[playerIndex].GamePadVibration = true;
		}
		ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		UpdateVisuals();
	}

	private void SensitivityMenuEntrySelectRight(object sender, PlayerIndexEventArgs e)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		ScreenManager.PersonalSettings[playerIndex].Sensitivity++;
		if (ScreenManager.PersonalSettings[playerIndex].Sensitivity > 9)
		{
			ScreenManager.PersonalSettings[playerIndex].Sensitivity = 9;
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
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		ScreenManager.PersonalSettings[playerIndex].Sensitivity--;
		if (ScreenManager.PersonalSettings[playerIndex].Sensitivity < 1)
		{
			ScreenManager.PersonalSettings[playerIndex].Sensitivity = 1;
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
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		ScreenManager.AddScreen(new ControllerLayoutSelectMenuScreen(ScreenManager.PersonalSettings[e.PlayerIndex].ControllerLayout), e.PlayerIndex);
	}
}
