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
            Group group = Group.FromID(1242521).WaitForResult(); // Ruby Studio

            Assert.IsNotNull(group);

            Type ty = typeof(Group);
            foreach (PropertyInfo info in ty.GetProperties())
            {
                Console.WriteLine("{0} = {1}", info.Name, info.GetGetMethod().Invoke(group, new object[] { }));
            }
        }

        [TestMethod]
        public void GetGroupRoles()
        {
            Group group = Group.FromID(1242521).WaitForResult(); // Ruby Studio

            Assert.IsTrue(group.Roles.Length > 0);

            Console.WriteLine("Role Count: {0}", group.Roles.Length);

            foreach(GroupRole role in group.Roles)
            {
                Console.WriteLine("Role {0} (Rank Number: {1})", role.Name, role.Rank);
            }
        }

        [TestMethod]
        public void GetGroupAllies()
        {
            Group group = Group.FromID(164852).WaitForResult(); // Dayren Fan Club!

            Group[] allies = group.GetAllies().WaitForResult();

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
        }

        [TestMethod]
        public void GetGroupEnemies()
        {
            Group group = Group.FromID(2722461).WaitForResult(); // War Clans of ROBLOX

            Group[] enemies = group.GetEnemies().WaitForResult();

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
        }

    }

    public static class AsyncHelper
    {
        public static T WaitForResult<T>(this Task<T> t)
        {
            t.Wait();
            return t.Result;
        }
    }
}
