using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Edg3en;

public class Content
{
    public ContentManager Manager { get; private set; }
    public Content(ContentManager manager)
    {
        Manager = manager;
    }

    public Texture2D Mouse_Pointer { get; set; }
}
