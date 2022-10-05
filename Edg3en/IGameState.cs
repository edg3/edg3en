namespace Edg3en;

public abstract class IGameState
{
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
}
