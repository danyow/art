using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace Art.DataProvider
{
    public class SecondaryTextureDataProvider: ArtDataProvider, ISecondaryTextureDataProvider
    {
        public SecondarySpriteTexture[] textures{ get => dataProvider.secondaryTextures; set => dataProvider.secondaryTextures = value; }
    }
}