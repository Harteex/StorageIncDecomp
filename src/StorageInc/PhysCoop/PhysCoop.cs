using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PhysCoop;

public class PhysCoop : Game
{
	public static GraphicsDeviceManager graphics;

	private ScreenManager screenManager;

	private bool altEnterWasDown;

	public PhysCoop()
	{
		((Game)this).Content.RootDirectory = "Content";
		graphics = new GraphicsDeviceManager((Game)(object)this);
		graphics.PreferredBackBufferWidth = 1280;
		graphics.PreferredBackBufferHeight = 720;
		screenManager = new ScreenManager((Game)(object)this);
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)screenManager);
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)new GamerServicesComponent((Game)(object)this));
		// Port: window options (Game Options menu, Alt+Enter). The scene renders at the window's
		// resolution and the UI is scaled, so any window size works.
		DisplayOptions.Load(screenManager.TitleOfGame);
		DisplayOptions.Configure(graphics);
		Window.AllowUserResizing = true;
	}

	/// <summary>Port: applies changed display options and saves them.</summary>
	public static void ApplyDisplayOptions(string titleName)
	{
		DisplayOptions.Apply(graphics);
		DisplayOptions.Save(titleName);
	}

	protected override void Update(GameTime gameTime)
	{
		// Port: Alt+Enter toggles fullscreen, like most PC games.
		KeyboardState keyboard = Keyboard.GetState();
		bool altEnter = keyboard.IsKeyDown(Keys.Enter) && (keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt));
		if (altEnter && !altEnterWasDown)
		{
			DisplayOptions.Current.Fullscreen = !DisplayOptions.Current.Fullscreen;
			ApplyDisplayOptions(screenManager.TitleOfGame);
		}
		altEnterWasDown = altEnter;
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		graphics.GraphicsDevice.Clear(Color.Black);
		base.Draw(gameTime);
	}
}
