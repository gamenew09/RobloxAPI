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

        public int ID
        {
            get;
            internal set;
        }

        public string Name
        {
            get;
            internal set;
        }

        public Asset EmblemAsset
        {
            get;
            internal set;
        }

        public override string ToString()
        {
            return string.Format("RobloxClan ({0}): ID: {1} Name: {2} Icon Asset: {3}", GetHashCode(), ID, Name, EmblemAsset);
        }

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
