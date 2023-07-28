using UnityEditor.U2D.Animation;

namespace Art.DataProvider
{
    public class MainSkeletonDataProvider: ArtDataProvider, IMainSkeletonDataProvider
    {
        public MainSkeletonData GetMainSkeletonData()
        {
            return new MainSkeletonData { bones = dataProvider.mainSkeletonBones };
        }
    }
}