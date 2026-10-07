// Port feature: window/display options (not in the Xbox original).
//
// Kept in their own file (Display.cfg, next to the game's other settings) instead of the original
// GlobalSettings.cfg: the original only loads its settings after "Press Start" and the storage
// device check, but the window mode must be known before the window is created.
using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;

namespace PhysCoop;

[Serializable]
public class DisplaySettings
{
	public bool Fullscreen;

	/// <summary>Index into <see cref="DisplayOptions.WindowSizes"/> (windowed mode only).</summary>
	public int WindowSize;

	public bool VSync = true;
}

public static class DisplayOptions
{
	public static readonly Point[] WindowSizes =
	{
		new Point(1280, 720), new Point(1600, 900), new Point(1920, 1080), new Point(2560, 1440), new Point(3840, 2160)
	};

	private const string FileName = "Display.cfg";

	public static DisplaySettings Current { get; private set; } = new DisplaySettings();

	private static string FilePath(string titleName) => Path.Combine(new LegacyStorageContainer(titleName).Path, FileName);

	public static void Load(string titleName)
	{
		try
		{
			string path = FilePath(titleName);
			if (File.Exists(path))
			{
				using FileStream stream = File.OpenRead(path);
				Current = (DisplaySettings)new XmlSerializer(typeof(DisplaySettings)).Deserialize(stream);
			}
		}
		catch (Exception e)
		{
			Console.WriteLine("Display settings not loaded, using defaults: " + e.Message);
			Current = new DisplaySettings();
		}
		Current.WindowSize = Math.Clamp(Current.WindowSize, 0, WindowSizes.Length - 1);
	}

	public static void Save(string titleName)
	{
		try
		{
			using FileStream stream = File.Create(FilePath(titleName));
			new XmlSerializer(typeof(DisplaySettings)).Serialize(stream, Current);
		}
		catch (Exception e)
		{
			Console.WriteLine("Display settings not saved: " + e.Message);
		}
	}

	public static void Reset()
	{
		Current = new DisplaySettings();
	}

	/// <summary>Sets the preferred back buffer before the window exists (call from the Game constructor).</summary>
	public static void Configure(GraphicsDeviceManager graphics)
	{
		SetPreferences(graphics);
	}

	/// <summary>Applies the current settings to the running game.</summary>
	public static void Apply(GraphicsDeviceManager graphics)
	{
		SetPreferences(graphics);
		graphics.ApplyChanges();
	}

	private static void SetPreferences(GraphicsDeviceManager graphics)
	{
		Point size = WindowSizes[Current.WindowSize];
		if (Current.Fullscreen)
		{
			// FNA's fullscreen is borderless at the desktop resolution.
			DisplayMode desktop = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
			size = new Point(desktop.Width, desktop.Height);
		}
		graphics.IsFullScreen = Current.Fullscreen;
		graphics.PreferredBackBufferWidth = size.X;
		graphics.PreferredBackBufferHeight = size.Y;
		graphics.SynchronizeWithVerticalRetrace = Current.VSync;
	}

	public static string WindowSizeText => $"{WindowSizes[Current.WindowSize].X}x{WindowSizes[Current.WindowSize].Y}";
}
