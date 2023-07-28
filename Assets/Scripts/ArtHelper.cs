using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Art.Data;
using Art.Methods;
using UnityEditor;
using UnityEngine;

namespace Art
{
    // public enum PixarMethodType
    // {
    //     None,
    //     PixelatedMethod,
    //     ColorizationRgb,
    //     ColorizationRbgPct,
    //     ColorizationHsv,
    //     ColorizationHsv2,
    //     ColorizationHsvWeight,
    //     SectionMethod,
    // }

    [ExecuteInEditMode]
    public class ArtHelper: MonoBehaviour
    {
        /// <summary>
        /// 要捕获的目标游戏对象。
        /// </summary>
        [SerializeField]
        private GameObject target;

        /// <summary>
        /// 要捕获的动画剪辑。
        /// </summary>
        [SerializeField]
        private AnimationClip sourceClip;

        /// <summary>
        /// 捕获动画的FPS。
        /// </summary>
        [SerializeField]
        private int framesPerSecond = 30;

        /// <summary>
        /// 渲染精灵的输出分辨率。
        /// </summary>
        [SerializeField]
        private Vector2Int cellSize = new Vector2Int(100, 100);

        [SerializeField]
        private int blockSize = 4;

        [SerializeField]
        private int alphaCut = 32;

        /// <summary>
        /// 要采样的当前关键帧。
        /// </summary>
        [SerializeField]
        private int currentFrame;

        /// <summary>
        /// 用于渲染动画的相机。
        /// </summary>
        [SerializeField]
        private Camera captureCamera;

        public const int kDefaultAlphaCut = 10;

        // private static readonly Dictionary<PixarMethodType, PixarMethod> ColorMethods = new Dictionary<PixarMethodType, PixarMethod>
        // {
        //     { PixarMethodType.PixelatedMethod, new PixelatedMethod() },
        //     { PixarMethodType.ColorizationRgb, new ColorizationRgb() },
        //     { PixarMethodType.ColorizationRbgPct, new ColorizationRbgPct() },
        //     { PixarMethodType.ColorizationHsv, new ColorizationHsv() },
        //     { PixarMethodType.ColorizationHsv2, new ColorizationHsv2() },
        //     { PixarMethodType.ColorizationHsvWeight, new ColorizationHsvWeight() },
        // };
        //
        // private static readonly Dictionary<PixarMethodType, PixarMethod> NormalMethods = new Dictionary<PixarMethodType, PixarMethod>
        // {
        //     { PixarMethodType.PixelatedMethod, new PixelatedMethod() }, { PixarMethodType.SectionMethod, new SectionMethod() },
        // };

        // [SerializeField]
        // private List<PixarMethodType> colorMethods = new List<PixarMethodType>();

        // private IEnumerable<PixarMethod> selectedColorMethods => colorMethods.Where(t => ColorMethods.ContainsKey(t)).Select(t => ColorMethods[t]);

        // [SerializeField]
        // private List<PixarMethodType> normalMethods = new List<PixarMethodType>();

        // private IEnumerable<PixarMethod> selectedNormalMethods
        //     => normalMethods.Where(t => NormalMethods.ContainsKey(t)).Select(t => NormalMethods[t]);

        private IEnumerable<PixarMethod> selectedColorMethods => GetComponents<PixarMethod>().Where(t => t.methodType == MethodType.DiffuseMap);
        private IEnumerable<PixarMethod> selectedNormalMethods => GetComponents<PixarMethod>().Where(t => t.methodType == MethodType.NormalMap);

        /// <summary>
        /// The current capture routine in progress.
        /// </summary>
        private IEnumerator _currentCaptureRoutine;

        /// <summary>
        /// 将动画片段采样到目标对象上。
        /// </summary>
        /// <param name="time"></param>
        public void SampleAnimation(float time)
        {
            if(sourceClip == null || target == null)
            {
                Debug.LogWarning("SourceClip and Target should be set before sample animation!");
                return;
            }
            sourceClip.SampleAnimation(target, time);
        }

        [ContextMenu("Capture")]
        public void Capture()
        {
            // EditorUtility.
            // StartCoroutine(CaptureAnimation(SaveCapture));
            RunRoutine(CaptureAnimation(SaveCapture));
        }

        public IEnumerator CaptureAnimation(Action<Texture2D, Texture2D> onComplete)
        {
            if(sourceClip == null || target == null)
            {
                Debug.LogWarning("SourceClip and Target should be set before capture animation!");
                yield break;
            }

            var numFrames = (int)(sourceClip.length * framesPerSecond);
            var gridCellCount = SqrtCeil(numFrames);
            var atlasSize = cellSize * gridCellCount;
            var atlasPos = new Vector2Int(0, atlasSize.y - cellSize.y);

            if(atlasSize.x > 4096 || atlasSize.y > 4096)
            {
                Debug.LogErrorFormat(
                    "Error attempting to capture an animation with a length and" +
                    "resolution that would produce a texture of size: {0}",
                    atlasSize
                );
            }

            var diffuseMap = new Texture2D(atlasSize.x, atlasSize.y, TextureFormat.ARGB32, false) { filterMode = FilterMode.Point, };
            ClearAtlas(diffuseMap, Color.clear);

            var normalMap = new Texture2D(atlasSize.x, atlasSize.y, TextureFormat.ARGB32, false) { filterMode = FilterMode.Point, };
            ClearAtlas(normalMap, new Color(0.5f, 0.5f, 1.0f, 0.0f));

            var rtFrame = new RenderTexture(cellSize.x, cellSize.y, 24, RenderTextureFormat.ARGB32)
            {
                filterMode = FilterMode.Point, antiAliasing = 1, hideFlags = HideFlags.HideAndDontSave,
            };

            var normalCaptureShader = Shader.Find("Hidden/ViewSpaceNormal");

            captureCamera.targetTexture = rtFrame;
            var cachedCameraColor = captureCamera.backgroundColor;

            try
            {
                for(currentFrame = 0; currentFrame < numFrames; currentFrame++)
                {
                    var currentTime = currentFrame / (float)numFrames * sourceClip.length;
                    SampleAnimation(currentTime);
                    yield return null;

                    captureCamera.backgroundColor = Color.clear;
                    captureCamera.Render();
                    Graphics.SetRenderTarget(rtFrame);
                    diffuseMap.ReadPixels(new Rect(0, 0, rtFrame.width, rtFrame.height), atlasPos.x, atlasPos.y);
                    diffuseMap.Apply();

                    captureCamera.backgroundColor = new Color(0.5f, 0.5f, 1.0f, 0.0f);
                    captureCamera.RenderWithShader(normalCaptureShader, "");
                    Graphics.SetRenderTarget(rtFrame);
                    normalMap.ReadPixels(new Rect(0, 0, rtFrame.width, rtFrame.height), atlasPos.x, atlasPos.y);
                    normalMap.Apply();

                    atlasPos.x += cellSize.x;

                    if((currentFrame + 1) % gridCellCount == 0)
                    {
                        atlasPos.x = 0;
                        atlasPos.y -= cellSize.y;
                    }
                }
                PostProcess(selectedColorMethods, diffuseMap);
                PostProcess(selectedNormalMethods, normalMap);
                onComplete.Invoke(diffuseMap, normalMap);
            }
            finally
            {
                Graphics.SetRenderTarget(null);
                captureCamera.targetTexture = null;
                captureCamera.backgroundColor = cachedCameraColor;
                DestroyImmediate(rtFrame);
            }
        }

        /// <summary>
        /// Starts running the editor routine.
        /// </summary>
        private void RunRoutine(IEnumerator routine)
        {
            _currentCaptureRoutine = routine;
            EditorApplication.update += UpdateRoutine;
        }

        /// <summary>
        /// Calls MoveNext on the routine each editor frame until the iterator terminates.
        /// </summary>
        private void UpdateRoutine()
        {
            if(!_currentCaptureRoutine.MoveNext())
            {
                EditorApplication.update -= UpdateRoutine;
                _currentCaptureRoutine = null;
            }
        }

        /// <summary>
        /// Returns the ceil square root of the input.
        /// </summary>
        private static int SqrtCeil(int input)
        {
            return Mathf.CeilToInt(Mathf.Sqrt(input));
        }

        /// <summary>
        /// Sets all the pixels in the texture to a specified color.
        /// </summary>
        private static void ClearAtlas(Texture2D texture, Color color)
        {
            var pixels = new Color[texture.width * texture.height];
            for(var i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            texture.SetPixels(pixels);
            texture.Apply();
        }

        private void PostProcess(IEnumerable<PixarMethod> methods, Texture2D texture)
        {
            var input = new PixelatedIn { texture = texture, blockSize = blockSize, alphaCut = alphaCut, };
            foreach(var t in methods)
            {
                input.texture = t.Process(input);
            }
        }

        /// <summary>
        /// Saves the captured animation sprite atlases to disk.
        /// </summary>
        private void SaveCapture(Texture2D diffuseMap, Texture2D normalMap)
        {
            var diffusePath = EditorUtility.SaveFilePanel("Save Capture", "", "NewCapture", "png");

            if(string.IsNullOrEmpty(diffusePath))
            {
                return;
            }

            var fileName = Path.GetFileNameWithoutExtension(diffusePath);
            var directory = Path.GetDirectoryName(diffusePath);
            var normalPath = $"{directory}/{fileName}{"NormalMap"}.{"png"}";

            File.WriteAllBytes(diffusePath, diffuseMap.EncodeToPNG());
            File.WriteAllBytes(normalPath, normalMap.EncodeToPNG());

            AssetDatabase.Refresh();
        }
    }
}
