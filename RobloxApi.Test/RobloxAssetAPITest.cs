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
            Asset asset = Asset.FromID(TestConstants.TestAssetId).WaitForResult(TestConstants.MaxMilisecondTimeout); // Places are assets.

            Assert.IsNotNull(asset);

            Type ty = typeof(Asset);
            foreach (PropertyInfo info in ty.GetProperties())
            {
                Console.WriteLine("{0} = {1}", info.Name, info.GetGetMethod().Invoke(asset, new object[] { }));
            }
        }

        [TestMethod]
        public void GetPackageContents()
        {
            Asset asset = Asset.FromID(27133145).WaitForResult(TestConstants.MaxMilisecondTimeout); // Places are assets.

            Assert.IsNotNull(asset);
            Assert.IsTrue(asset.AssetType == EAssetType.Package);

            Console.WriteLine("Package Contents of {0}:", asset.ID);

            Asset[] assets = (asset.GetAssetsInPackage().WaitForResult(TestConstants.MaxMilisecondTimeout));

            Assert.IsNotNull(assets.Length);
            Assert.IsTrue(assets.Length > 0);

            foreach (Asset packageAsset in assets)
            {
                Assert.IsTrue(packageAsset.ID > 0);
                Console.WriteLine("Part: {0}", packageAsset.ID);
            }
        }

        [TestMethod]
        public void GetPlaceThumbnail()
        {
            // Get place asset.
            Asset asset = Asset.FromID(TestConstants.TestAssetId).WaitForResult(TestConstants.MaxMilisecondTimeout); // Places are assets.

            Assert.IsNotNull(asset);

            // Get Asset Image URL.

            Task<string> imageUrl = asset.GetAssetImageURL(EThumbnailSize.Square250x250);

            imageUrl.Wait();

            // Test if imageUrl.Result is valid.
            Assert.IsNotNull(imageUrl.Result);
            Console.WriteLine("Image URL: {0}", imageUrl.Result);

            // Get Asset Thumbnail as a byte array. (PNG)

            Task<byte[]> imageData = asset.GetAssetImage(EThumbnailSize.Square250x250);

            imageData.Wait();

            // Test if imageData.Result is valid.
            Assert.IsNotNull(imageData.Result);
            Assert.IsTrue(imageData.Result.Length > 0);

            Console.WriteLine("Image Data Length: {0}", imageData.Result.Length);
        }
    }
}
