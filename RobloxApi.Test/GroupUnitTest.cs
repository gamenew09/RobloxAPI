using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Reflection;

namespace RobloxApi.Test
{
    [TestClass]
    public class GroupUnitTest
    {
        [TestMethod]
        public void GetGroupByID()
        {
            Task.Run(async () =>
            {
                Group group = await Group.FromID(TestConstants.TestGroupId); // Ruby Studio

                Assert.IsNotNull(group);

                Type ty = typeof(Group);
                foreach (PropertyInfo info in ty.GetProperties())
                {
                    Console.WriteLine("{0} = {1}", info.Name, info.GetGetMethod().Invoke(group, new object[] { }));
                }
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }

        [TestMethod]
        public void GetGroupRoles()
        {
            Task.Run(async () =>
            {
                Group group = await Group.FromID(TestConstants.TestGroupId); // Ruby Studio

                Assert.IsTrue(group.Roles.Length > 0);

                Console.WriteLine("Role Count: {0}", group.Roles.Length);

                foreach (GroupRole role in group.Roles)
                {
                    Console.WriteLine("Role {0} (Rank Number: {1})", role.Name, role.Rank);
                }
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }

        [TestMethod]
        public void GetGroupAllies()
        {
            Task.Run(async () =>
            {
                Group group = await Group.FromID(TestConstants.TestGroupId); // Dayren Fan Club!

                Group[] allies = await group.GetAllies();

                Assert.IsTrue(allies.Length > 0);

                Console.WriteLine("Allies count: {0}", allies.Length);
                Console.WriteLine("----");
                Type ty = typeof(Group);
                foreach (Group g in allies)
                {
                    Assert.IsTrue(g.Roles.Length > 0);
                    Assert.IsTrue(g.Name.Length > 0);
                    Assert.IsTrue(g.ID > 0);
                    foreach (PropertyInfo info in ty.GetProperties())
                    {
                        Console.WriteLine("{0} = {1}", info.Name, info.GetGetMethod().Invoke(g, new object[] { }));
                    }
                    Console.WriteLine("----");
                }
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }

        [TestMethod]
        public void GetGroupEnemies()
        {
            Task.Run(async () =>
            {
                Group group = await Group.FromID(TestConstants.TestGroupId); // War Clans of ROBLOX

                Group[] enemies = await group.GetEnemies();

                Assert.IsTrue(enemies.Length > 0);

                Console.WriteLine("Enemies count: {0}", enemies.Length);
                Console.WriteLine("----");
                Type ty = typeof(Group);
                foreach (Group g in enemies)
                {
                    Assert.IsTrue(g.Roles.Length > 0);
                    Assert.IsTrue(g.Name.Length > 0);
                    Assert.IsTrue(g.ID > 0);
                    // Descriptions can be empty and there can be no owner as well.
                    foreach (PropertyInfo info in ty.GetProperties())
                    {
                        Console.WriteLine("{0} = {1}", info.Name, info.GetGetMethod().Invoke(g, new object[] { }));
                    }
                    Console.WriteLine("----");
                }
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }

        [TestMethod]
        public void IsUserInGroup()
        {
            Task.Run(async () =>
            {
                Group group = await Group.FromID(1242521);

                Assert.IsNotNull(group);

                User user = await User.FromID(5762824);

                bool isUserInGroup = await group.IsUserInGroup(user);

                Assert.IsTrue(isUserInGroup);

                Console.WriteLine("Is {0} in {1}? {2}", user, group, isUserInGroup);
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }

    }
}
