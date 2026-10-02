using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Mathematics;

public static class ConvertUnits
{
	private static float _displayUnitsToSimUnitsRatio = 50f;

	private static float _simUnitsToDisplayUnitsRatio = 1f / _displayUnitsToSimUnitsRatio;

	public static void SetDisplayUnitToSimUnitRatio(float displayUnitsPerSimUnit)
	{
		_displayUnitsToSimUnitsRatio = displayUnitsPerSimUnit;
		_simUnitsToDisplayUnitsRatio = 1f / displayUnitsPerSimUnit;
	}

	public static float ToDisplayUnits(float simUnits)
	{
		return simUnits * _displayUnitsToSimUnitsRatio;
	}

	public static float ToSimUnits(float displayUnits)
	{
		return displayUnits * _simUnitsToDisplayUnitsRatio;
	}

	public static float ToSimUnits(double displayUnits)
	{
		return (float)displayUnits * _simUnitsToDisplayUnitsRatio;
	}

	public static float ToDisplayUnits(int simUnits)
	{
		return (float)simUnits * _displayUnitsToSimUnitsRatio;
	}

	public static float ToSimUnits(int displayUnits)
	{
		return (float)displayUnits * _simUnitsToDisplayUnitsRatio;
	}

	public static Vector2 ToDisplayUnits(Vector2 simUnits)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return _displayUnitsToSimUnitsRatio * simUnits;
	}

	public static void ToDisplayUnits(ref Vector2 simUnits, out Vector2 displayUnits)
	{
		Vector2.Multiply(ref simUnits, _displayUnitsToSimUnitsRatio, ref displayUnits);
	}

	public static Vector2 ToDisplayUnits(float x, float y)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return _displayUnitsToSimUnitsRatio * new Vector2(x, y);
	}

	public static void ToDisplayUnits(float x, float y, out Vector2 displayUnits)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		displayUnits = Vector2.Zero;
		displayUnits.X = x * _displayUnitsToSimUnitsRatio;
		displayUnits.Y = y * _displayUnitsToSimUnitsRatio;
	}

	public static Vector2 ToSimUnits(Vector2 displayUnits)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return _simUnitsToDisplayUnitsRatio * displayUnits;
	}

	public static void ToSimUnits(ref Vector2 displayUnits, out Vector2 simUnits)
	{
		Vector2.Multiply(ref displayUnits, _simUnitsToDisplayUnitsRatio, ref simUnits);
	}

	public static Vector2 ToSimUnits(float x, float y)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return _simUnitsToDisplayUnitsRatio * new Vector2(x, y);
	}

	public static Vector2 ToSimUnits(double x, double y)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		return _simUnitsToDisplayUnitsRatio * new Vector2((float)x, (float)y);
	}

	public static void ToSimUnits(float x, float y, out Vector2 simUnits)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		simUnits = Vector2.Zero;
		simUnits.X = x * _simUnitsToDisplayUnitsRatio;
		simUnits.Y = y * _simUnitsToDisplayUnitsRatio;
	}
}
