using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobloxApi
{
    public class Group
    {

        [JsonProperty("Id")]
        public int ID
        {
            get;
            private set;
        }

        [JsonProperty("Roles")]
        public GroupRole[] Roles
        {
            get;
            private set;
        }
        
        [JsonProperty("Name")]
        public string Name
        {
            get;
            private set;
        }

        [JsonProperty("Owner")]
        public User Owner
        {
            get;
            private set;
        }

        [JsonProperty("EmblemUrl")]
        public string EmblemUrl
        {
            get;
            private set;
        }

        [JsonProperty("Description")]
        public string Description
        {
            get;
            private set;
        }

        private Group()
        {

        }

        public Group(int groupId)
        {
            ID = groupId;
        }

        public static async Task<Group> FromID(int groupId)
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

            for(int i = 0; i < group.Roles.Length; i++)
            {
                JObject o = (JObject)roles[i];
                group.Roles[i] = new GroupRole((string)o["Name"], (int)o["Rank"]);
            }

            return group;
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

    }

    public class GroupRole
    {
        public string Name
        {
            get;
            private set;
        }

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
