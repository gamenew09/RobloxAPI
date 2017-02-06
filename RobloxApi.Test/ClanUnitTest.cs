using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace RobloxApi.Test
{
    [TestClass]
    public class ClanUnitTest
    {
        [TestMethod]
        public void GetClanByID()
        {
            Task.Run(async () =>
            {
                Clan clan = await Clan.FromID(TestConstants.TestClanId);

                Assert.IsNotNull(clan);

                Console.WriteLine("{0}", clan);
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }

        [TestMethod]
        public void GetClanFromUser()
        {
            Task.Run(async () =>
            {
                User user = await User.FromID(TestConstants.TestClanUserId);
                Clan clan = await user.GetClan();

                Assert.IsNotNull(clan);

                Console.WriteLine("Clan from User {0}: {1}", user, clan);
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }

        [TestMethod]
        public void GetClanFromGroup()
        {
            Task.Run(async () =>
            {
                Group group = await Group.FromID(TestConstants.TestClanId);
                Clan clan = await group.ToClan();

                Assert.IsNotNull(clan);

                Console.WriteLine("Clan from Group {0}: {1}", group, clan);
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }
    }
}
