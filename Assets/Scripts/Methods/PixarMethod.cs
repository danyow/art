using Art.Data;
using UnityEngine;

namespace Art.Methods
{
    public enum MethodType
    {
        DiffuseMap,
        NormalMap,
    }

    [RequireComponent(typeof(ArtHelper))]
    public abstract class PixarMethod: MonoBehaviour
    {
        public const int kDefaultAlphaCut = 10;

        public MethodType methodType;

        public abstract Texture2D Process(PixelatedIn input);
    }
}
