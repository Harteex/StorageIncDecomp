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
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
		SpriteFont smallFont = ScreenManager.SmallFont;
		ScreenManager.FadeBackBufferToBlack(TransitionAlpha / 2);
		Viewport viewport = ((DrawableGameComponent)ScreenManager).GraphicsDevice.Viewport;
		Vector2 val = new Vector2((float)viewport.Width, (float)viewport.Height);
		Vector2 val2 = smallFont.MeasureString(message);
		Vector2 val3 = (val - val2) / 2f;
		Rectangle val4 = new Rectangle((int)val3.X - 32, (int)val3.Y - 48, (int)val2.X + 64, (int)val2.Y + 96);
		Color val5 = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha);
		spriteBatch.Begin();
		spriteBatch.Draw(messageBackground, val4, val5);
		spriteBatch.DrawString(smallFont, message, val3 + new Vector2(1f, -31f), new Color((byte)0, (byte)0, (byte)0, TransitionAlpha));
		spriteBatch.DrawString(smallFont, message, val3 + new Vector2(0f, -32f), val5);
		infoRect = val4;
		DrawMyInfo(spriteBatch);
		spriteBatch.End();
	}
}
