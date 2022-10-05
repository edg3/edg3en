using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

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

    public void Clean(string gamestateName)
    {
        foreach (var k1 in GamestateAssetsPairs.Keys)
        {
            if (GamestateAssetsPairs[k1].Contains(gamestateName))
            {
                GamestateAssetsPairs[k1].Remove(gamestateName);
                if (GamestateAssetsPairs[k1].Count == 0)
                {
                    Assets.Remove(k1);
                    // TODO: Check what we need to specify for memory cleaning
                }
            }
        }
    }
}
