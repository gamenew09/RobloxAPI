using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Reflection;

namespace RobloxApi.Test
{
    [TestClass]
    public class RobloxAssetAPITest
    {
        [TestMethod]
        public void GetPlaceAssetMadeByUser()
        {
            Task.Run(async () =>
            {
                Asset asset = await Asset.FromID(TestConstants.TestAssetId); // Places are assets.

                Assert.IsNotNull(asset);

                Type ty = typeof(Asset);
                foreach (PropertyInfo info in ty.GetProperties())
                {
                    Console.WriteLine("{0} = {1}", info.Name, info.GetGetMethod().Invoke(asset, new object[] { }));
                }
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }

        [TestMethod]
        public void GetExclusiveAsset()
        {
            Task.Run(async () =>
            {
                Asset asset = await Asset.FromID(617605556); // Places are assets.

                Assert.IsNotNull(asset);

                Console.WriteLine("Exclusivity: {0}", await asset.GetExclusivity());
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }
        [TestMethod]
        public void GetPackageContents()
        {
            Task.Run(async () =>
            {
                Asset asset = await Asset.FromID(27133145); // Places are assets.

                Assert.IsNotNull(asset);
                Assert.IsTrue(asset.AssetType == EAssetType.Package);

                Console.WriteLine("Package Contents of {0}:", asset.ID);

                Asset[] assets = (await asset.GetAssetsInPackage());

                Assert.IsNotNull(assets.Length);
                Assert.IsTrue(assets.Length > 0);

                foreach (Asset packageAsset in assets)
                {
                    Assert.IsTrue(packageAsset.ID > 0);
                    Console.WriteLine("Part: {0}", packageAsset.ID);
                }
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }

        [TestMethod]
        public void GetPlaceThumbnail()
        {
            Task.Run(async () =>
            {
                // Get place asset.
                Asset asset = await Asset.FromID(TestConstants.TestAssetId); // Places are assets.

                Assert.IsNotNull(asset);

                // Get Asset Image URL.

                string imageUrl = await asset.GetAssetImageURL(EThumbnailSize.Square250x250);

                // Test if imageUrl.Result is valid.
                Assert.IsNotNull(imageUrl);
                Console.WriteLine("Image URL: {0}", imageUrl);

                // Get Asset Thumbnail as a byte array. (PNG)

                byte[] imageData = await asset.GetAssetImage(EThumbnailSize.Square250x250);

                // Test if imageData.Result is valid.
                Assert.IsNotNull(imageData);
                Assert.IsTrue(imageData.Length > 0);

                Console.WriteLine("Image Data Length: {0}", imageData.Length);
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }
    }
}
