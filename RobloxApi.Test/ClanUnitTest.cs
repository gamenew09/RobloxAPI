using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RobloxApi.Test
{
    [TestClass]
    public class ClanUnitTest
    {
        [TestMethod]
        public void GetClanByID()
        {
            Clan clan = Clan.FromID(TestConstants.TestClanId).WaitForResult(TestConstants.MaxMilisecondTimeout);

            Assert.IsNotNull(clan);

            Console.WriteLine("{0}", clan);
        }

        [TestMethod]
        public void GetClanFromUser()
        {
            User user = User.FromID(TestConstants.TestClanUserId).WaitForResult(TestConstants.MaxMilisecondTimeout);
            Clan clan = user.GetClan().WaitForResult(TestConstants.MaxMilisecondTimeout);

            Assert.IsNotNull(clan);

            Console.WriteLine("Clan from User {0}: {1}", user, clan);
        }

        [TestMethod]
        public void GetClanFromGroup()
        {
            Group group = Group.FromID(TestConstants.TestClanId).WaitForResult(TestConstants.MaxMilisecondTimeout);
            Clan clan = group.ToClan().WaitForResult(TestConstants.MaxMilisecondTimeout);

            Assert.IsNotNull(clan);

            Console.WriteLine("Clan from Group {0}: {1}", group, clan);
        }
    }
}
