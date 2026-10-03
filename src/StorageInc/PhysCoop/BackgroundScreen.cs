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
		SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
		Viewport viewport = ((DrawableGameComponent)ScreenManager).GraphicsDevice.Viewport;
		Rectangle val = new Rectangle(0, 0, viewport.Width, viewport.Height);
		byte transitionAlpha = TransitionAlpha;
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque); // XNA 3.0 SpriteBlendMode 0 = None
		spriteBatch.Draw(ScreenManager.MenuBackgroundTexture, val, new Color(transitionAlpha, transitionAlpha, transitionAlpha));
		spriteBatch.End();
	}
}
