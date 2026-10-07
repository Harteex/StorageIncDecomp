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
		OnCancel(e.PlayerIndex);
	}

	protected virtual void OnSelectLeft(int entryIndex, PlayerIndex playerIndex)
	{
		menuEntries[SelectedEntry].OnSelectLeft(playerIndex);
	}

	protected virtual void OnSelectRight(int entryIndex, PlayerIndex playerIndex)
	{
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
		SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
		SpriteFont font = ScreenManager.Font;
		Vector2 val = new Vector2(640f, 400f);
		float num = (float)(font.LineSpacing * menuEntries.Count + font.LineSpacing) + val.Y;
		Viewport viewport = ScreenManager.LogicalViewport;
		Rectangle titleSafeArea = viewport.TitleSafeArea;
		if (num > (float)(titleSafeArea.Bottom - 32))
		{
			viewport = ScreenManager.LogicalViewport;
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
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, ScreenManager.SpriteScale); // XNA 3.0 default: non-premultiplied alpha
		if ((object)GetType() == typeof(HowToPlayMenuScreen) || (object)GetType() == typeof(NagMenuScreen) || (object)GetType() == typeof(CreditsMenuScreen) || (object)GetType() == typeof(MapCompletedMenuScreen) || (object)GetType() == typeof(MapFailedMenuScreen))
		{
			if ((object)GetType() != typeof(MapCompletedMenuScreen) && (object)GetType() != typeof(MapFailedMenuScreen))
			{
				Texture2D val2 = infoBackground;
				viewport = ScreenManager.LogicalViewport;
				float num4 = (float)viewport.Width / 2f - (float)infoBackground.Width / 2f;
				viewport = ScreenManager.LogicalViewport;
				spriteBatch.Draw(val2, new Vector2(num4, (float)viewport.Height / 2f - (float)infoBackground.Height / 2f), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
			}
			Texture2D val3 = infoTexture;
			viewport = ScreenManager.LogicalViewport;
			float num5 = (float)viewport.Width / 2f - (float)infoTexture.Width / 2f;
			viewport = ScreenManager.LogicalViewport;
			spriteBatch.Draw(val3, new Vector2(num5, (float)viewport.Height / 2f - (float)infoTexture.Height / 2f), new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, TransitionAlpha));
		}
		else if ((object)GetType() == typeof(TutorialMessageMenuScreen))
		{
			TutorialMessageMenuScreen tutorialMessageMenuScreen = (TutorialMessageMenuScreen)this;
			Viewport viewport2 = ScreenManager.LogicalViewport;
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
				viewport = ScreenManager.LogicalViewport;
				titleSafeArea = viewport.TitleSafeArea;
				float num6 = titleSafeArea.Left;
				viewport = ScreenManager.LogicalViewport;
				titleSafeArea = viewport.TitleSafeArea;
				val8.X = MathHelper.Clamp(x, num6, (float)(titleSafeArea.Right - val7.Width));
				float y = val8.Y;
				viewport = ScreenManager.LogicalViewport;
				titleSafeArea = viewport.TitleSafeArea;
				float num7 = titleSafeArea.Top;
				viewport = ScreenManager.LogicalViewport;
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
				layoutVariant = ScreenManager.ControllerLayouts.Layouts[ScreenManager.PersonalSettings[(int)ControllingPlayer.Value].ControllerLayout];
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
				if (!ScreenManager.PersonalSettings[(int)ControllingPlayer.Value].SwitchThumbSticks)
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
					case (Buttons)1:
						text = "§";
						val4 = smallFont.MeasureString(text) / 2f;
						val = new Vector2(310f, 430f);
						val2 = new Vector2(val.X, val.Y) + new Vector2(0f - val3.X / 2f, (float)(smallFont.LineSpacing / 2));
						break;
					case (Buttons)3:
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
					case (Buttons)2:
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
