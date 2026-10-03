using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhysCoop.Oak;

namespace PhysCoop;

internal abstract class MenuScreen : GameScreen
{
	private List<MenuEntry> menuEntries = new List<MenuEntry>();

	public int SelectedEntry = 0;

	protected Vector2 arrowTarget2D;

	private string menuTitle;

	protected int inputDelay = 5;

	protected bool drawControllerLayout = false;

	protected int menuUpDownDelay = 20;

	protected int menuUpDownCounter = 0;

	protected Texture2D infoTexture;

	protected Texture2D infoBackground;

	protected IList<MenuEntry> MenuEntries => menuEntries;

	public MenuScreen(string menuTitle)
	{
		this.menuTitle = menuTitle;
		TransitionOnTime = TimeSpan.FromSeconds(0.5);
		TransitionOffTime = TimeSpan.FromSeconds(0.5);
	}

	public override void HandleInput(InputState input)
	{
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		if (inputDelay == 0)
		{
			if (menuUpDownCounter > 0)
			{
				menuUpDownCounter--;
			}
			if (input.IsMenuUp(ControllingPlayer, menuUpDownCounter == 0))
			{
				SelectedEntry--;
				if (SelectedEntry < 0)
				{
					SelectedEntry = menuEntries.Count - 1;
				}
				if ((object)GetType() == typeof(SelectMapMenuScreen))
				{
					ScreenManager.NewMapSelected(SelectedEntry);
				}
				ScreenManager.MenuSoundBank.PlayCue("Menu_UpDown");
				menuUpDownCounter = menuUpDownDelay;
			}
			if (input.IsMenuDown(ControllingPlayer, menuUpDownCounter == 0))
			{
				SelectedEntry++;
				if (SelectedEntry >= menuEntries.Count)
				{
					SelectedEntry = 0;
				}
				if ((object)GetType() == typeof(SelectMapMenuScreen))
				{
					ScreenManager.NewMapSelected(SelectedEntry);
				}
				ScreenManager.MenuSoundBank.PlayCue("Menu_UpDown");
				menuUpDownCounter = menuUpDownDelay;
			}
			if (input.IsMenuLeft(ControllingPlayer, out var playerIndex))
			{
				OnSelectLeft(SelectedEntry, playerIndex);
			}
			if (input.IsMenuRight(ControllingPlayer, out playerIndex))
			{
				OnSelectRight(SelectedEntry, playerIndex);
			}
			if (input.IsMenuX(ControllingPlayer, out playerIndex))
			{
				OnSelectX(playerIndex);
			}
			if (input.IsMenuY(ControllingPlayer, out playerIndex))
			{
				OnSelectY(playerIndex);
			}
			if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
			{
				OnSelectEntry(SelectedEntry, playerIndex);
			}
			else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
			{
				OnCancel(playerIndex);
			}
		}
		else
		{
			inputDelay--;
		}
	}

	protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!menuEntries[SelectedEntry].IsLocked)
		{
			menuEntries[SelectedEntry].OnSelectEntry(playerIndex);
		}
		else
		{
			ScreenManager.MenuSoundBank.PlayCue("Menu_Deny");
		}
	}

	protected virtual void OnCancel(PlayerIndex playerIndex)
	{
		ScreenManager.MenuSoundBank.PlayCue("Menu_Back");
		ExitScreen();
	}

	protected void OnCancel(object sender, PlayerIndexEventArgs e)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		OnCancel(e.PlayerIndex);
	}

	protected virtual void OnSelectLeft(int entryIndex, PlayerIndex playerIndex)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		menuEntries[SelectedEntry].OnSelectLeft(playerIndex);
	}

	protected virtual void OnSelectRight(int entryIndex, PlayerIndex playerIndex)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		menuEntries[SelectedEntry].OnSelectRight(playerIndex);
	}

	protected virtual void OnSelectX(PlayerIndex playerIndex)
	{
	}

	protected virtual void OnSelectY(PlayerIndex playerIndex)
	{
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		for (int num = menuEntries.Count - 1; num >= 0; num--)
		{
			menuEntries[num].IsLocked = false;
			if (Guide.IsTrialMode)
			{
				if (menuEntries[num].LockedOnTrial)
				{
					menuEntries[num].IsLocked = true;
				}
			}
			else if (menuEntries[num].RemoveOnFullversion)
			{
				menuEntries.RemoveAt(num);
				SelectedEntry = 0;
			}
		}
		for (int num = 0; num < menuEntries.Count; num++)
		{
			bool isSelected = IsActive && num == SelectedEntry;
			menuEntries[num].Update(this, isSelected, gameTime);
		}
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0700: Unknown result type (might be due to invalid IL or missing references)
		//IL_071b: Unknown result type (might be due to invalid IL or missing references)
		//IL_072a: Unknown result type (might be due to invalid IL or missing references)
		//IL_072f: Unknown result type (might be due to invalid IL or missing references)
		//IL_073d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_0780: Unknown result type (might be due to invalid IL or missing references)
		//IL_0799: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_0650: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		//IL_068c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0881: Unknown result type (might be due to invalid IL or missing references)
		//IL_089b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0baf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c50: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fe2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1004: Unknown result type (might be due to invalid IL or missing references)
		//IL_101b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1077: Unknown result type (might be due to invalid IL or missing references)
		//IL_1091: Unknown result type (might be due to invalid IL or missing references)
		//IL_109b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d76: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e10: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_11d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_11fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1218: Unknown result type (might be due to invalid IL or missing references)
		//IL_1106: Unknown result type (might be due to invalid IL or missing references)
		//IL_1135: Unknown result type (might be due to invalid IL or missing references)
		//IL_1143: Unknown result type (might be due to invalid IL or missing references)
		//IL_116b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1185: Unknown result type (might be due to invalid IL or missing references)
		//IL_1262: Unknown result type (might be due to invalid IL or missing references)
		//IL_127c: Unknown result type (might be due to invalid IL or missing references)
		//IL_129a: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1316: Unknown result type (might be due to invalid IL or missing references)
		//IL_1335: Unknown result type (might be due to invalid IL or missing references)
		//IL_1354: Unknown result type (might be due to invalid IL or missing references)
		//IL_1373: Unknown result type (might be due to invalid IL or missing references)
		//IL_1392: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_13d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_140e: Unknown result type (might be due to invalid IL or missing references)
		//IL_142d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b00: Unknown result type (might be due to invalid IL or missing references)
		//IL_097f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0984: Unknown result type (might be due to invalid IL or missing references)
		//IL_0992: Unknown result type (might be due to invalid IL or missing references)
		//IL_0999: Unknown result type (might be due to invalid IL or missing references)
		//IL_099e: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1649: Unknown result type (might be due to invalid IL or missing references)
		//IL_1663: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a43: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af0: Unknown result type (might be due to invalid IL or missing references)
		//IL_14f1: Unknown result type (might be due to invalid IL or missing references)
		SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
		SpriteFont font = ScreenManager.Font;
		Vector2 val = new Vector2(640f, 400f);
		float num = (float)(font.LineSpacing * menuEntries.Count + font.LineSpacing) + val.Y;
		Viewport viewport = ((DrawableGameComponent)ScreenManager).GraphicsDevice.Viewport;
		Rectangle titleSafeArea = viewport.TitleSafeArea;
		if (num > (float)(titleSafeArea.Bottom - 32))
		{
			viewport = ((DrawableGameComponent)ScreenManager).GraphicsDevice.Viewport;
			titleSafeArea = viewport.TitleSafeArea;
			val.Y = titleSafeArea.Bottom - 32 - (font.LineSpacing * menuEntries.Count + font.LineSpacing);
		}
		float num2 = 0f;
		for (int i = 0; i < menuEntries.Count; i++)
		{
			if (font.MeasureString(menuEntries[i].Text).X > num2)
			{
				num2 = font.MeasureString(menuEntries[i].Text).X;
			}
		}
		num2 = (int)Math.Ceiling(num2 / 16f) * 16;
		num2 /= 2f;
		val.X -= num2;
		float num3 = (float)Math.Pow(TransitionPosition, 2.0);
		spriteBatch.Begin();
		if ((object)GetType() == typeof(HowToPlayMenuScreen) || (object)GetType() == typeof(NagMenuScreen) || (object)GetType() == typeof(CreditsMenuScreen) || (object)GetType() == typeof(MapCompletedMenuScreen) || (object)GetType() == typeof(MapFailedMenuScreen))
		{
			if ((object)GetType() != typeof(MapCompletedMenuScreen) && (object)GetType() != typeof(MapFailedMenuScreen))
			{
				Texture2D val2 = infoBackground;
				viewport = ((DrawableGameComponent)ScreenManager).GraphicsDevice.Viewport;
				float num4 = (float)viewport.Width / 2f - (float)infoBackground.Width / 2f;
				viewport = ((DrawableGameComponent)ScreenManager).GraphicsDevice.Viewport;
				spriteBatch.Draw(val2, new Vector2(num4, (float)viewport.Height / 2f - (float)infoBackground.Height / 2f), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
			}
			Texture2D val3 = infoTexture;
			viewport = ((DrawableGameComponent)ScreenManager).GraphicsDevice.Viewport;
			float num5 = (float)viewport.Width / 2f - (float)infoTexture.Width / 2f;
			viewport = ((DrawableGameComponent)ScreenManager).GraphicsDevice.Viewport;
			spriteBatch.Draw(val3, new Vector2(num5, (float)viewport.Height / 2f - (float)infoTexture.Height / 2f), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
		}
		else if ((object)GetType() == typeof(TutorialMessageMenuScreen))
		{
			TutorialMessageMenuScreen tutorialMessageMenuScreen = (TutorialMessageMenuScreen)this;
			Viewport viewport2 = ((DrawableGameComponent)ScreenManager).GraphicsDevice.Viewport;
			Vector2 val4 = new Vector2((float)viewport2.Width, (float)viewport2.Height);
			Vector2 val5 = ScreenManager.SmallFont.MeasureString(tutorialMessageMenuScreen.message);
			Vector2 val6 = (val4 - val5) / 2f;
			Rectangle val7 = new Rectangle((int)val6.X - 32, (int)val6.Y - 48, (int)val5.X + 64, (int)val5.Y + 96);
			Vector2 val8 = new Vector2((float)val7.X, (float)val7.Y);
			if (arrowTarget2D != Vector2.Zero)
			{
				if (arrowTarget2D.X < 640f)
				{
					val8.X = arrowTarget2D.X + 128f;
				}
				else
				{
					val8.X = arrowTarget2D.X - 128f - (float)val7.Width;
				}
				if (arrowTarget2D.Y < 360f)
				{
					val8.Y = arrowTarget2D.Y + 128f;
				}
				else
				{
					val8.Y = arrowTarget2D.Y - 128f - (float)val7.Height;
				}
				float x = val8.X;
				viewport = ((DrawableGameComponent)ScreenManager).GraphicsDevice.Viewport;
				titleSafeArea = viewport.TitleSafeArea;
				float num6 = titleSafeArea.Left;
				viewport = ((DrawableGameComponent)ScreenManager).GraphicsDevice.Viewport;
				titleSafeArea = viewport.TitleSafeArea;
				val8.X = MathHelper.Clamp(x, num6, (float)(titleSafeArea.Right - val7.Width));
				float y = val8.Y;
				viewport = ((DrawableGameComponent)ScreenManager).GraphicsDevice.Viewport;
				titleSafeArea = viewport.TitleSafeArea;
				float num7 = titleSafeArea.Top;
				viewport = ((DrawableGameComponent)ScreenManager).GraphicsDevice.Viewport;
				titleSafeArea = viewport.TitleSafeArea;
				val8.Y = MathHelper.Clamp(y, num7, (float)(titleSafeArea.Bottom - val7.Height));
				val7.X = (int)val8.X;
				val7.Y = (int)val8.Y;
			}
			infoRect = val7;
			if (arrowTarget2D != Vector2.Zero)
			{
				Draw2D.DrawLine(spriteBatch, ScreenManager.FadeTexture, new Vector2((float)infoRect.Center.X, (float)infoRect.Center.Y) + Vector2.One, arrowTarget2D + Vector2.One, new Color((byte)0, (byte)0, (byte)0, TransitionAlpha));
				Draw2D.DrawLine(spriteBatch, ScreenManager.FadeTexture, new Vector2((float)infoRect.Center.X, (float)infoRect.Center.Y), arrowTarget2D, new Color((byte)170, (byte)180, (byte)188, TransitionAlpha));
			}
			spriteBatch.Draw(tutorialMessageMenuScreen.messageBackground, infoRect, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
			spriteBatch.DrawString(ScreenManager.SmallFont, tutorialMessageMenuScreen.message, new Vector2((float)(infoRect.Center.X + 1) - val5.X / 2f, (float)(infoRect.Center.Y + 1) - val5.Y / 2f) + new Vector2(0f, -32f), new Color((byte)0, (byte)0, (byte)0, TransitionAlpha));
			spriteBatch.DrawString(ScreenManager.SmallFont, tutorialMessageMenuScreen.message, new Vector2((float)infoRect.Center.X - val5.X / 2f, (float)infoRect.Center.Y - val5.Y / 2f) + new Vector2(0f, -32f), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
			DrawMyInfo(spriteBatch);
		}
		else
		{
			if ((object)GetType() != typeof(SelectMapMenuScreen) && (object)GetType() != typeof(MapCompletedMenuScreen) && (object)GetType() != typeof(MapFailedMenuScreen))
			{
				spriteBatch.Draw(ScreenManager.PlayerSlotBackground, new Rectangle((int)(val.X - (float)font.LineSpacing - 4f), (int)(val.Y - (float)font.LineSpacing), (int)(num2 * 2f + (float)(font.LineSpacing * 2)), font.LineSpacing * menuEntries.Count + font.LineSpacing), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
			}
			if ((object)GetType() == typeof(SelectMapMenuScreen))
			{
				val.X = 790f;
				val.Y = 360 - SelectedEntry * ScreenManager.MapEntryBackground.Height;
			}
			for (int i = 0; i < menuEntries.Count; i++)
			{
				MenuEntry menuEntry = menuEntries[i];
				bool isSelected = IsActive && i == SelectedEntry;
				if (ScreenManager.UseMapEntryBackgroundImages && (object)GetType() == typeof(SelectMapMenuScreen))
				{
					Color val9 = ScreenManager.MapDifficultyColor[ScreenManager.MapsContainer.Maps[i].Difficulty];
					spriteBatch.Draw(ScreenManager.MapEntryBackground, val + ScreenManager.MapEntryImageOffset + new Vector2(0f, (float)(-ScreenManager.MapEntryBackground.Height / 2)), new Color(val9.R, val9.G, val9.B, TransitionAlpha));
					if (menuEntry.IsLocked)
					{
						spriteBatch.Draw(ScreenManager.Icon_Locked, val + ScreenManager.MapEntryImageOffset + new Vector2((float)(ScreenManager.Icon_Locked.Width / 2 + 1), (float)(-ScreenManager.Icon_Locked.Height / 2)), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
					}
					else if (ScreenManager.GameSaveInfo.StageCompleted[ScreenManager.FindMapId(i)])
					{
						spriteBatch.Draw(ScreenManager.MapCompletedMarker, val + ScreenManager.MapEntryImageOffset + new Vector2(0f, (float)(-ScreenManager.MapCompletedMarker.Height / 2)), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
					}
				}
				menuEntry.Draw(this, val, isSelected, gameTime);
				if (ScreenManager.UseMapEntryBackgroundImages && (object)GetType() == typeof(SelectMapMenuScreen))
				{
					val.Y += (float)ScreenManager.MapEntryBackground.Height;
				}
				else
				{
					val.Y += (float)menuEntry.GetHeight(this);
				}
			}
			Vector2 val10 = new Vector2(640f, 128f);
			Vector2 val11 = font.MeasureString(menuTitle) / 2f;
			Color val12 = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha);
			float num8 = 1.25f;
			spriteBatch.DrawString(font, menuTitle, val10 + new Vector2(1f, 1f), new Color((byte)((float)(int)val12.R * 0.1f), (byte)((float)(int)val12.G * 0.1f), (byte)((float)(int)val12.B * 0.1f), val12.A), 0f, val11, num8, (SpriteEffects)0, 0f);
			spriteBatch.DrawString(font, menuTitle, val10, val12, 0f, val11, num8, (SpriteEffects)0, 0f);
		}
		Rectangle val13;
		if ((object)GetType() == typeof(GameSetupMenuScreen))
		{
			string text = "XXXXXXXXXXXXXXX";
			float num9 = font.MeasureString(text).X * 0.8f + 64f;
			val13 = new Rectangle(1147 - (int)num9, 0, (int)num9, 125);
			for (int i = 0; i < 4; i++)
			{
				val13.Y = 77 + i * 130;
				spriteBatch.Draw(ScreenManager.PlayerSlotBackground, val13, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
				if (ScreenManager.PlayerIsSignedIn[i])
				{
					text = ScreenManager.StoredGamer[i].Gamertag;
					spriteBatch.DrawString(font, text, new Vector2((float)(val13.X + 70), (float)(val13.Y + 5)), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha), 0f, Vector2.Zero, 0.8f, (SpriteEffects)0, 0f);
					spriteBatch.Draw(ScreenManager.StoredGamerProfile[i].GamerPicture, new Rectangle(val13.X + 4, val13.Top + 4, 64, 64), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
					if (ScreenManager.UsePersonalSettings)
					{
						DrawCenteredText(spriteBatch, "\u0099 Player Settings", new Vector2((float)(val13.X + val13.Width / 2), (float)val13.Bottom));
					}
				}
				else
				{
					DrawCenteredText(spriteBatch, "\u0098 Sign In", new Vector2((float)(val13.X + val13.Width / 2), (float)val13.Bottom));
				}
			}
		}
		if ((object)GetType() == typeof(GameSetupMenuScreen) || (object)GetType() == typeof(SelectMapMenuScreen))
		{
			val13 = new Rectangle(128, 256, 256, 144);
			spriteBatch.Draw(ScreenManager.PlayerSlotBackground, new Rectangle(val13.X - font.LineSpacing / 2, val13.Y - font.LineSpacing - 5, val13.Width + font.LineSpacing, (int)(font.MeasureString(ScreenManager.MapsContainer.Maps[ScreenManager.MapsContainer.ListMapIndex].Description).Y + (float)val13.Height + (float)font.LineSpacing * 1.5f)), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
			string text = ScreenManager.MapsContainer.Maps[ScreenManager.MapsContainer.ListMapIndex].Name;
			spriteBatch.DrawString(font, text, new Vector2((float)(val13.X + val13.Width / 2), (float)(val13.Y - font.LineSpacing / 2)), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha), 0f, font.MeasureString(text) / 2f, 1f, (SpriteEffects)0, 0f);
			spriteBatch.Draw(ScreenManager.MapImage, val13, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
			text = ScreenManager.MapsContainer.Maps[ScreenManager.MapsContainer.ListMapIndex].Description;
			spriteBatch.DrawString(font, text, new Vector2((float)val13.X, (float)(val13.Y + val13.Height + font.LineSpacing / 2)), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha), 0f, Vector2.Zero, 0.8f, (SpriteEffects)0, 0f);
		}
		if (drawControllerLayout)
		{
			ControllerLayouts.LayoutVariant layoutVariant;
			if (ControllingPlayer.HasValue)
			{
				layoutVariant = ScreenManager.ControllerLayouts.Layouts[ScreenManager.PersonalSettings[ControllingPlayer.Value].ControllerLayout];
				spriteBatch.DrawString(ScreenManager.Font, layoutVariant.Name, new Vector2(207f, 101f), new Color((byte)0, (byte)0, (byte)0, TransitionAlpha));
				spriteBatch.DrawString(ScreenManager.Font, layoutVariant.Name, new Vector2(206f, 100f), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
			}
			else
			{
				layoutVariant = ScreenManager.ControllerLayouts.Layouts[0];
				spriteBatch.DrawString(ScreenManager.Font, "Default Controls", new Vector2(257f, 101f), new Color((byte)0, (byte)0, (byte)0, TransitionAlpha));
				spriteBatch.DrawString(ScreenManager.Font, "Default Controls", new Vector2(256f, 100f), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
			}
			spriteBatch.Draw(ScreenManager.Icon_XboxController, new Vector2((float)(640 - ScreenManager.Icon_XboxController.Width / 2), (float)(360 - ScreenManager.Icon_XboxController.Height / 2)), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
			DrawLayoutButton(ScreenManager.ControllerLayouts.DescA, layoutVariant.MyA, spriteBatch);
			DrawLayoutButton(ScreenManager.ControllerLayouts.DescB, layoutVariant.MyB, spriteBatch);
			DrawLayoutButton(ScreenManager.ControllerLayouts.DescX, layoutVariant.MyX, spriteBatch);
			DrawLayoutButton(ScreenManager.ControllerLayouts.DescY, layoutVariant.MyY, spriteBatch);
			DrawLayoutButton(ScreenManager.ControllerLayouts.DescStart, layoutVariant.MyStart, spriteBatch);
			DrawLayoutButton(ScreenManager.ControllerLayouts.DescBack, layoutVariant.MyBack, spriteBatch);
			DrawLayoutButton(ScreenManager.ControllerLayouts.DescLeftBumper, layoutVariant.MyLeftBumper, spriteBatch);
			DrawLayoutButton(ScreenManager.ControllerLayouts.DescLeftStickButton, layoutVariant.MyLeftStickButton, spriteBatch);
			DrawLayoutButton(ScreenManager.ControllerLayouts.DescRightBumper, layoutVariant.MyRightBumper, spriteBatch);
			DrawLayoutButton(ScreenManager.ControllerLayouts.DescRightStickButton, layoutVariant.MyRightStickButton, spriteBatch);
			DrawLayoutButton(ScreenManager.ControllerLayouts.DescDPadLeft, layoutVariant.MyDPadLeft, spriteBatch);
			DrawLayoutButton(ScreenManager.ControllerLayouts.DescDPadRight, layoutVariant.MyDPadRight, spriteBatch);
			DrawLayoutButton(ScreenManager.ControllerLayouts.DescDPadUp, layoutVariant.MyDPadUp, spriteBatch);
			DrawLayoutButton(ScreenManager.ControllerLayouts.DescDPadDown, layoutVariant.MyDPadDown, spriteBatch);
			if (!layoutVariant.SwitchTriggers)
			{
				DrawLayoutButton(ScreenManager.ControllerLayouts.DescLeftTrigger, (Buttons)8388608, spriteBatch);
				DrawLayoutButton(ScreenManager.ControllerLayouts.DescRightTrigger, (Buttons)4194304, spriteBatch);
			}
			else
			{
				DrawLayoutButton(ScreenManager.ControllerLayouts.DescLeftTrigger, (Buttons)4194304, spriteBatch);
				DrawLayoutButton(ScreenManager.ControllerLayouts.DescRightTrigger, (Buttons)8388608, spriteBatch);
			}
			if (ControllingPlayer.HasValue)
			{
				if (!ScreenManager.PersonalSettings[ControllingPlayer.Value].SwitchThumbSticks)
				{
					DrawLayoutButton(ScreenManager.ControllerLayouts.DescLeftStick, (Buttons)268435456, spriteBatch);
					DrawLayoutButton(ScreenManager.ControllerLayouts.DescRightStick, (Buttons)16777216, spriteBatch);
				}
				else
				{
					DrawLayoutButton(ScreenManager.ControllerLayouts.DescLeftStick, (Buttons)16777216, spriteBatch);
					DrawLayoutButton(ScreenManager.ControllerLayouts.DescRightStick, (Buttons)268435456, spriteBatch);
				}
			}
			else
			{
				DrawLayoutButton(ScreenManager.ControllerLayouts.DescLeftStick, (Buttons)268435456, spriteBatch);
				DrawLayoutButton(ScreenManager.ControllerLayouts.DescRightStick, (Buttons)16777216, spriteBatch);
			}
		}
		DrawMyInfo(spriteBatch);
		if ((object)GetType() == typeof(MainMenuScreen) || (object)GetType() == typeof(StartMenuScreen))
		{
			spriteBatch.Draw(ScreenManager.TitleImage, new Vector2(ScreenManager.TitleImageCenterPos.X - (float)(ScreenManager.TitleImage.Width / 2), ScreenManager.TitleImageCenterPos.Y - (float)(ScreenManager.TitleImage.Height / 2)), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
		}
		spriteBatch.End();
	}

	private void DrawLayoutButton(string description, Buttons physicalButton, SpriteBatch spriteBatch)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Invalid comparison between Unknown and I4
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Invalid comparison between Unknown and I4
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Invalid comparison between Unknown and I4
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Invalid comparison between Unknown and I4
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Invalid comparison between Unknown and I4
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Invalid comparison between Unknown and I4
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Invalid comparison between Unknown and I4
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Invalid comparison between Unknown and I4
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Invalid comparison between Unknown and I4
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Invalid comparison between Unknown and I4
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Invalid comparison between Unknown and I4
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Invalid comparison between Unknown and I4
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Invalid comparison between Unknown and I4
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Invalid comparison between Unknown and I4
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected I4, but got Unknown
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		//IL_068c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Invalid comparison between Unknown and I4
		//IL_09ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09db: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Invalid comparison between Unknown and I4
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Invalid comparison between Unknown and I4
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Invalid comparison between Unknown and I4
		//IL_086d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0877: Unknown result type (might be due to invalid IL or missing references)
		//IL_087c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0889: Unknown result type (might be due to invalid IL or missing references)
		//IL_089d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Invalid comparison between Unknown and I4
		//IL_080b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0815: Unknown result type (might be due to invalid IL or missing references)
		//IL_081a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0827: Unknown result type (might be due to invalid IL or missing references)
		//IL_0857: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Invalid comparison between Unknown and I4
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Invalid comparison between Unknown and I4
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a16: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a43: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Invalid comparison between Unknown and I4
		//IL_0701: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0710: Unknown result type (might be due to invalid IL or missing references)
		//IL_071d: Unknown result type (might be due to invalid IL or missing references)
		//IL_073b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Unknown result type (might be due to invalid IL or missing references)
		//IL_0767: Unknown result type (might be due to invalid IL or missing references)
		//IL_076c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0771: Unknown result type (might be due to invalid IL or missing references)
		//IL_0780: Unknown result type (might be due to invalid IL or missing references)
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0940: Unknown result type (might be due to invalid IL or missing references)
		//IL_094a: Unknown result type (might be due to invalid IL or missing references)
		//IL_094f: Unknown result type (might be due to invalid IL or missing references)
		//IL_095c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0970: Unknown result type (might be due to invalid IL or missing references)
		//IL_0993: Unknown result type (might be due to invalid IL or missing references)
		//IL_0998: Unknown result type (might be due to invalid IL or missing references)
		//IL_099d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_08db: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_090b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0925: Unknown result type (might be due to invalid IL or missing references)
		//IL_092a: Unknown result type (might be due to invalid IL or missing references)
		//IL_092f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		if (!(description != string.Empty))
		{
			return;
		}
		Vector2 val = Vector2.Zero;
		Vector2 val2 = Vector2.Zero;
		Vector2 val3 = ScreenManager.SmallFont.MeasureString(description);
		Vector2 val4 = Vector2.Zero;
		SpriteFont smallFont = ScreenManager.SmallFont;
		string text = "";
		float num = 8f;
		if ((int)physicalButton <= 512)
		{
			if ((int)physicalButton <= 32)
			{
				if ((int)physicalButton <= 8)
				{
					switch (physicalButton - 1)
					{
					default:
						if ((int)physicalButton == 8)
						{
							text = "¥";
							val4 = smallFont.MeasureString(text) / 2f;
							val = new Vector2(310f, 430f);
							val2 = new Vector2(val.X, val.Y) + new Vector2(val4.X + num, (float)(-(smallFont.LineSpacing / 2)));
						}
						break;
					case 1:
						text = "§";
						val4 = smallFont.MeasureString(text) / 2f;
						val = new Vector2(310f, 430f);
						val2 = new Vector2(val.X, val.Y) + new Vector2(0f - val3.X / 2f, (float)(smallFont.LineSpacing / 2));
						break;
					case 3:
						text = "¤";
						val4 = smallFont.MeasureString(text) / 2f;
						val = new Vector2(310f, 430f);
						val2 = new Vector2(val.X, val.Y) + new Vector2(0f - (val4.X + num + val3.X), (float)(-(smallFont.LineSpacing / 2)));
						break;
					case 0:
						text = "¦";
						val4 = smallFont.MeasureString(text) / 2f;
						val = new Vector2(310f, 430f);
						val2 = new Vector2(val.X, val.Y) + new Vector2(0f - val3.X / 2f, 0f - (val3.Y + (float)(smallFont.LineSpacing / 2)));
						break;
					case 2:
						break;
					}
				}
				else if ((int)physicalButton != 16)
				{
					if ((int)physicalButton == 32)
					{
						text = "\u009b";
						val4 = smallFont.MeasureString(text) / 2f;
						val = new Vector2(590f, 450f);
						val2 = new Vector2(val.X, val.Y) + new Vector2(val4.X - val3.X, (float)(smallFont.LineSpacing / 2));
					}
				}
				else
				{
					text = "\u009a";
					val4 = smallFont.MeasureString(text) / 2f;
					val = new Vector2(690f, 450f);
					val2 = new Vector2(val.X, val.Y) + new Vector2(0f - val4.X, (float)(smallFont.LineSpacing / 2));
				}
			}
			else if ((int)physicalButton <= 128)
			{
				if ((int)physicalButton != 64)
				{
					if ((int)physicalButton == 128)
					{
						text = "";
						val4 = smallFont.MeasureString(text) / 2f;
						val = new Vector2(800f, 440f);
						val2 = new Vector2(val.X, val.Y + (float)ScreenManager.SmallFont.LineSpacing);
					}
				}
				else
				{
					text = "";
					val4 = smallFont.MeasureString(text) / 2f;
					val = new Vector2(490f, 290f);
					val2 = new Vector2(val.X - val3.X, val.Y + (float)ScreenManager.SmallFont.LineSpacing);
				}
			}
			else if ((int)physicalButton != 256)
			{
				if ((int)physicalButton == 512)
				{
					text = "\u009f";
					val4 = smallFont.MeasureString(text) / 2f;
					val = new Vector2(730f, 240f);
					val2 = new Vector2(val.X, val.Y) + new Vector2(val4.X + num, (float)(-(smallFont.LineSpacing / 2)));
				}
			}
			else
			{
				text = "\u009c";
				val4 = smallFont.MeasureString(text) / 2f;
				val = new Vector2(550f, 240f);
				val2 = new Vector2(val.X, val.Y) + new Vector2(0f - (val4.X + num + val3.X), (float)(-(smallFont.LineSpacing / 2)));
			}
		}
		else if ((int)physicalButton <= 32768)
		{
			if ((int)physicalButton <= 8192)
			{
				if ((int)physicalButton != 4096)
				{
					if ((int)physicalButton == 8192)
					{
						text = "\u0097";
						val4 = smallFont.MeasureString(text) / 2f;
						val = new Vector2(1005f, 330f);
						val2 = new Vector2(val.X, val.Y) + new Vector2(val4.X + num, (float)(-(smallFont.LineSpacing / 2)));
					}
				}
				else
				{
					text = "\u0096";
					val4 = smallFont.MeasureString(text) / 2f;
					val = new Vector2(970f, 365f);
					val2 = new Vector2(val.X, val.Y) + new Vector2(0f - val3.X / 2f, (float)(smallFont.LineSpacing / 2));
				}
			}
			else if ((int)physicalButton != 16384)
			{
				if ((int)physicalButton == 32768)
				{
					text = "\u0099";
					val4 = smallFont.MeasureString(text) / 2f;
					val = new Vector2(970f, 295f);
					val2 = new Vector2(val.X, val.Y) + new Vector2(0f - val3.X / 2f, 0f - (val3.Y + (float)(smallFont.LineSpacing / 2)));
				}
			}
			else
			{
				text = "\u0098";
				val4 = smallFont.MeasureString(text) / 2f;
				val = new Vector2(935f, 330f);
				val2 = new Vector2(val.X, val.Y) + new Vector2(0f - (val4.X + num + val3.X), (float)(-(smallFont.LineSpacing / 2)));
			}
		}
		else if ((int)physicalButton <= 8388608)
		{
			if ((int)physicalButton != 4194304)
			{
				if ((int)physicalButton == 8388608)
				{
					text = "\u009e";
					val4 = smallFont.MeasureString(text) / 2f;
					val = new Vector2(590f, 180f);
					val2 = new Vector2(val.X, val.Y) + new Vector2(0f - (val4.X + num + val3.X), (float)(-(smallFont.LineSpacing / 2)));
				}
			}
			else
			{
				text = "¡";
				val4 = smallFont.MeasureString(text) / 2f;
				val = new Vector2(710f, 180f);
				val2 = new Vector2(val.X, val.Y) + new Vector2(val4.X + num, (float)(-(smallFont.LineSpacing / 2)));
			}
		}
		else if ((int)physicalButton != 16777216)
		{
			if ((int)physicalButton == 268435456)
			{
				text = "";
				val4 = smallFont.MeasureString(text) / 2f;
				val = new Vector2(490f, 290f);
				val2 = new Vector2(val.X - val3.X, val.Y);
				Draw2D.DrawLine(spriteBatch, ScreenManager.FadeTexture, new Vector2(val3.X, val3.Y / 2f) + val2 + Vector2.One, new Vector2(558f, 311f), new Color((byte)0, (byte)0, (byte)0, (byte)((float)(int)TransitionAlpha * 0.5f)));
				Draw2D.DrawLine(spriteBatch, ScreenManager.FadeTexture, new Vector2(val3.X, val3.Y / 2f) + val2, new Vector2(557f, 310f), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
			}
		}
		else
		{
			text = "";
			val4 = smallFont.MeasureString(text) / 2f;
			val = new Vector2(800f, 440f);
			val2 = new Vector2(val.X, val.Y);
			Draw2D.DrawLine(spriteBatch, ScreenManager.FadeTexture, new Vector2(0f, val3.Y / 2f) + val2 + Vector2.One, new Vector2(679f, 361f), new Color((byte)0, (byte)0, (byte)0, (byte)((float)(int)TransitionAlpha * 0.5f)));
			Draw2D.DrawLine(spriteBatch, ScreenManager.FadeTexture, new Vector2(0f, val3.Y / 2f) + val2, new Vector2(678f, 360f), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
		}
		spriteBatch.DrawString(ScreenManager.SmallFont, text, val + Vector2.One - val4, new Color((byte)0, (byte)0, (byte)0, TransitionAlpha));
		spriteBatch.DrawString(ScreenManager.SmallFont, text, val - val4, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
		spriteBatch.DrawString(ScreenManager.SmallFont, description, val2 + Vector2.One, new Color((byte)0, (byte)0, (byte)0, TransitionAlpha));
		spriteBatch.DrawString(ScreenManager.SmallFont, description, val2, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
	}
}
