using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Interfaces;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Controllers;

public class AABBFluidContainer : IFluidContainer
{
	private AABB _aabb;

	public AABB AABB
	{
		get
		{
			return _aabb;
		}
		set
		{
			_aabb = value;
		}
	}

	public AABBFluidContainer()
	{
	}

	public AABBFluidContainer(AABB aabb)
	{
		_aabb = aabb;
	}

	public bool Intersect(AABB aabb)
	{
		return AABB.Intersect(aabb, _aabb);
	}

	public bool Contains(ref Vector2 vector)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return _aabb.Contains(vector);
	}
}
