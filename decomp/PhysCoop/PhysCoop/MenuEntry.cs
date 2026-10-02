using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysCoop;

internal class MenuEntry
{
	private string text;

	private float selectionFade;

	public bool RemoveOnFullversion;

	public bool LockedOnTrial;

	public bool IsUnlockFullVersionEntry;

	public bool IsLocked;

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
		}
	}

	public event EventHandler<PlayerIndexEventArgs> Selected;

	public event EventHandler<PlayerIndexEventArgs> SelectLeft;

	public event EventHandler<PlayerIndexEventArgs> SelectRight;

	protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (Selected != null)
		{
			Selected(this, new PlayerIndexEventArgs(playerIndex));
		}
	}

	protected internal virtual void OnSelectLeft(PlayerIndex playerIndex)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (SelectLeft != null)
		{
			SelectLeft(this, new PlayerIndexEventArgs(playerIndex));
		}
	}

	protected internal virtual void OnSelectRight(PlayerIndex playerIndex)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (SelectRight != null)
		{
			SelectRight(this, new PlayerIndexEventArgs(playerIndex));
		}
	}

	public MenuEntry(string text, bool lockedOnTrial, bool removeOnFullversion, bool reqPrivAllowPurchaseContent)
	{
		this.text = text;
		LockedOnTrial = lockedOnTrial;
		RemoveOnFullversion = removeOnFullversion;
		IsUnlockFullVersionEntry = reqPrivAllowPurchaseContent;
	}

	public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
	{
		float num = (float)gameTime.ElapsedGameTime.TotalSeconds * 4f;
		if (isSelected)
		{
			selectionFade = Math.Min(selectionFade + num, 1f);
		}
		else
		{
			selectionFade = Math.Max(selectionFade - num, 0f);
		}
	}

	public virtual void Draw(MenuScreen screen, Vector2 position, bool isSelected, GameTime gameTime)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		Color val;
		if (IsLocked)
		{
			val = (isSelected ? new Color((byte)170, (byte)50, (byte)50) : new Color((byte)120, (byte)30, (byte)30));
		}
		else
		{
			val = (Color)(isSelected ? Color.White : new Color((byte)130, (byte)130, (byte)130));
		}
		double totalSeconds = gameTime.TotalGameTime.TotalSeconds;
		float num = (float)Math.Sin(totalSeconds * 6.0) + 1f;
		float num2 = 1f + num * 0.02f * selectionFade;
		val = new Color(val.R, val.G, val.B, screen.TransitionAlpha);
		ScreenManager screenManager = screen.ScreenManager;
		SpriteBatch spriteBatch = screenManager.SpriteBatch;
		SpriteFont font = screenManager.Font;
		Vector2 val2 = new Vector2(font.MeasureString(text).X / 2f, (float)(font.LineSpacing / 2));
		position.X += font.MeasureString(text).X / 2f;
		spriteBatch.DrawString(font, text, position + new Vector2(1f, 1f), new Color((byte)((float)(int)val.R * 0.1f), (byte)((float)(int)val.G * 0.1f), (byte)((float)(int)val.B * 0.1f), val.A), 0f, val2, num2, (SpriteEffects)0, 0f);
		spriteBatch.DrawString(font, text, position, val, 0f, val2, num2, (SpriteEffects)0, 0f);
	}

	public virtual int GetHeight(MenuScreen screen)
	{
		return screen.ScreenManager.Font.LineSpacing;
	}
}
