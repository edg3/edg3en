namespace Edg3en;

public abstract class IGameState
{
    // Cleaning referencing code a little
    public Engine E => Engine.I;

    // Name => 'Menu' => 'content loaded bound to Menu' in 'ContentManager'
    public bool Loaded { get; set; } // Set to true at end of load
    public string Name { get; private set; }
    public Content Content { get; private set; }
    public IGameState(string name, Content content)
    {
        Name = name;
        this.Content = content;
    }

    public abstract void Load();
    public abstract void Draw();
    /// <summary>
    /// Update the game state, return bool
    /// - true => running
    /// - false => stopped
    /// </summary>
    /// <returns></returns>
    public abstract bool Update();

    // TODO: Think about it, 'unload' => 'new state added' -> sees it needs load -> it loads so it adds content tracking -> unloads previous state as new state will be shown
}
