using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobloxApi.Results;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;

namespace RobloxApi.Test
{
    [TestClass]
    public class SearchAPITest
    {
        [TestMethod]
        public void SearchCatalog()
        {
            Task.Run(async () =>
            {
                List<SearchResult> results = await RobloxAPI.Search(TestConstants.TestSearchKeyword, ESearchCategory.AllInCatalog);

                Assert.IsNotNull(results);

                Assert.IsTrue(results.Count != 0);

                Type ty = typeof(SearchResult);
                foreach (SearchResult res in results)
                {
                    foreach (PropertyInfo info in ty.GetProperties())
                    {
                        Console.WriteLine("{0} = {1}", info.Name, info.GetGetMethod().Invoke(res, new object[] { }));
                    }
                    Console.WriteLine("---");
                }
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }

        [TestMethod]
        public void SearchCatalogAsAssets()
        {
            Task.Run(async () =>
            {
                Asset[] results = await RobloxAPI.SearchAssets(TestConstants.TestSearchKeyword, ESearchCategory.AllInCatalog);

                Assert.IsNotNull(results);

                Assert.IsTrue(results.Length > 0);

                Type ty = typeof(Asset);
                foreach (Asset res in results)
                {
                    foreach (PropertyInfo info in ty.GetProperties())
                    {
                        Console.WriteLine("{0} = {1}", info.Name, info.GetGetMethod().Invoke(res, new object[] { }));
                    }
                    Console.WriteLine("---");
                }
            }).Wait(TestConstants.MaxMilisecondTimeout);
        }


    }
}
