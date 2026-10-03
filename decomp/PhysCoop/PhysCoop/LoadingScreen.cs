using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysCoop;

internal class LoadingScreen : GameScreen
{
	private bool loadingIsSlow;

	private bool otherScreensAreGone;

	private GameScreen[] screensToLoad;

	private LoadingScreen(ScreenManager screenManager, bool loadingIsSlow, GameScreen[] screensToLoad)
	{
		this.loadingIsSlow = loadingIsSlow;
		this.screensToLoad = screensToLoad;
		TransitionOnTime = TimeSpan.FromSeconds(0.5);
	}

	public static void Load(ScreenManager screenManager, bool loadingIsSlow, PlayerIndex? controllingPlayer, params GameScreen[] screensToLoad)
	{
		GameScreen[] screens = screenManager.GetScreens();
		foreach (GameScreen gameScreen in screens)
		{
			if ((object)gameScreen.GetType() != typeof(BackgroundScreen))
			{
				gameScreen.ExitScreen();
			}
		}
		LoadingScreen screen = new LoadingScreen(screenManager, loadingIsSlow, screensToLoad);
		screenManager.AddScreen(screen, controllingPlayer);
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		if (!otherScreensAreGone)
		{
			return;
		}
		GameScreen[] screens = ScreenManager.GetScreens();
		foreach (GameScreen gameScreen in screens)
		{
			if ((object)gameScreen.GetType() == typeof(BackgroundScreen))
			{
				gameScreen.ExitScreen();
			}
		}
		ScreenManager.RemoveScreen(this);
		screens = screensToLoad;
		foreach (GameScreen gameScreen in screens)
		{
			if (gameScreen != null)
			{
				ScreenManager.AddScreen(gameScreen, ControllingPlayer);
			}
		}
		((GameComponent)ScreenManager).Game.ResetElapsedTime();
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		if (ScreenState == ScreenState.Active)
		{
			otherScreensAreGone = true;
			GameScreen[] screens = ScreenManager.GetScreens();
			foreach (GameScreen gameScreen in screens)
			{
				if ((object)gameScreen.GetType() != typeof(BackgroundScreen) && (object)gameScreen.GetType() != typeof(LoadingScreen))
				{
					otherScreensAreGone = false;
				}
			}
		}
		if (loadingIsSlow)
		{
			SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
			SpriteFont font = ScreenManager.Font;
			Viewport viewport = ((DrawableGameComponent)ScreenManager).GraphicsDevice.Viewport;
			Vector2 val = new Vector2((float)viewport.Width, (float)viewport.Height);
			Vector2 val2 = font.MeasureString("Loading...");
			Vector2 val3 = (val - val2) / 2f;
			float x = font.MeasureString("Loading...").X;
			x = (int)Math.Ceiling(x / 16f) * 16;
			x /= 2f;
			spriteBatch.Begin();
			spriteBatch.Draw(ScreenManager.PlayerSlotBackground, new Rectangle((int)(val3.X - (float)font.LineSpacing - 4f), (int)(val3.Y - (float)font.LineSpacing), (int)(x * 2f + (float)(font.LineSpacing * 2)), font.LineSpacing * 3), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
			spriteBatch.DrawString(font, "Loading...", val3 + Vector2.One, new Color((byte)0, (byte)0, (byte)0, TransitionAlpha));
			spriteBatch.DrawString(font, "Loading...", val3, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
			spriteBatch.End();
		}
	}
}
