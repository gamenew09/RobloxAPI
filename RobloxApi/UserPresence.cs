using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace RobloxApi
{
    /// <summary>
    /// Locations used in <see cref="UserPresence.LocationType"/>
    /// </summary>
    public enum EPresenceLocation
    {
        // 6: Creating in Team Create
        // 5: ?
        // 4: Playing
        // 3: Creating
        // 2: Online
        // 1: ?
        // null (0): Offline
        /// <summary>
        /// The user is offline.
        /// </summary>
        Offline,
        /// <summary>
        /// Not sure what this is, if you know what this is, create an issue in the github repository: https://github.com/gamenew09/RobloxAPI
        /// </summary>
        Unknown1,
        /// <summary>
        /// The user is online on the website/app.
        /// </summary>
        Online,
        /// <summary>
        /// The user is in studio, not in a Team Create server.
        /// </summary>
        Creating,
        /// <summary>
        /// The user is playing a game, you may be able to get the game and placeid they are playing by using <see cref="UserPresence.GameId"/> and <see cref="UserPresence.PlaceId"/>.
        /// </summary>
        Playing,
        /// <summary>
        /// Not sure what this is, if you know what this is, create an issue in the github repository: https://github.com/gamenew09/RobloxAPI
        /// </summary>
        Unknown2,
        /// <summary>
        /// The user is in studio, in a Team Create server.
        /// </summary>
        TeamCreate
    }

    public class UserPresence
    {

        /// <summary>
        /// Gets presences for the list of user ids.
        /// </summary>
        /// <param name="userIds">The user ids to look up</param>
        /// <returns><see cref="UserPresence"/> array for userids.</returns>
        public static async Task<UserPresence[]> GetPresencesFor(params int[] userIds)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            string userIdsStr = "";
            for (int i = 0; i < userIds.Length; i++)
            {
                userIdsStr += userIds[i] + (i == userIds.Length - 1 ? "" : ",");
            }
            data.Add("userIds", userIdsStr);
            JObject obj = JObject.Parse(await HttpHelper.PostAndGetStringFromURL("https://api.roblox.com/users/online-status", data));
            JArray arr = obj["UserPresences"].ToObject<JArray>();

            List<UserPresence> presences = new List<UserPresence>();

            foreach (JObject o in arr)
            {
                UserPresence p = new UserPresence();

                if (o["GameId"].ToObject<object>() == null)
                    p._GameId = 0;
                else
                    p._GameId = o["GameId"].ToObject<int>();

                p._IsOnline = o["IsOnline"].ToObject<bool>();
                p._LastLocation = o["LastLocation"].ToObject<string>();
                p._LastOnline = o["LastOnline"].ToObject<string>();

                if (o["LocationType"].ToObject<object>() == null)
                    p._LocationType = EPresenceLocation.Offline;
                else
                    p._LocationType = (EPresenceLocation)o["LocationType"].ToObject<int>();

                if (o["PlaceId"].ToObject<object>() == null)
                    p._PlaceId = 0;
                else
                    p._PlaceId = o["PlaceId"].ToObject<int>();

                p._VisitorId = o["VisitorId"].ToObject<int>();

                presences.Add(p);
            }
            return presences.ToArray();
        }

        private int _GameId;
        private bool _IsOnline;
        private string _LastLocation;
        private string _LastOnline;
        private EPresenceLocation _LocationType;
        private int _PlaceId;
        private int _VisitorId;

        /// <summary>
        /// The game id of the place the user is in. (Universe ID?)
        /// </summary>
        public int GameId { get { return _GameId; } }
        /// <summary>
        /// Is the visitor online.
        /// </summary>
        public bool IsOnline { get { return _IsOnline; } }
        /// <summary>
        /// Last Location (Online, Creating, Playing, ...)
        /// </summary>
        public string LastLocation { get { return _LastLocation; } }
        /// <summary>
        /// Last DateTime online.
        /// Currently not sure what time zone this uses. Hopefully PST.
        /// </summary>
        public string LastOnline { get { return _LastOnline; } }
        // LocationType:
        // 6: Creating in Team Create
        // 5: ?
        // 4: Playing
        // 3: Creating
        // 2: Online
        // 1: ?
        // null (0): Offline
        /// <summary>
        /// The presense location of the visitor. Gives a bit more information than LastLocation sometimes.
        /// </summary>
        public EPresenceLocation LocationType { get { return _LocationType; } }
        /// <summary>
        /// The place id being visited currently.
        /// </summary>
        public int PlaceId { get { return _PlaceId; } }
        /// <summary>
        /// The user associated with this presence data.
        /// </summary>
        public int VisitorId { get { return _VisitorId; } }

        /// <summary>
        /// Gets a User object based on the <see cref="VisitorId"/>.
        /// </summary>
        /// <returns>User object from <see cref="VisitorId"/></returns>
        public async Task<User> GetVisitorAsUser()
        {
            return await User.FromID(VisitorId);
        }

        /// <summary>
        /// Gets the place (as an asset) the <see cref="VisitorId"/> is in currently.
        /// </summary>
        /// <returns>Place asset that <see cref="VisitorId"/> is in.</returns>
        public async Task<Asset> GetPlaceAsAsset()
        {
            if (PlaceId <= 0) // Make sure the asset id is somewhat valid. (Asset IDs cannot be below 0)
                return null;
            return await Asset.FromID(PlaceId);
        }

    }
}