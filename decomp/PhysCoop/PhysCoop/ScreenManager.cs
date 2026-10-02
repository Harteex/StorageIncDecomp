using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace PhysCoop;

public class ScreenManager : DrawableGameComponent
{
	private class TutorialMessage
	{
		public string message;

		public Vector2 arrowTarget2D;

		public int delayBeforeDisplay;
	}

	private bool showSplash = true;

	private bool allowSignOutIfInGame = true;

	private bool allowJoinInGame = true;

	public bool PauseOnAnyDisconnectedGamepad = true;

	public bool AllowMenuLoadGame = false;

	public bool AutoLoadSingleGlobalSave = true;

	public string TitleOfGame = "Storage Inc";

	public int FreeSpaceNeededForGlobalSettings = 10240;

	public int FreeSpaceNeededForEachProfileSettings = 25600;

	public int FreeSpaceNeededForEachSaveGame = 102400;

	public bool ShowMusicVolumeOption = true;

	public bool UseMenuMusic = true;

	public bool UseDifficultyMenu = false;

	public bool UseGameSetup = true;

	public bool UsePersonalSettings = true;

	public Vector2 TitleImageCenterPos = new Vector2(920f, 144f);

	public bool UseMapEntryBackgroundImages = true;

	public Vector2 MapEntryImageOffset = new Vector2(-48f, 0f);

	public bool UseNagScreens = true;

	public bool UseOptionSwitchThumbSticks = true;

	public bool UseOptionControllerLayouts = false;

	public bool UseOptionVibration = true;

	public bool UseOptionInvertLook = false;

	public bool UseOptionSensitivity = false;

	public bool[] PlayerIsInGame = new bool[4];

	public bool[] PlayerIsSignedIn = new bool[4];

	public Gamer[] StoredGamer = new Gamer[4];

	public GamerProfile[] StoredGamerProfile = new GamerProfile[4];

	private int[] gamePadVibrationTimeLeft = new int[4];

	private float[] vibrationLeft = new float[4];

	private float[] vibrationRight = new float[4];

	private bool[] resumeVibrationAfterMenu = new bool[4];

	public bool UserWantsToBuy = false;

	public PlayerIndex BuyerIndex = (PlayerIndex)0;

	private bool giveUpBuy = false;

	public GameSettings GlobalSettings = new GameSettings();

	public ProfileSettings[] PersonalSettings = new ProfileSettings[4];

	public SavedGame GameSaveInfo = new SavedGame();

	public int SaveId = 0;

	private List<GameScreen> screens = new List<GameScreen>();

	private List<GameScreen> screensToUpdate = new List<GameScreen>();

	private Queue<TutorialMessage> tutorialQueue = new Queue<TutorialMessage>();

	private TutorialMessage nextTutorialMessage;

	private InputState input;

	private AudioEngine menuAudioEngine;

	private WaveBank menuWaveBank;

	public SoundBank MenuSoundBank;

	private AudioCategory menuSoundCategory;

	private AudioCategory menuMusicCategory;

	private Cue menuMusicCue;

	public bool PlayMenuMusic = false;

	private float currentMenuMusicVolume = 0f;

	private SpriteBatch spriteBatch;

	private SpriteFont font;

	private SpriteFont smallFont;

	public Texture2D FadeTexture;

	private byte GamePlayScreenFade;

	public Texture2D TitleImage;

	private Texture2D splashLogo;

	public Texture2D MenuBackgroundTexture;

	public Texture2D MapImage;

	public Texture2D PlayerSlotBackground;

	public Texture2D MapEntryBackground;

	public Texture2D MapCompletedMarker;

	public Texture2D Icon_XboxController;

	public Texture2D Icon_Locked;

	public Texture2D BlankTexture;

	private float splashFade = 0f;

	private int splashTimer = 0;

	private bool splashIsFadingIn = true;

	private bool splashIsFadingOut = false;

	private bool isInitialized;

	private bool askForStorage = false;

	private IAsyncResult storageResult;

	public StorageDevice selectedStorage;

	private bool waitingForStorageResult = false;

	public bool NeedToUseStorage;

	public bool RequestCreateNewGame;

	public bool RequestSaveGame;

	public bool RequestLoadGame;

	public bool RequestSaveGlobalSettings;

	public bool RequestLoadGlobalSettings;

	public bool[] RequestSaveProfileSettings = new bool[4];

	public bool[] RequestLoadProfileSettings = new bool[4];

	public PlayerIndex MainPlayer;

	public bool AskForMainProfile = false;

	public MapContainer MapsContainer = new MapContainer();

	public Color[] MapDifficultyColor;

	private ResolveTexture2D resolveTarget;

	private Effect brightnessEffect;

	public ControllerLayouts ControllerLayouts = new ControllerLayouts();

	public SpriteBatch SpriteBatch => spriteBatch;

	public SpriteFont Font => font;

	public SpriteFont SmallFont => smallFont;

	private void ClearStorageRequests()
	{
		NeedToUseStorage = false;
		RequestCreateNewGame = false;
		RequestSaveGame = false;
		RequestLoadGame = false;
		RequestSaveGlobalSettings = false;
		RequestLoadGlobalSettings = false;
		RequestSaveProfileSettings[0] = false;
		RequestSaveProfileSettings[1] = false;
		RequestSaveProfileSettings[2] = false;
		RequestSaveProfileSettings[3] = false;
		RequestLoadProfileSettings[0] = false;
		RequestLoadProfileSettings[1] = false;
		RequestLoadProfileSettings[2] = false;
		RequestLoadProfileSettings[3] = false;
	}

	public ScreenManager(Game game)
		: base(game)
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		input = new InputState(this);
		SignedInGamer.SignedIn += PlayerSignedIn;
		SignedInGamer.SignedOut += PlayerSignedOut;
		for (int i = 0; i < 4; i++)
		{
			PlayerIsInGame[i] = false;
			PersonalSettings[i] = new ProfileSettings();
			vibrationLeft[i] = 0f;
			vibrationRight[i] = 0f;
			gamePadVibrationTimeLeft[i] = 0;
			resumeVibrationAfterMenu[i] = false;
		}
		MapDifficultyColor = new Color[4];
		ref Color reference = ref MapDifficultyColor[0];
		reference = new Color((byte)140, (byte)140, (byte)160);
		ref Color reference2 = ref MapDifficultyColor[1];
		reference2 = new Color((byte)223, (byte)173, (byte)126);
		ref Color reference3 = ref MapDifficultyColor[2];
		reference3 = new Color((byte)221, (byte)233, (byte)237);
		ref Color reference4 = ref MapDifficultyColor[3];
		reference4 = new Color((byte)251, (byte)239, (byte)30);
		ClearStorageRequests();
	}

	public override void Initialize()
	{
		((DrawableGameComponent)this).Initialize();
		isInitialized = true;
	}

	protected override void LoadContent()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected Obj, but got Unknown
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Expected Obj, but got Unknown
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Expected Obj, but got Unknown
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Expected Obj, but got Unknown
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Expected Obj, but got Unknown
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		ContentManager content = ((GameComponent)this).Game.Content;
		spriteBatch = new SpriteBatch(((DrawableGameComponent)this).GraphicsDevice);
		font = content.Load<SpriteFont>("ScreenManagerResources\\OakEngineFont64");
		smallFont = font;
		FadeTexture = content.Load<Texture2D>("ScreenManagerResources\\fadeTexture");
		splashLogo = content.Load<Texture2D>("ScreenManagerResources\\Splash_Logo");
		MenuBackgroundTexture = content.Load<Texture2D>("ScreenManagerResources\\MainMenu_Background");
		PlayerSlotBackground = content.Load<Texture2D>("ScreenManagerResources\\PlayerSlotBackground");
		TitleImage = content.Load<Texture2D>("ScreenManagerResources\\TitleImage");
		if (UseMapEntryBackgroundImages)
		{
			MapEntryBackground = content.Load<Texture2D>("ScreenManagerResources\\MapEntryImage");
			MapCompletedMarker = content.Load<Texture2D>("ScreenManagerResources\\MapCompleted");
		}
		BlankTexture = content.Load<Texture2D>("ScreenManagerResources\\blankTexture");
		Icon_Locked = content.Load<Texture2D>("ScreenManagerResources\\Locked");
		Icon_XboxController = content.Load<Texture2D>("ScreenManagerResources\\xboxControllerFront");
		MapImage = BlankTexture;
		PresentationParameters presentationParameters = ((DrawableGameComponent)this).GraphicsDevice.PresentationParameters;
		resolveTarget = new ResolveTexture2D(((DrawableGameComponent)this).GraphicsDevice, presentationParameters.BackBufferWidth, presentationParameters.BackBufferHeight, 1, presentationParameters.BackBufferFormat);
		brightnessEffect = content.Load<Effect>("ScreenManagerResources\\Brightness");
		menuAudioEngine = new AudioEngine("Content\\ScreenManagerResources\\menuAudio\\MenuSounds.xgs");
		menuWaveBank = new WaveBank(menuAudioEngine, "Content\\ScreenManagerResources\\menuAudio\\Wave Bank.xwb");
		MenuSoundBank = new SoundBank(menuAudioEngine, "Content\\ScreenManagerResources\\menuAudio\\Sound Bank.xsb");
		menuSoundCategory = menuAudioEngine.GetCategory("Default");
		menuMusicCategory = menuAudioEngine.GetCategory("Music");
		menuMusicCue = MenuSoundBank.GetCue("Menu_Music");
		foreach (GameScreen screen in screens)
		{
			screen.LoadContent();
		}
	}

	protected override void UnloadContent()
	{
		foreach (GameScreen screen in screens)
		{
			screen.UnloadContent();
		}
	}

	public override void Update(GameTime gameTime)
	{
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Expected I4, but got Unknown
		//IL_084f: Unknown result type (might be due to invalid IL or missing references)
		if (UseMenuMusic)
		{
			if (PlayMenuMusic)
			{
				if (!menuMusicCue.IsPlaying)
				{
					currentMenuMusicVolume = (float)GlobalSettings.MusicVolume / 5f;
					if (menuMusicCue.IsPrepared)
					{
						menuMusicCue.Play();
					}
					else
					{
						menuMusicCue = MenuSoundBank.GetCue("Menu_Music");
					}
				}
				if (currentMenuMusicVolume < (float)GlobalSettings.MusicVolume / 5f)
				{
					currentMenuMusicVolume += 0.02f;
				}
				else if (currentMenuMusicVolume > (float)GlobalSettings.MusicVolume / 5f)
				{
					currentMenuMusicVolume = (float)GlobalSettings.MusicVolume / 5f;
				}
			}
			else
			{
				if (currentMenuMusicVolume > 0f)
				{
					currentMenuMusicVolume -= 0.015f;
				}
				if (currentMenuMusicVolume < 0f)
				{
					currentMenuMusicVolume = 0f;
					menuMusicCue.Stop((AudioStopOptions)1);
				}
			}
		}
		menuMusicCategory.SetVolume(currentMenuMusicVolume);
		menuSoundCategory.SetVolume((float)GlobalSettings.SoundVolume / 5f);
		menuAudioEngine.Update();
		if (showSplash)
		{
			if (splashIsFadingIn)
			{
				splashTimer++;
				splashFade = (float)splashTimer / 50f;
				if (splashTimer > 50)
				{
					splashTimer = 0;
					splashIsFadingIn = false;
				}
			}
			else if (splashIsFadingOut)
			{
				splashTimer++;
				splashFade = 1f - (float)splashTimer / 40f;
				if (splashTimer <= 40)
				{
					return;
				}
				showSplash = false;
				splashLogo = null;
				GamerCollectionEnumerator<SignedInGamer> enumerator = ((GamerCollection<SignedInGamer>)(object)Gamer.SignedInGamers).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						SignedInGamer current = enumerator.Current;
						int num = (int)current.PlayerIndex;
						PlayerIsSignedIn[num] = true;
						StoredGamer[num] = (Gamer)(object)current;
						StoredGamerProfile[num] = ((Gamer)current).GetProfile();
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
				}
				GoToStartMenu();
			}
			else
			{
				splashTimer++;
				if (splashTimer > 90)
				{
					splashTimer = 0;
					splashIsFadingOut = true;
				}
			}
			return;
		}
		HandleUnlock();
		if (AskForMainProfile && !Guide.IsVisible)
		{
			AskForMainProfile = false;
			bool flag = false;
			GamerCollectionEnumerator<SignedInGamer> enumerator = ((GamerCollection<SignedInGamer>)(object)Gamer.SignedInGamers).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					SignedInGamer current = enumerator.Current;
					if (current.PlayerIndex == MainPlayer && !current.IsGuest)
					{
						GoToMainMenu();
						flag = true;
						askForStorage = true;
						break;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
			if (!flag)
			{
				Guide.ShowSignIn(4, false);
				GoToStartMenu();
			}
		}
		if (AskForMainProfile)
		{
			return;
		}
		if (askForStorage || waitingForStorageResult)
		{
			RequestStorage();
			return;
		}
		if (NeedToUseStorage)
		{
			if (selectedStorage == null || !selectedStorage.IsConnected)
			{
				GoToStartMenu();
				if (selectedStorage != null)
				{
					AddScreen(new MessageBoxScreen("The Storage Device has been disconnected.\nYou will now be returned to the start menu.", p_allowCancel: false), null);
				}
				selectedStorage = null;
				return;
			}
			if (RequestCreateNewGame)
			{
				CreateNewGame();
			}
			if (RequestSaveGlobalSettings)
			{
				SaveGlobalSettings();
			}
			if (RequestLoadGlobalSettings)
			{
				LoadGlobalSettings();
			}
			if (RequestSaveGame)
			{
				SaveGame(SaveId);
			}
			if (RequestLoadGame)
			{
				LoadGame(SaveId);
			}
			for (int i = 0; i < 4; i++)
			{
				if (RequestSaveProfileSettings[i])
				{
					SavePersonalSettings((PlayerIndex)i);
				}
				if (RequestLoadProfileSettings[i])
				{
					LoadPersonalSettings((PlayerIndex)i);
				}
			}
			ClearStorageRequests();
			return;
		}
		input.Update();
		screensToUpdate.Clear();
		foreach (GameScreen screen in screens)
		{
			screensToUpdate.Add(screen);
		}
		bool flag2 = !((GameComponent)this).Game.IsActive;
		bool flag3 = false;
		bool flag4 = false;
		while (screensToUpdate.Count > 0)
		{
			GameScreen current2 = screensToUpdate[screensToUpdate.Count - 1];
			screensToUpdate.RemoveAt(screensToUpdate.Count - 1);
			if ((object)current2.GetType() == typeof(GameplayScreen))
			{
				if (flag3)
				{
					if (GamePlayScreenFade >= 3)
					{
						GamePlayScreenFade -= 3;
					}
					else
					{
						GamePlayScreenFade = 0;
					}
					(current2 as GameplayScreen).SilenceGame();
				}
				else
				{
					if (GamePlayScreenFade < 128)
					{
						GamePlayScreenFade += 3;
					}
					if (GamePlayScreenFade > 128)
					{
						GamePlayScreenFade = 128;
					}
				}
			}
			else
			{
				flag4 = true;
			}
			if (!current2.IsPopup)
			{
				flag3 = false;
			}
			current2.Update(gameTime, flag2, flag3);
			if (current2.ScreenState == ScreenState.TransitionOn || current2.ScreenState == ScreenState.Active)
			{
				if (!flag2)
				{
					current2.HandleInput(input);
					flag2 = true;
				}
				if (current2.IsPopup)
				{
					flag3 = true;
				}
			}
		}
		if (flag4)
		{
			for (int i = 0; i < 4; i++)
			{
				if (resumeVibrationAfterMenu[i])
				{
					GamePad.SetVibration((PlayerIndex)i, 0f, 0f);
				}
				else
				{
					ClearGamepadVibration((PlayerIndex)i);
				}
			}
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			if (PlayerIsInGame[i] && gamePadVibrationTimeLeft[i] > 0)
			{
				GamePad.SetVibration((PlayerIndex)i, vibrationLeft[i], vibrationRight[i]);
				gamePadVibrationTimeLeft[i]--;
			}
			else
			{
				ClearGamepadVibration((PlayerIndex)i);
			}
		}
		if (nextTutorialMessage == null)
		{
			if (tutorialQueue.Count > 0)
			{
				nextTutorialMessage = tutorialQueue.Dequeue();
			}
			return;
		}
		nextTutorialMessage.delayBeforeDisplay--;
		if (nextTutorialMessage.delayBeforeDisplay <= 0)
		{
			AddScreen(new TutorialMessageMenuScreen(nextTutorialMessage.message, nextTutorialMessage.arrowTarget2D), null);
			nextTutorialMessage = null;
		}
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		if (showSplash)
		{
			((DrawableGameComponent)this).GraphicsDevice.Clear(Color.Black);
			spriteBatch.Begin();
			SpriteBatch val = spriteBatch;
			Texture2D val2 = splashLogo;
			Viewport viewport = ((DrawableGameComponent)this).GraphicsDevice.Viewport;
			float num = viewport.Width / 2 - splashLogo.Width / 2;
			viewport = ((DrawableGameComponent)this).GraphicsDevice.Viewport;
			val.Draw(val2, new Vector2(num, (float)(viewport.Height / 2 - splashLogo.Height / 2)), new Color(1f, 1f, 1f, splashFade));
			spriteBatch.End();
		}
		else
		{
			foreach (GameScreen screen in screens)
			{
				if (screen.ScreenState != ScreenState.Hidden)
				{
					screen.Draw(gameTime);
					if ((object)screen.GetType() == typeof(GameplayScreen) && GamePlayScreenFade < 128)
					{
						FadeBackBufferToBlack(128 - GamePlayScreenFade);
					}
				}
			}
		}
		PostProcessImage();
	}

	public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
	{
		screen.ControllingPlayer = controllingPlayer;
		screen.ScreenManager = this;
		screen.IsExiting = false;
		if (isInitialized)
		{
			screen.LoadContent();
		}
		screens.Add(screen);
	}

	public void RemoveScreen(GameScreen screen)
	{
		if (isInitialized)
		{
			screen.UnloadContent();
		}
		screens.Remove(screen);
		screensToUpdate.Remove(screen);
	}

	public GameScreen[] GetScreens()
	{
		return screens.ToArray();
	}

	public void FadeBackBufferToBlack(int alpha)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Viewport viewport = ((DrawableGameComponent)this).GraphicsDevice.Viewport;
		spriteBatch.Begin();
		spriteBatch.Draw(FadeTexture, new Rectangle(0, 0, viewport.Width, viewport.Height), new Color((byte)0, (byte)0, (byte)0, (byte)alpha));
		spriteBatch.End();
	}

	private void RequestStorage()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		if (askForStorage && !Guide.IsVisible)
		{
			askForStorage = false;
			waitingForStorageResult = true;
			storageResult = Guide.BeginShowStorageDeviceSelector(MainPlayer, (AsyncCallback)null, (object)null);
		}
		if (!waitingForStorageResult || !storageResult.IsCompleted)
		{
			return;
		}
		selectedStorage = Guide.EndShowStorageDeviceSelector(storageResult);
		waitingForStorageResult = false;
		if (selectedStorage == null || !selectedStorage.IsConnected)
		{
			GoToStartMenu();
			AddScreen(new MessageBoxScreen("A valid storage device is required\nto play this game.", p_allowCancel: false), null);
			return;
		}
		if (selectedStorage.FreeSpace < FreeSpaceNeededForGlobalSettings + FreeSpaceNeededForEachSaveGame * 2 + FreeSpaceNeededForEachProfileSettings)
		{
			GoToStartMenu();
			AddScreen(new MessageBoxScreen("Not enough Free Space on selected Storage Device.", p_allowCancel: false), null);
			return;
		}
		NeedToUseStorage = true;
		RequestLoadGlobalSettings = true;
		GamerCollectionEnumerator<SignedInGamer> enumerator = ((GamerCollection<SignedInGamer>)(object)Gamer.SignedInGamers).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SignedInGamer current = enumerator.Current;
				RequestLoadProfileSettings[current.PlayerIndex] = true;
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public void AllSignedInIsInGame()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 4; i++)
		{
			PlayerIsInGame[i] = false;
		}
		GamerCollectionEnumerator<SignedInGamer> enumerator = ((GamerCollection<SignedInGamer>)(object)Gamer.SignedInGamers).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SignedInGamer current = enumerator.Current;
				PlayerIsInGame[current.PlayerIndex] = true;
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public void GoToStartMenu()
	{
		ClearStorageRequests();
		ClearAllGamepadVibrations();
		bool flag = true;
		foreach (GameScreen screen in screens)
		{
			if ((object)screen.GetType() != typeof(BackgroundScreen))
			{
				screen.ExitScreen();
			}
			else
			{
				flag = false;
			}
		}
		if (flag)
		{
			AddScreen(new BackgroundScreen(), null);
		}
		AddScreen(new StartMenuScreen(), null);
		PlayMenuMusic = true;
	}

	public void GoToMainMenu()
	{
		ClearAllGamepadVibrations();
		AllSignedInIsInGame();
		bool flag = true;
		foreach (GameScreen screen in screens)
		{
			if ((object)screen.GetType() != typeof(BackgroundScreen))
			{
				screen.ExitScreen();
			}
			else
			{
				flag = false;
			}
		}
		if (flag)
		{
			AddScreen(new BackgroundScreen(), null);
		}
		AddScreen(new MainMenuScreen(this), null);
		PlayMenuMusic = true;
	}

	public void GoToGameSetup()
	{
		ClearAllGamepadVibrations();
		AllSignedInIsInGame();
		bool flag = true;
		foreach (GameScreen screen in screens)
		{
			if ((object)screen.GetType() != typeof(BackgroundScreen))
			{
				screen.ExitScreen();
			}
			else
			{
				flag = false;
			}
		}
		if (flag)
		{
			AddScreen(new BackgroundScreen(), null);
		}
		AddScreen(new GameSetupMenuScreen(this), null);
		PlayMenuMusic = true;
	}

	private void PlayerSignedIn(object sender, SignedInEventArgs e)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		PlayerIsSignedIn[e.Gamer.PlayerIndex] = true;
		StoredGamer[e.Gamer.PlayerIndex] = (Gamer)(object)e.Gamer;
		StoredGamerProfile[e.Gamer.PlayerIndex] = ((Gamer)e.Gamer).GetProfile();
		NeedToUseStorage = true;
		RequestLoadProfileSettings[e.Gamer.PlayerIndex] = true;
		bool flag = false;
		foreach (GameScreen screen in screens)
		{
			if ((object)screen.GetType() == typeof(GameplayScreen))
			{
				flag = true;
			}
		}
		if (flag && allowJoinInGame)
		{
			PlayerIsInGame[e.Gamer.PlayerIndex] = true;
		}
		else if (!flag)
		{
			PlayerIsInGame[e.Gamer.PlayerIndex] = true;
		}
	}

	private void PlayerSignedOut(object sender, SignedOutEventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		ClearGamepadVibration(e.Gamer.PlayerIndex);
		PlayerIsSignedIn[e.Gamer.PlayerIndex] = false;
		if (e.Gamer.PlayerIndex == MainPlayer)
		{
			GoToStartMenu();
			AddScreen(new MessageBoxScreen("The Main Player Signed Out,\nyou are now returned to the Start Menu.", p_allowCancel: false), null);
		}
		else if (!allowSignOutIfInGame && PlayerIsInGame[e.Gamer.PlayerIndex])
		{
			GoToMainMenu();
			AddScreen(new MessageBoxScreen("A Player Signed Out.\nYou are now returned to the Main Menu.", p_allowCancel: false), null);
		}
		else
		{
			for (int num = screens.Count - 1; num >= 0; num--)
			{
				if (screens[num] is MenuScreen && screens[num].ControllingPlayer == (PlayerIndex?)e.Gamer.PlayerIndex)
				{
					RemoveScreen(screens[num]);
				}
			}
			if (screens.Count == 0)
			{
				GoToMainMenu();
			}
			if (screens.Count == 1)
			{
				foreach (GameScreen screen in screens)
				{
					if ((object)screen.GetType() != typeof(GameplayScreen))
					{
						GoToMainMenu();
					}
				}
			}
		}
		PlayerIsInGame[e.Gamer.PlayerIndex] = false;
	}

	private void HandleUnlock()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (!UserWantsToBuy || Guide.IsVisible)
		{
			return;
		}
		bool flag = false;
		GamerCollectionEnumerator<SignedInGamer> enumerator = ((GamerCollection<SignedInGamer>)(object)Gamer.SignedInGamers).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SignedInGamer current = enumerator.Current;
				if (current.PlayerIndex == BuyerIndex)
				{
					flag = true;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		if (flag && Gamer.SignedInGamers[BuyerIndex].IsSignedInToLive && Gamer.SignedInGamers[BuyerIndex].Privileges.AllowPurchaseContent && !Gamer.SignedInGamers[BuyerIndex].IsGuest)
		{
			UserWantsToBuy = false;
			giveUpBuy = false;
			Guide.ShowMarketplace(BuyerIndex);
		}
		else if (giveUpBuy)
		{
			AddScreen(new MessageBoxScreen("You must Sign In with a LIVE profile that is\n allowed to purchase content.", p_allowCancel: false), null);
			giveUpBuy = false;
			UserWantsToBuy = false;
		}
		else
		{
			giveUpBuy = true;
		}
	}

	public void LoadGlobalSettings()
	{
		StorageContainer val = selectedStorage.OpenContainer(TitleOfGame);
		string path = Path.Combine(val.Path, "GlobalSettings.cfg");
		if (File.Exists(path))
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameSettings));
			FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
			GlobalSettings = (GameSettings)xmlSerializer.Deserialize(fileStream);
			fileStream.Close();
		}
		else
		{
			GlobalSettings = new GameSettings();
		}
		val.Dispose();
	}

	public void SaveGlobalSettings()
	{
		StorageContainer val = selectedStorage.OpenContainer(TitleOfGame);
		string path = Path.Combine(val.Path, "GlobalSettings.cfg");
		FileStream fileStream = File.Open(path, FileMode.Create);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameSettings));
		xmlSerializer.Serialize(fileStream, GlobalSettings);
		fileStream.Close();
		val.Dispose();
	}

	public void LoadPersonalSettings(PlayerIndex playerIndex)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected I4, but got Unknown
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		PersonalSettings[playerIndex] = new ProfileSettings();
		string text = "Guest";
		GamerCollectionEnumerator<SignedInGamer> enumerator = ((GamerCollection<SignedInGamer>)(object)Gamer.SignedInGamers).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SignedInGamer current = enumerator.Current;
				if (current.PlayerIndex != playerIndex)
				{
					continue;
				}
				text = ((Gamer)current).Gamertag;
				if (UseOptionInvertLook)
				{
					PersonalSettings[playerIndex].InvertLook = current.GameDefaults.InvertYAxis;
				}
				if (UseOptionSwitchThumbSticks)
				{
					PersonalSettings[playerIndex].SwitchThumbSticks = current.GameDefaults.MoveWithRightThumbStick;
				}
				if (UseOptionSensitivity)
				{
					ControllerSensitivity controllerSensitivity = current.GameDefaults.ControllerSensitivity;
					switch ((int)controllerSensitivity)
					{
					case 0:
						PersonalSettings[playerIndex].Sensitivity = 3;
						break;
					case 1:
						PersonalSettings[playerIndex].Sensitivity = 5;
						break;
					case 2:
						PersonalSettings[playerIndex].Sensitivity = 7;
						break;
					}
				}
				if (PersonalSettings[playerIndex].ControllerLayout > ControllerLayouts.Layouts.Count - 1)
				{
					PersonalSettings[playerIndex].ControllerLayout = 0;
				}
				break;
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		StorageContainer val = selectedStorage.OpenContainer(TitleOfGame);
		string path = Path.Combine(val.Path, text + ".cfg");
		if (File.Exists(path))
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProfileSettings));
			FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
			PersonalSettings[playerIndex] = (ProfileSettings)xmlSerializer.Deserialize(fileStream);
			fileStream.Close();
		}
		val.Dispose();
	}

	public void SavePersonalSettings(PlayerIndex playerIndex)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		string text = "Guest";
		GamerCollectionEnumerator<SignedInGamer> enumerator = ((GamerCollection<SignedInGamer>)(object)Gamer.SignedInGamers).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SignedInGamer current = enumerator.Current;
				if (current.PlayerIndex == playerIndex)
				{
					text = ((Gamer)current).Gamertag;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		StorageContainer val = selectedStorage.OpenContainer(TitleOfGame);
		string path = Path.Combine(val.Path, text + ".cfg");
		FileStream fileStream = File.Open(path, FileMode.Create);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProfileSettings));
		xmlSerializer.Serialize(fileStream, PersonalSettings[playerIndex]);
		fileStream.Close();
		val.Dispose();
	}

	public void CreateNewGame()
	{
		GameSaveInfo = new SavedGame();
		SaveId = 0;
		StorageContainer val = selectedStorage.OpenContainer(TitleOfGame);
		string path = Path.Combine(val.Path, "Save" + SaveId + ".sav");
		if (AutoLoadSingleGlobalSave)
		{
			if (File.Exists(path))
			{
				val.Dispose();
				LoadGame(0);
				{
					foreach (GameScreen screen in screens)
					{
						if ((object)screen.GetType() == typeof(GameSetupMenuScreen))
						{
							(screen as GameSetupMenuScreen).UpdateText();
						}
					}
					return;
				}
			}
			val.Dispose();
			SaveGame(0);
		}
		else
		{
			while (File.Exists(path))
			{
				SaveId++;
				path = Path.Combine(val.Path, "Save" + SaveId + ".sav");
			}
			val.Dispose();
		}
	}

	public void LoadGame(int saveId)
	{
		StorageContainer val = selectedStorage.OpenContainer(TitleOfGame);
		string path = Path.Combine(val.Path, "Save" + saveId + ".sav");
		if (File.Exists(path))
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(SavedGame));
			FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
			GameSaveInfo = (SavedGame)xmlSerializer.Deserialize(fileStream);
			fileStream.Close();
		}
		else
		{
			GameSaveInfo = new SavedGame();
		}
		val.Dispose();
		NewMapSelected(FindMapListIndex(GameSaveInfo.CurrentMapId));
	}

	public void SaveGame(int saveId)
	{
		if (selectedStorage.FreeSpace < FreeSpaceNeededForEachSaveGame)
		{
			AddScreen(new MessageBoxScreen("Not enough Free Space on selected Storage Device.\nThe game was not saved.", p_allowCancel: false), null);
			return;
		}
		StorageContainer val = selectedStorage.OpenContainer(TitleOfGame);
		string path = ((!AutoLoadSingleGlobalSave) ? Path.Combine(val.Path, "Save" + saveId + ".sav") : Path.Combine(val.Path, "Save0.sav"));
		FileStream fileStream = File.Open(path, FileMode.Create);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(SavedGame));
		xmlSerializer.Serialize(fileStream, GameSaveInfo);
		fileStream.Close();
		val.Dispose();
	}

	public int FindMapListIndex(int mapId)
	{
		foreach (MapContainer.MapItem map in MapsContainer.Maps)
		{
			if (map.Id == mapId)
			{
				return MapsContainer.Maps.IndexOf(map);
			}
		}
		MapsContainer.ListMapIndex = 0;
		GameSaveInfo.CurrentMapId = 1;
		return 1;
	}

	public int FindMapId(int listIndex)
	{
		return MapsContainer.Maps[listIndex].Id;
	}

	public void NewMapSelected(int listIndex)
	{
		MapsContainer.ListMapIndex = listIndex;
		GameSaveInfo.CurrentMapId = FindMapId(listIndex);
		ContentManager content = ((GameComponent)this).Game.Content;
		char paddingChar = '0';
		string text = "stages\\mapimages\\image" + GameSaveInfo.CurrentMapId.ToString().PadLeft(3, paddingChar);
		if (File.Exists(Path.Combine(StorageContainer.TitleLocation, "Content\\" + text + ".xnb")))
		{
			MapImage = content.Load<Texture2D>(text);
		}
		else
		{
			MapImage = BlankTexture;
		}
	}

	private void PostProcessImage()
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		((DrawableGameComponent)this).GraphicsDevice.ResolveBackBuffer(resolveTarget);
		brightnessEffect.Parameters["Intensity"].SetValue((float)GlobalSettings.Brightness / 5f);
		spriteBatch.Begin((SpriteBlendMode)0, (SpriteSortMode)0, (SaveStateMode)0);
		brightnessEffect.Begin();
		brightnessEffect.CurrentTechnique.Passes[0].Begin();
		spriteBatch.Draw((Texture2D)(object)resolveTarget, new Rectangle(0, 0, 1280, 720), Color.White);
		brightnessEffect.CurrentTechnique.Passes[0].End();
		brightnessEffect.End();
		spriteBatch.End();
	}

	public void ClearAllGamepadVibrations()
	{
		for (int i = 0; i < 4; i++)
		{
			GamePad.SetVibration((PlayerIndex)i, 0f, 0f);
			resumeVibrationAfterMenu[i] = false;
			gamePadVibrationTimeLeft[i] = 0;
			vibrationLeft[i] = 0f;
			vibrationRight[i] = 0f;
		}
	}

	public void ClearGamepadVibration(PlayerIndex playerIndex)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		GamePad.SetVibration(playerIndex, 0f, 0f);
		resumeVibrationAfterMenu[playerIndex] = false;
		gamePadVibrationTimeLeft[playerIndex] = 0;
		vibrationLeft[playerIndex] = 0f;
		vibrationRight[playerIndex] = 0f;
	}

	public void SetGamePadVibration(PlayerIndex playerIndex, float leftMotor, float rightMotor, bool resumeAfterMenu, int vibrationTime)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (PlayerIsInGame[playerIndex] && PersonalSettings[playerIndex].GamePadVibration)
		{
			GamePad.SetVibration(playerIndex, leftMotor, rightMotor);
			resumeVibrationAfterMenu[playerIndex] = resumeAfterMenu;
			gamePadVibrationTimeLeft[playerIndex] = vibrationTime;
			vibrationLeft[playerIndex] = leftMotor;
			vibrationRight[playerIndex] = rightMotor;
		}
	}

	public void AddTutorialMessage(string message, Vector2 arrowTarget2D, int delayBeforeDisplay)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		TutorialMessage tutorialMessage = new TutorialMessage();
		tutorialMessage.message = message;
		tutorialMessage.arrowTarget2D = arrowTarget2D;
		tutorialMessage.delayBeforeDisplay = delayBeforeDisplay;
		tutorialQueue.Enqueue(tutorialMessage);
	}
}
