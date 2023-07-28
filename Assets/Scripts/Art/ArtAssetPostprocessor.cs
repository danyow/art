using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace Art
{
    public class ArtAssetPostprocessor: AssetPostprocessor
    {
        private static readonly Vector2 Pivot = Vector2.one * 0.5f;

        private void OnPostprocessTexture(Texture2D texture)
        {
            if(!assetPath.StartsWith("Assets/Textures/"))
            {
                return;
            }

            var textureImporter = assetImporter as TextureImporter;
            if(textureImporter != null)
            {
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spriteImportMode = SpriteImportMode.Multiple;
                textureImporter.spritePixelsPerUnit = 1;
                textureImporter.alphaIsTransparency = true;
                textureImporter.mipmapEnabled = false;
                textureImporter.wrapMode = TextureWrapMode.Clamp;
                textureImporter.filterMode = FilterMode.Point;
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                textureImporter.spritePivot = Pivot;
                textureImporter.spriteBorder = Vector4.zero;
                textureImporter.npotScale = TextureImporterNPOTScale.None;
                textureImporter.SetPlatformTextureSettings(
                    new TextureImporterPlatformSettings
                    {
                        name = "Standalone",
                        overridden = true,
                        maxTextureSize = 8192,
                        format = TextureImporterFormat.RGBA32,
                        textureCompression = TextureImporterCompression.Uncompressed,
                        crunchedCompression = false,
                        allowsAlphaSplitting = false,
                        compressionQuality = 100,
                        androidETC2FallbackOverride = AndroidETC2FallbackOverride.Quality32Bit,
                    }
                );
            }

            // assetImporter.SaveAndReimport();

            var temp = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

            var croppedSize = new Vector2Int(100, 100);

            var factories = new SpriteDataProviderFactories();
            factories.Init();
            var provider = factories.GetSpriteEditorDataProviderFromObject(textureImporter);
            provider.InitSpriteEditorDataProvider();

            var nameFileIdDataProvider = provider.GetDataProvider<ISpriteNameFileIdDataProvider>();

            var spriteRects = new List<SpriteRect>();
            var nameFileIdPairs = new List<SpriteNameFileIdPair>();
            var spriteSheet = new List<SpriteMetaData>();

            // GetOriginalTextureSize(textureImporter, out var originalWidth, out var originalHeight);

            var textureSize = GetImageSize(assetPath);
            textureSize = textureSize == Vector2.zero ? new Vector2(texture.width, texture.height) : textureSize;

            var yMax = textureSize.y - croppedSize.y;
            var xMax = textureSize.x - croppedSize.x;

            var yIndex = 0;
            for(var y = 0; y <= yMax; y += croppedSize.y)
            {
                var xIndex = 0;
                for(var x = 0; x <= xMax; x += croppedSize.x)
                {
                    var name = $"{texture.name}_{xIndex}_{yIndex}";
                    var rect = new Rect(x, y, croppedSize.x, croppedSize.y);
                    var spriteRect = new SpriteRect
                    {
                        alignment = SpriteAlignment.Center, border = Vector4.zero, name = name, pivot = Pivot, rect = rect,
                    };
                    var sprite = Sprite.Create(texture, rect, spriteRect.pivot, 1, 1, SpriteMeshType.Tight, Vector4.one, false, null);
                    assetImporter.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Sprite), name), sprite);
                    nameFileIdPairs.Add(new SpriteNameFileIdPair(name, sprite.GetSpriteID()));
                    xIndex++;
                    spriteRects.Add(spriteRect);
                    spriteSheet.Add(new SpriteMetaData { alignment = 0, border = Vector4.zero, name = name, pivot = Pivot, rect = rect, });
                }
                yIndex++;
            }

            if(textureImporter != null)
            {
                textureImporter.spritesheet = spriteSheet.ToArray();

                var serializedObject = new SerializedObject(textureImporter);

                var sprites = serializedObject.FindProperty("m_SpriteSheet.m_Sprites");

                Debug.Log("");
            }

            nameFileIdDataProvider.SetNameFileIdPairs(nameFileIdPairs);
            provider.SetSpriteRects(spriteRects.ToArray());
            provider.Apply();

            assetImporter.SaveAndReimport();

            // AssetDatabase.WriteImportSettingsIfDirty(assetPath);
            // AssetDatabase.SaveAssetIfDirty(texture);

            // AssetDatabase.Refresh();
            EditorApplication.delayCall += () =>
            {
                AssetDatabase.WriteImportSettingsIfDirty(assetPath);

                // Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                // SpriteImportData importData = new SpriteImportData(sprite, assetPath);
                //
                // if (importData.HasMeshPrefab)
                // {
                //     PrefabUtil.TryRename(importData.assetPath, importData.MeshPrefab);
                // }
            };
        }

        // private SerializedObject GetSerializedObject(UnityEngine.Object target)
        // {
        //     var serializedObject = default(SerializedObject);
        //
        //     if (!m_SerializedObjects.TryGetValue(target, out serializedObject))
        //     {
        //         serializedObject = new SerializedObject(target);
        //         m_SerializedObjects[target] = serializedObject;
        //     }
        //
        //     return serializedObject;
        // }
        //

        private static Vector2 GetImageSize(string path)
        {
            try
            {
                var imageData = File.ReadAllBytes(Environment.CurrentDirectory + "/" + path);
                var texture = new Texture2D(2, 2); // Create a temporary Texture2D
                if(texture.LoadImage(imageData))   // Load the image data into the texture
                {
                    var width = texture.width;
                    var height = texture.height;
                    return new Vector2(width, height);
                }
                Debug.LogError("Failed to load image from path: " + path);
            }
            catch(Exception e)
            {
                Debug.LogError("Error reading image from path: " + path + "\n" + e.Message);
            }
            return Vector2.zero;
        }

        private void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
        {
            var textureImporter = assetImporter as TextureImporter;
            var textureImporterSettings = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(textureImporterSettings);

            // if (textureImporterSettings.IsSingleSprite())
            // {
            //     //override mesh(support first sprite only)
            //     foreach (var sprite in sprites)
            //     {
            //         SpriteProcessor spriteProcessor = new SpriteProcessor(sprite, assetPath);
            //         spriteProcessor.OverrideGeometry();
            //         spriteProcessor.UpdateMeshInMeshPrefab();
            //         break;
            //     }
            // }

            // //auto rename
            // if (SpriteAssistSettings.instance.enableRenameMeshPrefabAutomatically)
            // {
            //     EditorApplication.delayCall += () =>
            //     {
            //         Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            //         SpriteImportData importData = new SpriteImportData(sprite, assetPath);
            //
            //         if (importData.HasMeshPrefab)
            //         {
            //             PrefabUtil.TryRename(importData.assetPath, importData.MeshPrefab);
            //         }
            //     };
            // }
        }

        // void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
        // {
        //     if(!s_Initialized) return;
        //
        //     string guid = AssetDatabase.AssetPathToGUID(assetPath);
        //
        //     if(s_SpriteMeshToTextureCache.ContainsValue(guid))
        //     {
        //         foreach(Sprite sprite in sprites)
        //         {
        //             foreach(KeyValuePair<string,string> pair in s_SpriteMeshToTextureCache)
        //             {
        //                 if(pair.Value == guid)
        //                 {
        //                     SpriteMesh spriteMesh = LoadSpriteMesh(AssetDatabase.GUIDToAssetPath(pair.Key));
        //
        //                     if(spriteMesh && spriteMesh.sprite && sprite.name == spriteMesh.sprite.name)
        //                     {
        //                         DoSpriteOverride(spriteMesh,sprite);
        //                         break;
        //                     }
        //                 }
        //             }
        //         }
        //     }
        // }
        //
        // static void DoSpriteOverride(SpriteMesh spriteMesh, Sprite sprite)
        // {
        //     SpriteMeshData spriteMeshData = SpriteMeshUtils.LoadSpriteMeshData(spriteMesh);
        //
        //     if(spriteMeshData) 
        //     {
        //         Rect rect = SpriteMeshUtils.CalculateSpriteRect(spriteMesh,5);
        //         Rect spriteRect = sprite.rect;
        //
        //         Vector2 factor = new Vector2(spriteRect.width/rect.width,spriteRect.height/rect.height);
        //
        //         Vector2[] newVertices = new List<Vector2>(spriteMeshData.vertices).ConvertAll( v => MathUtils.ClampPositionInRect(Vector2.Scale(v,factor),spriteRect) - spriteRect.position ).ToArray();
        //         ushort[] newIndices = new List<int>(spriteMeshData.indices).ConvertAll<ushort>( i => (ushort)i ).ToArray();
        //
        //         sprite.OverrideGeometry(newVertices, newIndices);
        //     }
        // }

        // private static void OnPostprocessAllAssets(
        //     string[] importedAssets,
        //     string[] deletedAssets,
        //     string[] movedAssets,
        //     string[] movedFromAssetPaths
        // )
        // {
        //     foreach(var asset in importedAssets)
        //     {
        //         if(!asset.StartsWith("Assets/Textures/"))
        //         {
        //             return;
        //         }
        //
        //         AssetDatabase.ImportAsset(asset, ImportAssetOptions.ForceUpdate);
        //     }
        // }
    }
}