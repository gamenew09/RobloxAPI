using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using static RobloxApi.FriendList;

namespace RobloxApi
{
    public class User
    {

        public int ID
        {
            get;
            internal set;
        }

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

        public User()
        {

        }

        public User(int userId)
        {
            ID = userId;
        }

        public async Task Update()
        {
            string data = await HttpHelper.GetStringFromURL(string.Format("https://api.roblox.com/users/{0}", ID));

            JObject obj = JObject.Parse(data);

            Username = (string)obj["Username"];
        }

        public async Task<bool> CanManageAsset(Asset asset)
        {
            string data = await HttpHelper.GetStringFromURL(string.Format("https://api.roblox.com/users/{0}/canmanage/{1}", ID, asset.AssetId));
            JObject obj = JObject.Parse(data);
            return obj.Value<bool?>("CanManage") ?? false;
        }

        public static async Task<User> FromID(int userId)
        {
            string data = await HttpHelper.GetStringFromURL(string.Format("https://api.roblox.com/users/{0}", userId));

            JObject obj = JObject.Parse(data);

            User user = new User();
            user.ID = userId;

            await user.Update();

            return user;
        }

        private FriendList _FriendList;

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

            public User User
            {
                get;
                private set;
            }

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
