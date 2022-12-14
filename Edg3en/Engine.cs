using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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

    // Mouse locations
    public int Mouse_X => M_Current.X;
    public int Mouse_Y => M_Current.Y;

    // SpriteBatch
    public SpriteBatch SpriteBatch { get; private set; } = null;

    // Loading screen
    public IGameState LoadingState { get; private set; } = null;

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

    // Need to think this through a little - might need a different wrapping
    public int Window_Width => _gdm.PreferredBackBufferWidth;
    public int Window_Height => _gdm.PreferredBackBufferHeight;

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

    ~Engine()
    {
        SpriteBatch?.Dispose();
    }

    public void LoadContent(bool SettingPointerMyself)
    {
        SpriteBatch = new SpriteBatch(_gdm.GraphicsDevice);

        if (!SettingPointerMyself)
        {
            // Make a small red pointer dot if we dont set one afterwards
            Content.Mouse_Pointer = new Texture2D(_gdm.GraphicsDevice, 1, 1);
            Rectangle r = new Rectangle(0, 0, 1, 1);
            Color[] c = new Color[1];
            c[0] = Color.Red;
            Content.Mouse_Pointer.SetData<Color>(c, 0, 1);
        }

        // Load first gamestate content - maybe?
    }

    public void UnloadContent()
    {
        if (null != LoadingState) Content.Clean(LoadingState.Name);
    }

    Thread LoadingThread { get; set; } = null;
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
                    // TODO: double check logical order of load - I believe this hits before the content starts to 'Load'?
                    Thread cleanUp = new Thread(() =>
                    {
                        Thread.Sleep(1000); /* I believe 1 second should be enough for loading? like, load the reused first? */
                        Content.Clean(lastState.Name);
                    });
                    cleanUp.Start();
                }
            }
            else
            {
                LoadingState?.Update();
                if (null == LoadingThread)
                {
                    LoadingThread = new Thread(() =>
                    {
                        lastState.Load();
                        var instance = LoadingThread;
                        LoadingThread = null;
                        instance.Abort();
                    });
                    LoadingThread.Start();
                }
            }
        }
    }

    private Rectangle Mouse_PointerPos { get; set; } = new Rectangle(0, 0, 1, 1);
    public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
    {
        if (null == graphicsDevice) return; /* Hmm... This calls like update when it isn't ready to start */

        // Render current state
        // TODO : Work out 'popup state' to throw in where it updates the popup, draws the current state, and 
        graphicsDevice.Clear(BackgroundColor);
        SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

        // Thought: 2D vs 3D? Can I still interweave the 2D like this to show 'in front'?
        if (_states.Count > 0)
        {
            var lastState = _states.Last();
            if (lastState.Loaded)
            {
                lastState.Draw();
            }
            else
            {
                LoadingState?.Draw();
            }
        }

        if (Mouse_PointerVisible)
        {
            SpriteBatch.Draw(Content.Mouse_Pointer, new Rectangle(M_Current.X, M_Current.Y, Content.Mouse_Pointer.Width, Content.Mouse_Pointer.Height), Color.White);
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

    // TODO: Revisit all input below and double check logic
    public bool IsKeyPressed(Keys k)
    {
        return KB_Current.IsKeyDown(k) && KB_Previous.IsKeyUp(k);
    }

    public bool IsKeyDown(Keys k)
    {
        return KB_Current.IsKeyDown(k);
    }

    public bool IsMouseScrollUp()
    {
        return M_Current.ScrollWheelValue > M_Previous.ScrollWheelValue;
    }

    public bool IsMouseScrolllDown()
    {
        return M_Current.ScrollWheelValue < M_Previous.ScrollWheelValue;
    }

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

    public bool Mouse_PointerVisible { get; private set; } = false;
    public void ShowMousePointer()
    {
        Mouse_PointerVisible = true;
    }

    public bool RectContains(Rectangle rect, int x, int y)
    {
        return
            rect.X <= x &&
            rect.Y <= y &&
            rect.X + rect.Width >= x &&
            rect.Y + rect.Height >= y;
    }

    public bool MouseContained(Rectangle target)
    {
        return RectContains(target, Mouse_X, Mouse_Y);
    }

    public void SetLoadingState(IGameState loadState)
    {
        LoadingState = loadState;
        loadState.Load();
    }
}

public enum MouseButtons
{
    // TODO: Extra buttons, perhaps?
    Left,
    Middle,
    Right
}
