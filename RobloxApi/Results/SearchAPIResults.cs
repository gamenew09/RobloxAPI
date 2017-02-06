using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobloxApi.Results
{
    // https://search.roblox.com/catalog/json?Category=9&Keyword=pendulum%20fasten
    public class SearchResult
    {
        public int AssetId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AbsoluteUrl { get; set; }
        public string Price { get; set; }
        public string Updated { get; set; }
        public string Favorited { get; set; }
        public string Sales { get; set; }
        public string Remaining { get; set; }
        public string Creator { get; set; }
        public string CreatorAbsoluteUrl { get; set; }
        public string PrivateSales { get; set; }
        public int PriceView { get; set; }
        public string BestPrice { get; set; }
        public int ContentRatingTypeID { get; set; }
        public bool IsServerSideThumbnailLookupInCatalogEnabled { get; set; }
        public string AudioUrl { get; set; }
        public bool IsLargeItem { get; set; }
        public bool IsThumbnailFinal { get; set; }
        public bool IsThumbnailUnapproved { get; set; }
        public string ThumbnailUrl { get; set; }
        public string BcOverlayUrl { get; set; }
        public string LimitedOverlayUrl { get; set; }
        public string DeadlineOverlayUrl { get; set; }
        public string LimitedAltText { get; set; }
        public string NewOverlayUrl { get; set; }
        public string SaleOverlayUrl { get; set; }
        public string IosOverlayUrl { get; set; }
        public string XboxOverlayUrl { get; set; }
        public string GooglePlayOverlayUrl { get; set; }
        public string AmazonOverlayUrl { get; set; }
        public bool IsTransparentBackground { get; set; }
        public int AssetTypeID { get; set; }
        public int CreatorID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsForSale { get; set; }
        public bool IsPublicDomain { get; set; }
        public bool IsLimited { get; set; }
        public bool IsLimitedUnique { get; set; }
        public int MinimumMembershipLevel { get; set; }
        public object OffSaleDeadline { get; set; }
        public int ProductId { get; set; }

        public async Task<Asset> ToAsset()
        {
            if (AssetId == 0)
                return null;
            return await Asset.FromID(AssetId);
        }
    }
}
