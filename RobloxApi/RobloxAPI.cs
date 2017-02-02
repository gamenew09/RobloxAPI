using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RobloxApi.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RobloxApi
{
    public static class RobloxAPI
    {

        /// <summary>
        /// Searches using keywords and category.
        /// </summary>
        /// <param name="keywords">Keywords for search</param>
        /// <param name="category">What category.</param>
        /// <returns>The results of the search as a async task.</returns>
        public static async Task<List<SearchResult>> Search(string keywords, ESearchCategory category)
        {
            string url = string.Format("https://search.roblox.com/catalog/json?Category={0}&Keywords={1}", (int)category, System.Uri.EscapeDataString(keywords));

            string data = await HttpHelper.GetStringFromURL(url);

            List<SearchResult> results = JsonConvert.DeserializeObject<List<SearchResult>>(data);

            Console.WriteLine(results.Count);

            return results;
        }

    }
}
