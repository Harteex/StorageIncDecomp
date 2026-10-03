using System;

namespace PhysCoop;

[Serializable]
public class ProfileSettings
{
	public int ControllerLayout;

	public int Sensitivity;

	public bool SwitchThumbSticks;

	public bool InvertLook;

	public bool GamePadVibration;

	public bool ArcadeReverseSteering;

	public ProfileSettings()
	{
		ControllerLayout = 0;
		Sensitivity = 5;
		SwitchThumbSticks = false;
		InvertLook = false;
		GamePadVibration = true;
		ArcadeReverseSteering = true;
	}
}
