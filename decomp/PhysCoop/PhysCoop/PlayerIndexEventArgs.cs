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
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return playerIndex;
		}
	}

	public PlayerIndexEventArgs(PlayerIndex playerIndex)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		this.playerIndex = playerIndex;
	}
}
