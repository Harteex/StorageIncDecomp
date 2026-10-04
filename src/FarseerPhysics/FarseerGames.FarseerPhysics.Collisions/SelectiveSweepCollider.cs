using System.Collections.Generic;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Interfaces;

namespace FarseerGames.FarseerPhysics.Collisions;

public sealed class SelectiveSweepCollider : IBroadPhaseCollider
{
	private sealed class Stub
	{
		public bool Begin;

		public float Value;

		public Wrapper Wrapper;

		public Stub(Wrapper wrapper, bool begin)
		{
			Wrapper = wrapper;
			Begin = begin;
		}
	}

	private sealed class StubComparer : IComparer<Stub>
	{
		// Port fix: the original tie-break returned left.Begin ? -1 : 1 without looking at
		// right, so two Begin (or two End) stubs at the same value each compared "less" than
		// the other. .NET CF's sort tolerated that; .NET 8's List.Sort can detect it and
		// throws ("IComparer.Compare() method returns inconsistent results"), which happened
		// when new goods spawned aligned with existing ones. Same order otherwise: by value,
		// then Begin before End (touching AABBs still overlap); same-kind ties are equal.
		public int Compare(Stub left, Stub right)
		{
			int byValue = left.Value.CompareTo(right.Value);
			if (byValue != 0)
			{
				return byValue;
			}
			if (left.Begin == right.Begin)
			{
				return 0;
			}
			return left.Begin ? -1 : 1;
		}
	}

	private sealed class Wrapper
	{
		private Stub _xBegin;

		private Stub _xEnd;

		private Stub _yBegin;

		private Stub _yEnd;

		public Geom Geom;

		public float Max;

		public float Min;

		public LinkedListNode<Wrapper> Node;

		public bool ShouldAddNode;

		public Wrapper(Geom body)
		{
			Geom = body;
			Node = new LinkedListNode<Wrapper>(this);
			_xBegin = new Stub(this, begin: true);
			_xEnd = new Stub(this, begin: false);
			_yBegin = new Stub(this, begin: true);
			_yEnd = new Stub(this, begin: false);
		}

		public void AddStubs(List<Stub> xStubs, List<Stub> yStubs)
		{
			xStubs.Add(_xBegin);
			xStubs.Add(_xEnd);
			yStubs.Add(_yBegin);
			yStubs.Add(_yEnd);
		}

		public void Update()
		{
			AABB aABB = Geom.AABB;
			ShouldAddNode = aABB.Min.X != aABB.Max.X || aABB.Min.Y != aABB.Max.Y;
			_xBegin.Value = aABB.Min.X;
			_xEnd.Value = aABB.Max.X;
			_yBegin.Value = aABB.Min.Y;
			_yEnd.Value = aABB.Max.Y;
		}

		public void SetX()
		{
			Min = _xBegin.Value;
			Max = _xEnd.Value;
		}

		public void SetY()
		{
			Min = _yBegin.Value;
			Max = _yEnd.Value;
		}
	}

	private static StubComparer _comparer = new StubComparer();

	private LinkedList<Wrapper> _currentBodies = new LinkedList<Wrapper>();

	private PhysicsSimulator _physicsSimulator;

	private List<Wrapper> _wrappers;

	private List<Stub> _xStubs;

	private List<Stub> _yStubs;

	public event BroadPhaseCollisionHandler OnBroadPhaseCollision;

	public SelectiveSweepCollider(PhysicsSimulator physicsSimulator)
	{
		_physicsSimulator = physicsSimulator;
		_wrappers = new List<Wrapper>();
		_xStubs = new List<Stub>();
		_yStubs = new List<Stub>();
	}

	public void Add(Geom geom)
	{
		Wrapper wrapper = new Wrapper(geom);
		_wrappers.Add(wrapper);
		wrapper.AddStubs(_xStubs, _yStubs);
	}

	public void ProcessRemovedGeoms()
	{
		if (_wrappers.RemoveAll(WrapperIsRemoved) > 0)
		{
			_xStubs.RemoveAll(StubIsRemoved);
			_yStubs.RemoveAll(StubIsRemoved);
		}
	}

	public void ProcessDisposedGeoms()
	{
		if (_wrappers.RemoveAll(WrapperIsDisposed) > 0)
		{
			_xStubs.RemoveAll(StubIsDisposed);
			_yStubs.RemoveAll(StubIsDisposed);
		}
	}

	public void Update()
	{
		InternalUpdate();
		DetectInternal(ShouldDoX());
	}

	public void Clear()
	{
		_wrappers.Clear();
		_xStubs.Clear();
		_yStubs.Clear();
	}

	private static bool WrapperIsDisposed(Wrapper wrapper)
	{
		return wrapper.Geom.IsDisposed;
	}

	private static bool StubIsDisposed(Stub stub)
	{
		return stub.Wrapper.Geom.IsDisposed;
	}

	private static bool WrapperIsRemoved(Wrapper wrapper)
	{
		return wrapper.Geom.isRemoved;
	}

	private static bool StubIsRemoved(Stub stub)
	{
		return stub.Wrapper.Geom.isRemoved;
	}

	public void InternalUpdate()
	{
		for (int i = 0; i < _wrappers.Count; i++)
		{
			_wrappers[i].Update();
		}
		_xStubs.Sort(_comparer);
		_yStubs.Sort(_comparer);
	}

	private bool ShouldDoX()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < _xStubs.Count; i++)
		{
			if (_xStubs[i].Begin)
			{
				num += num2++;
			}
			else
			{
				num2--;
			}
			if (_yStubs[i].Begin)
			{
				num3 += num4++;
			}
			else
			{
				num4--;
			}
		}
		return num < num3;
	}

	private void DetectInternal(bool doX)
	{
		List<Stub> list = (doX ? _xStubs : _yStubs);
		_currentBodies.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			Stub stub = list[i];
			Wrapper wrapper = stub.Wrapper;
			if (stub.Begin)
			{
				if (doX)
				{
					wrapper.SetY();
				}
				else
				{
					wrapper.SetX();
				}
				Geom geom = wrapper.Geom;
				for (LinkedListNode<Wrapper> linkedListNode = _currentBodies.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
				{
					Wrapper value = linkedListNode.Value;
					Geom geom2 = value.Geom;
					if (wrapper.Min <= value.Max && value.Min <= wrapper.Max)
					{
						if (OnBroadPhaseCollision != null)
						{
							if (OnBroadPhaseCollision(geom, geom2))
							{
								OnCollision(geom, geom2);
							}
						}
						else
						{
							OnCollision(geom, geom2);
						}
					}
				}
				if (wrapper.ShouldAddNode)
				{
					_currentBodies.AddLast(wrapper.Node);
				}
			}
			else if (wrapper.ShouldAddNode)
			{
				_currentBodies.Remove(wrapper.Node);
			}
		}
	}

	private void OnCollision(Geom geom1, Geom geom2)
	{
		if (geom1.Body.IsGoods)
		{
			if (!geom1.Body.HasMoved() && ((geom2.Body.IsStatic && !geom2.Body.IsInGateZone) || (geom2.Body.IsGoods && !geom2.Body.HasMoved())))
			{
				foreach (Arbiter arbiter2 in _physicsSimulator.arbiterList)
				{
					if ((arbiter2.GeomA == geom1 && arbiter2.GeomB == geom2) || (arbiter2.GeomA == geom2 && arbiter2.GeomB == geom1))
					{
						_physicsSimulator.arbiterList.Remove(arbiter2);
						break;
					}
				}
				return;
			}
		}
		else if (geom2.Body.IsGoods && !geom2.Body.HasMoved() && ((geom1.Body.IsStatic && !geom1.Body.IsInGateZone) || (geom1.Body.IsGoods && !geom1.Body.HasMoved())))
		{
			foreach (Arbiter arbiter3 in _physicsSimulator.arbiterList)
			{
				if ((arbiter3.GeomA == geom1 && arbiter3.GeomB == geom2) || (arbiter3.GeomA == geom2 && arbiter3.GeomB == geom1))
				{
					_physicsSimulator.arbiterList.Remove(arbiter3);
					break;
				}
			}
			return;
		}
		if (geom1.Body.IsTruck && !geom2.Body.IsInGateZone && geom2.Body.IsStatic && !geom1.Body.HasMoved())
		{
			foreach (Arbiter arbiter4 in _physicsSimulator.arbiterList)
			{
				if ((arbiter4.GeomA == geom1 && arbiter4.GeomB == geom2) || (arbiter4.GeomA == geom2 && arbiter4.GeomB == geom1))
				{
					_physicsSimulator.arbiterList.Remove(arbiter4);
					break;
				}
			}
			return;
		}
		if (geom2.Body.IsTruck && !geom1.Body.IsInGateZone && geom1.Body.IsStatic && !geom2.Body.HasMoved())
		{
			foreach (Arbiter arbiter5 in _physicsSimulator.arbiterList)
			{
				if ((arbiter5.GeomA == geom1 && arbiter5.GeomB == geom2) || (arbiter5.GeomA == geom2 && arbiter5.GeomB == geom1))
				{
					_physicsSimulator.arbiterList.Remove(arbiter5);
					break;
				}
			}
			return;
		}
		if (geom1.body.Enabled && geom2.body.Enabled && (geom1.collisionGroup != geom2.collisionGroup || geom1.collisionGroup == 0 || geom2.collisionGroup == 0) && geom1.collisionEnabled && geom2.collisionEnabled && (!geom1.body.isStatic || !geom2.body.isStatic) && geom1.body != geom2.body && !(((geom1.collisionCategories & geom2.collidesWith) == 0) & ((geom2.collisionCategories & geom1.collidesWith) == 0)))
		{
			Arbiter arbiter = _physicsSimulator.arbiterPool.Fetch();
			arbiter.ConstructArbiter(geom1, geom2, _physicsSimulator);
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
