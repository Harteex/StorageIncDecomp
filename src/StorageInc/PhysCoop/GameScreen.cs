using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace PhysCoop;

public abstract class GameScreen
{
	protected class InfoEntry
	{
		public string Text;

		public bool RemoveOnFullversion;

		public InfoEntry(string text, bool removeOnFullversion)
		{
			Text = text;
			RemoveOnFullversion = removeOnFullversion;
		}
	}

	protected bool instantAppear = false;

	protected bool instantDisappear = false;

	protected Rectangle infoRect;

	protected List<InfoEntry> infoEntryList;

	private bool isPopup = false;

	private TimeSpan transitionOnTime = TimeSpan.Zero;

	private TimeSpan transitionOffTime = TimeSpan.Zero;

	private float transitionPosition = 1f;

	private ScreenState screenState = ScreenState.TransitionOn;

	private bool isExiting = false;

	private bool otherScreenHasFocus;

	private ScreenManager screenManager;

	private PlayerIndex? controllingPlayer;

	public bool IsPopup
	{
		get
		{
			return isPopup;
		}
		protected set
		{
			isPopup = value;
		}
	}

	public TimeSpan TransitionOnTime
	{
		get
		{
			return transitionOnTime;
		}
		protected set
		{
			transitionOnTime = value;
		}
	}

	public TimeSpan TransitionOffTime
	{
		get
		{
			return transitionOffTime;
		}
		protected set
		{
			transitionOffTime = value;
		}
	}

	public float TransitionPosition
	{
		get
		{
			return transitionPosition;
		}
		protected set
		{
			transitionPosition = value;
		}
	}

	public byte TransitionAlpha => (byte)(255f - TransitionPosition * 255f);

	public ScreenState ScreenState
	{
		get
		{
			return screenState;
		}
		protected set
		{
			screenState = value;
		}
	}

	public bool IsExiting
	{
		get
		{
			return isExiting;
		}
		protected internal set
		{
			isExiting = value;
		}
	}

	public bool IsActive => !otherScreenHasFocus && (screenState == ScreenState.TransitionOn || screenState == ScreenState.Active);

	public ScreenManager ScreenManager
	{
		get
		{
			return screenManager;
		}
		internal set
		{
			screenManager = value;
		}
	}

	public PlayerIndex? ControllingPlayer
	{
		get
		{
			return controllingPlayer;
		}
		internal set
		{
			controllingPlayer = value;
		}
	}

	protected void DrawMyInfo(SpriteBatch spritebatch)
	{
		if (infoEntryList == null)
		{
			return;
		}
		if (infoRect.IsEmpty)
		{
			Viewport viewport = ScreenManager.LogicalViewport;
			infoRect = viewport.TitleSafeArea;
		}
		for (int num = infoEntryList.Count - 1; num >= 0; num--)
		{
			if (!Guide.IsTrialMode && infoEntryList[num].RemoveOnFullversion)
			{
				infoEntryList.RemoveAt(num);
			}
		}
		if (infoEntryList.Count <= 0)
		{
			return;
		}
		float num2 = infoRect.Bottom - screenManager.SmallFont.LineSpacing / 2 - 3;
		float num3 = (float)infoRect.Width / (float)infoEntryList.Count;
		foreach (InfoEntry infoEntry in infoEntryList)
		{
			Vector2 val = screenManager.SmallFont.MeasureString(infoEntry.Text) / 2f;
			float num4 = num3 * (float)infoEntryList.IndexOf(infoEntry) + num3 / 2f + (float)infoRect.X;
			spritebatch.DrawString(screenManager.SmallFont, infoEntry.Text, new Vector2(num4 + 1f, num2 + 1f), new Color((byte)10, (byte)10, (byte)10, TransitionAlpha), 0f, val, 1f, (SpriteEffects)0, 1f);
			spritebatch.DrawString(screenManager.SmallFont, infoEntry.Text, new Vector2(num4, num2), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha), 0f, val, 1f, (SpriteEffects)0, 1f);
		}
	}

	public void DrawCenteredText(SpriteBatch spritebatch, string text, Vector2 centerPosition)
	{
		Vector2 val = screenManager.SmallFont.MeasureString(text) / 2f;
		centerPosition.Y -= (float)(screenManager.SmallFont.LineSpacing / 2 + 3);
		spritebatch.DrawString(screenManager.SmallFont, text, centerPosition + new Vector2(1f, 1f), new Color((byte)10, (byte)10, (byte)10, TransitionAlpha), 0f, val, 1f, (SpriteEffects)0, 1f);
		spritebatch.DrawString(screenManager.SmallFont, text, centerPosition, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha), 0f, val, 1f, (SpriteEffects)0, 1f);
	}

	public virtual void LoadContent()
	{
	}

	public virtual void UnloadContent()
	{
	}

	public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		this.otherScreenHasFocus = otherScreenHasFocus;
		if (isExiting)
		{
			screenState = ScreenState.TransitionOff;
			if (!UpdateTransition(gameTime, transitionOffTime, 1) || instantDisappear)
			{
				ScreenManager.RemoveScreen(this);
			}
			return;
		}
		if (coveredByOtherScreen)
		{
			if (UpdateTransition(gameTime, transitionOffTime, 1))
			{
				screenState = ScreenState.TransitionOff;
			}
			else
			{
				screenState = ScreenState.Hidden;
			}
			return;
		}
		if (UpdateTransition(gameTime, transitionOnTime, -1))
		{
			screenState = ScreenState.TransitionOn;
		}
		else
		{
			screenState = ScreenState.Active;
		}
		if (instantAppear)
		{
			screenState = ScreenState.Active;
			transitionPosition = 0f;
		}
	}

	private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
	{
		float num = ((!(time == TimeSpan.Zero)) ? ((float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds)) : 1f);
		transitionPosition += num * (float)direction;
		if ((direction < 0 && transitionPosition <= 0f) || (direction > 0 && transitionPosition >= 1f))
		{
			transitionPosition = MathHelper.Clamp(transitionPosition, 0f, 1f);
			return false;
		}
		return true;
	}

	public virtual void HandleInput(InputState input)
	{
	}

	public virtual void Draw(GameTime gameTime)
	{
	}

	public void ExitScreen()
	{
		if (TransitionOffTime == TimeSpan.Zero)
		{
			ScreenManager.RemoveScreen(this);
		}
		else
		{
			isExiting = true;
		}
	}
}
