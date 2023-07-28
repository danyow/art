using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine.Assertions;
using UnityEngine.U2D;

namespace Art.DataProvider
{
    public class SpriteBoneDataProvider: ArtDataProvider, ISpriteBoneDataProvider
    {
        public List<SpriteBone> GetBones(GUID guid)
        {
            var sprite = (ArtSpriteMetaData)dataProvider.GetSpriteData(guid);
            Assert.IsNotNull(sprite, string.Format("Sprite not found for GUID:{0}", guid.ToString()));
            return sprite.spriteBone != null ? sprite.spriteBone.ToList() : new List<SpriteBone>();
        }

        public void SetBones(GUID guid, List<SpriteBone> bones)
        {
            var sprite = dataProvider.GetSpriteDataFromAllMode(guid);
            if(sprite != null)
            {
                ((ArtSpriteMetaData)sprite).spriteBone = bones;
            }
        }
    }
}