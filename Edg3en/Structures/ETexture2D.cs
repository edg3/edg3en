using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Edg3en.Structures;

public class ETexture2D
{
    // TODO: Work out every idea I have to add in to this
    public Texture2D Texture { get; set; }
    public ETexture2D(Texture2D texture)
    {
        Texture = texture;
        Target = new Rectangle(0, 0, Texture.Width, Texture.Height);
        Target_Hover = new Rectangle(0, 0, Texture.Width, Texture.Height);
        Color = Color.White;
        Color = Color.White;
    }

    public Rectangle Target { get; set; }
    public Rectangle Target_Hover { get; set; }
    public Rectangle GetTarget()
    {
        if (Engine.I.MouseContained(Target)) return Target_Hover;

        return Target;
    }

    public Color Color { get; set; }
    public Color Color_Hover { get; set; }
    public Color GetColor()
    {
        if (Engine.I.MouseContained(Target)) return Color_Hover;

        return Color;
    }

    #region ASSISTANT CODE
    public bool ContainsMouse()
    {
        return Engine.I.RectContains(Target, Engine.I.Mouse_X, Engine.I.Mouse_Y);
    }

    public void SetTarget(int x, int y)
    {
        Target = new Rectangle(x, y, Target.Width, Target.Height);
    }

    public void SetTarget(int x, int y, int w, int h)
    {
        Target = new Rectangle(x, y, w, h);
    }

    public void SetTarget_Hover(int x, int y)
    {
        Target_Hover = new Rectangle(x, y, Target.Width, Target.Height);
    }

    public void SetTarget_Hover(int x, int y, int w, int h)
    {
        Target_Hover = new Rectangle(x, y, w, h);
    }

    public void SetColor(Color c)
    {
        Color = c;
    }

    public void SetColor_Hover(Color c)
    {
        Color_Hover = c;
    }

    // Idea was simple:
    // - will most likely only need this logic setup for UI in 3D games
    // - chances are it could be 'bundled functions' above as well
    //    - bundled meaning: 'set target AND set targetHover' as 1 line => readable code; just need to consider it more
    public void Render()
    {
        Engine.I.SpriteBatch.Draw(Texture, GetTarget(), GetColor());
    }

    public void RenderIfInWindow()
    {
        if (InWindow())
            Engine.I.SpriteBatch.Draw(Texture, GetTarget(), GetColor());
    }

    public bool InWindow()
    {
        var bounds = Engine.I.Game.GraphicsDevice.PresentationParameters.Bounds;
        return
            bounds.Contains(Target.X, Target.Y) ||
            bounds.Contains(Target.X + Target.Width, Target.Y) ||
            bounds.Contains(Target.X, Target.Y + Target.Height) ||
            bounds.Contains(Target.X + Target.Width, Target.Y + Target.Height);
    }

    // TODO: Workout and consider 'offset animations' for 2D 'worlds'
    //        - As in, 'player moves 1 down', 'logic paused for offset of -32', 'player stays in fixed pos but its offset -32 ticks moves world around backwards till in state teritory'... (wording may be confusing)
    #endregion

}
