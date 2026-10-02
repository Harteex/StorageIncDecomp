using System.Collections.ObjectModel;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace PhysCoop;

public class Gate
{
	public Vector3 Inside;

	public Vector2 InsideMin;

	public Vector2 InsideMax;

	public Vector3 Outside;

	private Body myPhysicsBody;

	public Geom MyGeom;

	public bool IsOpening = false;

	public float OpenAmount = 0f;

	private Texture2D myDefaultTexture;

	public bool IsBlocked = false;

	private bool isInGate;

	private Model myModel;

	private float rotation;

	private GameplayScreen myScreen;

	public Goods myDelivery;

	public Gate(GameplayScreen p_screen, Texture2D tex, Vector3 p_info, bool p_IsInGate)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		myScreen = p_screen;
		myModel = myScreen.m_Gate;
		myDefaultTexture = tex;
		isInGate = p_IsInGate;
		rotation = p_info.Z;
		Inside = new Vector3(p_info.X, 0f, p_info.Y);
		InsideMin.X = Inside.X - 48f;
		InsideMin.Y = Inside.Z - 48f;
		InsideMax.X = Inside.X + 48f;
		InsideMax.Y = Inside.Z + 48f;
		Outside = Inside + Vector3.Transform(new Vector3(96f, 0f, 0f), Matrix.CreateRotationY(0f - rotation));
		if (isInGate)
		{
			myPhysicsBody = BodyFactory.Instance.CreateRectangleBody(myScreen.physicsSimulator, 80f, 80f, 1f);
			myPhysicsBody.Position = new Vector2(Inside.X, Inside.Z);
			myPhysicsBody.IsStatic = true;
			myPhysicsBody.IsInGateZone = true;
			MyGeom = GeomFactory.Instance.CreateRectangleGeom(myScreen.physicsSimulator, myPhysicsBody, 80f, 80f);
			MyGeom.CollisionResponseEnabled = false;
		}
	}

	public void Draw(Effect p_Effect, ref Matrix p_View, ref Matrix p_Projection, int p_id)
	{
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0704: Unknown result type (might be due to invalid IL or missing references)
		//IL_0745: Unknown result type (might be due to invalid IL or missing references)
		//IL_0786: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)myModel.Meshes).Count; i++)
		{
			if ((!isInGate || myScreen.timeLine.NextGate != p_id) && ((ReadOnlyCollection<ModelMesh>)(object)myModel.Meshes)[i].Name == "Model:Next")
			{
				continue;
			}
			Vector3 val = ((!(((ReadOnlyCollection<ModelMesh>)(object)myModel.Meshes)[i].Name == "Model:Door")) ? Vector3.Zero : new Vector3(0f, (0f - OpenAmount) * 46f, 0f));
			for (int j = 0; j < ((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myModel.Meshes)[i].MeshParts).Count; j++)
			{
				((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myModel.Meshes)[i].MeshParts)[j].Effect = p_Effect;
				if (IsBlocked)
				{
					((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myModel.Meshes)[i].MeshParts)[j].Effect.Parameters["xTexture"].SetValue((Texture)(object)myScreen.t_Gate_In_Blocked);
				}
				else
				{
					((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myModel.Meshes)[i].MeshParts)[j].Effect.Parameters["xTexture"].SetValue((Texture)(object)myDefaultTexture);
				}
				((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myModel.Meshes)[i].MeshParts)[j].Effect.Parameters["xWorld"].SetValue(Matrix.CreateRotationY(0f - rotation) * Matrix.CreateTranslation(Inside + val));
				((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myModel.Meshes)[i].MeshParts)[j].Effect.Parameters["xView"].SetValue(p_View);
				((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myModel.Meshes)[i].MeshParts)[j].Effect.Parameters["xProjection"].SetValue(p_Projection);
			}
			((ReadOnlyCollection<ModelMesh>)(object)myModel.Meshes)[i].Draw();
		}
		if (isInGate)
		{
			Vector3 val2 = Vector3.Zero;
			if (myScreen.timeLine.NextGate == p_id)
			{
				val2 = new Vector3(0f, -45f + 45f * (myScreen.timeLine.DeliverClock / (float)myScreen.timeLine.DeliverLength), 0f);
			}
			for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)myScreen.m_GateLights.Meshes).Count; i++)
			{
				for (int j = 0; j < ((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myScreen.m_GateLights.Meshes)[i].MeshParts).Count; j++)
				{
					((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myScreen.m_GateLights.Meshes)[i].MeshParts)[j].Effect = p_Effect;
					if (myScreen.timeLine.NextGate == p_id)
					{
						((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myScreen.m_GateLights.Meshes)[i].MeshParts)[j].Effect.Parameters["xTexture"].SetValue((Texture)(object)myScreen.t_RedLight);
					}
					else
					{
						((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myScreen.m_GateLights.Meshes)[i].MeshParts)[j].Effect.Parameters["xTexture"].SetValue((Texture)(object)myScreen.t_Black);
					}
					((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myScreen.m_GateLights.Meshes)[i].MeshParts)[j].Effect.Parameters["xWorld"].SetValue(Matrix.CreateRotationY(0f - rotation) * Matrix.CreateTranslation(Inside + val2));
					((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myScreen.m_GateLights.Meshes)[i].MeshParts)[j].Effect.Parameters["xView"].SetValue(p_View);
					((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myScreen.m_GateLights.Meshes)[i].MeshParts)[j].Effect.Parameters["xProjection"].SetValue(p_Projection);
				}
				((ReadOnlyCollection<ModelMesh>)(object)myScreen.m_GateLights.Meshes)[i].Draw();
			}
			return;
		}
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)myScreen.m_GateId.Meshes).Count; i++)
		{
			switch (p_id)
			{
			case 0:
				if (((ReadOnlyCollection<ModelMesh>)(object)myScreen.m_GateId.Meshes)[i].Name != "Model:A")
				{
					continue;
				}
				break;
			case 1:
				if (((ReadOnlyCollection<ModelMesh>)(object)myScreen.m_GateId.Meshes)[i].Name != "Model:B")
				{
					continue;
				}
				break;
			case 2:
				if (((ReadOnlyCollection<ModelMesh>)(object)myScreen.m_GateId.Meshes)[i].Name != "Model:C")
				{
					continue;
				}
				break;
			case 3:
				if (((ReadOnlyCollection<ModelMesh>)(object)myScreen.m_GateId.Meshes)[i].Name != "Model:D")
				{
					continue;
				}
				break;
			}
			for (int j = 0; j < ((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myScreen.m_GateId.Meshes)[i].MeshParts).Count; j++)
			{
				((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myScreen.m_GateId.Meshes)[i].MeshParts)[j].Effect = p_Effect;
				((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myScreen.m_GateId.Meshes)[i].MeshParts)[j].Effect.Parameters["xTexture"].SetValue((Texture)(object)myScreen.t_GateId);
				((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myScreen.m_GateId.Meshes)[i].MeshParts)[j].Effect.Parameters["xWorld"].SetValue(Matrix.CreateScale(1.2f) * Matrix.CreateTranslation(Inside));
				((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myScreen.m_GateId.Meshes)[i].MeshParts)[j].Effect.Parameters["xView"].SetValue(p_View);
				((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)myScreen.m_GateId.Meshes)[i].MeshParts)[j].Effect.Parameters["xProjection"].SetValue(p_Projection);
			}
			((ReadOnlyCollection<ModelMesh>)(object)myScreen.m_GateId.Meshes)[i].Draw();
		}
	}

	public void CheckBlocked()
	{
		IsBlocked = false;
		foreach (Arbiter arbiter in myScreen.physicsSimulator.ArbiterList)
		{
			bool flag = false;
			if (arbiter.GeometryA == MyGeom)
			{
				for (int i = 0; i < 4; i++)
				{
					if (arbiter.GeometryB == myScreen.LiftPointGeom[i])
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					IsBlocked = true;
					break;
				}
			}
			else
			{
				if (!(arbiter.GeometryB == MyGeom))
				{
					continue;
				}
				for (int i = 0; i < 4; i++)
				{
					if (arbiter.GeometryA == myScreen.LiftPointGeom[i])
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					IsBlocked = true;
					break;
				}
			}
		}
		if (myScreen.TutorialUnblockeableInGate)
		{
			IsBlocked = false;
		}
	}

	public void Anim(SoundBank soundBank)
	{
		if (IsOpening && OpenAmount < 1f)
		{
			if (OpenAmount == 0f)
			{
				soundBank.PlayCue("GateOpen");
			}
			OpenAmount += 0.1f;
			OpenAmount = MathHelper.Clamp(OpenAmount, 0f, 1f);
		}
		if (!IsOpening && OpenAmount > 0f)
		{
			if (OpenAmount == 1f)
			{
				soundBank.PlayCue("GateClose");
			}
			OpenAmount -= 0.1f;
			OpenAmount = MathHelper.Clamp(OpenAmount, 0f, 1f);
		}
	}

	public void NewArrival(Gate p_gate)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		myDelivery = new Goods(myScreen, myScreen.RandomGoodsType(), new Vector2(Outside.X, Outside.Z), 0f);
		myDelivery.IsInGame = false;
		myDelivery.MyPeriod = myScreen.SelectBalancedPeriod();
		if (myScreen.GreysInPlay < myScreen.goodsList.Count / myScreen.GreyGoalAmountDivisor)
		{
			myDelivery.IsGrey = true;
			myScreen.GreysInPlay++;
		}
		else
		{
			myScreen.GoodsBalance[myDelivery.MyPeriod]++;
		}
		myDelivery.MyGeom.CollisionResponseEnabled = false;
		myDelivery.DeliveryPathPos = 0f;
		IsOpening = true;
		MyGeom.CollisionResponseEnabled = true;
		if (myScreen.TutorialMode)
		{
			myDelivery.IsGrey = false;
			myDelivery.MyPeriod = 1;
		}
		myScreen.goodsList.Add(myDelivery);
	}

	public void UpdateArrival()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (myDelivery == null)
		{
			return;
		}
		Vector2 val = new Vector2(Outside.X, Outside.Z);
		Vector2 val2 = new Vector2(Inside.X, Inside.Z) - val;
		float num = MathHelper.SmoothStep(0f, 1f, myDelivery.DeliveryPathPos);
		myDelivery.MyPhysicsBody.Position = val + val2 * num;
		if (myDelivery.DeliveryPathPos < 1f)
		{
			if ((double)OpenAmount > 0.8)
			{
				myDelivery.DeliveryPathPos += 0.02f;
			}
			if (myScreen.TutorialMode)
			{
				myScreen.TutorialUnblockeableInGate = false;
			}
			return;
		}
		myDelivery.IsInGame = true;
		myDelivery.MyGeom.CollisionResponseEnabled = true;
		IsOpening = false;
		MyGeom.CollisionResponseEnabled = false;
		myDelivery = null;
		if (myScreen.TutorialMode)
		{
			myScreen.TutorialCompletedArrival = true;
		}
	}
}
