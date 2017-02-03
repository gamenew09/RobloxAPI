using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using static RobloxApi.FriendList;
using Newtonsoft.Json;
using System.Net;

namespace RobloxApi
{
    public class User
    {

        /// <summary>
        /// The ID of this user.
        /// </summary>
        [JsonProperty("Id")]
        public int ID
        {
            get;
            internal set;
        }

        /// <summary>
        /// The username of this user.
        /// </summary>
        [JsonProperty("Name")]
        public string Username
        {
            get;
            internal set;
        }

        public static explicit operator int(User usr)
        {
            return usr.ID;
        }

        public static explicit operator User(int usrId)
        {
            return new User(usrId);
        }

        public override string ToString()
        {
            return string.Format("RobloxUser ({0}): ID: {1} Name: {2}", GetHashCode(), ID, Username);
        }

        public User()
        {

        }

        public User(int userId)
        {
            ID = userId;
        }

        /// <summary>
        /// Updates the Username.
        /// </summary>
        /// <returns></returns>
        public async Task Update()
        {
            string data = await HttpHelper.GetStringFromURL(string.Format("https://api.roblox.com/users/{0}", ID));

            JObject obj = JObject.Parse(data);

            Username = (string)obj["Username"];
        }

        /// <summary>
        /// Can this user manage the asset provided?
        /// </summary>
        /// <param name="asset">The asset to test.</param>
        /// <returns>Can this user manage the asset?</returns>
        public async Task<bool> CanManageAsset(Asset asset)
        {
            string data = await HttpHelper.GetStringFromURL(string.Format("https://api.roblox.com/users/{0}/canmanage/{1}", ID, asset.ID));
            JObject obj = JObject.Parse(data);
            return obj.Value<bool?>("CanManage") ?? false;
        }

        /// <summary>
        /// Gets a user object from a user id.
        /// </summary>
        /// <param name="userId">The user id for the user</param>
        /// <returns>The user object</returns>
        public static async Task<User> FromID(int userId)
        {
            try
            {
                string data = await HttpHelper.GetStringFromURL(string.Format("https://api.roblox.com/users/{0}", userId));

                JObject obj = JObject.Parse(data);

                User user = new User();
                user.ID = userId;

                await user.Update();

                return user;
            }
            catch (WebException)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the membership level of this user. 
        /// <para>See <see cref="EMembershipLevel"/> </para>
        /// </summary>
        /// <returns></returns>
        public async Task<EMembershipLevel> GetMembershipLevel()
        {
            // https://www.roblox.com/Thumbs/BCOverlay.ashx?username=Shedletsky
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(string.Format("https://www.roblox.com/Thumbs/BCOverlay.ashx?username={0}", Username));
            req.AllowAutoRedirect = true; // We need this so we can get the url of the overlay image.

            HttpWebResponse resp = (HttpWebResponse)await req.GetResponseAsync();

            if (resp.ResponseUri.OriginalString == "https://static.rbxcdn.com/images/empty.png")
                return EMembershipLevel.None;

            string image = resp.ResponseUri.OriginalString.Substring(47).Replace("Only.png", "");

            switch (image)
            {
                case "bc":
                    return EMembershipLevel.BuildersClub;
                case "tbc":
                    return EMembershipLevel.TurboBuildersClub;
                case "obc":
                    return EMembershipLevel.OutrageousBuildersClub;
                default:
                    return EMembershipLevel.None;
            }
        }

        /// <summary>
        /// Is the user friends with another user specified?
        /// </summary>
        /// <param name="user">The user to check our friendship with.</param>
        /// <returns>Is this user friends with the specified user?</returns>
        public async Task<bool> IsFriendsWith(User user)
        {
            try
            {
                return (await HttpHelper.GetStringFromURL(string.Format("https://assetgame.roblox.com/Game/LuaWebService/HandleSocialRequest.ashx?method=IsFriendsWith&playerId={0}&userId={1}", ID, user.ID))).Contains("true");
            }
            catch { return false; }
        }

        /// <summary>
        /// Gets the clan the user is in, returns null if the user is not in a clan.
        /// </summary>
        /// <returns>Gets the clan the user is in, returns null if the user is not in a clan.</returns>
        public async Task<Clan> GetClan()
        {
            string data = await HttpHelper.GetStringFromURL(string.Format("https://api.roblox.com/clans/get-by-user?userId={0}", ID));

            JObject obj = JObject.Parse(data);

            if(obj.Value<int>("Id") == 0) // User is not in a clan.
            {
                return null;
            }

            Clan clan = new Clan();
            clan.ID = obj.Value<int>("Id");

            clan.Name = obj.Value<string>("Name");

            clan.EmblemAsset = await Asset.FromID(obj.Value<int>("EmblemAssetId"));
            return clan;
        }

        private FriendList _FriendList;

        /// <summary>
        /// Checks if the user owns the asset provided.
        /// </summary>
        /// <param name="asset">The asset to check.</param>
        /// <returns>Does the user own the asset?</returns>
        public async Task<bool> OwnsAsset(Asset asset)
        {
            // https://api.roblox.com/ownership/hasasset?userId={0}&assetId={1}
            string data = await HttpHelper.GetStringFromURL(string.Format("https://api.roblox.com/ownership/hasasset?userId={0}&assetId={1}", ID, asset.ID));
            return data.ToLower() == "true";
        }

        /// <summary>
        /// Friends List for this user.
        /// </summary>
        public FriendList FriendList
        {
            get
            {
                if(_FriendList == null)
                    _FriendList = new FriendList(this);
                return _FriendList;
            }
        }

    }

    public class FriendList : IEnumerable<Page>
    {

        public class Page : IEnumerable<Entry>
        {

            internal Page()
            {
                _Entries = new List<Entry>();
            }

            internal void Add(Entry entry)
            {
                _Entries.Add(entry);
            }

            /// <summary>
            /// Is the friend count equal or above 50?
            /// </summary>
            public bool MightHaveAnotherPage
            {
                get { return _Entries.Count >= 50; }
            }

            private List<Entry> _Entries;

            public Entry this[int index]
            {
                get { return Get(index); }
            }

            public int Count
            {
                get { return _Entries.Count; }
            }

            public Entry Get(int index)
            {
                return _Entries[index];
            }

            public IEnumerator<Entry> GetEnumerator()
            {
                return _Entries.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public bool ContainsUserID(int userId)
            {
                foreach(Entry entry in _Entries)
                    if (entry.User.ID == userId)
                        return true;
                return false;
            }
        }

        public class Entry
        {

            public static explicit operator User(Entry entry)
            {
                return entry.User;
            }

            public static explicit operator int(Entry entry)
            {
                return entry.User.ID;
            }

            /// <summary>
            /// The user that is friends with another user.
            /// </summary>
            public User User
            {
                get;
                private set;
            }

            /// <summary>
            /// Was the user online as of the friends being refreshed?
            /// </summary>
            public bool IsOnline
            {
                get;
                private set;
            }

            internal Entry(string username, int userId, bool isOnline)
            {
                User user = new User();
                user.ID = userId;
                user.Username = username;

                User = user;

                IsOnline = isOnline;
            }

        }

        public FriendList(User user)
        {
            User = user;
        }

        public User User
        {
            get;
            private set;
        }

        internal Dictionary<int, Page> _Pages = new Dictionary<int, Page>();

        public async Task<Page> Get(int page)
        {
            if(page <= 0)
            {
                throw new ArgumentException("Argument should be above 0.", "page");
            }
            if (_Pages.ContainsKey(page))
                return _Pages[page];

            string data = await HttpHelper.GetStringFromURL(string.Format("https://api.roblox.com/users/{0}/friends?page={1}", User.ID, page));

            JArray arr = JArray.Parse(data);

            Page pg = new Page();

            foreach (JObject obj in arr)
                pg.Add(new Entry((string)obj["Username"], (int)obj["Id"], (bool)obj["IsOnline"]));

            _Pages.Add(page, pg);

            return pg;
        }

        /// <summary>
        /// Iterates through pages using <see cref="Get(int)"/>.
        /// 
        /// This will allow you to iterate through the friends list using foreach.
        /// </summary>
        /// <returns>Page Count</returns>
        public async Task<int> LoadPages()
        {
            int page = 1;
            try // There could be a better way, but this will do for now.
            {
                while(page <= 10000) // 500000k friend max per user.
                {
                    Page pg = await Get(page);
                    if (pg.Count == 0 || pg.Count < 50)
                        break;
                    page++;
                }
            }
            catch { }
            return page;
        }

        /// <summary>
        /// Calls <see cref="LoadPages"/> and then converts <see cref="_Pages"/> into an array.
        /// </summary>
        /// <returns>Friend List as Page[].</returns>
        public async Task<Page[]> GetPagesAsArray()
        {
            int count = await LoadPages();
            Page[] pages = new Page[count];
            for(int i = 1; i <= count; i++)
                pages[i - 1] = await Get(i);

            return pages;
        }

        public IEnumerator<Page> GetEnumerator()
        {
            return new FriendsListEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class FriendsListEnumerator : IEnumerator<Page>
        {
            private FriendList _List;

            private int _Page;

            public FriendsListEnumerator(FriendList list)
            {
                _List = list;
            }

            public Page Current
            {
                get
                {
                    return _List._Pages[_Page];
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
                _List = null;
            }

            public bool MoveNext()
            {
                if(_List._Pages.Count - 1 >= _Page)
                    return false;

                _Page++;
                return true;
            }

            public void Reset()
            {
                _Page = 0;
            }
        }
    }
}
