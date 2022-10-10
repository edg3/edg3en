using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Edg3en.Structures;

public class ETexture2DTilesheet
{
    private Texture2D Tileset { get; set; }
    public int Size_X { get; private set; }
    public int Size_Y { get; private set; }

    public int Max_X { get; private set; }
    public int Max_Y { get; private set; }

    public ETexture2DTilesheet(Texture2D texture, int size_x, int size_y)
    {
        Tileset = texture;
        Size_X = size_x;
        Size_Y = size_y;

        Color = Color.White;
        Color_Hover = Color.White;

        Max_X = texture.Width / size_x;
        Max_Y = texture.Height / size_y;
    }

    public Color Color { get; set; }
    public Color Color_Hover { get; set; }

    #region ASSISTANT CODE
    public bool ContainsMouse(Rectangle target)
    {
        return Engine.I.RectContains(target, Engine.I.Mouse_X, Engine.I.Mouse_Y);
    }

    public void SetColor(Color c)
    {
        Color = c;
    }

    public void SetColor_Hover(Color c)
    {
        Color_Hover = c;
    }

    public void Render(Rectangle target, int offset_x, int offset_y)
    {
        if (offset_x < 0 || offset_y < 0 || offset_x >= Max_X || offset_y >= Max_Y) return;   
        Engine.I.SpriteBatch.Draw(Tileset, target, new Rectangle(offset_x * Size_X, offset_y * Size_Y, Size_X, Size_Y), Color.White);
    }

    public void RenderIfInWindow(Rectangle target, int offset_x, int offset_y)
    {
        if (offset_x < 0 || offset_y < 0 || offset_x >= Max_X || offset_y >= Max_Y) return;   
        if (InWindow(target))
            Engine.I.SpriteBatch.Draw(Tileset, target, new Rectangle(offset_x * Size_X, offset_y * Size_Y, Size_X, Size_Y), Color.White);
    }

    public bool InWindow(Rectangle target)
    {
        var bounds = Engine.I.Game.GraphicsDevice.PresentationParameters.Bounds;
        return
            bounds.Contains(target.X, target.Y) ||
            bounds.Contains(target.X + target.Width, target.Y) ||
            bounds.Contains(target.X, target.Y + target.Height) ||
            bounds.Contains(target.X + target.Width, target.Y + target.Height);
    }

    // TODO: see ETexture2D end TODO
    #endregion

}
