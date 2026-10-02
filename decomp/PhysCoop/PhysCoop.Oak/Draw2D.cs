using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysCoop.Oak;

public static class Draw2D
{
	public static void DrawLine(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 end, Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = end - start;
		int num = (int)Math.Sqrt(Math.Pow(val.X, 2.0) + Math.Pow(val.Y, 2.0));
		val.Normalize();
		float num2 = (float)Math.Atan2(val.Y, val.X);
		spriteBatch.Draw(texture, new Rectangle((int)start.X, (int)start.Y, num, 1), (Rectangle?)null, color, num2, Vector2.Zero, (SpriteEffects)0, 0f);
	}
}
