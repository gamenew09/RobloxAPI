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
            Task<Asset> assetTask = Asset.FromID(1818); // Places are assets.

            assetTask.Wait();

            Asset asset = assetTask.Result;

            Assert.IsNotNull(asset);

            Type ty = typeof(Asset);
            foreach (PropertyInfo info in ty.GetProperties())
            {
                Console.WriteLine("{0} = {1}", info.Name, info.GetGetMethod().Invoke(asset, new object[] { }));
            }
        }

        [TestMethod]
        public void GetPlaceThumbnail()
        {
            // Get place asset.
            Task<Asset> assetTask = Asset.FromID(1818); // Places are assets.

            assetTask.Wait();

            Asset asset = assetTask.Result;

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
