using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Interfaces;

namespace FarseerGames.FarseerPhysics.Collisions;

public class BruteForceCollider : IBroadPhaseCollider
{
	private Geom _geometryA;

	private Geom _geometryB;

	private PhysicsSimulator _physicsSimulator;

	public event BroadPhaseCollisionHandler OnBroadPhaseCollision;

	public BruteForceCollider(PhysicsSimulator physicsSimulator)
	{
		_physicsSimulator = physicsSimulator;
	}

	public void ProcessRemovedGeoms()
	{
	}

	public void ProcessDisposedGeoms()
	{
	}

	public void Add(Geom geom)
	{
	}

	public void Update()
	{
		DoCollision();
	}

	private void DoCollision()
	{
		for (int i = 0; i < _physicsSimulator.geomList.Count - 1; i++)
		{
			for (int j = i + 1; j < _physicsSimulator.geomList.Count; j++)
			{
				_geometryA = _physicsSimulator.geomList[i];
				_geometryB = _physicsSimulator.geomList[j];
				if (!_geometryA.body.Enabled || !_geometryB.body.Enabled || (_geometryA.collisionGroup == _geometryB.collisionGroup && _geometryA.collisionGroup != 0 && _geometryB.collisionGroup != 0) || !_geometryA.collisionEnabled || !_geometryB.collisionEnabled || (_geometryA.body.isStatic && _geometryB.body.isStatic) || _geometryA.body == _geometryB.body || (((_geometryA.CollisionCategories & _geometryB.CollidesWith) == 0) & ((_geometryB.CollisionCategories & _geometryA.CollidesWith) == 0)))
				{
					continue;
				}
				bool flag = true;
				if (_geometryA.AABB.min.X > _geometryB.AABB.max.X || _geometryB.AABB.min.X > _geometryA.AABB.max.X)
				{
					flag = false;
				}
				else if (_geometryA.AABB.min.Y > _geometryB.AABB.Max.Y || _geometryB.AABB.min.Y > _geometryA.AABB.Max.Y)
				{
					flag = false;
				}
				if (OnBroadPhaseCollision != null)
				{
					flag = OnBroadPhaseCollision(_geometryA, _geometryB);
				}
				if (flag)
				{
					Arbiter arbiter = _physicsSimulator.arbiterPool.Fetch();
					arbiter.ConstructArbiter(_geometryA, _geometryB, _physicsSimulator);
					if (!_physicsSimulator.arbiterList.Contains(arbiter))
					{
						_physicsSimulator.arbiterList.Add(arbiter);
					}
					else
					{
						_physicsSimulator.arbiterPool.Insert(arbiter);
					}
				}
			}
		}
	}
}
