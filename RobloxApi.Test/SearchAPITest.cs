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
            List<SearchResult> results = RobloxAPI.Search(TestConstants.TestSearchKeyword, ESearchCategory.AllInCatalog).WaitForResult(TestConstants.MaxMilisecondTimeout);

            Assert.IsNotNull(results);

            Assert.IsTrue(results.Count != 0);

            Type ty = typeof(SearchResult);
            foreach (SearchResult res in results)
            {
                foreach(PropertyInfo info in ty.GetProperties())
                {
                    Console.WriteLine("{0} = {1}", info.Name, info.GetGetMethod().Invoke(res, new object[] { }));
                }
                Console.WriteLine("---");
            }
        }

        [TestMethod]
        public void SearchCatalogAsAssets()
        {
            Asset[] results = RobloxAPI.SearchAssets(TestConstants.TestSearchKeyword, ESearchCategory.AllInCatalog).WaitForResult(TestConstants.MaxMilisecondTimeout);

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
        }


    }
}
