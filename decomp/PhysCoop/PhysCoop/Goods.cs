using System;
using System.Collections.ObjectModel;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace PhysCoop;

public class Goods
{
	private float defMOI;

	private Model myModel;

	public Body MyPhysicsBody;

	public Geom MyGeom;

	public int MyPeriod;

	private Texture2D myTexture;

	private GameplayScreen myScreen;

	public bool IsLifted = false;

	public float LiftHeight = 0f;

	public bool IsInGame = true;

	private float myMass = 1f;

	public int MyValue;

	public float DeliveryPathPos = 0f;

	private int orderGate;

	private bool IsMovingOut = false;

	private bool IsNotCentered = true;

	public bool ShouldBeDeleted = false;

	public bool IsGrey = false;

	public Goods(GameplayScreen p_screen, int p_type, Vector2 p_pos, float p_rot)
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		myScreen = p_screen;
		MyValue = 100;
		orderGate = myScreen.rnd.Next(myScreen.OutGates.Length);
		switch (p_type)
		{
		case 22:
			MyPhysicsBody = CreatePhysicsGoods(32, 32, p_pos, p_rot, myMass);
			myModel = myScreen.m_Bags2x2;
			myTexture = myScreen.t_Bags2x2;
			break;
		case 32:
			MyPhysicsBody = CreatePhysicsGoods(48, 32, p_pos, p_rot, myMass);
			myModel = myScreen.m_Cardboard3x2;
			myTexture = myScreen.t_Cardboard3x2;
			break;
		case 33:
			MyPhysicsBody = CreatePhysicsGoods(48, 48, p_pos, p_rot, myMass);
			myModel = myScreen.m_Mdf3x3;
			myTexture = myScreen.t_Mdf3x3;
			break;
		case 42:
			MyPhysicsBody = CreatePhysicsGoods(64, 32, p_pos, p_rot, myMass);
			myModel = myScreen.m_Pipes4x2;
			myTexture = myScreen.t_Pipes4x2;
			break;
		case 43:
			MyPhysicsBody = CreatePhysicsGoods(64, 48, p_pos, p_rot, myMass);
			myModel = myScreen.m_Crates4x3;
			myTexture = myScreen.t_Crates4x3;
			break;
		}
	}

	private Body CreatePhysicsGoods(int p_width, int p_height, Vector2 p_pos, float p_rot, float p_mass)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Body body = BodyFactory.Instance.CreateRectangleBody(myScreen.physicsSimulator, p_width, p_height, p_mass);
		body.Position = p_pos;
		body.Rotation = p_rot;
		body.IsGoods = true;
		body.MomentOfInertia *= 10f;
		MyGeom = GeomFactory.Instance.CreateRectangleGeom(myScreen.physicsSimulator, body, p_width, p_height);
		return body;
	}

	public void DrawInfo2D(SpriteBatch spriteBatch, ref Matrix p_View, ref Matrix p_Projection)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		if (IsInGame)
		{
			Rectangle value;
			if (MyPeriod != 0)
			{
				value = (IsGrey ? new Rectangle(128, 0, 32, 32) : new Rectangle((int)((float)MyPeriod * 32f) - 32, 32, 32, 32));
			}
			else
			{
				value = new Rectangle((int)((float)orderGate * 32f), 0, 32, 32);
			}
			Viewport viewport = ((DrawableGameComponent)myScreen.ScreenManager).GraphicsDevice.Viewport;
			Vector3 val = viewport.Project(new Vector3(MyPhysicsBody.Position.X, 46f + LiftHeight, MyPhysicsBody.Position.Y), p_Projection, p_View, Matrix.Identity);
			Rectangle val2 = new Rectangle((int)Math.Round(val.X - 14f), (int)Math.Round(val.Y - 14f), 28, 28);
			spriteBatch.Draw(myScreen.t_GoodsStatus, val2, (Rectangle?)value, new Color(1f, 1f, 1f, myScreen.timeLine.FlipAlpha));
			if (MyPeriod == 0 && MyValue != 0)
			{
				spriteBatch.Draw(myScreen.t_OrderOut_Notify, new Vector2(val.X, val.Y), (Rectangle?)new Rectangle(0, 0, myScreen.t_OrderOut_Notify.Width, myScreen.t_OrderOut_Notify.Height), new Color(1f, 1f, 1f, myScreen.timeLine.FlipAlpha), myScreen.timeLine.OrderOutNotifyRotation, new Vector2((float)(myScreen.t_OrderOut_Notify.Width / 2), (float)(myScreen.t_OrderOut_Notify.Height / 2)), 1f, (SpriteEffects)0, 0f);
			}
		}
	}

	public void Draw(Effect p_Effect, ref Matrix p_View, ref Matrix p_Projection, ref bool p_shadowPass)
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myModel.Meshes)[0].MeshParts)[0].Effect = p_Effect;
		((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myModel.Meshes)[0].MeshParts)[0].Effect.Parameters["xTexture"].SetValue((Texture)(object)myTexture);
		((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myModel.Meshes)[0].MeshParts)[0].Effect.Parameters["xWorld"].SetValue(Matrix.CreateRotationY(0f - MyPhysicsBody.Rotation) * Matrix.CreateTranslation(new Vector3(MyPhysicsBody.Position.X, LiftHeight, MyPhysicsBody.Position.Y)));
		((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myModel.Meshes)[0].MeshParts)[0].Effect.Parameters["xView"].SetValue(p_View);
		((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myModel.Meshes)[0].MeshParts)[0].Effect.Parameters["xProjection"].SetValue(p_Projection);
		((ReadOnlyCollection<ModelMesh>)(object)myModel.Meshes)[0].Draw();
	}

	public void Lift()
	{
		IsLifted = true;
		MyPhysicsBody.Mass = 0.25f;
		defMOI = MyPhysicsBody.MomentOfInertia;
		MyPhysicsBody.MomentOfInertia /= 80f;
	}

	public void Drop()
	{
		IsLifted = false;
		MyPhysicsBody.Mass = myMass;
		MyPhysicsBody.MomentOfInertia = defMOI;
	}

	public void NewPeriod()
	{
		if (MyPeriod > 0)
		{
			if (!IsGrey)
			{
				myScreen.GoodsBalance[MyPeriod]--;
				MyPeriod--;
				myScreen.GoodsBalance[MyPeriod]++;
			}
			else if (!myScreen.TutorialMode && (myScreen.rnd.Next(100) < 10 || myScreen.GreysInPlay > myScreen.goodsList.Count / myScreen.GreyGoalAmountDivisor))
			{
				IsGrey = false;
				myScreen.GreysInPlay--;
				myScreen.GoodsBalance[MyPeriod]++;
			}
		}
		else if (MyValue != 0)
		{
			if (!IsMovingOut && IsInGame)
			{
				MyValue = 0;
			}
			myScreen.LooseLife();
		}
	}

	public void CheckOrderOut(SoundBank soundBank)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		if (!myScreen.TutorialBlockOutgoing && MyPeriod == 0 && IsInGame && !IsLifted && !IsMovingOut && MyPhysicsBody.Position.X > myScreen.OutGates[orderGate].InsideMin.X && MyPhysicsBody.Position.X < myScreen.OutGates[orderGate].InsideMax.X && MyPhysicsBody.Position.Y > myScreen.OutGates[orderGate].InsideMin.Y && MyPhysicsBody.Position.Y < myScreen.OutGates[orderGate].InsideMax.Y)
		{
			IsInGame = false;
			MyGeom.CollisionResponseEnabled = false;
			DeliveryPathPos = 0f;
			IsMovingOut = true;
			soundBank.PlayCue("GoodsSlide");
		}
	}

	public void UpdateMoveOut()
	{
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if (!IsMovingOut)
		{
			return;
		}
		if (IsNotCentered)
		{
			Vector2 val = new Vector2(myScreen.OutGates[orderGate].Inside.X, myScreen.OutGates[orderGate].Inside.Z);
			if (Vector2.Distance(MyPhysicsBody.Position, val) > 1f)
			{
				MyPhysicsBody.ApplyForce(Vector2.Normalize(val - MyPhysicsBody.Position) * (myMass * 5000f));
			}
			else
			{
				IsNotCentered = false;
			}
			return;
		}
		Vector2 val2 = new Vector2(myScreen.OutGates[orderGate].Inside.X, myScreen.OutGates[orderGate].Inside.Z);
		Vector2 val3 = new Vector2(myScreen.OutGates[orderGate].Outside.X, myScreen.OutGates[orderGate].Outside.Z) - val2;
		float num = MathHelper.SmoothStep(0f, 1f, DeliveryPathPos);
		MyPhysicsBody.Position = val2 + val3 * num;
		if (DeliveryPathPos < 1f)
		{
			if ((double)myScreen.OutGates[orderGate].OpenAmount > 0.8)
			{
				DeliveryPathPos += 0.02f;
			}
			myScreen.OutGates[orderGate].IsOpening = true;
		}
		else
		{
			myScreen.OrderCompleted(MyValue);
			myScreen.physicsSimulator.Remove(MyGeom);
			myScreen.physicsSimulator.Remove(MyPhysicsBody);
			ShouldBeDeleted = true;
			myScreen.GoodsBalance[MyPeriod]--;
		}
	}
}
