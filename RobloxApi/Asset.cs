using HtmlAgilityPack;
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

    /// <summary>
    /// Type of creator.
    /// </summary>
    public enum ECreatorType
    {
        Unknown = -1,
        User,
        Group,
    }

    /// <summary>
    /// Size of thumbnails supported by ROBLOX.
    /// </summary>
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
        public static explicit operator int(Asset asset)
        {
            return asset.ID;
        }

        public static explicit operator Asset(int assetId)
        {
            return new Asset(assetId);
        }

        public override string ToString()
        {
            return string.Format("RobloxAsset ({0}): ID: {1} Name: {2}", GetHashCode(), ID, Name);
        }

        /// <summary>
        /// The ID of the Asset.
        /// </summary>
        [JsonProperty("AssetId")]
        public int ID { get; internal set; }
        public int ProductId { get; internal set; }
        /// <summary>
        /// The name shown on the website.
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// The description shown on the website.
        /// </summary>
        public string Description { get; internal set; }
        /// <summary>
        /// What type of asset is this?
        /// <see cref="EAssetType"/>
        /// </summary>
        public EAssetType AssetType { get; internal set; }
        /// <summary>
        /// The creator, as a user, of the asset, will be null if it is a group that created this asset.
        /// </summary>
        public User UserCreator { get; internal set; }
        /// <summary>
        /// The creator, as a group, of the asset, will be null if it is a user that created this asset.
        /// </summary>
        public Group GroupCreator { get; internal set; }
        /// <summary>
        /// What type of creator made this? User or Group.
        /// </summary>
        public ECreatorType CreatorType { get; internal set; }
        /// <summary>
        /// Game Icon Image?
        /// </summary>
        public int IconImageAssetId { get; internal set; }
        /// <summary>
        /// When was this asset created?
        /// </summary>
        public DateTime Created { get; internal set; }
        /// <summary>
        /// When was this asset last updated?
        /// </summary>
        public DateTime Updated { get; internal set; }
        /// <summary>
        /// How much does this asset cost?
        /// </summary>
        public int PriceInRobux { get; internal set; }

        // PriceInTickets is no longer used.

        /// <summary>
        /// How many times did this asset sell?
        /// </summary>
        public int Sales { get; internal set; }
        /// <summary>
        /// Does the website consider this item as new?
        /// </summary>
        public bool IsNew { get; internal set; }
        /// <summary>
        /// Is the item currently for sale?
        /// </summary>
        public bool IsForSale { get; internal set; }
        /// <summary>
        /// Can the asset be opened in studio?
        /// </summary>
        public bool IsPublicDomain { get; internal set; }
        /// <summary>
        /// Is the item limited?
        /// </summary>
        public bool IsLimited { get; internal set; }
        /// <summary>
        /// Is the item limited unique? (limitedu)
        /// </summary>
        public bool IsLimitedUnique { get; internal set; }
        /// <summary>
        /// How many of this item is remaining? This value is -1 if this has unlimited stock?
        /// </summary>
        public int Remaining { get; internal set; }
        /// <summary>
        /// What is the minimum membership level to get this membership.
        /// </summary>
        public EMembershipLevel MinimumMembershipLevel { get; internal set; }
        //public int ContentRatingTypeId { get; private set; }
        /// <summary>
        /// Is this asset only for 13 year olds and above?
        /// </summary>
        public bool Is13OrOver { get; internal set; }

        public Asset()
        {

        }

        public enum EItemExclusivity
        {
            None,

            AppleIOS,
            GooglePlay,
            AmazonAppstore,

            XboxOne,
        }

        /// <summary>
        /// Gets what platform this item is for.
        /// </summary>
        /// <returns>What platform this item is for.</returns>
        public async Task<EItemExclusivity> GetExclusivity()
        {
            try
            {
                string url = string.Format("https://www.roblox.com/catalog/{0}/{1}", ID, Name);

                string data = await HttpHelper.GetStringFromURL(url);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(data);

                HtmlNode node = doc.DocumentNode
                    .Descendants("div")
                    .Where(d =>
                        d.Attributes.Contains("class") &&
                        d.Attributes["class"].Value.Contains("item-note") &&
                        d.Attributes["class"].Value.Contains("has-price-label")
                    ).FirstOrDefault();

                if (node == null)
                    return EItemExclusivity.None;

                if (node.InnerText.Contains("Google Play"))
                    return EItemExclusivity.GooglePlay;
                if (node.InnerText.Contains("Xbox One"))
                    return EItemExclusivity.XboxOne;
                if (node.InnerText.Contains("Amazon"))
                    return EItemExclusivity.AmazonAppstore;
                if (node.InnerText.ToLower().Contains("ios"))
                    return EItemExclusivity.AppleIOS;

                return EItemExclusivity.None;
            }
            catch { return EItemExclusivity.None; }
        }

        /// <summary>
        /// Gets the asset data from https://assetgame.roblox.com/Asset/?id=
        /// </summary>
        /// <returns>The asset downloaded as a string</returns>
        public async Task<string> DownloadAsString()
        {
            return await HttpHelper.GetStringFromURL("https://assetgame.roblox.com/Asset/?id=" + ID);
        }

        /// <summary>
        /// Does the user id provided have the asset?
        /// </summary>
        /// <param name="userId">The user to check.</param>
        /// <returns>Does the user id provided have the asset?</returns>
        public async Task<bool> DoesUserHave(int userId)
        {
            return await HttpHelper.GetStringFromURL(string.Format("http://api.roblox.com/Ownership/HasAsset?userId={0}&assetId={1}", userId, ID)) == "true";
        }

        /// <summary>
        /// Sets the AssetId.
        /// </summary>
        /// <param name="assetId">The assetId to set.</param>
        public Asset(int assetId)
        {
            ID = assetId;
        }

        /// <summary>
        /// Gets the asset thumbnail url for a certain size.
        /// </summary>
        /// <param name="size">The size to get the thumbnail for.</param>
        /// <returns>The thumbnail to the image with the size.</returns>
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
            
            return await HttpHelper.GetStringFromURL(string.Format("https://www.roblox.com/Thumbs/RawAsset.ashx?assetId={0}&imageFormat=png&width={1}&height={2}", ID, sizeStr.Split('x')[0], sizeStr.Split('x')[1]));
        }

        /// <summary>
        /// Downloads the asset's thumbnail
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
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
        /// If this asset is a package, get the assets within the package.
        /// </summary>
        /// <returns>Assets within this package.</returns>
        public async Task<Asset[]> GetAssetsInPackage()
        {
            try
            {
                string data = await HttpHelper.GetStringFromURL(string.Format("http://assetgame.roblox.com/Game/GetAssetIdsForPackageId?packageId={0}", ID));

                if (data.StartsWith("{\""))
                    return null;

                JArray obj = JArray.Parse(data);

                List<Asset> assets = new List<Asset>();

                foreach (JToken tok in obj)
                    assets.Add(await FromID(tok.Value<int>()));

                return assets.ToArray();
            }
            catch { return null; }
        }

        /// <summary>
        /// Gets an <see cref="Asset"/> with information filled in.
        /// </summary>
        /// <param name="assetId">The asset id to pull from</param>
        /// <returns>Asset filled with information</returns>
        public static async Task<Asset> FromID(int assetId)
        {
            try
            {
                string data = await HttpHelper.GetStringFromURL(string.Format("https://api.roblox.com/marketplace/productinfo?assetId={0}", assetId));
                JObject obj = JObject.Parse(data);

                Asset asset = new Asset();
                asset.ID = (int)obj["AssetId"];
                asset.ProductId = (int?)obj["ProductId"] ?? -1;

                asset.Name = (string)obj["Name"];
                asset.Description = (string)obj["Description"];

                asset.AssetType = (EAssetType)(int)obj["AssetTypeId"]; // eww.

                string creatorType = (string)obj["Creator"]["CreatorType"];

                if (creatorType == "Group")
                {
                    Group group = await Group.FromID((int)obj["Creator"]["CreatorTargetId"]);
                    asset.CreatorType = ECreatorType.Group;
                }
                else
                {
                    User user = new User();
                    user.ID = (int)obj["Creator"]["CreatorTargetId"];
                    user.Username = (string)obj["Creator"]["Name"];
                    asset.CreatorType = ECreatorType.User;
                }

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
            catch(WebException)
            {
                return null;
            }
        }
    }
}
