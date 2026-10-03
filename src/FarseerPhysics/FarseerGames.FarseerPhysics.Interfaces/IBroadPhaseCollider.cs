using FarseerGames.FarseerPhysics.Collisions;

namespace FarseerGames.FarseerPhysics.Interfaces;

public interface IBroadPhaseCollider
{
	event BroadPhaseCollisionHandler OnBroadPhaseCollision;

	void ProcessRemovedGeoms();

	void ProcessDisposedGeoms();

	void Add(Geom geom);

	void Update();
}
