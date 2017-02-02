using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RobloxApi
{
    public class Group
    {

        /// <summary>
        /// The ID of the group.
        /// </summary>
        [JsonProperty("Id")]
        public int ID
        {
            get;
            private set;
        }

        /// <summary>
        /// Roles in a group as an array.
        /// </summary>
        [JsonProperty("Roles")]
        public GroupRole[] Roles
        {
            get;
            private set;
        }
        
        /// <summary>
        /// The name of the group as shown on the website.
        /// </summary>
        [JsonProperty("Name")]
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// The owner of the group as a User, will be null if no one owns the group.
        /// </summary>
        [JsonProperty("Owner")]
        public User Owner
        {
            get;
            private set;
        }
        
        /// <summary>
        /// The icon of the group.
        /// </summary>
        [JsonProperty("EmblemUrl")]
        public string EmblemUrl
        {
            get;
            private set;
        }

        /// <summary>
        /// The description of the group as shown on the website. Can be empty.
        /// </summary>
        [JsonProperty("Description")]
        public string Description
        {
            get;
            private set;
        }

        private Group()
        {

        }

        /// <summary>
        /// Gets the clan of this group if any, will be null if no clan exists for this group.
        /// </summary>
        /// <returns>Gets the clan of this group if any, will be null if no clan exists for this group.</returns>
        public async Task<Clan> ToClan()
        {
            return await Clan.FromID(ID);
        }

        public Group(int groupId)
        {
            ID = groupId;
        }

        public override string ToString()
        {
            return string.Format("RobloxGroup ({0}): ID: {1} Name: {2}", GetHashCode(), ID, Name);
        }

        /// <summary>
        /// Gets a group object using the groupId.
        /// </summary>
        /// <param name="groupId">Group to get</param>
        /// <returns>The group object</returns>
        public static async Task<Group> FromID(int groupId)
        {
            try
            {
                Group group = new Group();
                group.ID = groupId;

                string data = await HttpHelper.GetStringFromURL(string.Format("https://api.roblox.com/groups/{0}", groupId));

                JObject obj = JObject.Parse(data);

                group.Name = (string)obj["Name"];

                JObject jOwner = (JObject)obj["Owner"];

                if (jOwner != null)
                {
                    User owner = new User((int)jOwner["Id"]);
                    owner.Username = (string)jOwner["Name"];

                    group.Owner = owner;
                }

                group.EmblemUrl = (string)obj["EmblemUrl"];
                group.Description = (string)obj["Description"];

                JArray roles = obj.Value<JArray>("Roles");

                group.Roles = new GroupRole[roles.Count];

                for (int i = 0; i < group.Roles.Length; i++)
                {
                    JObject o = (JObject)roles[i];
                    group.Roles[i] = new GroupRole((string)o["Name"], (int)o["Rank"]);
                }

                return group;
            }
            catch (WebException)
            {
                return null;
            }
        }

        private struct GroupResult_t
        {
#pragma warning disable 0649 // They actually do get set by using reflection.
            public bool FinalPage;
            public List<Group> Groups;
#pragma warning restore 0649
        }

        private async Task<GroupResult_t> GetAllyPage(int page)
        {
            string data = await HttpHelper.GetStringFromURL(string.Format("https://api.roblox.com/groups/{0}/allies?page={1}", ID, page));

            return JsonConvert.DeserializeObject<GroupResult_t>(data);
        }

        /// <summary>
        /// Gets a list of Allied Groups.
        /// </summary>
        /// <returns>List of allied groups.</returns>
        public async Task<Group[]> GetAllies()
        {
            int c = 1;
            List<Group> allies = new List<Group>();
            while(c < 1000)
            {
                GroupResult_t res = await GetAllyPage(c);
                Console.WriteLine("{0} {1}", res.FinalPage, c);
                foreach (Group g in res.Groups)
                    allies.Add(g);
                c++;
                if (res.FinalPage)
                    break;
            }
            return allies.ToArray();
        }

        private async Task<GroupResult_t> GetEnemyPage(int page)
        {
            string data = await HttpHelper.GetStringFromURL(string.Format("https://api.roblox.com/groups/{0}/enemies?page={1}", ID, page));

            return JsonConvert.DeserializeObject<GroupResult_t>(data);
        }

        /// <summary>
        /// Gets a list of Enemy Groups.
        /// </summary>
        /// <returns>List of Enemy groups.</returns>
        public async Task<Group[]> GetEnemies()
        {
            int c = 1;
            List<Group> allies = new List<Group>();
            while (c < 1000)
            {
                GroupResult_t res = await GetEnemyPage(c);
                allies.AddRange(res.Groups);
                c++;
                if (res.FinalPage)
                    break;
            }
            return allies.ToArray();
        }

        public static explicit operator int(Group group)
        {
            return group.ID;
        }

        public static explicit operator Group(int groupId)
        {
            return new Group(groupId);
        }

    }

    public class GroupRole
    {
        /// <summary>
        /// The name of the group role.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// The rank number shown in the group manager.
        /// </summary>
        public int Rank
        {
            private set;
            get;
        }

        internal GroupRole()
        {

        }

        internal GroupRole(string name, int rank)
        {
            Name = name;
            Rank = rank;
        }
    }
}
