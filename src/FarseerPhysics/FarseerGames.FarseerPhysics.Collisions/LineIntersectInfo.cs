using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

public class LineIntersectInfo
{
	private Geom _geom;

	private List<Vector2> _points;

	public List<Vector2> Points => _points;

	public Geom Geom => _geom;

	public LineIntersectInfo(Geom geom, List<Vector2> points)
	{
		_geom = geom;
		_points = points;
	}
}
