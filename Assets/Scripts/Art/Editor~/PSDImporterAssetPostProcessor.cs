using System.Collections.Generic;

namespace UnityEditor.U2D.ART
{
    class PSDImporterAssetPostProcessor : AssetPostprocessor
    {
        static List<PSDImporter> s_AssetImporter; 
        public override int GetPostprocessOrder() => int.MinValue;

        void OnPreprocessAsset()
        {
            if (assetImporter is PSDImporter)
            {
                if(s_AssetImporter == null)
                    s_AssetImporter = new List<PSDImporter>();
                s_AssetImporter.Add((PSDImporter)assetImporter);   
            }
        }

        internal static bool ContainsImporter(PSDImporter importer)
        {
            return s_AssetImporter == null ? false : s_AssetImporter.Contains(importer);
        }
        
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            s_AssetImporter = null;
        }
    }
}