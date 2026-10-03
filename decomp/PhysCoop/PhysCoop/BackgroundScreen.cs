using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysCoop;

internal class BackgroundScreen : GameScreen
{
	public BackgroundScreen()
	{
		TransitionOnTime = TimeSpan.FromSeconds(0.5);
		TransitionOffTime = TimeSpan.FromSeconds(0.5);
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen: false);
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
		Viewport viewport = ((DrawableGameComponent)ScreenManager).GraphicsDevice.Viewport;
		Rectangle val = new Rectangle(0, 0, viewport.Width, viewport.Height);
		byte transitionAlpha = TransitionAlpha;
		spriteBatch.Begin((SpriteBlendMode)0);
		spriteBatch.Draw(ScreenManager.MenuBackgroundTexture, val, new Color(transitionAlpha, transitionAlpha, transitionAlpha));
		spriteBatch.End();
	}
}
