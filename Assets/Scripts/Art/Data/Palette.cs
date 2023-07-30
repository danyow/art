using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Art.Data
{
    [CreateAssetMenu(fileName = "New ColorPalette", menuName = "ColorPalette", order = 1)]
    public class Palette: ScriptableObject
    {
        public WeightedColor[] weightedColors;

        public Color32[] colors{ get { return weightedColors.Select(wColor => wColor.color).ToArray(); } }

        public Dictionary<Color32, float> weightTable{ get { return weightedColors.ToDictionary(itm => itm.color, itm => itm.weight); } }

        public int colorCount => weightedColors.Length;
    }
}
