using System;
using Microsoft.Xna.Framework;

namespace PhysCoop;

internal class PlayerIndexEventArgs : EventArgs
{
	private PlayerIndex playerIndex;

	public PlayerIndex PlayerIndex
	{
		get
		{
			return playerIndex;
		}
	}

	public PlayerIndexEventArgs(PlayerIndex playerIndex)
	{
		this.playerIndex = playerIndex;
	}
}
