using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobloxApi
{
    /// <summary>
    /// Search Category
    /// </summary>
    public enum ESearchCategory
    {
        AllInCatalog = 1, // Except Library Search.
        Collectibles = 2,
        Clothing = 3,
        BodyParts = 4,
        Gear = 5,
        // Library Search
        Models = 6,
        Plugins = 7,
        Decals = 8,
        Audio = 9,
        Mesh = 10
    }
}
