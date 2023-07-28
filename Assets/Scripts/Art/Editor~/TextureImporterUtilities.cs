using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Common;

namespace UnityEditor.U2D.ART
{
    internal static class TextureImporterUtilities
    {
        public static TextureImporterPlatformSettings GetPlatformTextureSettings(BuildTarget buildTarget, in List<TextureImporterPlatformSettings> platformSettings)
        {
            // var buildTargetName = TexturePlatformSettingsHelper.GetBuildTargetGroupName(buildTarget);
            TextureImporterPlatformSettings settings = null;
            // settings = platformSettings.SingleOrDefault(x => x.name == buildTargetName && x.overridden == true);
            // settings = settings ?? platformSettings.SingleOrDefault(x => x.name == TexturePlatformSettingsHelper.defaultPlatformName);

            if (settings == null)
            {
                settings = new TextureImporterPlatformSettings();
                // settings.name = buildTargetName;
                settings.overridden = false;
            }
            return settings;
        }   
        
        
        // public static string GetBuildTargetGroupName(BuildTarget target)
        // {
        //     var targetGroup = BuildPipeline.GetBuildTargetGroup(target);
        //     foreach (var bp in BuildPlatforms.instance.buildPlatforms)
        //     {
        //         if (bp.targetGroup == targetGroup)
        //             return bp.name;
        //     }
        //     return TextureImporter.defaultPlatformName;
        // }
    }
}