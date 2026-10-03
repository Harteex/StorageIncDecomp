using System.Collections.Generic;

namespace FarseerGames.FarseerPhysics.Controllers;

public class ControllerList : List<Controller>
{
	public delegate void ContentsChangedEventHandler(Controller controller);

	private List<Controller> _markedForRemovalList = new List<Controller>();

	public ContentsChangedEventHandler Added;

	public ContentsChangedEventHandler Removed;

	public new void Add(Controller controller)
	{
		base.Add(controller);
		if (Added != null)
		{
			Added(controller);
		}
	}

	public new void Remove(Controller controller)
	{
		base.Remove(controller);
		if (Removed != null)
		{
			Removed(controller);
		}
	}

	public void RemoveDisposed()
	{
		for (int i = 0; i < Count; i++)
		{
			if (IsDisposed(base[i]))
			{
				_markedForRemovalList.Add(base[i]);
			}
		}
		for (int j = 0; j < _markedForRemovalList.Count; j++)
		{
			Remove(_markedForRemovalList[j]);
		}
		_markedForRemovalList.Clear();
	}

	internal static bool IsDisposed(Controller controller)
	{
		return controller.IsDisposed;
	}
}
