#define DEBUG
using System.Collections.Generic;
using System.Diagnostics;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Interfaces;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

public class SweepAndPruneCollider : IBroadPhaseCollider
{
	public struct CollisionPair
	{
		public Geom Geom1;

		public Geom Geom2;

		public CollisionPair(Geom g1, Geom g2)
		{
			if (g1 < g2)
			{
				Geom1 = g1;
				Geom2 = g2;
			}
			else
			{
				Geom1 = g2;
				Geom2 = g1;
			}
		}

		public override int GetHashCode()
		{
			return Geom1.Id * 10000 + Geom2.Id;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is CollisionPair))
			{
				return false;
			}
			return Equals((CollisionPair)obj);
		}

		public bool Equals(CollisionPair other)
		{
			if (Geom1 == other.Geom1)
			{
				return Geom2 == other.Geom2;
			}
			return false;
		}

		public static bool operator ==(CollisionPair first, CollisionPair second)
		{
			return first.Equals(second);
		}

		public static bool operator !=(CollisionPair first, CollisionPair second)
		{
			return !first.Equals(second);
		}
	}

	public class CollisionPairDictionary : Dictionary<CollisionPair, bool>
	{
		public void RemovePair(Geom g1, Geom g2)
		{
			CollisionPair key = new CollisionPair(g1, g2);
			Remove(key);
		}

		public void AddPair(Geom g1, Geom g2)
		{
			CollisionPair key = new CollisionPair(g1, g2);
			if (!ContainsKey(key))
			{
				Add(key, value: true);
			}
		}
	}

	public class Extent
	{
		public ExtentInfo Info;

		public bool IsMin;

		public float Value;

		public Extent(ExtentInfo info, float value, bool isMin)
		{
			Info = info;
			Value = value;
			IsMin = isMin;
		}
	}

	public class ExtentInfo
	{
		public Geom Geometry;

		public Extent Max;

		public Extent Min;

		public List<Geom> Overlaps;

		public List<Geom> UnderConsideration;

		public ExtentInfo(Geom g, float min, float max)
		{
			Geometry = g;
			UnderConsideration = new List<Geom>();
			Overlaps = new List<Geom>();
			Min = new Extent(this, min, isMin: true);
			Max = new Extent(this, max, isMin: false);
		}
	}

	private class ExtentInfoList : List<ExtentInfo>
	{
		public SweepAndPruneCollider Owner;

		public ExtentInfoList(SweepAndPruneCollider sap)
		{
			Owner = sap;
		}

		public void MoveUnderConsiderationToOverlaps()
		{
			for (int i = 0; i < Count; i++)
			{
				if (base[i].UnderConsideration.Count == 0)
				{
					continue;
				}
				Geom geometry = base[i].Geometry;
				int count = base[i].Overlaps.Count;
				base[i].Overlaps.AddRange(base[i].UnderConsideration);
				base[i].UnderConsideration.Clear();
				for (int j = count; j < base[i].Overlaps.Count; j++)
				{
					Geom geom = base[i].Overlaps[j];
					if (!DoCollision(geometry, geom))
					{
						continue;
					}
					if (Owner.OnBroadPhaseCollision != null)
					{
						if (Owner.OnBroadPhaseCollision(geometry, geom))
						{
							Owner.CollisionPairs.AddPair(geometry, geom);
						}
					}
					else
					{
						Owner.CollisionPairs.AddPair(geometry, geom);
					}
				}
			}
		}
	}

	public class ExtentList : List<Extent>
	{
		public SweepAndPruneCollider Owner;

		public ExtentList(SweepAndPruneCollider sap)
		{
			Owner = sap;
		}

		private int InsertIntoSortedList(Extent newExtent)
		{
			int i = 0;
			if (Count == 0)
			{
				Add(newExtent);
				return 0;
			}
			for (; i < Count && (base[i].Value < newExtent.Value || (base[i].Value == newExtent.Value && base[i].IsMin && !newExtent.IsMin)); i++)
			{
			}
			Insert(i, newExtent);
			return i;
		}

		public void IncrementalInsertExtent(ExtentInfo ourInfo)
		{
			Extent min = ourInfo.Min;
			Extent max = ourInfo.Max;
			Debug.Assert(min.Value < max.Value);
			int num = InsertIntoSortedList(min);
			int num2 = InsertIntoSortedList(max);
			Geom geometry = ourInfo.Geometry;
			int i;
			for (i = num + 1; i != num2; i++)
			{
				if (base[i].IsMin)
				{
					base[i].Info.UnderConsideration.Add(geometry);
				}
			}
			i = num - 1;
			while (i >= 0 && !base[i].IsMin)
			{
				i--;
			}
			if (i < 0)
			{
				return;
			}
			List<Geom> underConsideration = ourInfo.UnderConsideration;
			Extent extent = base[i];
			underConsideration.Add(extent.Info.Geometry);
			underConsideration.AddRange(extent.Info.UnderConsideration);
			underConsideration.Remove(geometry);
			while (i != num)
			{
				if (!extent.IsMin)
				{
					underConsideration.Remove(extent.Info.Geometry);
					if (ourInfo.Overlaps.Remove(extent.Info.Geometry))
					{
						Owner.CollisionPairs.RemovePair(geometry, extent.Info.Geometry);
					}
				}
				extent = base[++i];
			}
		}

		public void IncrementalSort()
		{
			if (Count < 2)
			{
				return;
			}
			for (int i = 0; i < Count - 1; i++)
			{
				int index = i + 1;
				Extent extent = base[index];
				Extent extent2 = base[i];
				if (extent2.Value <= extent.Value)
				{
					continue;
				}
				Extent value = extent;
				if (extent.IsMin)
				{
					while (extent2.Value > extent.Value)
					{
						if (extent2.IsMin)
						{
							extent.Info.UnderConsideration.Remove(extent2.Info.Geometry);
							if (extent.Info.Overlaps.Remove(extent2.Info.Geometry))
							{
								Owner.CollisionPairs.RemovePair(extent.Info.Geometry, extent2.Info.Geometry);
							}
							extent2.Info.UnderConsideration.Add(extent.Info.Geometry);
						}
						else
						{
							extent.Info.UnderConsideration.Add(extent2.Info.Geometry);
						}
						base[index--] = base[i--];
						if (i < 0)
						{
							break;
						}
						extent2 = base[i];
					}
				}
				else
				{
					while (extent2.Value > extent.Value)
					{
						if (extent2.IsMin)
						{
							extent2.Info.UnderConsideration.Remove(extent.Info.Geometry);
							if (extent2.Info.Overlaps.Remove(extent.Info.Geometry))
							{
								Owner.CollisionPairs.RemovePair(extent.Info.Geometry, extent2.Info.Geometry);
							}
						}
						base[index--] = base[i--];
						if (i < 0)
						{
							break;
						}
						extent2 = base[i];
					}
				}
				base[index] = value;
			}
		}
	}

	private const float _floatTolerance = 0.01f;

	private PhysicsSimulator _physicsSimulator;

	private ExtentList _xExtentList;

	private ExtentInfoList _xInfoList;

	private ExtentList _yExtentList;

	private ExtentInfoList _yInfoList;

	public CollisionPairDictionary CollisionPairs;

	public event BroadPhaseCollisionHandler OnBroadPhaseCollision;

	public SweepAndPruneCollider(PhysicsSimulator physicsSimulator)
	{
		_physicsSimulator = physicsSimulator;
		_xExtentList = new ExtentList(this);
		_yExtentList = new ExtentList(this);
		_xInfoList = new ExtentInfoList(this);
		_yInfoList = new ExtentInfoList(this);
		CollisionPairs = new CollisionPairDictionary();
	}

	public void ProcessDisposedGeoms()
	{
		if (_xInfoList.RemoveAll((ExtentInfo i) => i.Geometry.IsDisposed) > 0)
		{
			_xExtentList.RemoveAll((Extent n) => n.Info.Geometry.IsDisposed);
		}
		if (_yInfoList.RemoveAll((ExtentInfo i) => i.Geometry.IsDisposed) > 0)
		{
			_yExtentList.RemoveAll((Extent n) => n.Info.Geometry.IsDisposed);
		}
		ForceNonIncrementalUpdate();
	}

	public void ProcessRemovedGeoms()
	{
		if (_xInfoList.RemoveAll((ExtentInfo i) => i.Geometry.isRemoved) > 0)
		{
			_xExtentList.RemoveAll((Extent n) => n.Info.Geometry.isRemoved);
		}
		if (_yInfoList.RemoveAll((ExtentInfo i) => i.Geometry.isRemoved) > 0)
		{
			_yExtentList.RemoveAll((Extent n) => n.Info.Geometry.isRemoved);
		}
		ForceNonIncrementalUpdate();
	}

	public void Add(Geom geom)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		ExtentInfo extentInfo = new ExtentInfo(geom, geom.AABB.Min.X, geom.AABB.Max.X);
		_xInfoList.Add(extentInfo);
		_xExtentList.IncrementalInsertExtent(extentInfo);
		ExtentInfo extentInfo2 = new ExtentInfo(geom, geom.AABB.Min.Y, geom.AABB.Max.Y);
		_yInfoList.Add(extentInfo2);
		_yExtentList.IncrementalInsertExtent(extentInfo2);
	}

	public void Update()
	{
		UpdateExtentValues();
		_xExtentList.IncrementalSort();
		_yExtentList.IncrementalSort();
		_xInfoList.MoveUnderConsiderationToOverlaps();
		_yInfoList.MoveUnderConsiderationToOverlaps();
		HandleCollisions();
	}

	public static bool DoCollision(Geom g1, Geom g2)
	{
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		if (!g1.body.Enabled || !g2.body.Enabled)
		{
			return false;
		}
		if (g1.collisionGroup == g2.collisionGroup && g1.collisionGroup != 0 && g2.collisionGroup != 0)
		{
			return false;
		}
		if (!g1.collisionEnabled || !g2.collisionEnabled)
		{
			return false;
		}
		if (g1.body.isStatic && g2.body.isStatic)
		{
			return false;
		}
		if (g1.body == g2.body)
		{
			return false;
		}
		if (((g1.collisionCategories & g2.collidesWith) == 0) & ((g2.collisionCategories & g1.collidesWith) == 0))
		{
			return false;
		}
		AABB aABB = new AABB();
		AABB aABB2 = new AABB();
		aABB.min = g1.aabb.min;
		aABB.max = g1.aabb.max;
		aABB2.min = g2.aabb.min;
		aABB2.max = g2.aabb.max;
		ref Vector2 min = ref aABB.min;
		min.X -= 0.01f;
		ref Vector2 min2 = ref aABB.min;
		min2.Y -= 0.01f;
		ref Vector2 max = ref aABB.max;
		max.X += 0.01f;
		ref Vector2 max2 = ref aABB.max;
		max2.Y += 0.01f;
		ref Vector2 min3 = ref aABB2.min;
		min3.X -= 0.01f;
		ref Vector2 min4 = ref aABB2.min;
		min4.Y -= 0.01f;
		ref Vector2 max3 = ref aABB2.max;
		max3.X += 0.01f;
		ref Vector2 max4 = ref aABB2.max;
		max4.Y += 0.01f;
		return AABB.Intersect(aABB, aABB2);
	}

	private void UpdateExtentValues()
	{
		for (int i = 0; i < _xInfoList.Count; i++)
		{
			ExtentInfo extentInfo = _xInfoList[i];
			ExtentInfo extentInfo2 = _yInfoList[i];
			AABB aabb = extentInfo.Geometry.aabb;
			extentInfo.Min.Value = aabb.min.X - 0.01f;
			extentInfo.Max.Value = aabb.max.X + 0.01f;
			extentInfo2.Min.Value = aabb.min.Y - 0.01f;
			extentInfo2.Max.Value = aabb.max.Y + 0.01f;
		}
	}

	private void HandleCollisions()
	{
		foreach (CollisionPair key in CollisionPairs.Keys)
		{
			Arbiter arbiter = _physicsSimulator.arbiterPool.Fetch();
			arbiter.ConstructArbiter(key.Geom1, key.Geom2, _physicsSimulator);
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

	public void Run()
	{
		Update();
	}

	public void ForceNonIncrementalUpdate()
	{
		UpdateExtentValues();
		CollisionPairs.Clear();
		Debug.Assert(_xInfoList.Count == _yInfoList.Count);
		for (int i = 0; i < _xInfoList.Count; i++)
		{
			_xInfoList[i].Overlaps.Clear();
			_xInfoList[i].UnderConsideration.Clear();
			_yInfoList[i].Overlaps.Clear();
			_yInfoList[i].UnderConsideration.Clear();
		}
		_xExtentList.Sort((Extent l, Extent r) => l.Value.CompareTo(r.Value));
		_yExtentList.Sort((Extent l, Extent r) => l.Value.CompareTo(r.Value));
		List<Geom> list = new List<Geom>();
		for (int i = 0; i < 2; i++)
		{
			list.Clear();
			ExtentList extentList = ((i == 0) ? _xExtentList : _yExtentList);
			foreach (Extent item in extentList)
			{
				if (item.IsMin)
				{
					item.Info.Overlaps.InsertRange(0, list);
					list.Add(item.Info.Geometry);
					continue;
				}
				list.Remove(item.Info.Geometry);
				Geom geometry = item.Info.Geometry;
				foreach (Geom overlap in item.Info.Overlaps)
				{
					if (DoCollision(geometry, overlap))
					{
						CollisionPairs.AddPair(geometry, overlap);
					}
				}
			}
		}
		HandleCollisions();
	}
}
