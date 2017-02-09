using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobloxApi.Test
{
    /// <summary>
    /// Most of the constants that the Unit Test uses, there are some values that can be changed that aren't in this class yet.
    /// </summary>
    public static class TestConstants
    {

        /// <summary>
        /// How long can the task run for until it times out.
        /// </summary>
        public const int MaxMilisecondTimeout = 10000; // 10 seconds.

        public const int TestUserId = 18586528; // Chris12902
        public const int TestGroupId = 7013; // The Roblox Assault Team
        public const int TestAssetId = 1818; // Classic: Crossroads
        public const int TestClanId = 790734; // VenturianTale
        public const int TestClanUserId = 648307; // issacfrye

        public const bool ExpectedManageResult = false;
        public const bool ExpectedAssetOwnResult = false;

        public const string TestSearchKeyword = "test";

    }
}
