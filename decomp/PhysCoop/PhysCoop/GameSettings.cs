using System;

namespace PhysCoop;

[Serializable]
public class GameSettings
{
	public int Brightness;

	public int SoundVolume;

	public int MusicVolume;

	public GameSettings()
	{
		Brightness = 5;
		SoundVolume = 5;
		MusicVolume = 5;
	}
}
