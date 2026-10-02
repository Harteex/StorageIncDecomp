using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PhysCoop;

internal class GameOptionsMenuScreen : MenuScreen
{
	private MenuEntry brightnessMenuEntry;

	private MenuEntry soundVolumeMenuEntry;

	private MenuEntry musicVolumeMenuEntry;

	public GameOptionsMenuScreen(ScreenManager sm)
		: base("Game Options")
	{
		IsPopup = true;
		ScreenManager = sm;
		brightnessMenuEntry = new MenuEntry(string.Empty, lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		soundVolumeMenuEntry = new MenuEntry(string.Empty, lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		if (sm.ShowMusicVolumeOption)
		{
			musicVolumeMenuEntry = new MenuEntry(string.Empty, lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		}
		MenuEntry menuEntry = new MenuEntry("Back", lockedOnTrial: false, removeOnFullversion: false, reqPrivAllowPurchaseContent: false);
		brightnessMenuEntry.SelectRight += BrightnessMenuEntrySelectRight;
		brightnessMenuEntry.SelectLeft += BrightnessMenuEntrySelectLeft;
		soundVolumeMenuEntry.SelectRight += SoundVolumeMenuEntrySelectRight;
		soundVolumeMenuEntry.SelectLeft += SoundVolumeMenuEntrySelectLeft;
		musicVolumeMenuEntry.SelectRight += MusicVolumeMenuEntrySelectRight;
		musicVolumeMenuEntry.SelectLeft += MusicVolumeMenuEntrySelectLeft;
		menuEntry.Selected += OnCancel;
		MenuEntries.Add(brightnessMenuEntry);
		MenuEntries.Add(soundVolumeMenuEntry);
		if (sm.ShowMusicVolumeOption)
		{
			MenuEntries.Add(musicVolumeMenuEntry);
		}
		MenuEntries.Add(menuEntry);
		infoEntryList = new List<InfoEntry>();
		infoEntryList.Add(new InfoEntry("£ Modify Option", removeOnFullversion: false));
		infoEntryList.Add(new InfoEntry("\u0099 Defaults", removeOnFullversion: false));
		infoEntryList.Add(new InfoEntry("\u0097 Back", removeOnFullversion: false));
		UpdateText();
	}

	private void UpdateText()
	{
		brightnessMenuEntry.Text = "Brightness   : " + ScreenManager.GlobalSettings.Brightness;
		soundVolumeMenuEntry.Text = "Sound Volume : " + ScreenManager.GlobalSettings.SoundVolume;
		musicVolumeMenuEntry.Text = "Music Volume : " + ScreenManager.GlobalSettings.MusicVolume;
	}

	protected override void OnCancel(PlayerIndex playerIndex)
	{
		ScreenManager.NeedToUseStorage = true;
		ScreenManager.RequestSaveGlobalSettings = true;
		ExitScreen();
		ScreenManager.MenuSoundBank.PlayCue("Menu_Back");
	}

	protected override void OnSelectY(PlayerIndex playerIndex)
	{
		ScreenManager.GlobalSettings = new GameSettings();
		UpdateText();
		ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
	}

	private void BrightnessMenuEntrySelectRight(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.GlobalSettings.Brightness++;
		if (ScreenManager.GlobalSettings.Brightness > 9)
		{
			ScreenManager.GlobalSettings.Brightness = 9;
			ScreenManager.MenuSoundBank.PlayCue("Menu_Deny");
		}
		else
		{
			ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		}
		UpdateText();
	}

	private void BrightnessMenuEntrySelectLeft(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.GlobalSettings.Brightness--;
		if (ScreenManager.GlobalSettings.Brightness < 1)
		{
			ScreenManager.GlobalSettings.Brightness = 1;
			ScreenManager.MenuSoundBank.PlayCue("Menu_Deny");
		}
		else
		{
			ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		}
		UpdateText();
	}

	private void SoundVolumeMenuEntrySelectRight(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.GlobalSettings.SoundVolume++;
		if (ScreenManager.GlobalSettings.SoundVolume > 9)
		{
			ScreenManager.GlobalSettings.SoundVolume = 9;
			ScreenManager.MenuSoundBank.PlayCue("Menu_Deny");
		}
		else
		{
			ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		}
		UpdateText();
	}

	private void SoundVolumeMenuEntrySelectLeft(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.GlobalSettings.SoundVolume--;
		if (ScreenManager.GlobalSettings.SoundVolume < 0)
		{
			ScreenManager.GlobalSettings.SoundVolume = 0;
			ScreenManager.MenuSoundBank.PlayCue("Menu_Deny");
		}
		else
		{
			ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		}
		UpdateText();
	}

	private void MusicVolumeMenuEntrySelectRight(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.GlobalSettings.MusicVolume++;
		if (ScreenManager.GlobalSettings.MusicVolume > 9)
		{
			ScreenManager.GlobalSettings.MusicVolume = 9;
			ScreenManager.MenuSoundBank.PlayCue("Menu_Deny");
		}
		else
		{
			ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		}
		UpdateText();
	}

	private void MusicVolumeMenuEntrySelectLeft(object sender, PlayerIndexEventArgs e)
	{
		ScreenManager.GlobalSettings.MusicVolume--;
		if (ScreenManager.GlobalSettings.MusicVolume < 0)
		{
			ScreenManager.GlobalSettings.MusicVolume = 0;
			ScreenManager.MenuSoundBank.PlayCue("Menu_Deny");
		}
		else
		{
			ScreenManager.MenuSoundBank.PlayCue("Menu_Change");
		}
		UpdateText();
	}
}
