using System;
using Microsoft.Xna.Framework;

namespace BallGame_Editor;

[Serializable]
public class StageData
{
	public int NumberOfWalls;

	public int[] NumberOfWallVertices;

	public Vector2[][] PolygonWallsArray;

	public Vector2[] PlayerArray;

	public int NumberOfInZones;

	public Vector3[] InZoneListArray;

	public int NumberOfOutZones;

	public Vector3[] OutZoneListArray;

	public int NumberOfPrePlacedGoods;

	public Vector4[] PrePlacedGoodsArray;

	public int NumberOfDark;

	public Rectangle[] DarkListArray;
}
