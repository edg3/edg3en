using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Edg3en;

public class Content
{
    public ContentManager Manager { get; private set; }
    public Content(ContentManager manager)
    {
        Manager = manager;
    }

    public Texture2D Mouse_Pointer { get; set; }

    Dictionary<string, List<string>> GamestateAssetsPairs = new Dictionary<string, List<string>>();
    Dictionary<string, object> Assets = new Dictionary<string, object>();

    public Texture2D GetTex2D(string gamestateName, string name)
    {
        if (GamestateAssetsPairs.ContainsKey(name))
        {
            if (!GamestateAssetsPairs[name].Contains(gamestateName))
                GamestateAssetsPairs[name].Add(gamestateName);
        }
        else
        {
            GamestateAssetsPairs.Add(name, new List<string>() { gamestateName });
        }

        if (Assets.ContainsKey(name))
        {
            if (Assets[name] != null)
                return Assets[name] as Texture2D;
        }

        Assets.Add(name, Manager.Load<Texture2D>(name));
        return Assets[name] as Texture2D;
    }

    public SpriteFont GetFont(string gamestateName, string name)
    {
        if (GamestateAssetsPairs.ContainsKey(name))
        {
            if (!GamestateAssetsPairs[name].Contains(gamestateName))
                GamestateAssetsPairs[name].Add(gamestateName);
        }
        else
        {
            GamestateAssetsPairs.Add(name, new List<string>() { gamestateName });
        }

        if (Assets.ContainsKey(name))
        {
            if (Assets[name] != null)
                return Assets[name] as SpriteFont;
        }

        Assets.Add(name, Manager.Load<SpriteFont>(name));
        return Assets[name] as SpriteFont;
    }

    public SoundEffect GetSoundEffect(string gamestateName, string name)
    {
        if (GamestateAssetsPairs.ContainsKey(name))
        {
            if (!GamestateAssetsPairs[name].Contains(gamestateName))
                GamestateAssetsPairs[name].Add(gamestateName);
        }
        else
        {
            GamestateAssetsPairs.Add(name, new List<string>() { gamestateName });
        }

        if (Assets.ContainsKey(name))
        {
            if (Assets[name] != null)
                return Assets[name] as SoundEffect;
        }

        Assets.Add(name, Manager.Load<SpriteFont>(name));
        return Assets[name] as SoundEffect;
    }

    public void Clean(string gamestateName)
    {
        foreach (var k1 in GamestateAssetsPairs.Keys)
        {
            if (GamestateAssetsPairs[k1].Contains(gamestateName))
            {
                GamestateAssetsPairs[k1].Remove(gamestateName);
                if (GamestateAssetsPairs[k1].Count == 0)
                {
                    var removed = Assets[k1];
                    Assets.Remove(k1);
                    var t = removed.GetType().ToString().Split('.').Last();
                    switch (t)
                    {
                        case "Texture2D":
                            (removed as Texture2D).Dispose();
                            break;
                        case "SpriteFont":
                            /* (removed as SpriteFont).Dispose doesn't exist; might need to do something else */
                            break;
                        case "SoundEffect":
                            (removed as SoundEffect).Dispose();
                            break;
                    }
                }
            }
        }
    }

    ~Content()
    {
        // Assumed Windows memory management app close will suffice
        GamestateAssetsPairs.Clear();
        Assets.Clear();
    }
}
