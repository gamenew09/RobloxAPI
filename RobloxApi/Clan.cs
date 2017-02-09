using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RobloxApi
{
    public class Clan
    {

        internal Clan()
        {

        }

        /// <summary>
        /// The ID of the clan.
        /// </summary>
        public int ID
        {
            get;
            internal set;
        }

        /// <summary>
        /// The Name of the clan.
        /// </summary>
        public string Name
        {
            get;
            internal set;
        }

        /// <summary>
        /// The asset/image that is associated with this clan.
        /// </summary>
        public Asset EmblemAsset
        {
            get;
            internal set;
        }

        public override string ToString()
        {
            return string.Format("RobloxClan ({0}): ID: {1} Name: {2} Icon Asset: {3}", GetHashCode(), ID, Name, EmblemAsset);
        }

        /// <summary>
        /// Gets a Clan object from a group/clan ID.
        /// </summary>
        /// <param name="clanId">The clan ID to get.</param>
        /// <returns>The clan object.</returns>
        public static async Task<Clan> FromID(int clanId)
        {
            try
            {
                string data = await HttpHelper.GetStringFromURL(string.Format("https://api.roblox.com/clans/get-by-id?clanId={0}", clanId));

                JObject obj = JObject.Parse(data);
                Clan clan = new Clan();
                clan.ID = clanId;

                clan.Name = obj.Value<string>("Name");

                clan.EmblemAsset = await Asset.FromID(obj.Value<int>("EmblemAssetId"));

                return clan;
            }
            catch(WebException ex)
            {
                HttpWebResponse resp = (HttpWebResponse)ex.Response;
                return null;
            }
        }

    }
}
