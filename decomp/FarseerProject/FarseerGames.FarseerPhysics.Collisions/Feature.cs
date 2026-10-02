using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

public struct Feature
{
	public float Distance;

	public Vector2 Normal;

	public Vector2 Position;

	public Feature(Vector2 position)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Position = position;
		Normal = new Vector2(0f, 0f);
		Distance = float.MaxValue;
	}

	public Feature(Vector2 position, Vector2 normal, float distance)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Position = position;
		Normal = normal;
		Distance = distance;
	}

	public override int GetHashCode()
	{
		return (int)(Normal.X + Normal.Y + Position.X + Position.Y + Distance);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Feature))
		{
			return false;
		}
		return Equals((Feature)obj);
	}

	public bool Equals(Feature other)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		return Normal == other.Normal && Position == other.Position && Distance == other.Distance;
	}

	public static bool operator ==(Feature first, Feature second)
	{
		return first.Equals(second);
	}

	public static bool operator !=(Feature first, Feature second)
	{
		return !first.Equals(second);
	}
}
