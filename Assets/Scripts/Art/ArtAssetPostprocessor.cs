using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace Art
{
    public class ArtAssetPostprocessor : AssetPostprocessor
    {
        public const string kAssetGeneratedPath = "Assets/Generated/";
        public const string kAssetGeneratedTexturesPath = "Assets/Generated/Textures/";
        public const string kAssetGeneratedAnimationsPath = "Assets/Generated/Animations/";

        private static readonly Vector2 Pivot = Vector2.one * 0.5f;

        private void OnPreprocessTexture()
        {
            if(!assetPath.StartsWith(kAssetGeneratedTexturesPath))
            {
                return;
            }

            var textureImporter = assetImporter as TextureImporter;

            if(textureImporter != null)
            {
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spriteImportMode = SpriteImportMode.Multiple;
                textureImporter.spritePixelsPerUnit = 100;
                textureImporter.alphaIsTransparency = true;
                textureImporter.mipmapEnabled = false;
                textureImporter.wrapMode = TextureWrapMode.Clamp;
                textureImporter.filterMode = FilterMode.Point;
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                textureImporter.spritePivot = Pivot;
                textureImporter.spriteBorder = Vector4.zero;
                textureImporter.npotScale = TextureImporterNPOTScale.None;
            }

            var croppedSize = new Vector2Int(100, 100);

            var factories = new SpriteDataProviderFactories();
            factories.Init();
            var provider = factories.GetSpriteEditorDataProviderFromObject(textureImporter);
            provider.InitSpriteEditorDataProvider();

            var spriteRects = new List<SpriteRect>();

            var texture2D = GetImageFile(assetPath);

            var yMax = texture2D.width - croppedSize.y;
            var xMax = texture2D.height - croppedSize.x;

            var yIndex = 0;

            for(var y = yMax; y >= 0; y -= croppedSize.y)
            {
                var xIndex = 0;

                for(var x = 0; x <= xMax; x += croppedSize.x)
                {
                    var name = $"{texture2D.name}_{yIndex}_{xIndex}";
                    var rect = new Rect(x, y, croppedSize.x, croppedSize.y);

                    var pixels = texture2D.GetPixels((int) rect.xMin, (int) rect.yMin, (int) rect.width, (int) rect.height);

                    if(!pixels.Any(t => t.a > 0))
                    {
                        continue;
                    }

                    var spriteRect = new SpriteRect
                    {
                        alignment = SpriteAlignment.Center,
                        border = Vector4.zero,
                        name = name, pivot = Pivot, rect = rect,
                    };

                    xIndex++;
                    spriteRects.Add(spriteRect);
                }

                yIndex++;
            }

            provider.SetSpriteRects(spriteRects.ToArray());
            provider.Apply();

            assetImporter.SaveAndReimport();
        }

        private static Texture2D GetImageFile(string path)
        {
            try
            {
                var imageData = File.ReadAllBytes(Environment.CurrentDirectory + "/" + path);
                var texture = new Texture2D(2, 2); // Create a temporary Texture2D

                if(texture.LoadImage(imageData)) // Load the image data into the texture
                {
                    texture.name = Path.GetFileNameWithoutExtension(path);
                    return texture;
                }

                Debug.LogError("Failed to load image from path: " + path);
            }
            catch(Exception e)
            {
                Debug.LogError("Error reading image from path: " + path + "\n" + e.Message);
            }

            return null;
        }

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths
        )
        {
            var texturePath = importedAssets.FirstOrDefault(t => t.StartsWith(kAssetGeneratedTexturesPath));

            if(texturePath == null)
            {
                return;
            }

            EditorApplication.delayCall += () =>
            {
                var assets = AssetDatabase.LoadAllAssetsAtPath(texturePath);
                var keyFrames = new ObjectReferenceKeyframe[assets.Length - 1];
                const float frameRate = 0.1f; // 动画播放速率
                var animationName = Path.GetFileNameWithoutExtension(texturePath);
                var path = Path.Combine(kAssetGeneratedAnimationsPath, animationName + ".anim");

                var animationClip = new AnimationClip
                {
                    frameRate = 1 / frameRate,
                    name = animationName,
                };

                var index = 0;

                foreach(var asset in assets)
                {
                    if(asset is not Sprite sprite)
                    {
                        continue;
                    }

                    keyFrames[index] = new ObjectReferenceKeyframe
                    {
                        time = index * frameRate,
                        value = sprite,
                    };

                    index++;
                }

                AnimationUtility.SetObjectReferenceCurve(
                    animationClip,
                    EditorCurveBinding.DiscreteCurve(string.Empty, typeof(SpriteRenderer), "m_Sprite"),
                    keyFrames
                );

                if(!Directory.Exists(kAssetGeneratedAnimationsPath))
                {
                    Directory.CreateDirectory(kAssetGeneratedAnimationsPath);
                }

                var controllerPath = Path.Combine(kAssetGeneratedAnimationsPath, animationName + ".controller");

                var animatorController = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

                var baseLayer = animatorController.layers[0];
                var stateMachine = baseLayer.stateMachine;
                var state = stateMachine.AddState(animationName);
                state.motion = animationClip;

                AssetDatabase.CreateAsset(animationClip, path);
                AssetDatabase.SaveAssets();
            };
        }
    }
}