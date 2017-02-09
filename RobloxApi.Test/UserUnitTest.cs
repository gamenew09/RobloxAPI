using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace RobloxApi.Test
{
    [TestClass]
    public class UserUnitTest
    {
        [TestMethod]
        public void GetUser()
        {
            Task.Run(async () =>
            {
                User user = await User.FromID(TestConstants.TestUserId);

                Assert.IsNotNull(user);

                Console.WriteLine("Id: {0} Username: {1}", user.ID, user.Username);
            }).Wait(TestConstants.MaxMilisecondTimeout);
            
        }

        [TestMethod]
        public void UserCanManageAsset()
        {
            Task.Run(async () =>
            {
                User user = await User.FromID(TestConstants.TestUserId);

                Assert.IsNotNull(user);

                bool canManageAsset = await user.CanManageAsset((Asset)TestConstants.TestAssetId);

#pragma warning disable 0162

                if (TestConstants.ExpectedManageResult)
                    Assert.IsTrue(canManageAsset);
                else
                    Assert.IsFalse(canManageAsset);

#pragma warning restore 0162

                Console.WriteLine("Can \"{0}\" managet asset {1}? {2}", user, TestConstants.TestAssetId, canManageAsset);
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }

        [TestMethod]
        public void UserOwnsAsset()
        {
            Task.Run(async () =>
            {
                User user = await User.FromID(TestConstants.TestUserId);

                Assert.IsNotNull(user);

                bool ownsAsset = await user.OwnsAsset((Asset)TestConstants.TestAssetId);

#pragma warning disable 0162

                if (TestConstants.ExpectedAssetOwnResult)
                    Assert.IsTrue(ownsAsset);
                else
                    Assert.IsFalse(ownsAsset);

#pragma warning restore 0162

                Console.WriteLine("Does \"{0}\" own asset {1}? {2}", user, TestConstants.TestAssetId, ownsAsset);
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }

        [TestMethod]
        public void GetFriendsPage1FromUser()
        {
            Task.Run(async () =>
            {
                User user = await User.FromID(TestConstants.TestUserId);

                Assert.IsNotNull(user);

                FriendList.Page firstPage = await user.FriendList.Get(1);

                Assert.IsNotNull(firstPage);

                Console.WriteLine("1st Page Count:{0}", firstPage);
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }

        [TestMethod]
        public void GetFriendsFromUser()
        {
            Task.Run(async () =>
            {
                User user = await User.FromID(TestConstants.TestUserId);

                Assert.IsNotNull(user);

                FriendList.Page[] friendPages = await user.FriendList.GetPagesAsArray();

                Assert.IsNotNull(friendPages);

                int pageIndex = 1;
                int entryCount = 0;
                foreach (FriendList.Page page in friendPages)
                {
                    Console.WriteLine("Page {0}:", pageIndex);
                    pageIndex++;
                    foreach (FriendList.Entry entry in page)
                    {
                        entryCount++;
                        Console.WriteLine("User {0} ({1}). IsOnline: {2}", entry.User.Username, entry.User.ID, entry.IsOnline);
                    }
                    Console.WriteLine("----");
                }

                Console.WriteLine("Total Pages: {0} Total Count: {1}", pageIndex, entryCount);
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }

        [TestMethod]
        public void GetStatusOfUsers()
        {
            Task.Run(async () =>
            {
                User user = await User.FromID(TestConstants.TestUserId);

                string nonNullStatusUser = await user.GetStatus();

                Assert.IsNotNull(nonNullStatusUser);

                user = await User.FromID(5762824);

                string nullStatusUser = await user.GetStatus();

                Assert.IsNull(nullStatusUser);

                Console.WriteLine("Non-Null Status: {0}\nNull Status: {1}", nonNullStatusUser, nullStatusUser);
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }

        [TestMethod]
        public void GetScrapedFriendCount()
        {
            Task.Run(async () =>
            {
                User user = await User.FromID(TestConstants.TestUserId);

                Assert.IsNotNull(user);

                int friendCount = await user.GetScrapedFriendCount();

                Assert.IsTrue(friendCount > 0);

                Console.WriteLine("User \"{0}\" Friend Count: {1}", user, friendCount);
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }

        [TestMethod]
        public void GetScrapedFollowerCount()
        {
            Task.Run(async () =>
            {
                User user = await User.FromID(TestConstants.TestUserId);

                Assert.IsNotNull(user);

                int friendCount = await user.GetScrapedFollowerCount();

                Assert.IsTrue(friendCount > 0);

                Console.WriteLine("User \"{0}\" Follower Count: {1}", user, friendCount);
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }

        [TestMethod]
        public void IsUserFriendsWith()
        {
            Task.Run(async () =>
            {
                User user = await User.FromID(TestConstants.TestUserId);

                Assert.IsNotNull(user);

                User friendTestUser = await User.FromID(5762824);

                Console.WriteLine("User {0} friends with {1}? {2}", user, friendTestUser, await user.IsFriendsWith(friendTestUser));
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }

        [TestMethod]
        public void GetUserMembership()
        {
            Task.Run(async () =>
            {
                User user = await User.FromID(TestConstants.TestUserId);

                Assert.IsNotNull(user);

                Console.WriteLine("User {0}'s membership level: {1}", user.Username, await user.GetMembershipLevel());
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }
    }
}
