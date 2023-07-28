using System.Linq;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.U2D;

namespace Art
{
    // [ScriptedImporter(22200003, new[] { "png", }, AllowCaching = true)]
    public partial class ArtImporter: ScriptedImporter
    {
        [SerializeField]
        private SecondarySpriteTexture[] m_SecondarySpriteTextures;

        internal SpriteBone[] mainSkeletonBones
        {
            get
            {
                var skeleton = skeletonAsset;
                return skeleton != null ? skeleton.GetSpriteBones() : null;
            }
        }

        internal SecondarySpriteTexture[] secondaryTextures{ get => m_SecondarySpriteTextures; set => m_SecondarySpriteTextures = value; }

        internal SpriteRect GetSpriteData(GUID guid)
        {
            var spriteImportData = GetSpriteImportData();
            var skip = inMosaicMode ? 0 : 1;
            return spriteImportModeToUse == SpriteImportMode.Multiple ?
                spriteImportData.Skip(skip).FirstOrDefault(x => x.spriteID == guid) :
                spriteImportData[0];
        }

        internal SpriteRect GetSpriteDataFromAllMode(GUID guid)
        {
            var spriteMetaData = ((m_RigSpriteImportData.FirstOrDefault(x => x.spriteID == guid) ??
                                   m_SharedRigSpriteImportData.FirstOrDefault(x => x.spriteID == guid)) ??
                                  m_MosaicSpriteImportData.FirstOrDefault(x => x.spriteID == guid)) ??
                                 m_SpriteImportData.FirstOrDefault(x => x.spriteID == guid);
            return spriteMetaData;
        }

        /// <summary>
        /// Implementation of ScriptedImporter.OnImportAsset
        /// </summary>
        /// <param name="ctx">
        /// This argument contains all the contextual information needed to process the import
        /// event and is also used by the custom importer to store the resulting Unity Asset.
        /// </param>
        public override void OnImportAsset(AssetImportContext ctx)
        {
            
            // // Load the texture asset
            // var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(ctx.assetPath);
            //
            //
            // var factories = new SpriteDataProviderFactories();
            // factories.Init();
            // var provider = factories.GetSpriteEditorDataProviderFromObject(texture);
            //  provider.GetDataProvider<ITextureDataProvider>();
            //
            //
            // // Create a new SpriteEditorDataProvider
            // ISpriteEditorDataProvider spriteEditorDataProvider = new TextureImporterDataProvider(texture);
            //
            // // Initialize the SpriteEditorDataProvider
            // spriteEditorDataProvider.InitSpriteEditorDataProvider();
            //
            // // Create a new SpriteRects object and set it to 20x20 size
            // // SpriteRect spriteRects = new SpriteRect();
            // // spriteRects.rect = new Rect
            // // {
            // //     new Rect(0, 0, 20, 20)
            // // };
            //
            // // Apply the SpriteRects to the texture
            // // spriteEditorDataProvider.Apply(spriteRects, null, null);
            //
            // // Create a new Sprite asset
            // var sprite = Sprite.Create(texture, new Rect(0, 0, 20, 20), new Vector2(0.5f, 0.5f));
            //
            // // Add the Sprite asset to the import context
            // ctx.AddObjectToAsset("sprite", sprite);
            // ctx.SetMainObject(sprite);
            //
            // Debug.Log(ctx);
            // var fileStream = new FileStream(ctx.assetPath, FileMode.Open, FileAccess.Read);
            // Document doc = null;
            //
            // if(m_ImportData == null) m_ImportData = ScriptableObject.CreateInstance<PSDImportData>();
            // m_ImportData.hideFlags = HideFlags.HideInHierarchy;
            //
            // try
            // {
            //     UnityEngine.Profiling.Profiler.BeginSample("OnImportAsset");
            //
            //     UnityEngine.Profiling.Profiler.BeginSample("PsdLoad");
            //     doc = PaintDotNet.Data.PhotoshopFileType.PsdLoad.Load(fileStream);
            //     UnityEngine.Profiling.Profiler.EndSample();
            //
            //     m_ImportData.CreatePSDLayerData(doc.Layers);
            //
            //     ValidatePSDLayerId(doc, m_LayerMappingOption);
            //     SetDocumentImportData(doc);
            //
            //     importData.documentSize = new Vector2Int(doc.width, doc.height);
            //     var singleSpriteMode = m_TextureImporterSettings.textureType == TextureImporterType.Sprite &&
            //                            m_TextureImporterSettings.spriteMode != (int)SpriteImportMode.Multiple;
            //     EnsureSingleSpriteExist();
            //
            //     TextureGenerationOutput output;
            //     if(m_TextureImporterSettings.textureType != TextureImporterType.Sprite ||
            //        m_MosaicLayers == false ||
            //        singleSpriteMode)
            //     {
            //         output = ImportFlattenImage(doc, ctx, singleSpriteMode);
            //     }
            //     else
            //     {
            //         output = ImportFromLayers(ctx);
            //     }
            //
            //     if(output.texture != null && output.sprites != null)
            //         SetPhysicsOutline(
            //             GetDataProvider<ISpritePhysicsOutlineDataProvider>(),
            //             output.sprites,
            //             definitionScale,
            //             pixelsPerUnit,
            //             m_GeneratePhysicsShape
            //         );
            //
            //     RegisterAssets(ctx, output);
            // }
            // catch(Exception e)
            // {
            //     Debug.LogError($"Failed to import file {assetPath}. Ex:{e.Message}");
            // }
            // finally
            // {
            //     fileStream.Close();
            //     if(doc != null) doc.Dispose();
            //     UnityEngine.Profiling.Profiler.EndSample();
            //     EditorUtility.SetDirty(this);
            // }
        }
    }
}