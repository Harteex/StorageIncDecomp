using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PhysCoop;

public class InputState
{
	public const int MaxInputs = 4;

	public readonly KeyboardState[] CurrentKeyboardStates;

	public readonly GamePadState[] CurrentGamePadStates;

	public readonly KeyboardState[] LastKeyboardStates;

	public readonly GamePadState[] LastGamePadStates;

	public readonly bool[] GamePadWasConnected;

	private ScreenManager myScreen;

	public InputState(ScreenManager manager)
	{
		CurrentKeyboardStates = new KeyboardState[4];
		CurrentGamePadStates = new GamePadState[4];
		LastKeyboardStates = new KeyboardState[4];
		LastGamePadStates = new GamePadState[4];
		GamePadWasConnected = new bool[4];
		myScreen = manager;
	}

	public bool PressedA(int i)
	{
		Buttons myA = myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyA;
		return LastGamePadStates[i].IsButtonUp(myA) && CurrentGamePadStates[i].IsButtonDown(myA);
	}

	public bool HoldA(int i)
	{
		return CurrentGamePadStates[i].IsButtonDown(myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyA);
	}

	public bool PressedB(int i)
	{
		Buttons myB = myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyB;
		return LastGamePadStates[i].IsButtonUp(myB) && CurrentGamePadStates[i].IsButtonDown(myB);
	}

	public bool HoldB(int i)
	{
		return CurrentGamePadStates[i].IsButtonDown(myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyB);
	}

	public bool PressedX(int i)
	{
		Buttons myX = myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyX;
		return LastGamePadStates[i].IsButtonUp(myX) && CurrentGamePadStates[i].IsButtonDown(myX);
	}

	public bool HoldX(int i)
	{
		return CurrentGamePadStates[i].IsButtonDown(myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyX);
	}

	public bool PressedY(int i)
	{
		Buttons myY = myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyY;
		return LastGamePadStates[i].IsButtonUp(myY) && CurrentGamePadStates[i].IsButtonDown(myY);
	}

	public bool HoldY(int i)
	{
		return CurrentGamePadStates[i].IsButtonDown(myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyY);
	}

	public bool PressedLeftBumper(int i)
	{
		Buttons myLeftBumper = myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyLeftBumper;
		return LastGamePadStates[i].IsButtonUp(myLeftBumper) && CurrentGamePadStates[i].IsButtonDown(myLeftBumper);
	}

	public bool HoldLeftBumper(int i)
	{
		return CurrentGamePadStates[i].IsButtonDown(myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyLeftBumper);
	}

	public bool PressedRightBumper(int i)
	{
		Buttons myRightBumper = myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyRightBumper;
		return LastGamePadStates[i].IsButtonUp(myRightBumper) && CurrentGamePadStates[i].IsButtonDown(myRightBumper);
	}

	public bool HoldRightBumper(int i)
	{
		return CurrentGamePadStates[i].IsButtonDown(myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyRightBumper);
	}

	public bool PressedStart(int i)
	{
		Buttons myStart = myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyStart;
		return LastGamePadStates[i].IsButtonUp(myStart) && CurrentGamePadStates[i].IsButtonDown(myStart);
	}

	public bool HoldStart(int i)
	{
		return CurrentGamePadStates[i].IsButtonDown(myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyStart);
	}

	public bool PressedBack(int i)
	{
		Buttons myBack = myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyBack;
		return LastGamePadStates[i].IsButtonUp(myBack) && CurrentGamePadStates[i].IsButtonDown(myBack);
	}

	public bool HoldBack(int i)
	{
		return CurrentGamePadStates[i].IsButtonDown(myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyBack);
	}

	public bool PressedDPadLeft(int i)
	{
		Buttons myDPadLeft = myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyDPadLeft;
		return LastGamePadStates[i].IsButtonUp(myDPadLeft) && CurrentGamePadStates[i].IsButtonDown(myDPadLeft);
	}

	public bool HoldDPadLeft(int i)
	{
		return CurrentGamePadStates[i].IsButtonDown(myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyDPadLeft);
	}

	public bool PressedDPadRight(int i)
	{
		Buttons myDPadRight = myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyDPadRight;
		return LastGamePadStates[i].IsButtonUp(myDPadRight) && CurrentGamePadStates[i].IsButtonDown(myDPadRight);
	}

	public bool HoldDPadRight(int i)
	{
		return CurrentGamePadStates[i].IsButtonDown(myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyDPadRight);
	}

	public bool PressedDPadUP(int i)
	{
		Buttons myDPadUp = myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyDPadUp;
		return LastGamePadStates[i].IsButtonUp(myDPadUp) && CurrentGamePadStates[i].IsButtonDown(myDPadUp);
	}

	public bool HoldDPadUP(int i)
	{
		return CurrentGamePadStates[i].IsButtonDown(myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyDPadUp);
	}

	public bool PressedDPadDown(int i)
	{
		Buttons myDPadDown = myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyDPadDown;
		return LastGamePadStates[i].IsButtonUp(myDPadDown) && CurrentGamePadStates[i].IsButtonDown(myDPadDown);
	}

	public bool HoldDPadDown(int i)
	{
		return CurrentGamePadStates[i].IsButtonDown(myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyDPadDown);
	}

	public bool PressedLeftStickButton(int i)
	{
		Buttons myLeftStickButton = myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyLeftStickButton;
		return LastGamePadStates[i].IsButtonUp(myLeftStickButton) && CurrentGamePadStates[i].IsButtonDown(myLeftStickButton);
	}

	public bool HoldDLeftStickButton(int i)
	{
		return CurrentGamePadStates[i].IsButtonDown(myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyLeftStickButton);
	}

	public bool PressedRightStickButton(int i)
	{
		Buttons myRightStickButton = myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyRightStickButton;
		return LastGamePadStates[i].IsButtonUp(myRightStickButton) && CurrentGamePadStates[i].IsButtonDown(myRightStickButton);
	}

	public bool HoldDRightStickButton(int i)
	{
		return CurrentGamePadStates[i].IsButtonDown(myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].MyRightStickButton);
	}

	public Vector2 LeftStick(int i)
	{
		GamePadThumbSticks thumbSticks;
		if (!myScreen.PersonalSettings[i].SwitchThumbSticks)
		{
			thumbSticks = CurrentGamePadStates[i].ThumbSticks;
			return thumbSticks.Left;
		}
		thumbSticks = CurrentGamePadStates[i].ThumbSticks;
		return thumbSticks.Right;
	}

	public Vector2 RightStick(int i)
	{
		float num = 1f;
		if (myScreen.UseOptionSensitivity)
		{
			num = (float)myScreen.PersonalSettings[i].Sensitivity / 5f * 2f;
		}
		GamePadThumbSticks thumbSticks;
		if (!myScreen.PersonalSettings[i].SwitchThumbSticks)
		{
			if (!myScreen.PersonalSettings[i].InvertLook)
			{
				thumbSticks = CurrentGamePadStates[i].ThumbSticks;
				return thumbSticks.Right * num;
			}
			thumbSticks = CurrentGamePadStates[i].ThumbSticks;
			return thumbSticks.Right * new Vector2(0f, -1f) * num;
		}
		if (!myScreen.PersonalSettings[i].InvertLook)
		{
			thumbSticks = CurrentGamePadStates[i].ThumbSticks;
			return thumbSticks.Left * num;
		}
		thumbSticks = CurrentGamePadStates[i].ThumbSticks;
		return thumbSticks.Left * new Vector2(0f, -1f) * num;
	}

	public float LeftTriggerCurrent(int i)
	{
		GamePadTriggers triggers;
		if (!myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].SwitchTriggers)
		{
			triggers = CurrentGamePadStates[i].Triggers;
			return triggers.Left;
		}
		triggers = CurrentGamePadStates[i].Triggers;
		return triggers.Right;
	}

	public float LeftTriggerLast(int i)
	{
		GamePadTriggers triggers;
		if (!myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].SwitchTriggers)
		{
			triggers = LastGamePadStates[i].Triggers;
			return triggers.Left;
		}
		triggers = LastGamePadStates[i].Triggers;
		return triggers.Right;
	}

	public float RightTriggerCurrent(int i)
	{
		GamePadTriggers triggers;
		if (!myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].SwitchTriggers)
		{
			triggers = CurrentGamePadStates[i].Triggers;
			return triggers.Right;
		}
		triggers = CurrentGamePadStates[i].Triggers;
		return triggers.Left;
	}

	public float RightTriggerLast(int i)
	{
		GamePadTriggers triggers;
		if (!myScreen.ControllerLayouts.Layouts[myScreen.PersonalSettings[i].ControllerLayout].SwitchTriggers)
		{
			triggers = LastGamePadStates[i].Triggers;
			return triggers.Right;
		}
		triggers = LastGamePadStates[i].Triggers;
		return triggers.Left;
	}

	public void Update()
	{
		for (int i = 0; i < 4; i++)
		{
			ref KeyboardState reference = ref LastKeyboardStates[i];
			reference = CurrentKeyboardStates[i];
			ref GamePadState reference2 = ref LastGamePadStates[i];
			reference2 = CurrentGamePadStates[i];
			ref KeyboardState reference3 = ref CurrentKeyboardStates[i];
			reference3 = Keyboard.GetState((PlayerIndex)i);
			ref GamePadState reference4 = ref CurrentGamePadStates[i];
			reference4 = GamePad.GetState((PlayerIndex)i);
			if (CurrentGamePadStates[i].IsConnected)
			{
				GamePadWasConnected[i] = true;
			}
		}
	}

	public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		if (controllingPlayer.HasValue)
		{
			playerIndex = (PlayerIndex)(int)controllingPlayer.Value;
			int num = (int)playerIndex;
			// Port: Alt+Enter toggles fullscreen (PhysCoop.Update), so it must not also select menu entries.
			if (key == Keys.Enter && (CurrentKeyboardStates[num].IsKeyDown(Keys.LeftAlt) || CurrentKeyboardStates[num].IsKeyDown(Keys.RightAlt)))
			{
				return false;
			}
			return CurrentKeyboardStates[num].IsKeyDown(key) && LastKeyboardStates[num].IsKeyUp(key);
		}
		return IsNewKeyPress(key, (PlayerIndex)0, out playerIndex) || IsNewKeyPress(key, (PlayerIndex)1, out playerIndex) || IsNewKeyPress(key, (PlayerIndex)2, out playerIndex) || IsNewKeyPress(key, (PlayerIndex)3, out playerIndex);
	}

	public bool IsKeyHeld(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		if (controllingPlayer.HasValue)
		{
			playerIndex = (PlayerIndex)(int)controllingPlayer.Value;
			int num = (int)playerIndex;
			return CurrentKeyboardStates[num].IsKeyDown(key);
		}
		return IsKeyHeld(key, (PlayerIndex)0, out playerIndex) || IsKeyHeld(key, (PlayerIndex)1, out playerIndex) || IsKeyHeld(key, (PlayerIndex)2, out playerIndex) || IsKeyHeld(key, (PlayerIndex)3, out playerIndex);
	}

	public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		if (controllingPlayer.HasValue)
		{
			playerIndex = (PlayerIndex)(int)controllingPlayer.Value;
			int num = (int)playerIndex;
			return CurrentGamePadStates[num].IsButtonDown(button) && LastGamePadStates[num].IsButtonUp(button);
		}
		return IsNewButtonPress(button, (PlayerIndex)0, out playerIndex) || IsNewButtonPress(button, (PlayerIndex)1, out playerIndex) || IsNewButtonPress(button, (PlayerIndex)2, out playerIndex) || IsNewButtonPress(button, (PlayerIndex)3, out playerIndex);
	}

	public bool IsButtonHeld(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		if (controllingPlayer.HasValue)
		{
			playerIndex = (PlayerIndex)(int)controllingPlayer.Value;
			int num = (int)playerIndex;
			return CurrentGamePadStates[num].IsButtonDown(button);
		}
		return IsButtonHeld(button, (PlayerIndex)0, out playerIndex) || IsButtonHeld(button, (PlayerIndex)1, out playerIndex) || IsButtonHeld(button, (PlayerIndex)2, out playerIndex) || IsButtonHeld(button, (PlayerIndex)3, out playerIndex);
	}

	public bool IsNewThumbDown(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		int result;
		if (controllingPlayer.HasValue)
		{
			playerIndex = (PlayerIndex)(int)controllingPlayer.Value;
			int num = (int)playerIndex;
			GamePadThumbSticks thumbSticks = CurrentGamePadStates[num].ThumbSticks;
			if (thumbSticks.Left.Y < -0.4f)
			{
				thumbSticks = LastGamePadStates[num].ThumbSticks;
				if (thumbSticks.Left.Y >= -0.4f)
				{
					result = 1;
					goto IL_00bc;
				}
			}
			thumbSticks = CurrentGamePadStates[num].ThumbSticks;
			if (thumbSticks.Right.Y < -0.4f)
			{
				thumbSticks = LastGamePadStates[num].ThumbSticks;
				result = ((thumbSticks.Right.Y >= -0.4f) ? 1 : 0);
			}
			else
			{
				result = 0;
			}
			goto IL_00bc;
		}
		return IsNewThumbDown((PlayerIndex)0, out playerIndex) || IsNewThumbDown((PlayerIndex)1, out playerIndex) || IsNewThumbDown((PlayerIndex)2, out playerIndex) || IsNewThumbDown((PlayerIndex)3, out playerIndex);
		IL_00bc:
		return (byte)result != 0;
	}

	public bool IsNewThumbUp(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		int result;
		if (controllingPlayer.HasValue)
		{
			playerIndex = (PlayerIndex)(int)controllingPlayer.Value;
			int num = (int)playerIndex;
			GamePadThumbSticks thumbSticks = CurrentGamePadStates[num].ThumbSticks;
			if (thumbSticks.Left.Y > 0.4f)
			{
				thumbSticks = LastGamePadStates[num].ThumbSticks;
				if (thumbSticks.Left.Y <= 0.4f)
				{
					result = 1;
					goto IL_00bc;
				}
			}
			thumbSticks = CurrentGamePadStates[num].ThumbSticks;
			if (thumbSticks.Right.Y > 0.4f)
			{
				thumbSticks = LastGamePadStates[num].ThumbSticks;
				result = ((thumbSticks.Right.Y <= 0.4f) ? 1 : 0);
			}
			else
			{
				result = 0;
			}
			goto IL_00bc;
		}
		return IsNewThumbUp((PlayerIndex)0, out playerIndex) || IsNewThumbUp((PlayerIndex)1, out playerIndex) || IsNewThumbUp((PlayerIndex)2, out playerIndex) || IsNewThumbUp((PlayerIndex)3, out playerIndex);
		IL_00bc:
		return (byte)result != 0;
	}

	public bool IsNewThumbLeft(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		int result;
		if (controllingPlayer.HasValue)
		{
			playerIndex = (PlayerIndex)(int)controllingPlayer.Value;
			int num = (int)playerIndex;
			GamePadThumbSticks thumbSticks = CurrentGamePadStates[num].ThumbSticks;
			if (thumbSticks.Left.X < -0.4f)
			{
				thumbSticks = LastGamePadStates[num].ThumbSticks;
				if (thumbSticks.Left.X >= -0.4f)
				{
					result = 1;
					goto IL_00bc;
				}
			}
			thumbSticks = CurrentGamePadStates[num].ThumbSticks;
			if (thumbSticks.Right.X < -0.4f)
			{
				thumbSticks = LastGamePadStates[num].ThumbSticks;
				result = ((thumbSticks.Right.X >= -0.4f) ? 1 : 0);
			}
			else
			{
				result = 0;
			}
			goto IL_00bc;
		}
		return IsNewThumbLeft((PlayerIndex)0, out playerIndex) || IsNewThumbLeft((PlayerIndex)1, out playerIndex) || IsNewThumbLeft((PlayerIndex)2, out playerIndex) || IsNewThumbLeft((PlayerIndex)3, out playerIndex);
		IL_00bc:
		return (byte)result != 0;
	}

	public bool IsNewThumbRight(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		int result;
		if (controllingPlayer.HasValue)
		{
			playerIndex = (PlayerIndex)(int)controllingPlayer.Value;
			int num = (int)playerIndex;
			GamePadThumbSticks thumbSticks = CurrentGamePadStates[num].ThumbSticks;
			if (thumbSticks.Left.X > 0.4f)
			{
				thumbSticks = LastGamePadStates[num].ThumbSticks;
				if (thumbSticks.Left.X <= 0.4f)
				{
					result = 1;
					goto IL_00bc;
				}
			}
			thumbSticks = CurrentGamePadStates[num].ThumbSticks;
			if (thumbSticks.Right.X > 0.4f)
			{
				thumbSticks = LastGamePadStates[num].ThumbSticks;
				result = ((thumbSticks.Right.X <= 0.4f) ? 1 : 0);
			}
			else
			{
				result = 0;
			}
			goto IL_00bc;
		}
		return IsNewThumbRight((PlayerIndex)0, out playerIndex) || IsNewThumbRight((PlayerIndex)1, out playerIndex) || IsNewThumbRight((PlayerIndex)2, out playerIndex) || IsNewThumbRight((PlayerIndex)3, out playerIndex);
		IL_00bc:
		return (byte)result != 0;
	}

	public bool IsThumbHeldDown(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		if (controllingPlayer.HasValue)
		{
			playerIndex = (PlayerIndex)(int)controllingPlayer.Value;
			int num = (int)playerIndex;
			GamePadThumbSticks thumbSticks = CurrentGamePadStates[num].ThumbSticks;
			int result;
			if (!(thumbSticks.Left.Y < -0.4f))
			{
				thumbSticks = CurrentGamePadStates[num].ThumbSticks;
				result = ((thumbSticks.Right.Y < -0.4f) ? 1 : 0);
			}
			else
			{
				result = 1;
			}
			return (byte)result != 0;
		}
		return IsThumbHeldDown((PlayerIndex)0, out playerIndex) || IsThumbHeldDown((PlayerIndex)1, out playerIndex) || IsThumbHeldDown((PlayerIndex)2, out playerIndex) || IsThumbHeldDown((PlayerIndex)3, out playerIndex);
	}

	public bool IsThumbHeldUp(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		if (controllingPlayer.HasValue)
		{
			playerIndex = (PlayerIndex)(int)controllingPlayer.Value;
			int num = (int)playerIndex;
			GamePadThumbSticks thumbSticks = CurrentGamePadStates[num].ThumbSticks;
			int result;
			if (!(thumbSticks.Left.Y > 0.4f))
			{
				thumbSticks = CurrentGamePadStates[num].ThumbSticks;
				result = ((thumbSticks.Right.Y > 0.4f) ? 1 : 0);
			}
			else
			{
				result = 1;
			}
			return (byte)result != 0;
		}
		return IsThumbHeldUp((PlayerIndex)0, out playerIndex) || IsThumbHeldUp((PlayerIndex)1, out playerIndex) || IsThumbHeldUp((PlayerIndex)2, out playerIndex) || IsThumbHeldUp((PlayerIndex)3, out playerIndex);
	}

	public bool IsThumbHeldLeft(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		if (controllingPlayer.HasValue)
		{
			playerIndex = (PlayerIndex)(int)controllingPlayer.Value;
			int num = (int)playerIndex;
			GamePadThumbSticks thumbSticks = CurrentGamePadStates[num].ThumbSticks;
			int result;
			if (!(thumbSticks.Left.X < -0.4f))
			{
				thumbSticks = CurrentGamePadStates[num].ThumbSticks;
				result = ((thumbSticks.Right.X < -0.4f) ? 1 : 0);
			}
			else
			{
				result = 1;
			}
			return (byte)result != 0;
		}
		return IsThumbHeldLeft((PlayerIndex)0, out playerIndex) || IsThumbHeldLeft((PlayerIndex)1, out playerIndex) || IsThumbHeldLeft((PlayerIndex)2, out playerIndex) || IsThumbHeldLeft((PlayerIndex)3, out playerIndex);
	}

	public bool IsThumbHeldRight(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		if (controllingPlayer.HasValue)
		{
			playerIndex = (PlayerIndex)(int)controllingPlayer.Value;
			int num = (int)playerIndex;
			GamePadThumbSticks thumbSticks = CurrentGamePadStates[num].ThumbSticks;
			int result;
			if (!(thumbSticks.Left.X > 0.4f))
			{
				thumbSticks = CurrentGamePadStates[num].ThumbSticks;
				result = ((thumbSticks.Right.X > 0.4f) ? 1 : 0);
			}
			else
			{
				result = 1;
			}
			return (byte)result != 0;
		}
		return IsThumbHeldRight((PlayerIndex)0, out playerIndex) || IsThumbHeldRight((PlayerIndex)1, out playerIndex) || IsThumbHeldRight((PlayerIndex)2, out playerIndex) || IsThumbHeldRight((PlayerIndex)3, out playerIndex);
	}

	public bool IsMenuSelect(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		return IsNewKeyPress((Keys)32, controllingPlayer, out playerIndex) || IsNewKeyPress((Keys)13, controllingPlayer, out playerIndex) || IsNewButtonPress((Buttons)4096, controllingPlayer, out playerIndex) || IsNewButtonPress((Buttons)16, controllingPlayer, out playerIndex);
	}

	public bool IsMenuCancel(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		return IsNewKeyPress((Keys)27, controllingPlayer, out playerIndex) || IsNewButtonPress((Buttons)8192, controllingPlayer, out playerIndex) || IsNewButtonPress((Buttons)32, controllingPlayer, out playerIndex);
	}

	public bool IsMenuUp(PlayerIndex? controllingPlayer, bool zeroDelay)
	{
		PlayerIndex playerIndex;
		if (zeroDelay)
		{
			return IsKeyHeld((Keys)38, controllingPlayer, out playerIndex) || IsButtonHeld((Buttons)1, controllingPlayer, out playerIndex) || IsThumbHeldUp(controllingPlayer, out playerIndex);
		}
		return IsNewKeyPress((Keys)38, controllingPlayer, out playerIndex) || IsNewButtonPress((Buttons)1, controllingPlayer, out playerIndex) || IsNewThumbUp(controllingPlayer, out playerIndex);
	}

	public bool IsMenuDown(PlayerIndex? controllingPlayer, bool zeroDelay)
	{
		PlayerIndex playerIndex;
		if (zeroDelay)
		{
			return IsKeyHeld((Keys)40, controllingPlayer, out playerIndex) || IsButtonHeld((Buttons)2, controllingPlayer, out playerIndex) || IsThumbHeldDown(controllingPlayer, out playerIndex);
		}
		return IsNewKeyPress((Keys)40, controllingPlayer, out playerIndex) || IsNewButtonPress((Buttons)2, controllingPlayer, out playerIndex) || IsNewThumbDown(controllingPlayer, out playerIndex);
	}

	public bool IsPauseGame(PlayerIndex? controllingPlayer)
	{
		PlayerIndex playerIndex;
		return IsNewKeyPress((Keys)27, controllingPlayer, out playerIndex) || IsNewButtonPress((Buttons)32, controllingPlayer, out playerIndex) || IsNewButtonPress((Buttons)16, controllingPlayer, out playerIndex);
	}

	public bool IsMenuLeft(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		return IsNewKeyPress((Keys)37, controllingPlayer, out playerIndex) || IsNewButtonPress((Buttons)4, controllingPlayer, out playerIndex) || IsNewThumbLeft(controllingPlayer, out playerIndex);
	}

	public bool IsMenuRight(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		return IsNewKeyPress((Keys)39, controllingPlayer, out playerIndex) || IsNewButtonPress((Buttons)8, controllingPlayer, out playerIndex) || IsNewThumbRight(controllingPlayer, out playerIndex);
	}

	public bool IsMenuA(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		return IsNewKeyPress((Keys)13, controllingPlayer, out playerIndex) || IsNewKeyPress((Keys)32, controllingPlayer, out playerIndex) || IsNewButtonPress((Buttons)4096, controllingPlayer, out playerIndex) || IsNewButtonPress((Buttons)16, controllingPlayer, out playerIndex);
	}

	public bool IsMenuB(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		return IsNewKeyPress((Keys)27, controllingPlayer, out playerIndex) || IsNewButtonPress((Buttons)8192, controllingPlayer, out playerIndex) || IsNewButtonPress((Buttons)32, controllingPlayer, out playerIndex);
	}

	public bool IsMenuX(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		return IsNewKeyPress((Keys)88, controllingPlayer, out playerIndex) || IsNewButtonPress((Buttons)16384, controllingPlayer, out playerIndex);
	}

	public bool IsMenuY(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
	{
		return IsNewKeyPress((Keys)89, controllingPlayer, out playerIndex) || IsNewButtonPress((Buttons)32768, controllingPlayer, out playerIndex);
	}
}
