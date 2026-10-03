using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PhysCoop;

internal class MessageBoxScreen : GameScreen
{
	private string message;

	private Texture2D messageBackground;

	private bool allowCancel;

	public event EventHandler<PlayerIndexEventArgs> Accepted;

	public event EventHandler<PlayerIndexEventArgs> Cancelled;

	public MessageBoxScreen(string p_message, bool p_allowCancel)
	{
		IsPopup = true;
		message = p_message;
		allowCancel = p_allowCancel;
		TransitionOnTime = TimeSpan.FromSeconds(0.2);
		TransitionOffTime = TimeSpan.FromSeconds(0.2);
		infoEntryList = new List<InfoEntry>();
		infoEntryList.Add(new InfoEntry("\u0096 Ok", removeOnFullversion: false));
		if (allowCancel)
		{
			infoEntryList.Add(new InfoEntry("\u0097 Cancel", removeOnFullversion: false));
		}
		infoRect = default;
	}

	public override void LoadContent()
	{
		ContentManager content = ((GameComponent)ScreenManager).Game.Content;
		messageBackground = content.Load<Texture2D>("ScreenManagerResources\\MessageBoxBg");
	}

	public override void HandleInput(InputState input)
	{
		if (input.IsMenuSelect(ControllingPlayer, out var playerIndex))
		{
			if (Accepted != null)
			{
				Accepted(this, new PlayerIndexEventArgs(playerIndex));
			}
			ExitScreen();
			ScreenManager.MenuSoundBank.PlayCue("Menu_Select");
		}
		else if (input.IsMenuCancel(ControllingPlayer, out playerIndex) && allowCancel)
		{
			if (Cancelled != null)
			{
				Cancelled(this, new PlayerIndexEventArgs(playerIndex));
			}
			ExitScreen();
			ScreenManager.MenuSoundBank.PlayCue("Menu_Back");
		}
	}

	public override void Draw(GameTime gameTime)
	{
		SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
		SpriteFont smallFont = ScreenManager.SmallFont;
		ScreenManager.FadeBackBufferToBlack(TransitionAlpha / 2);
		Viewport viewport = ((DrawableGameComponent)ScreenManager).GraphicsDevice.Viewport;
		Vector2 val = new Vector2((float)viewport.Width, (float)viewport.Height);
		Vector2 val2 = smallFont.MeasureString(message);
		Vector2 val3 = (val - val2) / 2f;
		Rectangle val4 = new Rectangle((int)val3.X - 32, (int)val3.Y - 48, (int)val2.X + 64, (int)val2.Y + 96);
		Color val5 = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha);
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied); // XNA 3.0 default: non-premultiplied alpha
		spriteBatch.Draw(messageBackground, val4, val5);
		spriteBatch.DrawString(smallFont, message, val3 + new Vector2(1f, -31f), new Color((byte)0, (byte)0, (byte)0, TransitionAlpha));
		spriteBatch.DrawString(smallFont, message, val3 + new Vector2(0f, -32f), val5);
		infoRect = val4;
		DrawMyInfo(spriteBatch);
		spriteBatch.End();
	}
}
