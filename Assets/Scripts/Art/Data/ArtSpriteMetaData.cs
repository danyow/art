using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.U2D;

namespace Art
{
    public class ArtSpriteMetaData: SpriteRect
    {
        public List<SpriteBone> spriteBone;
        public List<ArtSpriteOutline> spriteOutline;
        public List<Vertex2DMetaData> vertices;
        public List<ArtSpriteOutline> spritePhysicsOutline;
        public int[] indices;
        public Vector2Int[] edges;
        public float tessellationDetail;
        public Vector2Int uvTransform = Vector2Int.zero;
        public Vector2 spritePosition;

        public ArtSpriteMetaData()
        {
        }

        public ArtSpriteMetaData(SpriteRect sr)
        {
            alignment = sr.alignment;
            border = sr.border;
            name = sr.name;
            pivot = GetPivotValue(sr.alignment, sr.pivot);
            rect = sr.rect;
            spriteID = sr.spriteID;
        }

        public static Vector2 GetPivotValue(SpriteAlignment alignment, Vector2 customOffset)
        {
            return alignment switch
            {
                SpriteAlignment.BottomLeft   => new Vector2(0f, 0f),
                SpriteAlignment.BottomCenter => new Vector2(0.5f, 0f),
                SpriteAlignment.BottomRight  => new Vector2(1f, 0f),
                SpriteAlignment.LeftCenter   => new Vector2(0f, 0.5f),
                SpriteAlignment.Center       => new Vector2(0.5f, 0.5f),
                SpriteAlignment.RightCenter  => new Vector2(1f, 0.5f),
                SpriteAlignment.TopLeft      => new Vector2(0f, 1f),
                SpriteAlignment.TopCenter    => new Vector2(0.5f, 1f),
                SpriteAlignment.TopRight     => new Vector2(1f, 1f),
                SpriteAlignment.Custom       => customOffset,
                _                            => Vector2.zero,
            };
        }

        public static implicit operator UnityEditor.AssetImporters.SpriteImportData(ArtSpriteMetaData value)
        {
            var output = new UnityEditor.AssetImporters.SpriteImportData
            {
                name = value.name,
                alignment = value.alignment,
                rect = value.rect,
                border = value.border,
                pivot = value.pivot,
                tessellationDetail = value.tessellationDetail,
                spriteID = value.spriteID.ToString(),
            };
            if(value.spriteOutline != null)
            {
                output.outline = value.spriteOutline.Select(x => x.outline).ToList();
            }

            return output;
        }
    }
}