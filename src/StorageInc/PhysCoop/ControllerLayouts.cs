using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace PhysCoop;

public class ControllerLayouts
{
	public struct LayoutVariant
	{
		public bool SwitchTriggers;

		public string Name;

		public Buttons MyA;

		public Buttons MyB;

		public Buttons MyX;

		public Buttons MyY;

		public Buttons MyStart;

		public Buttons MyBack;

		public Buttons MyLeftBumper;

		public Buttons MyRightBumper;

		public Buttons MyLeftStickButton;

		public Buttons MyRightStickButton;

		public Buttons MyDPadLeft;

		public Buttons MyDPadRight;

		public Buttons MyDPadUp;

		public Buttons MyDPadDown;
	}

	public List<LayoutVariant> Layouts = new List<LayoutVariant>();

	public string DescA = "Lift/Drop Goods";

	public string DescB = "Horn";

	public string DescX = "";

	public string DescY = "";

	public string DescStart = "Pause/Menu";

	public string DescBack = "";

	public string DescLeftTrigger = "Lift/Drop Goods";

	public string DescRightTrigger = "Lift/Drop Goods";

	public string DescLeftBumper = "";

	public string DescRightBumper = "";

	public string DescLeftStick = "Forward/Reverse";

	public string DescRightStick = "Turn Left/Right";

	public string DescLeftStickButton = "";

	public string DescRightStickButton = "";

	public string DescDPadLeft = "";

	public string DescDPadRight = "";

	public string DescDPadUp = "";

	public string DescDPadDown = "";

	public ControllerLayouts()
	{
		LayoutVariant item = new LayoutVariant
		{
			SwitchTriggers = false,
			Name = "Default Controls",
			MyA = (Buttons)4096,
			MyB = (Buttons)8192,
			MyX = (Buttons)16384,
			MyY = (Buttons)32768,
			MyStart = (Buttons)16,
			MyBack = (Buttons)32,
			MyLeftBumper = (Buttons)256,
			MyRightBumper = (Buttons)512,
			MyLeftStickButton = (Buttons)64,
			MyRightStickButton = (Buttons)128,
			MyDPadLeft = (Buttons)4,
			MyDPadRight = (Buttons)8,
			MyDPadUp = (Buttons)1,
			MyDPadDown = (Buttons)2
		};
		Layouts.Add(item);
	}
}
