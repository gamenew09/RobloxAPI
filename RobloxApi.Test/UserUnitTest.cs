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
            Task<User> userTask = User.FromID(5762824);

            userTask.Wait();

            User user = userTask.Result;

            Assert.IsNotNull(user);

            Console.WriteLine("Id: {0} Username: {1}", user.ID, user.Username);
        }

        [TestMethod]
        public void UserCanManageAsset()
        {
            Task<User> userTask = User.FromID(5762824);

            userTask.Wait();

            User user = userTask.Result;

            Assert.IsNotNull(user);

            Task<bool> followingTask = user.CanManageAsset(556024903);

            followingTask.Wait();

            Assert.IsTrue(followingTask.Result);

            Console.WriteLine("Is {0} following user ID {1}? {2}", user.Username, 2032622, followingTask.Result);
        }

        [TestMethod]
        public void GetFriendsPage1FromUser()
        {
            Task<User> userTask = User.FromID(18586528);

            userTask.Wait();

            User user = userTask.Result;

            Assert.IsNotNull(user);

            Task<FriendList.Page> followingTask = user.FriendList.Get(1);

            followingTask.Wait();

            Assert.IsNotNull(followingTask.Result);

            Console.WriteLine("1st Page Count:{0}", followingTask.Result.Count);
        }

        [TestMethod]
        public void GetFriendsFromUser()
        {
            Task<User> userTask = User.FromID(18586528);

            userTask.Wait();

            User user = userTask.Result;

            Assert.IsNotNull(user);

            Task<FriendList.Page[]> followingTask = user.FriendList.GetPagesAsArray();

            followingTask.Wait();

            Assert.IsNotNull(followingTask.Result);

            int pageIndex = 1;
            int entryCount = 0;
            foreach(FriendList.Page page in followingTask.Result)
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
        }
    }
}
