using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using BallGame_Editor;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Dynamics.Joints;
using FarseerGames.FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace PhysCoop;

public class GameplayScreen : GameScreen
{
	public struct SpotLight
	{
		public Vector3 Position;

		public float Strength;

		public Vector3 Direction;

		public float ConeAngle;

		public float ConeDecay;

		public Matrix ViewMatrix;

		public Matrix ProjectionMatrix;
	}

	public class Life
	{
		public bool IsLost = false;

		private float x;

		private float y;

		private float color = 1f;

		private float pos = 0f;

		public Life(Vector2 pos)
		{
			x = pos.X;
			y = pos.Y;
		}

		public void Update()
		{
			if (IsLost)
			{
				if (color > 0f)
				{
					color -= 0.02f;
				}
				else if (pos < 1f)
				{
					pos += 0.01f;
					x = MathHelper.SmoothStep(128f, -64f, pos);
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch, Texture2D texture)
		{
			spriteBatch.Draw(texture, new Vector2(x, y), new Color(255f, color, color));
		}
	}

	public class AnimatedPaper
	{
		private float x;

		private float y;

		private float pos = 0f;

		public bool removeMe = false;

		private Vector2 startPos;

		private float random;

		public AnimatedPaper(Vector2 pos, float p_random)
		{
			startPos = pos;
			x = pos.X;
			y = pos.Y;
			random = p_random / 5f;
		}

		public void Update()
		{
			if (pos < 1f)
			{
				pos += 0.02f;
				x = MathHelper.SmoothStep(startPos.X, startPos.X + 155f, pos);
				y += random * (pos * 2.5f);
				if (pos >= 1f)
				{
					pos = 1f;
					removeMe = true;
				}
			}
			else
			{
				pos = 1f;
			}
		}

		public void Draw(SpriteBatch spriteBatch, ref Texture2D[] textures)
		{
			spriteBatch.Draw(textures[(int)MathHelper.Min((float)(int)(6f * (pos * 2f)), 6f)], new Vector2(x, y), Color.White);
		}
	}

	private const float wallFriction = 0.1f;

	private const float motorMinVolume = -25f;

	private const float motorMaxVolume = -1f;

	private const float truckMass = 0.6f;

	private const float playerMoveStrength = 1200f;

	private const float playerRotationStrength = 500f;

	private const float playerRotationRes = 0f;

	private const float playerLinearRes = 7.5f;

	private const float playerFriction = 0f;

	private const float playerMOImod = 0.5f;

	private const int t_Dark_size = 256;

	public int Lives = 3;

	public int OrdersLeft = 40;

	public int GreyGoalAmountDivisor = 3;

	public bool TutorialMode = false;

	public bool TutorialDeliverPause = false;

	public bool TutorialPeriodPause = false;

	public int TutorialStep = 0;

	public bool TutorialBlockOutgoing = false;

	public bool TutorialShowMark = false;

	public Vector3 TutorialMarkedPos = new Vector3(500f, 0f, 280f);

	public bool TutorialCompletedArrival = false;

	public bool TutorialSkippedPeriod = false;

	public bool TutorialUnblockeableInGate = false;

	public bool DeliverPause = true;

	private ContentManager content;

	private SpriteBatch spriteBatch;

	private int playerCount = 4;

	public Random rnd = new Random();

	public int OrdersTotal;

	private Rectangle pileDestRect;

	private Rectangle pileSourceRect;

	private Texture2D[] animatedPaperTextures;

	public int NumberOfPeriods = 5;

	public List<Life> LifeList;

	public List<AnimatedPaper> AnimatedPaperList = new List<AnimatedPaper>();

	private AudioEngine audioEngine;

	private WaveBank waveBank;

	private SoundBank soundBank;

	private AudioCategory soundCategory;

	private AudioCategory musicCategory;

	private StageData stageData;

	private int stageID;

	private Model truck;

	private Matrix[] truckTransforms;

	private Texture2D[] truckTextures;

	private Texture2D t_Walls;

	private Model m_Floor;

	private Color darkColor = Color.CornflowerBlue;

	private BasicEffect extractTexture;

	public Texture2D t_Black;

	private float[] forkHeight = new float[4];

	public int[] GoodsBalance;

	public int GreysInPlay;

	public List<Goods> goodsList = new List<Goods>();

	public Model m_Bags2x2;

	public Texture2D t_Bags2x2;

	public Model m_Cardboard3x2;

	public Texture2D t_Cardboard3x2;

	public Model m_Mdf3x3;

	public Texture2D t_Mdf3x3;

	public Model m_Pipes4x2;

	public Texture2D t_Pipes4x2;

	public Model m_Crates4x3;

	public Texture2D t_Crates4x3;

	public Model m_Gate;

	public Texture2D t_Gate_Out;

	public Texture2D t_Gate_In_Blocked;

	public Texture2D t_Gate_In;

	public Gate[] InGates;

	public Gate[] OutGates;

	public Texture2D t_GoodsStatus;

	public Model m_GateId;

	public Texture2D t_GateId;

	public Texture2D t_OrderOut_Notify;

	public Texture2D t_RedLight;

	public Model m_GateLights;

	public Model m_Cross;

	private Texture2D t_floorTexture;

	private int hudX = 128;

	private int hudY = 72;

	private Texture2D t_hud_Period_Meter;

	private Texture2D t_hud_Period_Border;

	private Texture2D t_hud_Lives;

	private Texture2D t_hud_Paper;

	private Texture2D t_hud_Pile;

	private Texture2D t_hud_Tray;

	private Texture2D t_hud_Tray_Empty;

	public TimeLine timeLine;

	private bool firstRun;

	private VertexDeclaration wallsVertexDecleration;

	private VertexPositionNormalTexture[] wallsVertices;

	private VertexBuffer wallsVertexBuffer;

	private VertexDeclaration darkVertexDecleration;

	private VertexPositionNormalTexture[] darkVertices;

	private VertexBuffer darkVertexBuffer;

	private VertexDeclaration borderVertexDecleration;

	private VertexPositionNormalTexture[] borderVertices;

	private VertexBuffer borderVertexBuffer;

	private Matrix viewMatrix;

	private Matrix projectionMatrix;

	private Vector3 camPosition;

	private Vector3 camTarget;

	private Cue[] motorCue = new Cue[4];

	private float[] oldStickY = new float[4];

	private bool[] gearForward = new bool[4];

	private bool[] gearLockLeft = new bool[4];

	private bool[] gearLockRight = new bool[4];

	private Body[] bodyPlayer = new Body[4];

	public Geom[] GeomPlayer = new Geom[4];

	private RevoluteJoint[] liftRevJoint = new RevoluteJoint[4];

	private AngleLimitJoint[] liftJoint = new AngleLimitJoint[4];

	public Geom[] LiftPointGeom = new Geom[4];

	private Goods[] liftedGoods = new Goods[4];

	private float rot;

	public PhysicsSimulator physicsSimulator;

	private Effect effect1Scene;

	private Effect effect2Lights;

	private Effect effect3Final;

	private VertexPositionTexture[] fsVertices;

	private RenderTarget2D colorTarget;

	private RenderTarget2D normalTarget;

	private RenderTarget2D depthTarget;

	private Texture2D colorMap;

	private Texture2D normalMap;

	private Texture2D depthMap;

	private VertexDeclaration fsVertexDeclaration;

	private Texture2D shadingMap;

	private RenderTarget2D shadingTarget;

	private RenderTarget2D shadowTarget;

	private Texture2D shadowMap;

	private Texture2D blackImage;

	private Effect effectShadowMap;

	private Texture2D t_Dark;

	private SpotLight[] spotLights;

	private void AddLight(SpotLight spotLight)
	{
		PhysCoop.graphics.GraphicsDevice.SetRenderTarget(shadingTarget);
		effect2Lights.CurrentTechnique = effect2Lights.Techniques["DeferredSpotLight"];
		effect2Lights.Parameters["xPreviousShadingContents"].SetValue((Texture)(object)shadingMap);
		effect2Lights.Parameters["xNormalMap"].SetValue((Texture)(object)normalMap);
		effect2Lights.Parameters["xDepthMap"].SetValue((Texture)(object)depthMap);
		effect2Lights.Parameters["xShadowMap"].SetValue((Texture)(object)shadowMap);
		effect2Lights.Parameters["xLightPosition"].SetValue(spotLight.Position);
		effect2Lights.Parameters["xLightStrength"].SetValue(spotLight.Strength);
		effect2Lights.Parameters["xConeDirection"].SetValue(spotLight.Direction);
		effect2Lights.Parameters["xConeAngle"].SetValue(spotLight.ConeAngle);
		effect2Lights.Parameters["xConeDecay"].SetValue(spotLight.ConeDecay);
		Matrix value = Matrix.Invert(viewMatrix * projectionMatrix);
		effect2Lights.Parameters["xViewProjectionInv"].SetValue(value);
		effect2Lights.Parameters["xLightViewProjection"].SetValue(spotLight.ViewMatrix * spotLight.ProjectionMatrix);
		effect2Lights.CurrentTechnique.Passes[0].Apply();
		PhysCoop.graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, fsVertices, 0, 2);
		PhysCoop.graphics.GraphicsDevice.SetRenderTarget(ScreenManager.SceneTarget);
		shadingMap = shadingTarget;
	}

	private Texture2D GenerateShadingMap()
	{
		shadingMap = blackImage;
		for (int i = 0; i < spotLights.Length; i++)
		{
			RenderShadowMap(spotLights[i]);
			AddLight(spotLights[i]);
		}
		return shadingTarget;
	}

	private void RenderShadowMap(SpotLight spotLight)
	{
		PhysCoop.graphics.GraphicsDevice.SetRenderTarget(shadowTarget);
		effectShadowMap.CurrentTechnique = effectShadowMap.Techniques["ShadowMap"];
		RenderScene(effectShadowMap, ref spotLight.ViewMatrix, ref spotLight.ProjectionMatrix, shadowPass: true);
		PhysCoop.graphics.GraphicsDevice.SetRenderTarget(ScreenManager.SceneTarget);
		shadowMap = shadowTarget;
	}

	private void CombineColorAndShading()
	{
		effect3Final.CurrentTechnique = effect3Final.Techniques["CombineColorAndShading"];
		effect3Final.Parameters["xColorMap"].SetValue((Texture)(object)colorMap);
		effect3Final.Parameters["xShadingMap"].SetValue((Texture)(object)shadingMap);
		effect3Final.Parameters["xAmbient"].SetValue(0.67f);
		effect3Final.CurrentTechnique.Passes[0].Apply();
		PhysCoop.graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, fsVertices, 0, 2);
	}

	private void DrawVisualShadowMap()
	{
		extractTexture.Texture = shadowMap;
		foreach (EffectPass pass in ((Effect)extractTexture).CurrentTechnique.Passes)
		{
			pass.Apply();
			PhysCoop.graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, fsVertices, 0, 2);
		}
	}

	public GameplayScreen(int p_stageID)
	{
		firstRun = true;
		stageID = p_stageID;
		if (stageID == 1)
		{
			TutorialMode = true;
		}
		TransitionOnTime = TimeSpan.FromSeconds(1.5);
		TransitionOffTime = TimeSpan.FromSeconds(0.5);
		camPosition = new Vector3(640f, 860f, 600f);
		camTarget = new Vector3(640f, 0f, 370f);
		truckTextures = new Texture2D[playerCount];
		// Port: the scene always renders at 1280x720 (ScreenManager.SceneTarget), so use that
		// aspect ratio rather than the back buffer's, which changes when the window is resized.
		projectionMatrix = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, (float)ScreenManager.NativeWidth / ScreenManager.NativeHeight, 200f, 1500f);
		// Port fix: Update only sets viewMatrix once the screen is active, but Draw runs first.
		// The zero matrix made AddLight invert a singular matrix (NaNs; one garbage frame on Xbox,
		// an exception in FNA debug builds). Same camera as Update.
		viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Forward);
		char paddingChar = '0';
		string text = stageID.ToString().PadLeft(3, paddingChar);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(StageData));
		FileStream fileStream = new FileStream(System.IO.Path.Combine(LegacyStorageContainer.TitleLocation, "Content", "stages", text + ".xml"), FileMode.Open, FileAccess.Read);
		stageData = (StageData)xmlSerializer.Deserialize(fileStream);
		fileStream.Close();
		physicsSimulator = new PhysicsSimulator();
		physicsSimulator.Gravity = new Vector2(0f, 0f);
		physicsSimulator.MaxContactsToDetect = 12;
		physicsSimulator.MaxContactsToResolve = 5;
		spotLights = new SpotLight[1];
		spotLights[0].Strength = 0.58f;
		spotLights[0].Position = new Vector3(-500f, 2000f, -800f);
		spotLights[0].Direction = Vector3.Normalize(new Vector3(550f, 0f, 230f) - spotLights[0].Position);
		spotLights[0].ConeAngle = 0.62f;
		spotLights[0].ConeDecay = 0f;
		spotLights[0].ViewMatrix = Matrix.CreateLookAt(spotLights[0].Position, spotLights[0].Position + spotLights[0].Direction, Vector3.Up);
		spotLights[0].ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(spotLights[0].ConeAngle, 1f, 1590f, 2950f);
		int num = 0;
		wallsVertexDecleration = VertexPositionNormalTexture.VertexDeclaration;
		int num2 = 0;
		for (int i = 0; i < stageData.NumberOfWalls; i++)
		{
			num2 += stageData.NumberOfWallVertices[i];
		}
		wallsVertices = new VertexPositionNormalTexture[(num2 - 1) * 6];
		Vector2 val;
		for (int i = 0; i < stageData.NumberOfWalls; i++)
		{
			for (int j = 0; j < stageData.NumberOfWallVertices[i] - 1; j++)
			{
				val = stageData.PolygonWallsArray[i][j] - stageData.PolygonWallsArray[i][j + 1];
				Vector3 val2 = new Vector3(val.Y, 0f, 0f - val.X);
				float num3 = val.Length() / 512f;
				ref VertexPositionNormalTexture reference = ref wallsVertices[num];
				reference = new VertexPositionNormalTexture(new Vector3(stageData.PolygonWallsArray[i][j].X, 48f, stageData.PolygonWallsArray[i][j].Y), val2, new Vector2(0f, 0f));
				num++;
				ref VertexPositionNormalTexture reference2 = ref wallsVertices[num];
				reference2 = new VertexPositionNormalTexture(new Vector3(stageData.PolygonWallsArray[i][j + 1].X, 48f, stageData.PolygonWallsArray[i][j + 1].Y), val2, new Vector2(num3, 0f));
				num++;
				ref VertexPositionNormalTexture reference3 = ref wallsVertices[num];
				reference3 = new VertexPositionNormalTexture(new Vector3(stageData.PolygonWallsArray[i][j].X, 0f, stageData.PolygonWallsArray[i][j].Y), val2, new Vector2(0f, 1f));
				num++;
				ref VertexPositionNormalTexture reference4 = ref wallsVertices[num];
				reference4 = new VertexPositionNormalTexture(new Vector3(stageData.PolygonWallsArray[i][j + 1].X, 48f, stageData.PolygonWallsArray[i][j + 1].Y), val2, new Vector2(num3, 0f));
				num++;
				ref VertexPositionNormalTexture reference5 = ref wallsVertices[num];
				reference5 = new VertexPositionNormalTexture(new Vector3(stageData.PolygonWallsArray[i][j + 1].X, 0f, stageData.PolygonWallsArray[i][j + 1].Y), val2, new Vector2(num3, 1f));
				num++;
				ref VertexPositionNormalTexture reference6 = ref wallsVertices[num];
				reference6 = new VertexPositionNormalTexture(new Vector3(stageData.PolygonWallsArray[i][j].X, 0f, stageData.PolygonWallsArray[i][j].Y), val2, new Vector2(0f, 1f));
				num++;
			}
		}
		for (int i = 0; i < wallsVertices.Length; i++)
		{
			wallsVertices[i].Normal.Normalize();
		}
		wallsVertexBuffer = new VertexBuffer(PhysCoop.graphics.GraphicsDevice, typeof(VertexPositionNormalTexture), wallsVertices.Length, BufferUsage.WriteOnly);
		wallsVertexBuffer.SetData<VertexPositionNormalTexture>(wallsVertices, 0, wallsVertices.Length);
		borderVertexDecleration = VertexPositionNormalTexture.VertexDeclaration;
		borderVertices = new VertexPositionNormalTexture[(num2 - 1) * 6];
		Vector2 val3 = Vector2.Zero;
		num = -1;
		for (int i = 0; i < stageData.NumberOfWalls; i++)
		{
			for (int j = 0; j < stageData.NumberOfWallVertices[i]; j++)
			{
				Vector2 val4 = ((j != 0) ? (stageData.PolygonWallsArray[i][j] - stageData.PolygonWallsArray[i][j - 1]) : (stageData.PolygonWallsArray[i][j + 1] - stageData.PolygonWallsArray[i][j]));
				val = ((j != stageData.NumberOfWallVertices[i] - 1) ? (stageData.PolygonWallsArray[i][j + 1] - stageData.PolygonWallsArray[i][j]) : (stageData.PolygonWallsArray[i][j] - stageData.PolygonWallsArray[i][j - 1]));
				Vector2 val5 = new Vector2(0f - val4.Y, val4.X);
				Vector2 val6 = new Vector2(0f - val.Y, val.X);
				Vector2 val7 = -(val6 + val5);
				Vector2.Normalize(val7);
				if (val7.X > 0f)
				{
					val7.X = 1f;
				}
				else if (val7.X < 0f)
				{
					val7.X = -1f;
				}
				if (val7.Y > 0f)
				{
					val7.Y = 1f;
				}
				else if (val7.Y < 0f)
				{
					val7.Y = -1f;
				}
				Vector2 val8 = stageData.PolygonWallsArray[i][j] + val7;
				if (j != stageData.NumberOfWallVertices[i] - 1)
				{
					num++;
					ref VertexPositionNormalTexture reference7 = ref borderVertices[num];
					reference7 = new VertexPositionNormalTexture(new Vector3(stageData.PolygonWallsArray[i][j].X, 49f, stageData.PolygonWallsArray[i][j].Y), Vector3.Up, new Vector2(0f, 1f));
					num++;
					ref VertexPositionNormalTexture reference8 = ref borderVertices[num];
					reference8 = new VertexPositionNormalTexture(new Vector3(val8.X, 49f, val8.Y), Vector3.Up, new Vector2(0f, 0f));
					num++;
					ref VertexPositionNormalTexture reference9 = ref borderVertices[num];
					reference9 = new VertexPositionNormalTexture(new Vector3(stageData.PolygonWallsArray[i][j + 1].X, 49f, stageData.PolygonWallsArray[i][j + 1].Y), Vector3.Up, new Vector2(1f, 1f));
				}
				if (j != 0)
				{
					num++;
					ref VertexPositionNormalTexture reference10 = ref borderVertices[num];
					reference10 = new VertexPositionNormalTexture(new Vector3(stageData.PolygonWallsArray[i][j].X, 49f, stageData.PolygonWallsArray[i][j].Y), Vector3.Up, new Vector2(1f, 1f));
					num++;
					ref VertexPositionNormalTexture reference11 = ref borderVertices[num];
					reference11 = new VertexPositionNormalTexture(new Vector3(val3.X, 49f, val3.Y), Vector3.Up, new Vector2(1f, 0f));
					num++;
					ref VertexPositionNormalTexture reference12 = ref borderVertices[num];
					reference12 = new VertexPositionNormalTexture(new Vector3(val8.X, 49f, val8.Y), Vector3.Up, new Vector2(1f, 0f));
				}
				val3 = val8;
			}
		}
		for (int i = 0; i < wallsVertices.Length; i++)
		{
			wallsVertices[i].Normal.Normalize();
		}
		borderVertexBuffer = new VertexBuffer(PhysCoop.graphics.GraphicsDevice, typeof(VertexPositionNormalTexture), borderVertices.Length, BufferUsage.WriteOnly);
		borderVertexBuffer.SetData<VertexPositionNormalTexture>(borderVertices, 0, borderVertices.Length);
		Vector3 up = Vector3.Up;
		darkVertexDecleration = VertexPositionNormalTexture.VertexDeclaration;
		darkVertices = new VertexPositionNormalTexture[stageData.NumberOfDark * 6 + 24];
		int num4 = 0;
		Body body;
		Geom geom;
		float num5;
		float num6;
		float num7;
		float num8;
		for (int i = 0; i < stageData.NumberOfDark; i++)
		{
			num5 = (float)stageData.DarkListArray[i].X / 256f;
			num6 = (float)(stageData.DarkListArray[i].X + stageData.DarkListArray[i].Width) / 256f;
			num7 = (float)stageData.DarkListArray[i].Y / 256f;
			num8 = (float)(stageData.DarkListArray[i].Y + stageData.DarkListArray[i].Height) / 256f;
			ref VertexPositionNormalTexture reference13 = ref darkVertices[num4];
			reference13 = new VertexPositionNormalTexture(new Vector3((float)stageData.DarkListArray[i].X, 48f, (float)stageData.DarkListArray[i].Y), up, new Vector2(num5, num7));
			num4++;
			ref VertexPositionNormalTexture reference14 = ref darkVertices[num4];
			reference14 = new VertexPositionNormalTexture(new Vector3((float)(stageData.DarkListArray[i].X + stageData.DarkListArray[i].Width), 48f, (float)stageData.DarkListArray[i].Y), up, new Vector2(num6, num7));
			num4++;
			ref VertexPositionNormalTexture reference15 = ref darkVertices[num4];
			reference15 = new VertexPositionNormalTexture(new Vector3((float)stageData.DarkListArray[i].X, 48f, (float)(stageData.DarkListArray[i].Y + stageData.DarkListArray[i].Height)), up, new Vector2(num5, num8));
			num4++;
			ref VertexPositionNormalTexture reference16 = ref darkVertices[num4];
			reference16 = new VertexPositionNormalTexture(new Vector3((float)stageData.DarkListArray[i].X, 48f, (float)(stageData.DarkListArray[i].Y + stageData.DarkListArray[i].Height)), up, new Vector2(num5, num8));
			num4++;
			ref VertexPositionNormalTexture reference17 = ref darkVertices[num4];
			reference17 = new VertexPositionNormalTexture(new Vector3((float)(stageData.DarkListArray[i].X + stageData.DarkListArray[i].Width), 48f, (float)stageData.DarkListArray[i].Y), up, new Vector2(num6, num7));
			num4++;
			ref VertexPositionNormalTexture reference18 = ref darkVertices[num4];
			reference18 = new VertexPositionNormalTexture(new Vector3((float)(stageData.DarkListArray[i].X + stageData.DarkListArray[i].Width), 48f, (float)(stageData.DarkListArray[i].Y + stageData.DarkListArray[i].Height)), up, new Vector2(num6, num8));
			body = BodyFactory.Instance.CreateRectangleBody(physicsSimulator, stageData.DarkListArray[i].Width, stageData.DarkListArray[i].Height, 5f);
			body.IsStatic = true;
			geom = GeomFactory.Instance.CreateRectangleGeom(physicsSimulator, body, stageData.DarkListArray[i].Width, stageData.DarkListArray[i].Height);
			geom.FrictionCoefficient = 0.1f;
			body.Position = new Vector2((float)(stageData.DarkListArray[i].X + stageData.DarkListArray[i].Width / 2), (float)(stageData.DarkListArray[i].Y + stageData.DarkListArray[i].Height / 2));
			num4++;
		}
		body = BodyFactory.Instance.CreateRectangleBody(physicsSimulator, 1024f, 160f, 10f);
		body.IsStatic = true;
		body.Position = new Vector2(640f, 16f);
		geom = GeomFactory.Instance.CreateRectangleGeom(physicsSimulator, body, 1024f, 160f);
		geom.FrictionCoefficient = 0.1f;
		num5 = 0f;
		num6 = 5f;
		num7 = 0f;
		num8 = 0.625f;
		ref VertexPositionNormalTexture reference19 = ref darkVertices[num4];
		reference19 = new VertexPositionNormalTexture(new Vector3(-128f, 48f, -64f), up, new Vector2(num5, num7));
		num4++;
		ref VertexPositionNormalTexture reference20 = ref darkVertices[num4];
		reference20 = new VertexPositionNormalTexture(new Vector3(1408f, 48f, -64f), up, new Vector2(num6, num7));
		num4++;
		ref VertexPositionNormalTexture reference21 = ref darkVertices[num4];
		reference21 = new VertexPositionNormalTexture(new Vector3(-128f, 48f, 96f), up, new Vector2(num5, num8));
		num4++;
		ref VertexPositionNormalTexture reference22 = ref darkVertices[num4];
		reference22 = new VertexPositionNormalTexture(new Vector3(-128f, 48f, 96f), up, new Vector2(num5, num8));
		num4++;
		ref VertexPositionNormalTexture reference23 = ref darkVertices[num4];
		reference23 = new VertexPositionNormalTexture(new Vector3(1408f, 48f, -64f), up, new Vector2(num6, num7));
		num4++;
		ref VertexPositionNormalTexture reference24 = ref darkVertices[num4];
		reference24 = new VertexPositionNormalTexture(new Vector3(1408f, 48f, 96f), up, new Vector2(num6, num8));
		num4++;
		body = BodyFactory.Instance.CreateRectangleBody(physicsSimulator, 256f, 720f, 10f);
		body.IsStatic = true;
		body.Position = new Vector2(64f, 360f);
		geom = GeomFactory.Instance.CreateRectangleGeom(physicsSimulator, body, 256f, 720f);
		geom.FrictionCoefficient = 0.1f;
		num5 = -0.25f;
		num6 = 193f / 256f;
		num7 = 0.3125f;
		num8 = 2.5f;
		ref VertexPositionNormalTexture reference25 = ref darkVertices[num4];
		reference25 = new VertexPositionNormalTexture(new Vector3(-64f, 48f, 80f), up, new Vector2(num5, num7));
		num4++;
		ref VertexPositionNormalTexture reference26 = ref darkVertices[num4];
		reference26 = new VertexPositionNormalTexture(new Vector3(193f, 48f, 80f), up, new Vector2(num6, num7));
		num4++;
		ref VertexPositionNormalTexture reference27 = ref darkVertices[num4];
		reference27 = new VertexPositionNormalTexture(new Vector3(-64f, 48f, 640f), up, new Vector2(num5, num8));
		num4++;
		ref VertexPositionNormalTexture reference28 = ref darkVertices[num4];
		reference28 = new VertexPositionNormalTexture(new Vector3(-64f, 48f, 640f), up, new Vector2(num5, num8));
		num4++;
		ref VertexPositionNormalTexture reference29 = ref darkVertices[num4];
		reference29 = new VertexPositionNormalTexture(new Vector3(193f, 48f, 80f), up, new Vector2(num6, num7));
		num4++;
		ref VertexPositionNormalTexture reference30 = ref darkVertices[num4];
		reference30 = new VertexPositionNormalTexture(new Vector3(193f, 48f, 640f), up, new Vector2(num6, num8));
		num4++;
		body = BodyFactory.Instance.CreateRectangleBody(physicsSimulator, 256f, 720f, 10f);
		body.IsStatic = true;
		body.Position = new Vector2(1216f, 360f);
		geom = GeomFactory.Instance.CreateRectangleGeom(physicsSimulator, body, 256f, 720f);
		geom.FrictionCoefficient = 0.1f;
		num5 = 4.2460938f;
		num6 = 5.25f;
		num7 = 0.3125f;
		num8 = 2.5f;
		ref VertexPositionNormalTexture reference31 = ref darkVertices[num4];
		reference31 = new VertexPositionNormalTexture(new Vector3(1087f, 48f, 80f), up, new Vector2(num5, num7));
		num4++;
		ref VertexPositionNormalTexture reference32 = ref darkVertices[num4];
		reference32 = new VertexPositionNormalTexture(new Vector3(1344f, 48f, 80f), up, new Vector2(num6, num7));
		num4++;
		ref VertexPositionNormalTexture reference33 = ref darkVertices[num4];
		reference33 = new VertexPositionNormalTexture(new Vector3(1087f, 48f, 640f), up, new Vector2(num5, num8));
		num4++;
		ref VertexPositionNormalTexture reference34 = ref darkVertices[num4];
		reference34 = new VertexPositionNormalTexture(new Vector3(1087f, 48f, 640f), up, new Vector2(num5, num8));
		num4++;
		ref VertexPositionNormalTexture reference35 = ref darkVertices[num4];
		reference35 = new VertexPositionNormalTexture(new Vector3(1344f, 48f, 80f), up, new Vector2(num6, num7));
		num4++;
		ref VertexPositionNormalTexture reference36 = ref darkVertices[num4];
		reference36 = new VertexPositionNormalTexture(new Vector3(1344f, 48f, 640f), up, new Vector2(num6, num8));
		num4++;
		body = BodyFactory.Instance.CreateRectangleBody(physicsSimulator, 1024f, 144f, 10f);
		body.IsStatic = true;
		body.Position = new Vector2(640f, 712f);
		geom = GeomFactory.Instance.CreateRectangleGeom(physicsSimulator, body, 1024f, 144f);
		geom.FrictionCoefficient = 0.1f;
		num5 = 0f;
		num6 = 5f;
		num7 = 2.5f;
		num8 = 3.0625f;
		ref VertexPositionNormalTexture reference37 = ref darkVertices[num4];
		reference37 = new VertexPositionNormalTexture(new Vector3(0f, 48f, 640f), up, new Vector2(num5, num7));
		num4++;
		ref VertexPositionNormalTexture reference38 = ref darkVertices[num4];
		reference38 = new VertexPositionNormalTexture(new Vector3(1280f, 48f, 640f), up, new Vector2(num6, num7));
		num4++;
		ref VertexPositionNormalTexture reference39 = ref darkVertices[num4];
		reference39 = new VertexPositionNormalTexture(new Vector3(0f, 48f, 784f), up, new Vector2(num5, num8));
		num4++;
		ref VertexPositionNormalTexture reference40 = ref darkVertices[num4];
		reference40 = new VertexPositionNormalTexture(new Vector3(0f, 48f, 784f), up, new Vector2(num5, num8));
		num4++;
		ref VertexPositionNormalTexture reference41 = ref darkVertices[num4];
		reference41 = new VertexPositionNormalTexture(new Vector3(1280f, 48f, 640f), up, new Vector2(num6, num7));
		num4++;
		ref VertexPositionNormalTexture reference42 = ref darkVertices[num4];
		reference42 = new VertexPositionNormalTexture(new Vector3(1280f, 48f, 784f), up, new Vector2(num6, num8));
		num4++;
		darkVertexBuffer = new VertexBuffer(PhysCoop.graphics.GraphicsDevice, typeof(VertexPositionNormalTexture), darkVertices.Length, BufferUsage.WriteOnly);
		darkVertexBuffer.SetData<VertexPositionNormalTexture>(darkVertices, 0, darkVertices.Length);
		for (int i = 0; i < playerCount; i++)
		{
			gearForward[i] = true;
			gearLockLeft[i] = false;
			gearLockRight[i] = false;
			bodyPlayer[i] = BodyFactory.Instance.CreateRectangleBody(physicsSimulator, 91f, 32f, 0.6f);
			bodyPlayer[i].Position = stageData.PlayerArray[i] + new Vector2(44f, 17f);
			bodyPlayer[i].LinearDragCoefficient = 7.5f;
			bodyPlayer[i].RotationalDragCoefficient = 0f;
			bodyPlayer[i].MomentOfInertia *= 0.5f;
			bodyPlayer[i].IsTruck = true;
			GeomPlayer[i] = GeomFactory.Instance.CreateRectangleGeom(physicsSimulator, bodyPlayer[i], 55f, 32f, new Vector2(-18f, 0f), 0f);
			GeomPlayer[i].CollisionGroup = i + 1;
			GeomPlayer[i].FrictionCoefficient = 0f;
			Geom obj = GeomPlayer[i];
			obj.OnCollision = (Geom.CollisionEventHandler)Delegate.Combine(obj.OnCollision, new Geom.CollisionEventHandler(TruckCollision));
			LiftPointGeom[i] = GeomFactory.Instance.CreateRectangleGeom(physicsSimulator, bodyPlayer[i], 5f, 5f, new Vector2(25f, 0f), 0f);
			LiftPointGeom[i].CollisionResponseEnabled = false;
			LiftPointGeom[i].CollisionGroup = i + 1;
		}
	}

	private static readonly SamplerState SceneSampler = new SamplerState
	{
		AddressU = TextureAddressMode.Wrap,
		AddressV = TextureAddressMode.Wrap,
		Filter = TextureFilter.Linear,
		MipMapLevelOfDetailBias = -1f,
		MaxMipLevel = 0
	};

	private static void Restore_3DSettings()
	{
		// XNA 3.0: CullMode 3 = CullCounterClockwiseFace; depth on; blending off (Add, One, Zero);
		// alpha test off. Sampler 0: Wrap (1), Linear (2) min/mag/mip, LOD bias -1.
		PhysCoop.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
		PhysCoop.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
		PhysCoop.graphics.GraphicsDevice.BlendState = BlendState.Opaque;
		PhysCoop.graphics.GraphicsDevice.SamplerStates[0] = SceneSampler;
	}

	public override void LoadContent()
	{
		if (content == null)
		{
			content = new ContentManager((IServiceProvider)((GameComponent)ScreenManager).Game.Services, "Content");
		}
		spriteBatch = new SpriteBatch(PhysCoop.graphics.GraphicsDevice);
		effect1Scene = content.Load<Effect>("Deferred1Scene");
		effect2Lights = content.Load<Effect>("Deferred2Lights");
		effect3Final = content.Load<Effect>("Deferred3Final");
		fsVertices = new VertexPositionTexture[4];
		ref VertexPositionTexture reference = ref fsVertices[0];
		reference = new VertexPositionTexture(new Vector3(-1f, 1f, 0f), new Vector2(0f, 0f));
		ref VertexPositionTexture reference2 = ref fsVertices[1];
		reference2 = new VertexPositionTexture(new Vector3(1f, 1f, 0f), new Vector2(1f, 0f));
		ref VertexPositionTexture reference3 = ref fsVertices[2];
		reference3 = new VertexPositionTexture(new Vector3(-1f, -1f, 0f), new Vector2(0f, 1f));
		ref VertexPositionTexture reference4 = ref fsVertices[3];
		reference4 = new VertexPositionTexture(new Vector3(1f, -1f, 0f), new Vector2(1f, 1f));
		fsVertexDeclaration = VertexPositionTexture.VertexDeclaration;
		// XNA 3.0 SurfaceFormat 1 = Color, 22 = Single. XNA 4 render targets own their depth buffer.
		// Port: the screen-space G-buffer targets follow the scene target's size (see
		// EnsureGBuffer); the shadow map is light-space, so it keeps the original 1280x720.
		EnsureGBuffer();
		shadowTarget = new RenderTarget2D(PhysCoop.graphics.GraphicsDevice, 1280, 720, false, SurfaceFormat.Single, DepthFormat.Depth24);
		blackImage = new Texture2D(PhysCoop.graphics.GraphicsDevice, 1280, 720, false, SurfaceFormat.Color);
		effectShadowMap = content.Load<Effect>("ShadowMap");
		truck = content.Load<Model>("graphics\\models\\m_Truck");
		truckTransforms = new Matrix[((ReadOnlyCollection<ModelBone>)(object)truck.Bones).Count];
		truck.CopyAbsoluteBoneTransformsTo(truckTransforms);
		extractTexture = (BasicEffect)((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)truck.Meshes)[0].MeshParts)[0].Effect;
		truckTextures[0] = extractTexture.Texture;
		truckTextures[1] = content.Load<Texture2D>("graphics\\textures\\Truck_1");
		truckTextures[2] = content.Load<Texture2D>("graphics\\textures\\Truck_2");
		truckTextures[3] = content.Load<Texture2D>("graphics\\textures\\Truck_3");
		m_Floor = content.Load<Model>("graphics\\models\\m_Floor");
		extractTexture = (BasicEffect)((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)m_Floor.Meshes)[0].MeshParts)[0].Effect;
		t_floorTexture = extractTexture.Texture;
		t_Walls = content.Load<Texture2D>("graphics\\textures\\Walls");
		m_Bags2x2 = content.Load<Model>("graphics\\models\\m_Bags2x2");
		extractTexture = (BasicEffect)((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)m_Bags2x2.Meshes)[0].MeshParts)[0].Effect;
		t_Bags2x2 = extractTexture.Texture;
		m_Cardboard3x2 = content.Load<Model>("graphics\\models\\m_Cardboard3x2");
		extractTexture = (BasicEffect)((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)m_Cardboard3x2.Meshes)[0].MeshParts)[0].Effect;
		t_Cardboard3x2 = extractTexture.Texture;
		m_Mdf3x3 = content.Load<Model>("graphics\\models\\m_Mdf3x3");
		extractTexture = (BasicEffect)((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)m_Mdf3x3.Meshes)[0].MeshParts)[0].Effect;
		t_Mdf3x3 = extractTexture.Texture;
		m_Pipes4x2 = content.Load<Model>("graphics\\models\\m_Pipes4x2");
		extractTexture = (BasicEffect)((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)m_Pipes4x2.Meshes)[0].MeshParts)[0].Effect;
		t_Pipes4x2 = extractTexture.Texture;
		m_Crates4x3 = content.Load<Model>("graphics\\models\\m_Crates4x3");
		extractTexture = (BasicEffect)((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)m_Crates4x3.Meshes)[0].MeshParts)[0].Effect;
		t_Crates4x3 = extractTexture.Texture;
		m_Gate = content.Load<Model>("graphics\\models\\m_Gate");
		extractTexture = (BasicEffect)((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)m_Gate.Meshes)[0].MeshParts)[0].Effect;
		t_Gate_In = extractTexture.Texture;
		t_Gate_In_Blocked = content.Load<Texture2D>("graphics\\textures\\Gate_In_Blocked");
		t_Gate_Out = content.Load<Texture2D>("graphics\\textures\\t_Gate_Out");
		t_GoodsStatus = content.Load<Texture2D>("graphics\\textures\\GoodsStatusSheet");
		m_GateId = content.Load<Model>("graphics\\models\\m_Gate_Id");
		extractTexture = (BasicEffect)((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)m_GateId.Meshes)[0].MeshParts)[0].Effect;
		t_GateId = extractTexture.Texture;
		t_Dark = content.Load<Texture2D>("graphics\\textures\\t_Dark");
		t_Black = content.Load<Texture2D>("graphics\\textures\\t_Black8x8");
		t_hud_Lives = content.Load<Texture2D>("graphics\\textures\\hud_Lives");
		t_hud_Period_Meter = content.Load<Texture2D>("graphics\\textures\\hud_Period_Meter");
		t_hud_Period_Border = content.Load<Texture2D>("graphics\\textures\\hud_Period_Border");
		t_hud_Paper = content.Load<Texture2D>("graphics\\textures\\Order_Top_Paper");
		t_hud_Pile = content.Load<Texture2D>("graphics\\textures\\OrderPile");
		t_hud_Tray = content.Load<Texture2D>("graphics\\textures\\Order_Tray");
		t_hud_Tray_Empty = content.Load<Texture2D>("graphics\\textures\\Order_Tray_Empty");
		t_OrderOut_Notify = content.Load<Texture2D>("graphics\\textures\\OrderOut_Notify");
		m_GateLights = content.Load<Model>("graphics\\models\\m_GateLights");
		extractTexture = (BasicEffect)((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)m_GateLights.Meshes)[0].MeshParts)[0].Effect;
		t_RedLight = extractTexture.Texture;
		m_Cross = content.Load<Model>("graphics\\models\\m_Cross");
		animatedPaperTextures = new Texture2D[7];
		animatedPaperTextures[0] = content.Load<Texture2D>("graphics\\textures\\AnimatedPaper\\ap01");
		animatedPaperTextures[1] = content.Load<Texture2D>("graphics\\textures\\AnimatedPaper\\ap02");
		animatedPaperTextures[2] = content.Load<Texture2D>("graphics\\textures\\AnimatedPaper\\ap03");
		animatedPaperTextures[3] = content.Load<Texture2D>("graphics\\textures\\AnimatedPaper\\ap04");
		animatedPaperTextures[4] = content.Load<Texture2D>("graphics\\textures\\AnimatedPaper\\ap05");
		animatedPaperTextures[5] = content.Load<Texture2D>("graphics\\textures\\AnimatedPaper\\ap06");
		animatedPaperTextures[6] = content.Load<Texture2D>("graphics\\textures\\AnimatedPaper\\ap07");
		audioEngine = new AudioEngine("Content\\audio\\GameSounds.xgs");
		waveBank = new WaveBank(audioEngine, "Content\\audio\\Wave Bank.xwb");
		soundBank = new SoundBank(audioEngine, "Content\\audio\\Sound Bank.xsb");
		soundCategory = audioEngine.GetCategory("Default");
		musicCategory = audioEngine.GetCategory("Music");
		for (int i = 0; i < 4; i++)
		{
			motorCue[i] = soundBank.GetCue("TruckMotor");
			motorCue[i].Play();
		}
		if (!firstRun)
		{
			return;
		}
		GamerCollectionEnumerator<SignedInGamer> enumerator = ((GamerCollection<SignedInGamer>)(object)Gamer.SignedInGamers).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SignedInGamer current = enumerator.Current;
				ScreenManager.PlayerIsInGame[(int)current.PlayerIndex] = true;
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		OrdersLeft = (int)((float)stageData.PrePlacedGoodsArray.Length / 2f + 15f);
		if (OrdersLeft > 127)
		{
			OrdersLeft = 127;
		}
		OrdersTotal = OrdersLeft;
		UpdateOrderPile();
		GoodsBalance = new int[NumberOfPeriods + 1];
		GreysInPlay = 0;
		LifeList = new List<Life>();
		for (int i = 0; i < Lives; i++)
		{
			LifeList.Add(new Life(new Vector2((float)hudX, (float)(hudY + 40 + i * t_hud_Lives.Height))));
		}
		InGates = new Gate[stageData.NumberOfInZones];
		for (int i = 0; i < stageData.NumberOfInZones; i++)
		{
			InGates[i] = new Gate(this, t_Gate_In, stageData.InZoneListArray[i], p_IsInGate: true);
		}
		OutGates = new Gate[stageData.NumberOfOutZones];
		for (int i = 0; i < stageData.NumberOfOutZones; i++)
		{
			OutGates[i] = new Gate(this, t_Gate_Out, stageData.OutZoneListArray[i], p_IsInGate: false);
		}
		if (stageData.PrePlacedGoodsArray.Length != 0)
		{
			if (!TutorialMode)
			{
				int num = 0;
				for (int i = 0; i < stageData.PrePlacedGoodsArray.Length; i++)
				{
					goodsList.Add(new Goods(this, (int)stageData.PrePlacedGoodsArray[i].W, new Vector2(stageData.PrePlacedGoodsArray[i].X, stageData.PrePlacedGoodsArray[i].Y), stageData.PrePlacedGoodsArray[i].Z));
					goodsList[i].MyPeriod = rnd.Next(NumberOfPeriods) + 1;
					num++;
					if (num == 3)
					{
						goodsList[i].IsGrey = true;
						num = 0;
						GreysInPlay++;
					}
					else
					{
						GoodsBalance[goodsList[i].MyPeriod]++;
					}
				}
			}
			else
			{
				int num = 0;
				for (int i = 0; i < 6; i++)
				{
					goodsList.Add(new Goods(this, (int)stageData.PrePlacedGoodsArray[i].W, new Vector2(stageData.PrePlacedGoodsArray[i].X, stageData.PrePlacedGoodsArray[i].Y), stageData.PrePlacedGoodsArray[i].Z));
					goodsList[i].MyPeriod = num + 1;
					num++;
					if (num == 5)
					{
						goodsList[i].MyPeriod = 5;
						goodsList[i].IsGrey = true;
						num = 0;
						GreysInPlay++;
					}
					else
					{
						GoodsBalance[goodsList[i].MyPeriod]++;
					}
				}
			}
		}
		timeLine = new TimeLine(this, stageData.PrePlacedGoodsArray.Length);
		firstRun = false;
		if (TutorialMode)
		{
			TutorialDeliverPause = true;
			timeLine.DeliverClock = (float)timeLine.DeliverLength * 0.75f;
			TutorialPeriodPause = true;
			timeLine.PeriodClock = (float)timeLine.PeriodLength * 0.75f;
			InGates[0].MyGeom.CollisionResponseEnabled = true;
			OrdersLeft = 30;
			TutorialBlockOutgoing = true;
			TutorialUnblockeableInGate = true;
		}
	}

	public override void UnloadContent()
	{
		SilenceTruckMotors();
		audioEngine.Dispose();
		physicsSimulator.Dispose();
		content.Unload();
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		if (IsActive)
		{
			physicsSimulator.Update((float)gameTime.ElapsedGameTime.Milliseconds * 0.001f);
			viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Forward);
		}
	}

	public override void HandleInput(InputState input)
	{
		musicCategory.SetVolume((float)ScreenManager.GlobalSettings.MusicVolume / 5f);
		soundCategory.SetVolume((float)ScreenManager.GlobalSettings.SoundVolume / 5f);
		audioEngine.Update();
		bool flag = false;
		if (!input.CurrentGamePadStates[(int)ScreenManager.MainPlayer].IsConnected)
		{
			flag = true;
		}
		else
		{
			GamerCollectionEnumerator<SignedInGamer> enumerator = ((GamerCollection<SignedInGamer>)(object)Gamer.SignedInGamers).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					SignedInGamer current = enumerator.Current;
					if (ScreenManager.PlayerIsInGame[(int)current.PlayerIndex] && (input.IsPauseGame(current.PlayerIndex) || (!input.CurrentGamePadStates[(int)current.PlayerIndex].IsConnected && ScreenManager.PauseOnAnyDisconnectedGamepad)))
					{
						flag = true;
						break;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
		}
		if (flag)
		{
			ScreenManager.AddScreen(new PauseMenuScreen(ScreenManager), null);
			SilenceTruckMotors();
			return;
		}
		for (int i = 0; i < playerCount; i++)
		{
			if (liftedGoods[i] != null)
			{
				if (forkHeight[i] < 8f)
				{
					forkHeight[i] += 0.5f;
				}
				liftedGoods[i].LiftHeight = forkHeight[i];
			}
			else if (forkHeight[i] > 0f)
			{
				forkHeight[i] -= 0.5f;
			}
			if (!ScreenManager.PlayerIsInGame[i])
			{
				bodyPlayer[i].AngularVelocity = 0f;
				motorCue[i].SetVariable("Volume", -96f);
				if (liftedGoods[i] != null)
				{
					liftJoint[i].Dispose();
					liftRevJoint[i].Dispose();
					liftedGoods[i].Drop();
					liftedGoods[i] = null;
				}
				continue;
			}
			if (gearLockLeft[i])
			{
				if (input.RightStick(i).X >= 0f || input.LeftStick(i).Y > 0f)
				{
					gearLockLeft[i] = false;
				}
			}
			else if (gearLockRight[i] && (input.RightStick(i).X <= 0f || input.LeftStick(i).Y > 0f))
			{
				gearLockRight[i] = false;
			}
			if (!gearLockLeft[i] && !gearLockRight[i])
			{
				bool flag2 = true;
				if (input.LeftStick(i).Y >= 0f)
				{
					gearForward[i] = true;
				}
				else if (input.LeftStick(i).Y < 0f)
				{
					gearForward[i] = false;
					if (oldStickY[i] < input.LeftStick(i).Y && input.LeftStick(i).Y - oldStickY[i] > 0.01f)
					{
						flag2 = false;
					}
				}
				if (flag2 && !gearForward[i])
				{
					if (input.RightStick(i).X > 0f)
					{
						gearLockRight[i] = true;
					}
					else if (input.RightStick(i).X < 0f)
					{
						gearLockLeft[i] = true;
					}
				}
			}
			oldStickY[i] = input.LeftStick(i).Y;
			bodyPlayer[i].AngularVelocity = 0f;
			if (gearForward[i] || ScreenManager.PersonalSettings[i].ArcadeReverseSteering)
			{
				bodyPlayer[i].ApplyAngularImpulse(input.RightStick(i).X * 500f);
			}
			else
			{
				bodyPlayer[i].ApplyAngularImpulse(0f - input.RightStick(i).X * 500f);
			}
			float num = MathHelper.WrapAngle(bodyPlayer[i].Rotation);
			bodyPlayer[i].ApplyForce(new Vector2((float)Math.Cos(num), (float)Math.Sin(num)) * (input.LeftStick(i).Y * 1200f));
			motorCue[i].SetVariable("Volume", -25f + Math.Abs(input.LeftStick(i).Y) * Math.Abs(24f));
			if (input.PressedA(i) || (input.LeftTriggerCurrent(i) > 0f && input.LeftTriggerLast(i) == 0f) || (input.RightTriggerCurrent(i) > 0f && input.RightTriggerLast(i) == 0f))
			{
				if (liftedGoods[i] != null)
				{
					liftJoint[i].Dispose();
					liftRevJoint[i].Dispose();
					liftedGoods[i].Drop();
					liftedGoods[i] = null;
					soundBank.PlayCue("TruckLiftDown");
				}
				else
				{
					foreach (Goods goods in goodsList)
					{
						if (LiftPointGeom[i].Collide(goods.MyGeom) && !goods.IsLifted && goods.IsInGame)
						{
							liftedGoods[i] = goods;
							liftedGoods[i].Lift();
							rot = liftedGoods[i].MyPhysicsBody.TotalRotation - bodyPlayer[i].TotalRotation;
							liftRevJoint[i] = new RevoluteJoint(bodyPlayer[i], liftedGoods[i].MyPhysicsBody, bodyPlayer[i].Position);
							physicsSimulator.Add(liftRevJoint[i]);
							liftJoint[i] = new AngleLimitJoint(bodyPlayer[i], liftedGoods[i].MyPhysicsBody, rot - 0.01f, rot + 0.01f);
							physicsSimulator.Add(liftJoint[i]);
							soundBank.PlayCue("TruckLiftUp");
							break;
						}
					}
				}
			}
			if (input.PressedB(i))
			{
				soundBank.PlayCue("TruckHorn");
			}
		}
		for (int i = AnimatedPaperList.Count - 1; i >= 0; i--)
		{
			if (AnimatedPaperList[i].removeMe)
			{
				AnimatedPaperList.RemoveAt(i);
			}
		}
		for (int i = 0; i < InGates.Length; i++)
		{
			InGates[i].CheckBlocked();
			InGates[i].UpdateArrival();
			InGates[i].Anim(soundBank);
		}
		for (int i = 0; i < OutGates.Length; i++)
		{
			OutGates[i].Anim(soundBank);
			OutGates[i].IsOpening = false;
		}
		for (int i = 0; i < goodsList.Count; i++)
		{
			if (!goodsList[i].IsLifted && goodsList[i].LiftHeight > 0f)
			{
				goodsList[i].LiftHeight -= 0.5f;
				if (goodsList[i].LiftHeight == 0f)
				{
					soundBank.PlayCue("GoodsDrop");
				}
			}
			goodsList[i].UpdateMoveOut();
			goodsList[i].CheckOrderOut(soundBank);
		}
		for (int i = goodsList.Count - 1; i >= 0; i--)
		{
			if (goodsList[i].ShouldBeDeleted)
			{
				goodsList.RemoveAt(i);
			}
			else
			{
				goodsList[i].MyPhysicsBody.AngularVelocity = 0f;
				if (!goodsList[i].IsLifted)
				{
					goodsList[i].MyPhysicsBody.LinearVelocity = Vector2.Zero;
				}
			}
		}
		timeLine.Update(soundBank);
		if (!TutorialMode)
		{
			return;
		}
		Viewport viewport;
		switch (TutorialStep)
		{
		case 0:
		{
			ScreenManager.AddTutorialMessage("Welcome to the tutorial.\nThe time will be paused during most steps,\nso take the time you need to complete each step.", Vector2.Zero, 120);
			ScreenManager.AddTutorialMessage("First, try out the controls of your Truck.\nUse \u009d to drive forward/reverse.\nUse \u00a0 to steer Left/Right.\nPress \u0096 or ¡ to Lift/Drop Goods.", Vector2.Zero, 20);
			viewport = ScreenManager.LogicalViewport;
			Vector3 val = viewport.Project(TutorialMarkedPos, projectionMatrix, viewMatrix, Matrix.Identity);
			Vector2 arrowTarget2D = new Vector2(val.X, val.Y);
			ScreenManager.AddTutorialMessage("When you are ready to proceed to the next step,\ndrop any goods here.", arrowTarget2D, 20);
			TutorialStep++;
			break;
		}
		case 1:
			TutorialShowMark = true;
			{
				foreach (Goods goods2 in goodsList)
				{
					if (goods2.IsInGame && !goods2.IsLifted && Vector2.Distance(goods2.MyPhysicsBody.Position, new Vector2(TutorialMarkedPos.X, TutorialMarkedPos.Z)) < 48f)
					{
						TutorialShowMark = false;
						ScreenManager.AddTutorialMessage("The goal of each map is to deliver an\namount of goods shown by this pile of papers.", new Vector2((float)pileDestRect.X, (float)(pileDestRect.Bottom - 20)), 20);
						viewport = ScreenManager.LogicalViewport;
						Vector3 val = viewport.Project(InGates[0].Inside + new Vector3(-48f, 24f, -48f), projectionMatrix, viewMatrix, Matrix.Identity);
						Vector2 arrowTarget2D = new Vector2(val.X, val.Y);
						ScreenManager.AddTutorialMessage("Goods arrive through a gate when\nthe red bars are full.\nWatch, as goods will now arrive.", arrowTarget2D, 20);
						TutorialDeliverPause = false;
						TutorialStep++;
						break;
					}
				}
				break;
			}
		case 2:
			if (TutorialCompletedArrival)
			{
				TutorialDeliverPause = true;
				viewport = ScreenManager.LogicalViewport;
				Vector3 val = viewport.Project(InGates[0].Inside, projectionMatrix, viewMatrix, Matrix.Identity);
				Vector2 arrowTarget2D = new Vector2(val.X, val.Y);
				ScreenManager.AddTutorialMessage("If the gate zone is blocked by anything,\neven a truck, when goods arrive, the delivery\nfails, and you lose one life.", arrowTarget2D, 20);
				viewport = ScreenManager.LogicalViewport;
				val = viewport.Project(InGates[0].Inside + new Vector3(-30f, 0f, 46f), projectionMatrix, viewMatrix, Matrix.Identity);
				arrowTarget2D = new Vector2(val.X, val.Y);
				ScreenManager.AddTutorialMessage("Red stripes indicate that the gate is now blocked.", arrowTarget2D, 30);
				ScreenManager.AddTutorialMessage("Each life is shown as a crate.\nWhen they are all gone, the map is failed.", new Vector2(160f, 128f), 20);
				viewport = ScreenManager.LogicalViewport;
				val = viewport.Project(InGates[0].Inside + new Vector3(0f, 48f, 0f), projectionMatrix, viewMatrix, Matrix.Identity);
				arrowTarget2D = new Vector2(val.X, val.Y);
				ScreenManager.AddTutorialMessage("A number on a goods indicates how many\nperiods are left before the period,\nduring which the goods must be delivered.\nThis one must be delivered during the next period.", arrowTarget2D, 30);
				ScreenManager.AddTutorialMessage("A period lasts until the period bar fills up.\nWatch, as the period will now switch.", new Vector2(160f, 80f), 20);
				TutorialStep++;
			}
			break;
		case 3:
			TutorialPeriodPause = false;
			TutorialStep++;
			break;
		case 4:
			if (timeLine.PeriodClock > 100f && TutorialSkippedPeriod)
			{
				TutorialPeriodPause = true;
				viewport = ScreenManager.LogicalViewport;
				Vector3 val = viewport.Project(new Vector3(goodsList[0].MyPhysicsBody.Position.X, 48f, goodsList[0].MyPhysicsBody.Position.Y), projectionMatrix, viewMatrix, Matrix.Identity);
				Vector2 arrowTarget2D = new Vector2(val.X, val.Y);
				ScreenManager.AddTutorialMessage("A letter indicates which gate a goods must\nbe delivered through before the end of\nthe current period.", arrowTarget2D, 20);
				ScreenManager.AddTutorialMessage("Each failed goods delivery will\ncost you one life.\nThese goods can still be delivered\nto save space, but does not count.", arrowTarget2D, 20);
				viewport = ScreenManager.LogicalViewport;
				val = viewport.Project(new Vector3(goodsList[4].MyPhysicsBody.Position.X, 48f, goodsList[4].MyPhysicsBody.Position.Y), projectionMatrix, viewMatrix, Matrix.Identity);
				arrowTarget2D = new Vector2(val.X, val.Y);
				ScreenManager.AddTutorialMessage("Stock goods must be stored until ordered.\nEach new period, there is a chance that it\nwill get a number.", arrowTarget2D, 20);
				ScreenManager.AddTutorialMessage("Ok, go ahead and deliver two goods\nto complete this tutorial.", arrowTarget2D, 20);
				TutorialStep++;
			}
			break;
		case 5:
			TutorialBlockOutgoing = false;
			OrdersLeft = 2;
			UpdateOrderPile();
			TutorialStep++;
			break;
		case 6:
			break;
		}
	}

	private void RenderScene(Effect selectedEffect, ref Matrix selectedView, ref Matrix selectedProjection, bool shadowPass)
	{
		selectedEffect.Parameters["xTexture"].SetValue((Texture)(object)t_Walls);
		selectedEffect.Parameters["xWorld"].SetValue(Matrix.Identity);
		selectedEffect.Parameters["xView"].SetValue(selectedView);
		selectedEffect.Parameters["xProjection"].SetValue(selectedProjection);
		selectedEffect.CurrentTechnique.Passes[0].Apply();
		PhysCoop.graphics.GraphicsDevice.SetVertexBuffer(wallsVertexBuffer);
		PhysCoop.graphics.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, wallsVertices.Length / 3);
		selectedEffect.Parameters["xTexture"].SetValue((Texture)(object)t_Black);
		selectedEffect.Parameters["xWorld"].SetValue(Matrix.Identity);
		selectedEffect.Parameters["xView"].SetValue(selectedView);
		selectedEffect.Parameters["xProjection"].SetValue(selectedProjection);
		selectedEffect.CurrentTechnique.Passes[0].Apply();
		PhysCoop.graphics.GraphicsDevice.SetVertexBuffer(borderVertexBuffer);
		PhysCoop.graphics.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, borderVertices.Length / 3);
		selectedEffect.Parameters["xTexture"].SetValue((Texture)(object)t_Dark);
		selectedEffect.Parameters["xWorld"].SetValue(Matrix.Identity);
		selectedEffect.Parameters["xView"].SetValue(selectedView);
		selectedEffect.Parameters["xProjection"].SetValue(selectedProjection);
		selectedEffect.CurrentTechnique.Passes[0].Apply();
		PhysCoop.graphics.GraphicsDevice.SetVertexBuffer(darkVertexBuffer);
		PhysCoop.graphics.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, darkVertices.Length / 3);
		((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)m_Floor.Meshes)[0].MeshParts)[0].Effect = selectedEffect;
		((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)m_Floor.Meshes)[0].MeshParts)[0].Effect.Parameters["xTexture"].SetValue((Texture)(object)t_floorTexture);
		((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)m_Floor.Meshes)[0].MeshParts)[0].Effect.Parameters["xWorld"].SetValue(Matrix.CreateTranslation(new Vector3(640f, 0f, 360f)));
		((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)m_Floor.Meshes)[0].MeshParts)[0].Effect.Parameters["xView"].SetValue(selectedView);
		((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)m_Floor.Meshes)[0].MeshParts)[0].Effect.Parameters["xProjection"].SetValue(selectedProjection);
		((ReadOnlyCollection<ModelMesh>)(object)m_Floor.Meshes)[0].Draw();
		for (int i = 0; i < playerCount; i++)
		{
			for (int j = 0; j < ((ReadOnlyCollection<ModelMesh>)(object)truck.Meshes).Count; j++)
			{
				if (!ScreenManager.PlayerIsInGame[i] && ((ReadOnlyCollection<ModelMesh>)(object)truck.Meshes)[j].Name == "Model:Driver")
				{
					continue;
				}
				for (int k = 0; k < ((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)truck.Meshes)[j].MeshParts).Count; k++)
				{
					((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)truck.Meshes)[j].MeshParts)[0].Effect = selectedEffect;
					((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)truck.Meshes)[j].MeshParts)[0].Effect.Parameters["xTexture"].SetValue((Texture)(object)truckTextures[i]);
					GamePadState state;
					GamePadThumbSticks thumbSticks;
					if (((ReadOnlyCollection<ModelMesh>)(object)truck.Meshes)[j].Name == "Model:LBWheel")
					{
						EffectParameter val = ((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)truck.Meshes)[j].MeshParts)[0].Effect.Parameters["xWorld"];
						Matrix val2 = truckTransforms[((ReadOnlyCollection<ModelMesh>)(object)truck.Meshes)[j].ParentBone.Index] * Matrix.CreateTranslation(new Vector3(40f, 6.5f, 13f));
						state = GamePad.GetState((PlayerIndex)i);
						thumbSticks = state.ThumbSticks;
						val.SetValue(val2 * Matrix.CreateRotationY(thumbSticks.Right.X * 0.7f) * Matrix.CreateTranslation(new Vector3(-40f, -6.5f, -13f)) * Matrix.CreateRotationY(0f - bodyPlayer[i].Rotation) * Matrix.CreateTranslation(new Vector3(bodyPlayer[i].Position.X, 0f, bodyPlayer[i].Position.Y)));
					}
					else if (((ReadOnlyCollection<ModelMesh>)(object)truck.Meshes)[j].Name == "Model:RBWheel")
					{
						EffectParameter val3 = ((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)truck.Meshes)[j].MeshParts)[0].Effect.Parameters["xWorld"];
						Matrix val4 = truckTransforms[((ReadOnlyCollection<ModelMesh>)(object)truck.Meshes)[j].ParentBone.Index] * Matrix.CreateTranslation(new Vector3(40f, 6.5f, -13f));
						state = GamePad.GetState((PlayerIndex)i);
						thumbSticks = state.ThumbSticks;
						val3.SetValue(val4 * Matrix.CreateRotationY(thumbSticks.Right.X * 0.7f) * Matrix.CreateTranslation(new Vector3(-40f, -6.5f, 13f)) * Matrix.CreateRotationY(0f - bodyPlayer[i].Rotation) * Matrix.CreateTranslation(new Vector3(bodyPlayer[i].Position.X, 0f, bodyPlayer[i].Position.Y)));
					}
					else if (((ReadOnlyCollection<ModelMesh>)(object)truck.Meshes)[j].Name == "Model:Fork")
					{
						((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)truck.Meshes)[j].MeshParts)[0].Effect.Parameters["xWorld"].SetValue(truckTransforms[((ReadOnlyCollection<ModelMesh>)(object)truck.Meshes)[j].ParentBone.Index] * Matrix.CreateRotationY(0f - bodyPlayer[i].Rotation) * Matrix.CreateTranslation(new Vector3(bodyPlayer[i].Position.X, forkHeight[i], bodyPlayer[i].Position.Y)));
					}
					else
					{
						((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)truck.Meshes)[j].MeshParts)[0].Effect.Parameters["xWorld"].SetValue(truckTransforms[((ReadOnlyCollection<ModelMesh>)(object)truck.Meshes)[j].ParentBone.Index] * Matrix.CreateRotationY(0f - bodyPlayer[i].Rotation) * Matrix.CreateTranslation(new Vector3(bodyPlayer[i].Position.X, 0f, bodyPlayer[i].Position.Y)));
					}
					((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)truck.Meshes)[j].MeshParts)[0].Effect.Parameters["xView"].SetValue(selectedView);
					((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)truck.Meshes)[j].MeshParts)[0].Effect.Parameters["xProjection"].SetValue(selectedProjection);
				}
				((ReadOnlyCollection<ModelMesh>)(object)truck.Meshes)[j].Draw();
			}
		}
		for (int i = 0; i < goodsList.Count; i++)
		{
			goodsList[i].Draw(selectedEffect, ref selectedView, ref selectedProjection, ref shadowPass);
		}
		for (int i = 0; i < InGates.Length; i++)
		{
			InGates[i].Draw(selectedEffect, ref selectedView, ref selectedProjection, i);
		}
		for (int i = 0; i < OutGates.Length; i++)
		{
			OutGates[i].Draw(selectedEffect, ref selectedView, ref selectedProjection, i);
		}
		if (TutorialShowMark)
		{
			ModelMeshPart val5 = ((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)m_Cross.Meshes)[0].MeshParts)[0];
			val5.Effect = selectedEffect;
			val5.Effect.Parameters["xTexture"].SetValue((Texture)(object)t_RedLight);
			val5.Effect.Parameters["xWorld"].SetValue(Matrix.CreateTranslation(TutorialMarkedPos));
			val5.Effect.Parameters["xView"].SetValue(selectedView);
			val5.Effect.Parameters["xProjection"].SetValue(selectedProjection);
			((ReadOnlyCollection<ModelMesh>)(object)m_Cross.Meshes)[0].Draw();
		}
	}

	/// <summary>Port: (re)creates the screen-space targets at the scene target's resolution.</summary>
	private void EnsureGBuffer()
	{
		int width = ScreenManager.RenderWidth;
		int height = ScreenManager.RenderHeight;
		if (colorTarget != null && colorTarget.Width == width && colorTarget.Height == height)
		{
			return;
		}
		colorTarget?.Dispose();
		normalTarget?.Dispose();
		depthTarget?.Dispose();
		shadingTarget?.Dispose();
		GraphicsDevice device = PhysCoop.graphics.GraphicsDevice;
		colorTarget = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
		normalTarget = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.None);
		depthTarget = new RenderTarget2D(device, width, height, false, SurfaceFormat.Single, DepthFormat.None);
		shadingTarget = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.None);
	}

	public override void Draw(GameTime gameTime)
	{
		EnsureGBuffer();
		Restore_3DSettings();
		PhysCoop.graphics.GraphicsDevice.SetRenderTargets(colorTarget, normalTarget, depthTarget);
		PhysCoop.graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
		effect1Scene.CurrentTechnique = effect1Scene.Techniques["MultipleTargets"];
		RenderScene(effect1Scene, ref viewMatrix, ref projectionMatrix, shadowPass: false);
		PhysCoop.graphics.GraphicsDevice.SetRenderTarget(ScreenManager.SceneTarget);
		colorMap = colorTarget;
		normalMap = normalTarget;
		depthMap = depthTarget;
		shadingMap = GenerateShadingMap();
		CombineColorAndShading();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, ScreenManager.SpriteScale); // XNA 3.0 SpriteBlendMode 1 = AlphaBlend
		for (int i = 0; i < goodsList.Count; i++)
		{
			goodsList[i].DrawInfo2D(spriteBatch, ref viewMatrix, ref projectionMatrix);
		}
		spriteBatch.Draw(t_hud_Period_Meter, new Rectangle(hudX + 4, hudY + 4, (int)(199f * (timeLine.PeriodClock / (float)timeLine.PeriodLength)), 6), Color.White);
		spriteBatch.Draw(t_hud_Period_Border, new Vector2((float)hudX, (float)hudY), Color.White);
		foreach (Life life in LifeList)
		{
			life.Update();
			life.Draw(spriteBatch, t_hud_Lives);
		}
		if (OrdersLeft != 0)
		{
			spriteBatch.Draw(t_hud_Pile, pileDestRect, (Rectangle?)pileSourceRect, Color.White);
			spriteBatch.Draw(t_hud_Paper, new Vector2(1116f, (float)(hudY + (257 - pileDestRect.Height))), Color.White);
			spriteBatch.Draw(t_hud_Tray, new Vector2(1115f, (float)(hudY + 261)), Color.White);
		}
		else
		{
			spriteBatch.Draw(t_hud_Tray_Empty, new Vector2(1115f, (float)(hudY + 261)), Color.White);
		}
		foreach (AnimatedPaper animatedPaper in AnimatedPaperList)
		{
			animatedPaper.Update();
			animatedPaper.Draw(spriteBatch, ref animatedPaperTextures);
		}
		spriteBatch.End();
		if (TransitionPosition > 0f)
		{
			ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
		}
	}

	public void OrderCompleted(int value)
	{
		if (value == 0)
		{
			return;
		}
		if (OrdersLeft > 0)
		{
			OrdersLeft--;
			UpdateOrderPile();
			soundBank.PlayCue("OrderCompleted");
			AnimatedPaperList.Add(new AnimatedPaper(new Vector2((float)pileDestRect.X, (float)(hudY + (251 - pileDestRect.Height))), rnd.Next(-5, 5)));
		}
		if (OrdersLeft <= 0)
		{
			ScreenManager.MenuSoundBank.PlayCue("MapComplete");
			SilenceTruckMotors();
			ScreenManager.GameSaveInfo.StageCompleted[stageID] = true;
			ScreenManager.AddScreen(new MapCompletedMenuScreen(), null);
			if (ScreenManager.AutoLoadSingleGlobalSave)
			{
				ScreenManager.NeedToUseStorage = true;
				ScreenManager.RequestSaveGame = true;
			}
		}
	}

	public int RandomGoodsType()
	{
		int result = 22;
		switch (rnd.Next(5))
		{
		case 0:
			result = 22;
			break;
		case 1:
			result = 32;
			break;
		case 2:
			result = 33;
			break;
		case 3:
			result = 42;
			break;
		case 4:
			result = 43;
			break;
		}
		return result;
	}

	public void LooseLife()
	{
		if (Lives > 1)
		{
			for (int i = 0; i < 4; i++)
			{
				ScreenManager.SetGamePadVibration((PlayerIndex)i, 0.3f, 0.3f, resumeAfterMenu: false, 60);
			}
			Lives--;
			LifeList[Lives].IsLost = true;
			soundBank.PlayCue("LostLife");
			return;
		}
		Lives--;
		if (Lives == 0)
		{
			LifeList[Lives].IsLost = true;
			ScreenManager.MenuSoundBank.PlayCue("MapFail");
			SilenceTruckMotors();
			ScreenManager.AddScreen(new MapFailedMenuScreen(), null);
		}
	}

	public int SelectBalancedPeriod()
	{
		int num = NumberOfPeriods;
		for (int num2 = NumberOfPeriods - 1; num2 > 1; num2--)
		{
			if (GoodsBalance[num2] < GoodsBalance[num])
			{
				int num3 = GoodsBalance[num] - GoodsBalance[num2];
				if (rnd.Next(100) < num3 * 40)
				{
					num = num2;
				}
			}
		}
		return num;
	}

	private void UpdateOrderPile()
	{
		float num = MathHelper.Clamp((float)(OrdersLeft * 2), 0f, 256f);
		int num2 = (int)(256f - num);
		pileSourceRect = new Rectangle(0, num2, 27, (int)num);
		pileDestRect = new Rectangle(1116, hudY + 14 + num2, 27, (int)num);
	}

	private bool TruckCollision(Geom geom1, Geom geom2, ContactList cl)
	{
		if (geom1.CollisionResponseEnabled && geom2.CollisionResponseEnabled)
		{
			if (cl[0].Separation < -0.5f)
			{
				soundBank.PlayCue("TruckCollideHard");
			}
			else if (cl[0].Separation < -0.1f)
			{
				soundBank.PlayCue("Bump");
			}
		}
		return true;
	}

	private void SilenceTruckMotors()
	{
		for (int i = 0; i < 4; i++)
		{
			motorCue[i].SetVariable("Volume", -96f);
		}
	}

	public void SilenceGame()
	{
		soundCategory.SetVolume(0f);
		audioEngine.Update();
	}
}
