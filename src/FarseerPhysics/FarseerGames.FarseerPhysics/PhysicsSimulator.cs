using System;
using System.Collections.Generic;
using System.Diagnostics;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Controllers;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Dynamics.Joints;
using FarseerGames.FarseerPhysics.Dynamics.Springs;
using FarseerGames.FarseerPhysics.Interfaces;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics;

public class PhysicsSimulator : IDisposable
{
	private const int _arbiterPoolSize = 10;

	private Body _body;

	private IBroadPhaseCollider _broadPhaseCollider;

	private bool _enabled = true;

	private Vector2 _gravity = Vector2.Zero;

	private Vector2 _gravityForce;

	private int _iterations = 5;

	private Stopwatch _sw = new Stopwatch();

	internal float allowedPenetration = 0.05f;

	internal float applyForcesTime = -1f;

	internal float applyImpulsesTime = -1f;

	internal ArbiterList arbiterList;

	internal Pool<Arbiter> arbiterPool;

	internal float biasFactor = 0.8f;

	internal List<Body> bodyAddList;

	internal BodyList bodyList;

	internal List<Body> bodyRemoveList;

	internal float broadPhaseCollisionTime = -1f;

	internal float cleanUpTime = -1f;

	internal List<Controller> controllerAddList;

	internal ControllerList controllerList;

	internal List<Controller> controllerRemoveList;

	public bool EnableDiagnostics;

	internal FrictionType frictionType = FrictionType.Average;

	internal List<Geom> geomAddList;

	internal GeomList geomList;

	internal List<Geom> geomRemoveList;

	internal List<Joint> jointAddList;

	internal JointList jointList;

	internal List<Joint> jointRemoveList;

	internal int maxContactsToDetect = 10;

	internal int maxContactsToResolve = 4;

	internal float narrowPhaseCollisionTime = -1f;

	internal List<Spring> springAddList;

	internal SpringList springList;

	internal List<Spring> springRemoveList;

	internal float updatePositionsTime = -1f;

	internal float updateTime = -1f;

	private InactivityController _inactivityController;

	private Scaling _scaling;

	public InactivityController InactivityController => _inactivityController;

	public IBroadPhaseCollider BroadPhaseCollider
	{
		get
		{
			return _broadPhaseCollider;
		}
		set
		{
			if (geomList.Count > 0)
			{
				throw new Exception("The GeomList must be empty when setting the broad phase collider type");
			}
			_broadPhaseCollider = value;
		}
	}

	public GeomList GeomList => geomList;

	public BodyList BodyList => bodyList;

	public ControllerList ControllerList => controllerList;

	public SpringList SpringList => springList;

	public JointList JointList => jointList;

	public ArbiterList ArbiterList => arbiterList;

	public Vector2 Gravity
	{
		get
		{
			return _gravity;
		}
		set
		{
			_gravity = value;
		}
	}

	public int Iterations
	{
		get
		{
			return _iterations;
		}
		set
		{
			_iterations = value;
		}
	}

	public float AllowedPenetration
	{
		get
		{
			return allowedPenetration;
		}
		set
		{
			allowedPenetration = value;
		}
	}

	public float BiasFactor
	{
		get
		{
			return biasFactor;
		}
		set
		{
			biasFactor = value;
		}
	}

	public int MaxContactsToDetect
	{
		get
		{
			return maxContactsToDetect;
		}
		set
		{
			maxContactsToDetect = value;
		}
	}

	public int MaxContactsToResolve
	{
		get
		{
			return maxContactsToResolve;
		}
		set
		{
			maxContactsToResolve = value;
		}
	}

	public float CleanUpTime => cleanUpTime;

	public float BroadPhaseCollisionTime => broadPhaseCollisionTime;

	public float NarrowPhaseCollisionTime => narrowPhaseCollisionTime;

	public float ApplyForcesTime => applyForcesTime;

	public float ApplyImpulsesTime => applyImpulsesTime;

	public float UpdatePositionsTime => updatePositionsTime;

	public float UpdateTime => updateTime;

	public FrictionType FrictionType
	{
		get
		{
			return frictionType;
		}
		set
		{
			frictionType = value;
		}
	}

	public bool Enabled
	{
		get
		{
			return _enabled;
		}
		set
		{
			_enabled = value;
		}
	}

	public Scaling Scaling
	{
		get
		{
			return _scaling;
		}
		set
		{
			_scaling = value;
		}
	}

	public PhysicsSimulator()
	{
		ConstructPhysicsSimulator(Vector2.Zero);
	}

	public PhysicsSimulator(Vector2 gravity)
	{
		ConstructPhysicsSimulator(gravity);
	}

	public void Dispose()
	{
		_inactivityController.Dispose();
	}

	private void ConstructPhysicsSimulator(Vector2 gravity)
	{
		geomList = new GeomList();
		geomAddList = new List<Geom>();
		geomRemoveList = new List<Geom>();
		bodyList = new BodyList();
		bodyAddList = new List<Body>();
		bodyRemoveList = new List<Body>();
		controllerList = new ControllerList();
		controllerAddList = new List<Controller>();
		controllerRemoveList = new List<Controller>();
		jointList = new JointList();
		jointAddList = new List<Joint>();
		jointRemoveList = new List<Joint>();
		springList = new SpringList();
		springAddList = new List<Spring>();
		springRemoveList = new List<Spring>();
		_broadPhaseCollider = new SelectiveSweepCollider(this);
		arbiterList = new ArbiterList();
		_gravity = gravity;
		arbiterPool = new Pool<Arbiter>(10);
		_inactivityController = new InactivityController(this);
		_scaling = new Scaling(0.001f, 0.01f);
	}

	public void Add(Geom geometry)
	{
		if (!geomAddList.Contains(geometry))
		{
			geomAddList.Add(geometry);
		}
	}

	public void Remove(Geom geometry)
	{
		geomRemoveList.Add(geometry);
	}

	public void Add(Body body)
	{
		if (!bodyAddList.Contains(body))
		{
			bodyAddList.Add(body);
		}
	}

	public void Remove(Body body)
	{
		bodyRemoveList.Add(body);
	}

	public void Add(Controller controller)
	{
		if (!controllerAddList.Contains(controller))
		{
			controllerAddList.Add(controller);
		}
	}

	public void Remove(Controller controller)
	{
		controllerRemoveList.Add(controller);
	}

	public void Add(Joint joint)
	{
		if (!jointAddList.Contains(joint))
		{
			jointAddList.Add(joint);
		}
	}

	public void Remove(Joint joint)
	{
		jointRemoveList.Add(joint);
	}

	public void Add(Spring spring)
	{
		if (!springAddList.Contains(spring))
		{
			springAddList.Add(spring);
		}
	}

	public void Remove(Spring spring)
	{
		springRemoveList.Add(spring);
	}

	public void Clear()
	{
		ConstructPhysicsSimulator(_gravity);
	}

	public void Update(float dt)
	{
		Update(dt, 0f);
	}

	public void Update(float dt, float dtReal)
	{
		if (dt == 0f)
		{
			return;
		}
		if (EnableDiagnostics)
		{
			_sw.Start();
		}
		if (_scaling.Enabled)
		{
			dt = _scaling.GetUpdateInterval(dt);
			if (dt == 0f)
			{
				return;
			}
			if (_scaling.UpdateInterval < dtReal)
			{
				_scaling.IncreaseUpdateInterval();
			}
			else
			{
				_scaling.DecreaseUpdateInterval();
			}
		}
		ProcessAddedItems();
		ProcessRemovedItems();
		ProcessDisposedItems();
		if (_enabled)
		{
			if (EnableDiagnostics)
			{
				cleanUpTime = _sw.ElapsedTicks;
			}
			DoBroadPhaseCollision();
			if (EnableDiagnostics)
			{
				broadPhaseCollisionTime = (float)_sw.ElapsedTicks - cleanUpTime;
			}
			DoNarrowPhaseCollision();
			if (EnableDiagnostics)
			{
				narrowPhaseCollisionTime = (float)_sw.ElapsedTicks - broadPhaseCollisionTime - cleanUpTime;
			}
			ApplyForces(dt);
			if (EnableDiagnostics)
			{
				applyForcesTime = (float)_sw.ElapsedTicks - narrowPhaseCollisionTime - broadPhaseCollisionTime - cleanUpTime;
			}
			ApplyImpulses(dt);
			if (EnableDiagnostics)
			{
				applyImpulsesTime = (float)_sw.ElapsedTicks - applyForcesTime - narrowPhaseCollisionTime - broadPhaseCollisionTime - cleanUpTime;
			}
			UpdatePositions(dt);
			if (EnableDiagnostics)
			{
				updatePositionsTime = (float)_sw.ElapsedTicks - applyImpulsesTime - applyForcesTime - narrowPhaseCollisionTime - broadPhaseCollisionTime - cleanUpTime;
			}
			if (EnableDiagnostics)
			{
				_sw.Stop();
				updateTime = _sw.ElapsedTicks;
				cleanUpTime = 1000f * cleanUpTime / (float)Stopwatch.Frequency;
				broadPhaseCollisionTime = 1000f * broadPhaseCollisionTime / (float)Stopwatch.Frequency;
				narrowPhaseCollisionTime = 1000f * narrowPhaseCollisionTime / (float)Stopwatch.Frequency;
				applyForcesTime = 1000f * applyForcesTime / (float)Stopwatch.Frequency;
				applyImpulsesTime = 1000f * applyImpulsesTime / (float)Stopwatch.Frequency;
				updatePositionsTime = 1000f * updatePositionsTime / (float)Stopwatch.Frequency;
				updateTime = 1000f * updateTime / (float)Stopwatch.Frequency;
				_sw.Reset();
			}
		}
	}

	public Geom Collide(float x, float y)
	{
		return Collide(new Vector2(x, y));
	}

	public Geom Collide(Vector2 point)
	{
		foreach (Geom geom in geomList)
		{
			if (geom.Collide(point))
			{
				return geom;
			}
		}
		return null;
	}

	public List<Geom> CollideAll(float x, float y)
	{
		return CollideAll(new Vector2(x, y));
	}

	public List<Geom> CollideAll(Vector2 point)
	{
		List<Geom> list = new List<Geom>();
		foreach (Geom geom in geomList)
		{
			if (geom.Collide(point))
			{
				list.Add(geom);
			}
		}
		return list;
	}

	private void DoBroadPhaseCollision()
	{
		_broadPhaseCollider.Update();
	}

	private void DoNarrowPhaseCollision()
	{
		for (int i = 0; i < arbiterList.Count; i++)
		{
			arbiterList[i].Collide();
		}
		arbiterList.RemoveContactCountEqualsZero(arbiterPool);
	}

	private void ApplyForces(float dt)
	{
		for (int i = 0; i < controllerList.Count; i++)
		{
			if (controllerList[i].Enabled)
			{
				controllerList[i].Update(dt);
			}
		}
		for (int i = 0; i < springList.Count; i++)
		{
			if (springList[i].Enabled)
			{
				springList[i].Update(dt);
			}
		}
		for (int i = 0; i < bodyList.Count; i++)
		{
			_body = bodyList[i];
			if (_body.Enabled)
			{
				_body.ApplyImpulses();
				if (!_body.IgnoreGravity)
				{
					_gravityForce.X = _gravity.X * _body.mass;
					_gravityForce.Y = _gravity.Y * _body.mass;
					_body.force.X = _body.force.X + _gravityForce.X;
					_body.force.Y = _body.force.Y + _gravityForce.Y;
				}
				_body.IntegrateVelocity(dt);
				_body.ClearForce();
				_body.ClearTorque();
			}
		}
	}

	private void ApplyImpulses(float dt)
	{
		float inverseDt = 1f / dt;
		for (int i = 0; i < jointList.Count; i++)
		{
			if (jointList[i].Enabled)
			{
				jointList[i].PreStep(inverseDt);
			}
		}
		for (int i = 0; i < arbiterList.Count; i++)
		{
			arbiterList[i].PreStepImpulse(inverseDt);
		}
		for (int j = 0; j < _iterations; j++)
		{
			for (int i = 0; i < jointList.Count; i++)
			{
				if (jointList[i].Enabled)
				{
					jointList[i].Update();
				}
			}
			for (int i = 0; i < arbiterList.Count; i++)
			{
				arbiterList[i].ApplyImpulse();
			}
		}
	}

	private void UpdatePositions(float dt)
	{
		for (int i = 0; i < bodyList.Count; i++)
		{
			if (bodyList[i].Enabled)
			{
				bodyList[i].IntegratePosition(dt);
			}
		}
	}

	private void ProcessAddedItems()
	{
		for (int i = 0; i < geomAddList.Count; i++)
		{
			if (!geomList.Contains(geomAddList[i]))
			{
				geomAddList[i].isRemoved = false;
				geomList.Add(geomAddList[i]);
				_broadPhaseCollider.Add(geomAddList[i]);
			}
		}
		geomAddList.Clear();
		for (int i = 0; i < bodyAddList.Count; i++)
		{
			if (!bodyList.Contains(bodyAddList[i]))
			{
				bodyList.Add(bodyAddList[i]);
			}
		}
		bodyAddList.Clear();
		for (int i = 0; i < controllerAddList.Count; i++)
		{
			if (!controllerList.Contains(controllerAddList[i]))
			{
				controllerList.Add(controllerAddList[i]);
			}
		}
		controllerAddList.Clear();
		for (int i = 0; i < jointAddList.Count; i++)
		{
			if (!jointList.Contains(jointAddList[i]))
			{
				jointList.Add(jointAddList[i]);
			}
		}
		jointAddList.Clear();
		for (int i = 0; i < springAddList.Count; i++)
		{
			if (!springList.Contains(springAddList[i]))
			{
				springList.Add(springAddList[i]);
			}
		}
		springAddList.Clear();
	}

	private void ProcessRemovedItems()
	{
		for (int i = 0; i < geomRemoveList.Count; i++)
		{
			geomRemoveList[i].isRemoved = true;
			geomList.Remove(geomRemoveList[i]);
			for (int num = arbiterList.Count; num > 0; num--)
			{
				if (arbiterList[num - 1].GeometryA == geomRemoveList[i] || arbiterList[num - 1].GeometryB == geomRemoveList[i])
				{
					arbiterList.Remove(arbiterList[num - 1]);
				}
			}
		}
		if (geomRemoveList.Count > 0)
		{
			_broadPhaseCollider.ProcessRemovedGeoms();
		}
		geomRemoveList.Clear();
		for (int i = 0; i < bodyRemoveList.Count; i++)
		{
			bodyList.Remove(bodyRemoveList[i]);
		}
		bodyRemoveList.Clear();
		for (int i = 0; i < controllerRemoveList.Count; i++)
		{
			controllerList.Remove(controllerRemoveList[i]);
		}
		controllerRemoveList.Clear();
		for (int i = 0; i < jointRemoveList.Count; i++)
		{
			jointList.Remove(jointRemoveList[i]);
		}
		jointRemoveList.Clear();
		for (int i = 0; i < springRemoveList.Count; i++)
		{
			springList.Remove(springRemoveList[i]);
		}
		springRemoveList.Clear();
	}

	private void ProcessDisposedItems()
	{
		for (int i = 0; i < controllerList.Count; i++)
		{
			controllerList[i].Validate();
		}
		for (int i = 0; i < jointList.Count; i++)
		{
			jointList[i].Validate();
		}
		for (int i = 0; i < springList.Count; i++)
		{
			springList[i].Validate();
		}
		int num = geomList.RemoveDisposed();
		if (num > 0)
		{
			_broadPhaseCollider.ProcessDisposedGeoms();
		}
		bodyList.RemoveDisposed();
		controllerList.RemoveDisposed();
		springList.RemoveDisposed();
		jointList.RemoveDisposed();
		arbiterList.RemoveContainsDisposedBody(arbiterPool);
	}
}
