using System;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

public sealed class Grid
{
	private AABB _aabb;

	private float _gridCellSize;

	private float _gridCellSizeInv;

	private float[,] _nodes;

	private Vector2[] _points;

	public Vector2[] Points => _points;

	public Grid Clone()
	{
		Grid grid = new Grid();
		grid._gridCellSize = _gridCellSize;
		grid._gridCellSizeInv = _gridCellSizeInv;
		grid._aabb = _aabb;
		grid._nodes = (float[,])_nodes.Clone();
		grid._points = (Vector2[])_points.Clone();
		return grid;
	}

	public void ComputeGrid(Geom geometry, float gridCellSize)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		Matrix matrix = geometry.Matrix;
		Matrix identity = Matrix.Identity;
		geometry.Matrix = identity;
		_aabb = new AABB(geometry.AABB);
		_gridCellSize = gridCellSize;
		_gridCellSizeInv = 1f / gridCellSize;
		int num = (int)Math.Ceiling((double)(_aabb.Max.X - _aabb.Min.X) * (double)_gridCellSizeInv) + 1;
		int num2 = (int)Math.Ceiling((double)(_aabb.Max.Y - _aabb.Min.Y) * (double)_gridCellSizeInv) + 1;
		_nodes = new float[num, num2];
		_points = new Vector2[num * num2];
		int num3 = 0;
		Vector2 min = _aabb.Min;
		int num4 = 0;
		while (num4 < num)
		{
			min.Y = _aabb.Min.Y;
			int num5 = 0;
			while (num5 < num2)
			{
				_nodes[num4, num5] = geometry.GetNearestDistance(min);
				_points[num3] = min;
				num3++;
				num5++;
				min.Y += gridCellSize;
			}
			num4++;
			min.X += gridCellSize;
		}
		geometry.Matrix = matrix;
	}

	public bool Intersect(ref Vector2 vector, out Feature feature)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		if (_aabb.Contains(ref vector))
		{
			int num = (int)Math.Floor((vector.X - _aabb.Min.X) * _gridCellSizeInv);
			int num2 = (int)Math.Floor((vector.Y - _aabb.Min.Y) * _gridCellSizeInv);
			float num3 = (vector.X - (_gridCellSize * (float)num + _aabb.Min.X)) * _gridCellSizeInv;
			float num4 = (vector.Y - (_gridCellSize * (float)num2 + _aabb.Min.Y)) * _gridCellSizeInv;
			float num5 = _nodes[num, num2];
			float num6 = _nodes[num + 1, num2];
			float num7 = _nodes[num, num2 + 1];
			float num8 = _nodes[num + 1, num2 + 1];
			if (num5 <= 0f || num6 <= 0f || num7 <= 0f || num8 <= 0f)
			{
				float num9 = MathHelper.Lerp(num7, num8, num3);
				float num10 = MathHelper.Lerp(num5, num6, num3);
				float num11 = MathHelper.Lerp(num10, num9, num4);
				if (num11 <= 0f)
				{
					float num12 = MathHelper.Lerp(num6, num8, num4);
					float num13 = MathHelper.Lerp(num5, num7, num4);
					Vector2 zero = Vector2.Zero;
					zero.X = num12 - num13;
					zero.Y = num9 - num10;
					if (zero.X != 0f || zero.Y != 0f)
					{
						Vector2.Normalize(ref zero, ref zero);
						feature = new Feature(vector, zero, num11);
						return true;
					}
				}
			}
		}
		feature = default;
		return false;
	}
}
