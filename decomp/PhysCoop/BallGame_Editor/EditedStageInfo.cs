using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BallGame_Editor;

public class EditedStageInfo
{
	public class VerticeData
	{
		public List<Vector2> Vertices = new List<Vector2>();
	}

	public class InZone
	{
		public Vector2 Position;

		public float Rotation;

		public InZone(Vector2 p_pos)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			Position = p_pos;
			Rotation = 0f;
		}
	}

	public class OutZone
	{
		public Vector2 Position;

		public float Rotation;

		public OutZone(Vector2 p_pos)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			Position = p_pos;
			Rotation = 0f;
		}
	}

	public class PrePlacedGoods
	{
		public Vector2 Position;

		public float Rotation;

		public int Type;

		public PrePlacedGoods(Vector2 p_pos)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			Position = p_pos;
			Rotation = 0f;
			Type = 22;
		}
	}

	public List<VerticeData> PolygonWalls = new List<VerticeData>();

	public List<Vector2> PlayerList = new List<Vector2>();

	public List<InZone> InZoneList = new List<InZone>();

	public List<OutZone> OutZoneList = new List<OutZone>();

	public List<Rectangle> Dark = new List<Rectangle>();

	public List<PrePlacedGoods> PrePlacedGoodsList = new List<PrePlacedGoods>();
}
