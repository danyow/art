using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.Assertions;

namespace Art.DataProvider
{
    public class SpriteOutlineDataProvider: ArtDataProvider, ISpriteOutlineDataProvider
    {
        public List<Vector2[]> GetOutlines(GUID guid)
        {
            var sprite = (ArtSpriteMetaData)dataProvider.GetSpriteData(guid);
            Assert.IsNotNull(sprite, string.Format("Sprite not found for GUID:{0}", guid.ToString()));

            var outline = sprite.spriteOutline;
            return outline != null ? outline.Select(x => x.outline).ToList() : new List<Vector2[]>();
        }

        public void SetOutlines(GUID guid, List<Vector2[]> data)
        {
            var sprite = dataProvider.GetSpriteDataFromAllMode(guid);
            if(sprite != null)
            {
                ((ArtSpriteMetaData)sprite).spriteOutline = data.Select(x => new ArtSpriteOutline() { outline = x }).ToList();
            }
        }

        public float GetTessellationDetail(GUID guid)
        {
            return ((ArtSpriteMetaData)dataProvider.GetSpriteData(guid)).tessellationDetail;
        }

        public void SetTessellationDetail(GUID guid, float value)
        {
            var sprite = dataProvider.GetSpriteDataFromAllMode(guid);
            if(sprite != null)
            {
                ((ArtSpriteMetaData)sprite).tessellationDetail = value;
            }
        }
    }
}