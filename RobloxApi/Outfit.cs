using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobloxApi
{
    public class Outfit
    {

        private int _ID;
        private string _Name;
        private List<Asset> _Assets;
        private int _HeadColor, _TorsoColor,
            _RightArmColor, _LeftArmColor, _RightLegColor,
            _LeftLegColor;

        public Outfit()
        {

        }

        public Outfit(int outfitId)
        {
            _ID = outfitId;
        }

        /// <summary>
        /// The ID of the Outfit.
        /// </summary>
        public int ID
        {
            get { return _ID; }
        }

        /// <summary>
        /// The name of the Outfit.
        /// </summary>
        public string Name
        {
            get { return _Name; }
        }

        /// <summary>
        /// The assets that are on this Asset.
        /// </summary>
        public List<Asset> Assets
        {
            get { return _Assets; }
        }

        /// <summary>
        /// The head color ID.
        /// </summary>
        public int HeadColor
        {
            get { return _HeadColor; }
        }

        /// <summary>
        /// The torso color ID.
        /// </summary>
        public int TorsoColor
        {
            get { return _TorsoColor; }
        }

        /// <summary>
        /// The right arm color ID.
        /// </summary>
        public int RightArmColor
        {
            get { return _RightArmColor; }
        }

        /// <summary>
        /// The left arm color ID.
        /// </summary>
        public int LeftArmColor
        {
            get { return _LeftArmColor; }
        }

        /// <summary>
        /// The right leg color ID.
        /// </summary>
        public int RightLegColor
        {
            get { return _RightLegColor; }
        }

        /// <summary>
        /// The left leg color ID.
        /// </summary>
        public int LeftLegColor
        {
            get { return _LeftLegColor; }
        }

        private async Task<Outfit> Parse(JObject obj, bool fillAssets = true)
        {
            _ID = obj.Value<int>("id");
            _Name = obj.Value<string>("name");

            _Assets = new List<Asset>();
            foreach(JToken tok in obj.Value<JArray>("assets"))
            {
                if (fillAssets)
                    _Assets.Add(await Asset.FromID(tok.Value<int>("id")));
                else
                    _Assets.Add(new Asset(tok.Value<int>("id")));
            }

            JObject bodyColors = obj.Value<JObject>("bodyColors");

            _HeadColor = bodyColors.Value<int>("headColorId");
            _TorsoColor = bodyColors.Value<int>("torsoColorId");
            _RightArmColor = bodyColors.Value<int>("rightArmColorId");
            _LeftArmColor = bodyColors.Value<int>("leftArmColorId");
            _RightLegColor = bodyColors.Value<int>("rightLegColorId");
            _LeftLegColor = bodyColors.Value<int>("leftLegColorId");

            return this;
        }

        /// <summary>
        /// Gets an outfit object from an outfit ID.
        /// </summary>
        /// <param name="outfitID">The outfit to get.</param>
        /// <returns>The outfit object from ID.</returns>
        public static async Task<Outfit> FromID(int outfitID)
        {
            return await new Outfit().Parse(
                JObject.Parse(
                    await HttpHelper.GetStringFromURL(
                        string.Format(
                            "https://avatar.roblox.com/v1/outfits/{0}/details", 
                            outfitID)
                            )
                        ), 
                true);
        }

    }

    /// <summary>
    /// Finish up the Enum From http://wiki.roblox.com/index.php?raw=true&title=BrickColor_codes.
    /// Over 1000 ids. Eek
    /// </summary>
    public enum EBodyColor
    {
        White = 1,
        Grey = 2,
        LightYellow = 3,
        BrickYellow = 5,
        LightGreen = 6,
        LightReddishViolet = 9,
        PastelBlue = 11,
        LightOrangeBrown = 12,
        Nougat = 18,
        BrightRed = 21,
        MedReddishViolet = 22,
        BrightBlue = 23,
        BrightYellow = 24,
        Black = 26,
        DarkGrey = 27,
        DarkGreen = 28,
        MediumGreen = 29,
        LigYellowishOrange = 36,
        BrightGreen = 37,
        DarkOrange = 38,
        LightBluishViolet,
        Transparent,
        TransparentRed,
        TransparentLgBlue,
        TransparentBlue,
        TransparentYellow,
        LightBlue,
        TransparentFluReddishOrange,
        TransparentGreen,
        TransparentFluGreen,
        PhosphWhite,
        LightRed = 100,
        MediumRed,
        MediumBlue,
        LightGrey,
        BrightViolet,
        BrYellowishOrange,
        BrightOrange,
        BrightBluishGreen,
        EarthYellow,
        BrightBluishViolet = 110,
        TransparentBrown,
        MediumBluishViolet,
        TransparentMediReddishViolet,
        MedYellowishGreen = 115,
        MedBluishGreen,
        LightBluishGreen = 118,
        BrYellowishGreen,
        LigYellowishGreen,
        MedYellowishOrange,
        BrReddishOrange,
        BrightReddishViolet,
        LightOrange,
        TransparentBrightBluishViolet,
        Gold,
        DarkNougat,
        Silver = 131,
        NeonOrange = 133,
        NeonGreen,
        SandBlue,SandViolet,
        MediumOrange,
        Sandyellow
    }
}
