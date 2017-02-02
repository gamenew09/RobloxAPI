using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RobloxApi
{

    public enum ECreatorType
    {
        Unknown = -1,
        User,
        Group,
    }

    public enum EThumbnailSize
    {
        Unknown = -1,
        Square48x48,
        Rectangle60x62,
        Square75x75,
        Square100x100,
        Square110x110,
        Rectangle160x100,
        Square250x250,
        Square352x352,
        Rectangle420x230,
        Square420x420
    }

    public class Asset
    {
        public static implicit operator int(Asset asset)
        {
            return asset.AssetId;
        }

        public static implicit operator Asset(int assetId)
        {
            return new Asset(assetId);
        }

        public int AssetId { get; internal set; }
        public int ProductId { get; internal set; }
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public EAssetType AssetType { get; internal set; }
        public int CreatorID { get; internal set; } // Use "User" eventually, might just use User.
        public ECreatorType CreatorType { get; internal set; }
        public int IconImageAssetId { get; internal set; }
        public DateTime Created { get; internal set; }
        public DateTime Updated { get; internal set; }
        public int PriceInRobux { get; internal set; }
        // PriceInTickets is no longer used.
        public int Sales { get; internal set; }
        public bool IsNew { get; internal set; }
        public bool IsForSale { get; internal set; }
        public bool IsPublicDomain { get; internal set; }
        public bool IsLimited { get; internal set; }
        public bool IsLimitedUnique { get; internal set; }
        public int Remaining { get; internal set; }
        public EMembershipLevel MinimumMembershipLevel { get; internal set; }
        //public int ContentRatingTypeId { get; private set; }
        public bool Is13OrOver { get; internal set; }

        public Asset()
        {

        }

        /// <summary>
        /// Gets the asset data from https://assetgame.roblox.com/Asset/?id=
        /// </summary>
        /// <returns>The asset downloaded as a string</returns>
        public async Task<string> DownloadAsString()
        {
            return await HttpHelper.GetStringFromURL("https://assetgame.roblox.com/Asset/?id=" + AssetId);
        }

        /// <summary>
        /// Does the user id provided have the asset?
        /// </summary>
        /// <param name="userId">The user to check.</param>
        /// <returns>Does the user id provided have the asset?</returns>
        public async Task<bool> DoesUserHave(int userId)
        {
            return await HttpHelper.GetStringFromURL(string.Format("http://api.roblox.com/Ownership/HasAsset?userId={0}&assetId={1}", userId, AssetId)) == "true";
        }

        /// <summary>
        /// Sets the AssetId.
        /// </summary>
        /// <param name="assetId">The assetId to set.</param>
        public Asset(int assetId)
        {
            AssetId = assetId;
        }

        public async Task<string> GetAssetImageURL(EThumbnailSize size)
        {
            string sizeStr = "";
            switch(size)
            {
                case EThumbnailSize.Rectangle160x100:
                    sizeStr = "160x100";
                    break;
                case EThumbnailSize.Rectangle420x230:
                    sizeStr = "420x230";
                    break;
                case EThumbnailSize.Rectangle60x62:
                    sizeStr = "60x62";
                    break;
                case EThumbnailSize.Square100x100:
                    sizeStr = "100x100";
                    break;
                case EThumbnailSize.Square110x110:
                    sizeStr = "110x110";
                    break;
                case EThumbnailSize.Square250x250:
                    sizeStr = "250x250";
                    break;
                case EThumbnailSize.Square352x352:
                    sizeStr = "352x352";
                    break;
                case EThumbnailSize.Square420x420:
                    sizeStr = "420x420";
                    break;
                case EThumbnailSize.Square48x48:
                    sizeStr = "48x48";
                    break;
                case EThumbnailSize.Square75x75:
                    sizeStr = "75x75";
                    break;
                default:
                    throw new ArgumentException("Size must not be Unknown.", "size");
            }
            
            return await HttpHelper.GetStringFromURL(string.Format("https://www.roblox.com/Thumbs/RawAsset.ashx?assetId={0}&imageFormat=png&width={1}&height={2}", AssetId, sizeStr.Split('x')[0], sizeStr.Split('x')[1]));
        }

        public async Task<byte[]> GetAssetImage(EThumbnailSize size)
        {
            string url = await GetAssetImageURL(size);

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.UserAgent = "CSharp.RobloxAPI";
            WebResponse resp = await req.GetResponseAsync();

            byte[] imageBuffer = new byte[resp.ContentLength];

            await resp.GetResponseStream().ReadAsync(imageBuffer, 0, imageBuffer.Length);

            return imageBuffer;
        }

        /// <summary>
        /// Gets an <see cref="Asset"/> with information filled in.
        /// </summary>
        /// <param name="assetId">The asset id to pull from</param>
        /// <returns>Asset filled with information</returns>
        public static async Task<Asset> FromID(int assetId)
        {
            string data = await HttpHelper.GetStringFromURL(string.Format("https://api.roblox.com/marketplace/productinfo?assetId={0}", assetId));
            JObject obj = JObject.Parse(data);

            Asset asset = new Asset();
            asset.AssetId = (int)obj["AssetId"];
            asset.ProductId = (int?)obj["ProductId"] ?? -1;

            asset.Name = (string)obj["Name"];
            asset.Description = (string)obj["Description"];

            asset.AssetType = (EAssetType)(int)obj["AssetTypeId"]; // eww.

            string creatorType = (string)obj["Creator"]["CreatorType"];

            asset.CreatorID = (int)obj["Creator"]["CreatorTargetId"];

            if (creatorType == "Group")
                asset.CreatorType = ECreatorType.Group;
            else
                asset.CreatorType = ECreatorType.User;

            asset.IconImageAssetId = (int?)obj["IconImageAssetId"] ?? 0;

            asset.Created = DateTime.Parse((string)obj["Created"]);
            asset.Updated = DateTime.Parse((string)obj["Updated"]);

            asset.PriceInRobux = (int?)obj["PriceInRobux"] ?? 0; // We don't use PriceInTickets since Tickets are gone from roblox.
            asset.Sales = (int?)obj["Sales"] ?? 0;

            asset.IsNew = (bool?)obj["IsNew"] ?? false;
            asset.IsForSale = (bool?)obj["IsForSale"] ?? false;
            asset.IsPublicDomain = (bool?)obj["IsPublicDomain"] ?? false;
            asset.IsLimited = (bool?)obj["IsLimited"] ?? false;
            asset.IsLimitedUnique = (bool?)obj["IsLimitedUnique"] ?? false;
            if (obj.Value<int?>("Remaining") != null)
                asset.Remaining = (int)obj["Remaining"];
            else
                asset.Remaining = -1; // Infinite amount of this item.

            asset.MinimumMembershipLevel = (EMembershipLevel)(int)obj["MinimumMembershipLevel"];
            asset.Is13OrOver = ((int?)obj["ContentRatingTypeId"] == 1);

            return asset;
        }
    }
}
