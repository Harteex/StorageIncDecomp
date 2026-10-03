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
		content = new ContentManager((IServiceProvider)game.Services);
	}

	protected override void LoadContent()
	{
		spriteBatch = new SpriteBatch(((DrawableGameComponent)this).GraphicsDevice);
		spriteFont = content.Load<SpriteFont>("Content\\ScreenManagerResources\\FrameRateFont");
	}

	protected override void UnloadContent()
	{
		content.Unload();
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
		frameCounter++;
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied); // XNA 3.0 default: non-premultiplied alpha
		spriteBatch.DrawString(spriteFont, frameRate.ToString(), new Vector2(65f, 33f), Color.Black);
		spriteBatch.DrawString(spriteFont, frameRate.ToString(), new Vector2(64f, 32f), Color.White);
		spriteBatch.End();
	}
}
