using System.Linq;
using UnityEngine;

/// <summary>
/// Loads sliced sprite frames from animation strip textures in Resources/Animations.
/// </summary>
public static class SpriteAnimationLibrary
{
    public static Sprite[] LoadStrip(string resourcePath)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(resourcePath);
        if (sprites == null || sprites.Length == 0) return null;

        return sprites.OrderBy(s => s.name, new FrameNameComparer()).ToArray();
    }

    private class FrameNameComparer : System.Collections.Generic.IComparer<string>
    {
        public int Compare(string a, string b)
        {
            int ai = ExtractIndex(a);
            int bi = ExtractIndex(b);
            return ai.CompareTo(bi);
        }

        private static int ExtractIndex(string name)
        {
            if (string.IsNullOrEmpty(name)) return 0;

            int underscore = name.LastIndexOf('_');
            if (underscore >= 0 && int.TryParse(name.Substring(underscore + 1), out int index))
            {
                return index;
            }

            return 0;
        }
    }
}
