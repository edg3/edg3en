# edg3en
Building an Engine wrapper on 'FNA-XNA/FNA'; aim to use Vulkan through it using 'thatcosmonaut/FNA3D'

# Links
- FNA: https://github.com/FNA-XNA/FNA
- FNA Wiki: https://github.com/FNA-XNA/FNA/wiki

# Building
- First version based on needs to build 'Explosive Hamster' (SokoBomber's successor)
- The game will stay private, but I want to make my own idea of a full Game Engine for things such as GGJ, or games I feel like making

# How To
- Get FNA ready with a base project
- Add reference to Edg3en
- Create Instance
```
class ExplosiveHamsterGame : Game
{
	/// <summary>
	/// Engine instance
	/// </summary>
	public static Engine Engine { get; private set; } = null;

	[STAThread]
	static void Main(string[] args)
	{
		using (var g = new ExplosiveHamsterGame())
		{
			g.Run();
		}
	}

	private ExplosiveHamsterGame()
	{
		if (null != Engine) throw new Exception();

		GraphicsDeviceManager gdm = new GraphicsDeviceManager(this);
		Engine = new Engine(this, gdm, Content, 1280, 720, false, true, Color.Black);
		Engine.AddState(new MenuState(Engine.Content));
		Engine.ShowMousePointer();
	}

	protected override void Initialize()
	{
		Engine.Initialize(Content);
		base.Initialize();
	}

	protected override void LoadContent()
	{
		Engine.LoadContent();
		base.LoadContent();
	}

	protected override void UnloadContent()
	{
		Engine.UnloadContent();
		base.UnloadContent();
	}

	protected override void Update(GameTime gameTime)
	{
		Engine.Update(gameTime);
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		Engine.Draw(gameTime, GraphicsDevice);
		base.Draw(gameTime);
	}
}
```
- Create an IGameState
```
using Edg3en;
using Microsoft.Xna.Framework.Graphics;

namespace ExplosiveHamster.GameStates
{
    public class MenuState : IGameState
    {
        public MenuState(Content content) : base("Menu", content)
        {
        }

        public override void Draw()
        {
            Engine.I.SpriteBatch.Draw(sample, new Microsoft.Xna.Framework.Rectangle(0,0,1156,816), Microsoft.Xna.Framework.Color.White);
        }

        Texture2D sample;

        public override void Load()
        {
            sample = Content.Manager.Load<Texture2D>("Content/input.tilemap");

            Loaded = true;
        }

        public override bool Update()
        {
            if (Engine.I.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape)) return false;

            return true;
        }
    }
}
```

# Idea
Felt like sticking to C# after tons of experimenting with Vulkan in C++; stumbled onto 'FNA-XNA' and have been reading through tons shared on their discord.