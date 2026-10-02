using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PhysCoop;

public class FrameRateCounter : DrawableGameComponent
{
	private ContentManager content;

	private SpriteBatch spriteBatch;

	private SpriteFont spriteFont;

	private int frameRate = 0;

	private int frameCounter = 0;

	private TimeSpan elapsedTime = TimeSpan.Zero;

	public FrameRateCounter(Game game)
		: base(game)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected Obj, but got Unknown
		content = new ContentManager((IServiceProvider)game.Services);
	}

	protected override void LoadGraphicsContent(bool loadAllContent)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected Obj, but got Unknown
		if (loadAllContent)
		{
			spriteBatch = new SpriteBatch(((DrawableGameComponent)this).GraphicsDevice);
			spriteFont = content.Load<SpriteFont>("Content\\ScreenManagerResources\\FrameRateFont");
		}
	}

	protected override void UnloadGraphicsContent(bool unloadAllContent)
	{
		if (unloadAllContent)
		{
			content.Unload();
		}
	}

	public override void Update(GameTime gameTime)
	{
		elapsedTime += gameTime.ElapsedGameTime;
		if (elapsedTime > TimeSpan.FromSeconds(1.0))
		{
			elapsedTime -= TimeSpan.FromSeconds(1.0);
			frameRate = frameCounter;
			frameCounter = 0;
		}
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		frameCounter++;
		spriteBatch.Begin();
		spriteBatch.DrawString(spriteFont, frameRate.ToString(), new Vector2(65f, 33f), Color.Black);
		spriteBatch.DrawString(spriteFont, frameRate.ToString(), new Vector2(64f, 32f), Color.White);
		spriteBatch.End();
	}
}
