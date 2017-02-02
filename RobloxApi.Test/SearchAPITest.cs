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
            Task<List<SearchResult>> results = RobloxAPI.Search("test", ESearchCategory.AllInCatalog);

            results.Wait(10000);

            Assert.IsNotNull(results.Result);

            Assert.IsTrue(results.Result.Count != 0);

            Type ty = typeof(SearchResult);
            foreach (SearchResult res in results.Result)
            {
                foreach(PropertyInfo info in ty.GetProperties())
                {
                    Console.WriteLine("{0} = {1}", info.Name, info.GetGetMethod().Invoke(res, new object[] { }));
                }
                Console.WriteLine("---");
            }
        }

        
    }
}
