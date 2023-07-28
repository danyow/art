using System;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.Assertions;

namespace Art.DataProvider
{
    public class SpriteMeshDataProvider: ArtDataProvider, ISpriteMeshDataProvider
    {
        public Vertex2DMetaData[] GetVertices(GUID guid)
        {
            var sprite = (ArtSpriteMetaData)dataProvider.GetSpriteData(guid);
            Assert.IsNotNull(sprite, string.Format("Sprite not found for GUID:{0}", guid.ToString()));
            var v = sprite.vertices;
            return v != null ? v.ToArray() : new Vertex2DMetaData[0];
        }

        public void SetVertices(GUID guid, Vertex2DMetaData[] vertices)
        {
            var sprite = dataProvider.GetSpriteDataFromAllMode(guid);
            if(sprite != null)
            {
                ((ArtSpriteMetaData)sprite).vertices = vertices.ToList();
            }
        }

        public int[] GetIndices(GUID guid)
        {
            var sprite = (ArtSpriteMetaData)dataProvider.GetSpriteData(guid);
            Assert.IsNotNull(sprite, string.Format("Sprite not found for GUID:{0}", guid.ToString()));
            var v = sprite.indices;
            return v ?? Array.Empty<int>();
        }

        public void SetIndices(GUID guid, int[] indices)
        {
            var sprite = dataProvider.GetSpriteDataFromAllMode(guid);
            if(sprite != null)
            {
                ((ArtSpriteMetaData)sprite).indices = indices;
            }
        }

        public Vector2Int[] GetEdges(GUID guid)
        {
            var sprite = (ArtSpriteMetaData)dataProvider.GetSpriteData(guid);
            Assert.IsNotNull(sprite, string.Format("Sprite not found for GUID:{0}", guid.ToString()));
            var v = sprite.edges;
            return v ?? Array.Empty<Vector2Int>();
        }

        public void SetEdges(GUID guid, Vector2Int[] edges)
        {
            var sprite = dataProvider.GetSpriteDataFromAllMode(guid);
            if(sprite != null)
            {
                ((ArtSpriteMetaData)sprite).edges = edges;
            }
        }
    }
}