using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Edg3en;

public class Engine
{
    // Instance of Engine
    public static Engine I { get; private set; } = null;

    // Graphics device manager
    private GraphicsDeviceManager _gdm { get; set; }

    // If we need to reference the game directly anywhere
    public Game Game { get; private set; }

    // For content management
    public Content Content { get; private set; }

    // So states can even change it if I want
    public Color BackgroundColor { get; set; } = Color.CornflowerBlue;

    // Keyboard and Mouse States
    KeyboardState KB_Previous = Keyboard.GetState();
    KeyboardState KB_Current = Keyboard.GetState();
    MouseState M_Previous = Mouse.GetState();
    MouseState M_Current = Mouse.GetState();

    // Mouse Pointer
    public bool Mouse_ShowPointer { get; set; }

    // SpriteBatch
    public SpriteBatch SpriteBatch { get; private set; } = null;

    // Start with defaults
    public Engine(Game game, GraphicsDeviceManager gdm, ContentManager cm)
    {
        if (null != I) throw new Exception("Can only run one engine at a time.");
        I = this;

        _gdm = gdm;
        Content = new Content(cm);

        gdm.PreferredBackBufferWidth = 1280;
        gdm.PreferredBackBufferHeight = 720;
        gdm.IsFullScreen = false;
        gdm.SynchronizeWithVerticalRetrace = true;

        Game = game;
    }

    // Start with specified values
    public Engine(Game game, GraphicsDeviceManager gdm, ContentManager cm, int width, int height, bool fullscreen, bool syncVertical, Color backgroundColor)
    {
        if (null != I) throw new Exception("Can only run one engine at a time.");
        I = this;

        _gdm = gdm;
        Content = new Content(cm);

        gdm.PreferredBackBufferWidth = width;
        gdm.PreferredBackBufferHeight = height;
        gdm.IsFullScreen = fullscreen;
        gdm.SynchronizeWithVerticalRetrace = syncVertical;

        BackgroundColor = backgroundColor;

        Game = game;
    }

    public void Initialize(ContentManager manager)
    {
        Content = new Content(manager);
        // Get First Game State setup
    }

    public void LoadContent()
    {
        SpriteBatch = new SpriteBatch(_gdm.GraphicsDevice);

        Content.Mouse_Pointer = new Texture2D(_gdm.GraphicsDevice, 1, 1);
        Rectangle r = new Rectangle(0, 0, 1, 1);
        Color[] c = new Color[1];
        c[0] = Color.Red;
        Content.Mouse_Pointer.SetData<Color>(c, 0, 1);

        // Load first gamestate content - maybe?
    }

    public void UnloadContent()
    {
        // TODO : Create content management service
        //         - track EVERY gamestate that isn't destroyed that uses it; if there is no longer a game state that has an item it can remove it from memory
        //         - Check memory refreshing things to be a cool guy
        // Clean up all remaining content in memory
        // TODO : Think about: Unloading 'main menu' assets from memory as a flag on game state then when we go back to it we reload them - less memory use; could be cool and useful
    }

    public void Update(GameTime gameTime)
    {
        // Input Wrapping
        KB_Previous = KB_Current;
        KB_Current = Keyboard.GetState();
        M_Previous = M_Current;
        M_Current = Mouse.GetState();

        // If no states left - Exit the game
        if (_states.Count == 0) Game?.Exit(); /* works approximately */
        else
        {
            var lastState = _states.Last();
            if (lastState.Loaded)
            {
                // False means 'exit state'
                if (!lastState.Update())
                {
                    // This is possibly lazy state - you as a user needs to 
                    _states.Remove(lastState);
                    // TODO: clean up memory using
                    // totally_cleaning_memory(lastState.Name);
                }
            }
            else
            {
                // TODO: Loading screen vibes - launch load thread
                lastState.Load();
            }
        }
    }

    private Rectangle Mouse_PointerPos { get; set; } = new Rectangle(0, 0, 1, 1);
    public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
    {
        if (null == graphicsDevice) return; /* Hmm... This calls like update when it isn't ready to start */

        // Render current state
        // TODO : Work out 'loading screen state' which can be first state in memory?
        // TODO : Work out 'popup state' to throw in where it updates the popup, draws the current state, and 
        graphicsDevice.Clear(BackgroundColor);
        SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

        // Thought: 2D vs 3D? Can I still interweave the 2D like this to show 'in front'?
        if (_states.Count > 0)
        {
            var lastState = _states.Last();
            if (lastState.Loaded)
            {
                lastState.Draw();
            }
        }

        if (Mouse_PointerVisible)
        {
            SpriteBatch.Draw(Content.Mouse_Pointer, new Rectangle(M_Current.X, M_Current.Y, 2, 2), Color.White);
        }

        SpriteBatch.End();
    }

    private List<IGameState> _states = new List<IGameState>();

    public void AddState(IGameState state)
    {
        _states.Add(state);
    }

    public void RemoveState()
    {
        _states.RemoveAt(_states.Count - 1);
    }

    // TODO: consider - maybe 'remove state named' ?

    // TODO: Revisit all input below and double check logic
    public bool IsKeyPressed(Keys k)
    {
        return KB_Current.IsKeyDown(k) && KB_Previous.IsKeyUp(k);
    }

    public bool IsKeyDown(Keys k)
    {
        return KB_Current.IsKeyDown(k);
    }

    // TODO: Mouse Scroll
    public bool IsMouseClicked(MouseButtons mb)
    {
        switch (mb)
        {
            case MouseButtons.Left: return M_Current.LeftButton == ButtonState.Pressed && M_Previous.LeftButton == ButtonState.Released;
            case MouseButtons.Middle: return M_Current.MiddleButton == ButtonState.Pressed && M_Previous.MiddleButton == ButtonState.Released;
            case MouseButtons.Right: return M_Current.RightButton == ButtonState.Pressed && M_Previous.RightButton == ButtonState.Released;
        }
        return false;
    }

    public bool IsMouseDown(MouseButtons mb)
    {
        switch (mb)
        {
            case MouseButtons.Left: return M_Current.LeftButton == ButtonState.Pressed;
            case MouseButtons.Middle: return M_Current.MiddleButton == ButtonState.Pressed;
            case MouseButtons.Right: return M_Current.RightButton == ButtonState.Pressed;
        }
        return false;
    }

    // TODO: Gamepad

    public bool Mouse_PointerVisible { get; set; } = false;
    public void ShowMousePointer()
    {
        Mouse_PointerVisible = true;
    }
}

public enum MouseButtons
{
    // TODO: Extra buttons, perhaps?
    Left,
    Middle,
    Right
}
