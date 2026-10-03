using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace PhysCoop;

public class PhysCoop : Game
{
	public static GraphicsDeviceManager graphics;

	private ScreenManager screenManager;

	public PhysCoop()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected Obj, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected Obj, but got Unknown
		((Game)this).Content.RootDirectory = "Content";
		graphics = new GraphicsDeviceManager((Game)(object)this);
		graphics.PreferredBackBufferWidth = 1280;
		graphics.PreferredBackBufferHeight = 720;
		screenManager = new ScreenManager((Game)(object)this);
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)screenManager);
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)new GamerServicesComponent((Game)(object)this));
	}

	protected override void Draw(GameTime gameTime)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		graphics.GraphicsDevice.Clear(Color.Black);
		((Game)this).Draw(gameTime);
	}
}
