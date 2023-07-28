// using UnityEditor;
// using UnityEditor.AssetImporters;
// using UnityEditor.U2D.Sprites;
// using UnityEngine;
//
// namespace Art.Art
// {
//     public class ArtScriptedImporter : ScriptedImporter
//     {
//         public override void OnImportAsset(AssetImportContext ctx)
//         {
//             // Load the texture asset
//             Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(ctx.assetPath);
//         
//             // Create a new SpriteEditorDataProvider
//             ISpriteEditorDataProvider spriteEditorDataProvider = new TextureImporterDataProvider(texture);
//         
//             // Initialize the SpriteEditorDataProvider
//             spriteEditorDataProvider.InitSpriteEditorDataProvider();
//         
//             // Create a new SpriteRects object and set it to 20x20 size
//             // SpriteRect spriteRects = new SpriteRect();
//             // spriteRects.rect = new Rect
//             // {
//             //     new Rect(0, 0, 20, 20)
//             // };
//         
//             // Apply the SpriteRects to the texture
//             // spriteEditorDataProvider.Apply(spriteRects, null, null);
//         
//             // Create a new Sprite asset
//             Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 20, 20), new Vector2(0.5f, 0.5f));
//         
//             // Add the Sprite asset to the import context
//             ctx.AddObjectToAsset("sprite", sprite);
//             ctx.SetMainObject(sprite);
//         }
//     }
// }