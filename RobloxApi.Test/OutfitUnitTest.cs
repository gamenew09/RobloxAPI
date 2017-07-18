using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Reflection;

namespace RobloxApi.Test
{
    [TestClass]
    public class OutfitUnitTest
    {
        [TestMethod]
        public void GetOutfitFromID()
        {
            Task.Run(async () =>
            {
                Outfit outfit = await Outfit.FromID(64117892);

                Type ty = typeof(Outfit);
                foreach (PropertyInfo info in ty.GetProperties())
                {
                    Console.WriteLine("{0} = {1}", info.Name, info.GetGetMethod().Invoke(outfit, new object[] { }));
                }
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }
    }
}
