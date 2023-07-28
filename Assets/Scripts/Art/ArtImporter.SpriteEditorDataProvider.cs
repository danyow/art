using System;
using System.Collections.Generic;
using System.Linq;
using Art.DataProvider;
using UnityEditor;
using UnityEditor.U2D.Animation;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.U2D.Animation;
using Object = UnityEngine.Object;

namespace Art
{
    public partial class ArtImporter: ISpriteEditorDataProvider
    {
        [SerializeField]
        bool m_MosaicLayers = true;

        [SerializeField]
        bool m_CharacterMode = true;

        [SerializeField]
        string m_SkeletonAssetReferenceID = null;

        // SpriteData for shared rig mode
        [SerializeField]
        List<ArtSpriteMetaData> m_SharedRigSpriteImportData = new List<ArtSpriteMetaData>();

        // SpriteData for Rig mode
        [SerializeField]
        List<ArtSpriteMetaData> m_RigSpriteImportData = new List<ArtSpriteMetaData>();

        // SpriteData for both single and multiple mode
        [SerializeField]
        List<ArtSpriteMetaData> m_SpriteImportData =
            new List<ArtSpriteMetaData>(); // we use index 0 for single sprite and the rest for multiple sprites

        // SpriteData for Mosaic mode
        [SerializeField]
        List<ArtSpriteMetaData> m_MosaicSpriteImportData = new List<ArtSpriteMetaData>();

        bool inMosaicMode => spriteImportModeToUse == SpriteImportMode.Multiple && m_MosaicLayers;

        bool inCharacterMode => inMosaicMode && m_CharacterMode;

        SkeletonAsset skeletonAsset => AssetDatabase.LoadAssetAtPath<SkeletonAsset>(AssetDatabase.GUIDToAssetPath(m_SkeletonAssetReferenceID));

        SpriteImportMode spriteImportModeToUse => m_TextureImporterSettings.textureType != TextureImporterType.Sprite ?
            SpriteImportMode.None :
            (SpriteImportMode)m_TextureImporterSettings.spriteMode;

        [SerializeField]
        TextureImporterSettings m_TextureImporterSettings = new TextureImporterSettings
        {
            mipmapEnabled = true,
            mipmapFilter = TextureImporterMipFilter.BoxFilter,
            sRGBTexture = true,
            borderMipmap = false,
            mipMapsPreserveCoverage = false,
            alphaTestReferenceValue = 0.5f,
            readable = false,

#if ENABLE_TEXTURE_STREAMING
            streamingMipmaps = true,
#endif

            fadeOut = false,
            mipmapFadeDistanceStart = 1,
            mipmapFadeDistanceEnd = 3,
            convertToNormalMap = false,
            heightmapScale = 0.25F,
            normalMapFilter = 0,
            generateCubemap = TextureImporterGenerateCubemap.AutoCubemap,
            cubemapConvolution = 0,
            seamlessCubemap = false,
            npotScale = TextureImporterNPOTScale.ToNearest,
            spriteMode = (int)SpriteImportMode.Multiple,
            spriteExtrude = 1,
            spriteMeshType = SpriteMeshType.Tight,
            spriteAlignment = (int)SpriteAlignment.Center,
            spritePivot = new Vector2(0.5f, 0.5f),
            spritePixelsPerUnit = 100.0f,
            spriteBorder = new Vector4(0.0f, 0.0f, 0.0f, 0.0f),
            alphaSource = TextureImporterAlphaSource.FromInput,
            alphaIsTransparency = true,
            spriteTessellationDetail = -1.0f,
            textureType = TextureImporterType.Sprite,
            textureShape = TextureImporterShape.Texture2D,
            filterMode = FilterMode.Bilinear,
            aniso = 1,
            mipmapBias = 0.0f,
            wrapModeU = TextureWrapMode.Repeat,
            wrapModeV = TextureWrapMode.Repeat,
            wrapModeW = TextureWrapMode.Repeat,
            swizzleR = TextureImporterSwizzle.R,
            swizzleG = TextureImporterSwizzle.G,
            swizzleB = TextureImporterSwizzle.B,
            swizzleA = TextureImporterSwizzle.A,
        };

        /// <summary>
        /// Implementation for ISpriteEditorDataProvider.GetSpriteRects.
        /// </summary>
        /// <returns>An array of SpriteRect for the current import mode.</returns>
        SpriteRect[] ISpriteEditorDataProvider.GetSpriteRects()
        {
            return GetSpriteRects();
        }

        internal SpriteRect[] GetSpriteRects()
        {
            var spriteImportData = GetSpriteImportData();
            var skip = inMosaicMode ? 0 : 1;
            return spriteImportModeToUse == SpriteImportMode.Multiple ?
                spriteImportData.Skip(skip).Select(x => new ArtSpriteMetaData(x) as SpriteRect).ToArray() :
                new SpriteRect[] { new ArtSpriteMetaData(spriteImportData[0]), };
        }

        /// <summary>
        /// Implementation for ISpriteEditorDataProvider.SetSpriteRects.
        /// </summary>
        /// <param name="spriteRects">Set the SpriteRect data for the current import mode.</param>
        void ISpriteEditorDataProvider.SetSpriteRects(SpriteRect[] spriteRects)
        {
            SetSpriteRects(spriteRects);
        }

        internal void SetSpriteRects(SpriteRect[] spriteRects)
        {
            var spriteImportData = GetSpriteImportData();
            if(spriteImportModeToUse == SpriteImportMode.Multiple)
            {
                var singleSpriteID = inMosaicMode ? new GUID() : spriteImportData[0].spriteID;
                spriteImportData.RemoveAll(
                    data => data.spriteID != singleSpriteID && spriteRects.FirstOrDefault(x => x.spriteID == data.spriteID) == null
                );
                foreach(var sr in spriteRects)
                {
                    var importData = spriteImportData.FirstOrDefault(x => x.spriteID == sr.spriteID);
                    if(importData == null)
                    {
                        spriteImportData.Add(new ArtSpriteMetaData(sr));
                    }
                    else
                    {
                        importData.name = sr.name;
                        importData.alignment = sr.alignment;
                        importData.border = sr.border;
                        importData.pivot = sr.pivot;
                        importData.rect = sr.rect;
                    }
                }
            }
            else if(spriteRects.Length == 1 &&
                    (spriteImportModeToUse == SpriteImportMode.Single || spriteImportModeToUse == SpriteImportMode.Polygon))
            {
                if(spriteImportData[0].spriteID == spriteRects[0].spriteID)
                {
                    spriteImportData[0].name = spriteRects[0].name;
                    spriteImportData[0].alignment = spriteRects[0].alignment;
                    m_TextureImporterSettings.spriteAlignment = (int)spriteRects[0].alignment;
                    m_TextureImporterSettings.spriteBorder = spriteImportData[0].border = spriteRects[0].border;
                    m_TextureImporterSettings.spritePivot = spriteImportData[0].pivot = spriteRects[0].pivot;
                    spriteImportData[0].rect = spriteRects[0].rect;
                }
                else
                {
                    spriteImportData[0] = new ArtSpriteMetaData(spriteRects[0]);
                }
            }
        }

        void ISpriteEditorDataProvider.Apply()
        {
            Apply();
        }

        /// <summary>
        /// Implementation for ISpriteEditorDataProvider.Apply.
        /// </summary>
        internal void Apply()
        {
            // Do this so that asset change save dialog will not show
            var originalValue = EditorPrefs.GetBool("VerifySavingAssets", false);
            EditorPrefs.SetBool("VerifySavingAssets", false);
            AssetDatabase.ForceReserializeAssets(new[] { assetPath }, ForceReserializeAssetsOptions.ReserializeMetadata);
            EditorPrefs.SetBool("VerifySavingAssets", originalValue);
        }

        /// <summary>
        /// Implementation for ISpriteEditorDataProvider.InitSpriteEditorDataProvider.
        /// </summary>
        void ISpriteEditorDataProvider.InitSpriteEditorDataProvider()
        {
            InitSpriteEditorDataProvider();
        }

        void InitSpriteEditorDataProvider()
        {
        }

        /// <summary>
        /// Implementation for ISpriteEditorDataProvider.GetDataProvider.
        /// </summary>
        /// <typeparam name="T">Data provider type to retrieve.</typeparam>
        /// <returns></returns>
        T ISpriteEditorDataProvider.GetDataProvider<T>()
        {
            return GetDataProvider<T>();
        }

        internal T GetDataProvider<T>() where T: class
        {
            if(typeof(T) == typeof(ISpriteBoneDataProvider))
            {
                return new SpriteBoneDataProvider { dataProvider = this } as T;
            }

            if(typeof(T) == typeof(ISpriteMeshDataProvider))
            {
                return new SpriteMeshDataProvider { dataProvider = this } as T;
            }
            if(typeof(T) == typeof(ISpriteOutlineDataProvider))
            {
                return new SpriteOutlineDataProvider { dataProvider = this } as T;
            }

            if(typeof(T) == typeof(ISpritePhysicsOutlineDataProvider))
            {
                return new SpritePhysicsOutlineProvider { dataProvider = this } as T;
            }

            // if (typeof(T) == typeof(ITextureDataProvider))
            // {
            //     return new TextureDataProvider { dataProvider = this } as T;
            // }
            // if (typeof(T) == typeof(ICharacterDataProvider))
            // {
            //     return inCharacterMode ? new CharacterDataProvider { dataProvider = this } as T : null;
            // }
            if(typeof(T) == typeof(IMainSkeletonDataProvider))
            {
                return inCharacterMode && skeletonAsset != null ? new MainSkeletonDataProvider { dataProvider = this } as T : null;
            }
            if(typeof(T) == typeof(ISecondaryTextureDataProvider))
            {
                return new SecondaryTextureDataProvider { dataProvider = this } as T;
            }
            return this as T;
        }

        /// <summary>
        /// Implementation for ISpriteEditorDataProvider.HasDataProvider.
        /// </summary>
        /// <param name="type">Data provider type to query.</param>
        /// <returns>True if data provider is supported, false otherwise.</returns>
        bool ISpriteEditorDataProvider.HasDataProvider(Type type)
        {
            return HasDataProvider(type);
        }

        internal bool HasDataProvider(Type type)
        {
            if(inCharacterMode)
            {
                if(type == typeof(ICharacterDataProvider))
                {
                    return true;
                }
                if(type == typeof(IMainSkeletonDataProvider) && skeletonAsset != null)
                {
                    return true;
                }
            }
            if(type == typeof(ISpriteBoneDataProvider) ||
               type == typeof(ISpriteMeshDataProvider) ||
               type == typeof(ISpriteOutlineDataProvider) ||
               type == typeof(ISpritePhysicsOutlineDataProvider) ||
               type == typeof(ITextureDataProvider) ||
               type == typeof(ISecondaryTextureDataProvider))
            {
                return true;
            }
            else
            {
                return type.IsAssignableFrom(GetType());
            }
        }

        List<ArtSpriteMetaData> GetSpriteImportData()
        {
            if(inMosaicMode)
            {
                if(inCharacterMode)
                {
                    if(skeletonAsset != null)
                    {
                        return m_SharedRigSpriteImportData;
                    }
                    return m_RigSpriteImportData;
                }
                return m_MosaicSpriteImportData;
            }
            return m_SpriteImportData;
        }

        SpriteImportMode ISpriteEditorDataProvider.spriteImportMode => spriteImportModeToUse;
        Object ISpriteEditorDataProvider.targetObject => targetObject;
        internal Object targetObject => this;

        /// <summary>
        /// Implementation for ISpriteEditorDataProvider.pixelsPerUnit.
        /// </summary>
        float ISpriteEditorDataProvider.pixelsPerUnit => pixelsPerUnit;

        internal float pixelsPerUnit => m_TextureImporterSettings.spritePixelsPerUnit;
    }
}