using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ExileCore2;
using ExileCore2.PoEMemory.Components;
using ExileCore2.PoEMemory.Models;
using ExileCore2.Shared.Enums;
using ExileCore2.Shared.Helpers;
using ImGuiNET;
using MoreLinq;
using Newtonsoft.Json;
using Vector2 = System.Numerics.Vector2;

namespace LevelingHelper
{
    public class LevelingHelper : BaseSettingsPlugin<LevelingHelperSettings>
    {
        private static SoundController _soundController;
        private static readonly object Locker = new object();
        private static DateTime _timeLastPlayed = DateTime.UnixEpoch;
        private static string _lastArea = "";
        


        #region ExpTable

        private readonly uint[] ExpTable =
        {
            0,
            525,
            1760,
            3781,
            7184,
            12186,
            19324,
            29377,
            43181,
            61693,
            8599,
            117506,
            157384,
            207736,
            269997,
            346462,
            439268,
            551295,
            685171,
            843709,
            1030734,
            1249629,
            1504995,
            1800847,
            2142652,
            2535122,
            2984677,
            3496798,
            4080655,
            4742836,
            5490247,
            6334393,
            7283446,
            8384398,
            9541110,
            10874351,
            12361842,
            14018289,
            15859432,
            17905634,
            20171471,
            22679999,
            25456123,
            28517857,
            31897771,
            35621447,
            39721017,
            44225461,
            49176560,
            54607467,
            60565335,
            67094245,
            74247659,
            82075627,
            90631041,
            99984974,
            110197515,
            121340161,
            133497202,
            146749362,
            161191120,
            176922628,
            194049893,
            212684946,
            232956711,
            255001620,
            278952403,
            304972236,
            333233648,
            363906163,
            397194041,
            433312945,
            472476370,
            514937180,
            560961898,
            610815862,
            664824416,
            723298169,
            786612664,
            855129128,
            929261318,
            1009443795,
            1096169525,
            1189918242,
            1291270350,
            1400795257,
            1519130326,
            1646943474,
            1784977296,
            1934009687,
            2094900291,
            2268549086,
            2455921256,
            2658074992,
            2876116901,
            3111280300,
            3364828162,
            3638186694,
            3932818530,
            4250334444,
        };

        #endregion


        private class Area
        {
            public Area(string name, int level, float levelWarning)
            {
                Name = name;
                Level = level;
                LevelWarning = levelWarning;
            }

            public string Name { get; set; }
            public int Level { get; set; }
            public float LevelWarning { get; set; }
            public override string ToString()
            {
                return $"Name: {Name}, Level: {Level}, Level Warning: {LevelWarning.ToString("F2")}";
            }


        }

        List<Area> Areas = new List<Area>
        {
           
            
            new Area("Kriar Peaks", 56, 51f),
            new Area("Etched Ravine", 56, 51f),
            new Area("Qimah Reservoir", 56, 51f),
            new Area("Holten Estate", 56, 51f),
            new Area("The Galai Gates", 56, 51f),
            new Area("Qimah", 56, 51f),
            new Area("Wolvenhold", 56, 51f),
            new Area("The Cuachic Vault", 56, 51f),
            new Area("Pools of Khatal", 55, 50f),
            new Area("Sel Khari Sanctuary ", 55, 50f),
            new Area("The Blackwood", 55, 50f),
            new Area("Holten", 55, 50f),
            new Area("Glacial Tarn", 55, 50f),
            new Area("Howling Caves", 55, 50f),
            new Area("Kriar Village", 54, 49f),
            new Area("Scorched Farmlands", 54, 49f),
            new Area("The Khari Crossing", 54, 49f),
            new Area("Ashen Forest", 54, 49f),
            new Area("Stones of Serle", 54, 49f),
            new Area("Kingsmarch", 53, 48f),
            new Area("Plunder's Point", 53, 48f),
            new Area("Ngakanu", 53, 47f),
            new Area("Isle of Decay", 53, 48f),
            new Area("Brinerot Cove", 53, 48f),
            new Area("Heart of the Tribe", 53, 47f),
            new Area("Arastas", 52, 46f),
            new Area("The Excavation", 52, 46f),
            new Area("Trial of the Ancestors", 51, 46f),
            new Area("Journey's End", 47, 42f),
            new Area("Halls of the Dead", 47, 44f),
            new Area("Singing Caverns", 47, 42f),
            new Area("Solitary Confinement", 47, 42f),
            new Area("Volcanic Warrens", 47, 42f),
            new Area("Kedge Bay", 46, 41f),
            new Area("Shoreline Hideout", 46, 41f),
            new Area("Whakapanu Island", 46, 42f),
            new Area("Abandoned Prison", 46, 42f),
            new Area("Eye of Hinekora", 46, 44f),
            new Area("Isle of Kin", 46, 41f),
            new Area("Shrike Island", 46, 43f),
            new Area("The Black Chambers", 45, 40f),
            new Area("Aggorat", 44, 39f),
            new Area("Ziggurat Encampment", 44, 39f),
            new Area("Library of Kamasa", 43, 38f),
            new Area("Utzaal", 43, 38f),
            new Area("Temple of Kopec", 42, 37f),
            new Area("The Molten Vault", 41, 36f),
            new Area("Apex of Filth", 41, 36f),
            new Area("The Drowned City", 40, 35f),
            new Area("The Matlan Waterways", 39, 34f),
            new Area("Jiquani's Sanctum", 38, 33f),
            new Area("The Temple of Chaos", 38, 33f),
            new Area("The Trial of Chaos", 38, 33f),
            new Area("Jiquani's Machinarium", 37, 32f),
            new Area("The Azak Bog", 36, 31f),
            new Area("Chimeral Wetlands", 36, 31f),
            new Area("The Venom Crypts", 35, 31f),
            new Area("Infested Barrens", 35, 31f),
            new Area("Jungle Ruins", 34, 30f),
            new Area("Sandswept Marsh", 33, 29f),
            new Area("The Ardura Caravan", 32, 28f),
            new Area("Dreadnought Vanguard", 32, 27f),
            new Area("The Dreadnought", 31, 26f),
            new Area("The Dreadnought's Wake", 30, 26f),
            new Area("The Spires of Deshar", 30, 25f),
            new Area("Karui Boss Showcase", 30, 26f),
            new Area("Path of Mourning", 29, 25f),
            new Area("Deshar", 28, 24f),
            new Area("Buried Shrines", 23, 20f),
            new Area("Trial of the Sekhemas", 22, 18f),
            new Area("Lightless Passage", 22, 18f),
            new Area("Abyssal Depths", 22, 18f),
            new Area("The Well of Souls", 22, 18f),
            new Area("The Titan Grotto", 22, 23.5f),
            new Area("The Bone Pits", 22, 21.5f),
            new Area("The Lost City", 22, 19f),
            new Area("Valley of the Titans", 21, 22.5f),
            new Area("Mastodon Badlands", 21, 21f),
            new Area("Keth", 21, 18.5f),
            new Area("The Halani Gates", 20, 17.5f),
            new Area("Traitor's Passage", 19, 16.5f),
            new Area("Mawdun Mine", 18, 15.5f),
            new Area("Mawdun Quarry", 17, 14f),
            new Area("Vastiri Outskirts", 16, 13f),
            new Area("Ogham Manor", 15, 12f),
            new Area("Root Hollow", 15, 12f),
            new Area("Clearfell Encampment", 15, 12f),
            new Area("The Manor Ramparts", 14, 11f),
            new Area("Ogham Village", 13, 10f),
            new Area("Ogham Farmlands", 12, 9f),
            new Area("Freythorn", 11, 8f),
            new Area("Hunting Grounds", 10, 7f),
            new Area("Tomb of the Consort", 8, 6f),
            new Area("Mausoleum of the Praetor", 8, 7f),
            new Area("Cemetery of the Eternals", 7, 5f),
            new Area("The Grim Tangle", 6, 4f),
            new Area("The Red Vale", 5, 2f),
            new Area("The Grelwood", 4, 1f),
            new Area("Mud Burrow", 3, 1f),
            new Area("Clearfell", 2, 1f),
        };

        public override bool Initialise()
        {
            base.Initialise();
            lock (Locker) _soundController = GameController.SoundController;
            _soundController.PreloadSound("stopkilling", Path.Combine(DirectoryFullName, "stop killing.wav"));
            _soundController.PreloadSound("keepkilling", Path.Combine(DirectoryFullName, "keep killing.wav"));
            // load areas from file
            string path = Path.Combine(DirectoryFullName, "AreaInfo.txt");
            if (File.Exists(path))
            {
                LoadConfig(path);
            }
            else
            {
                SaveConfig(path);
            }

            return true;
        }

        private void SaveConfig(string path)
        {
            using (StreamWriter file = File.CreateText(path))
            {
                string json = JsonConvert.SerializeObject(Areas, Formatting.Indented);

                file.Write(json);
            }
        }

        private void LoadConfig(string path)
        {
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                Areas = JsonConvert.DeserializeObject<List<Area>>(json);
            }
        }

        private double GetExpPct(int Level, uint Exp)
        {
            if (Level >= 100) return 0.0f;
            uint LevelStartExp = ExpTable[Level - 1];
            uint ExpNeededForNextLevel = ExpTable[Level];

            uint CurrLevelExp = Exp - LevelStartExp;
            uint NextLevelExp = ExpNeededForNextLevel - LevelStartExp;
            double LevelPct = (double)CurrLevelExp / NextLevelExp;
            return LevelPct;
        }

        private double ExpPct(int level, int areaLevel)
        {
            int effDiff = Math.Abs(areaLevel - level) - 3 - level / 16;
            if (effDiff <= 0) return 1.0;
            return Math.Pow((level + 5) / (level + 5 + Math.Pow(effDiff, 2.5)), 1.5);
        }

        public override void Render()
        {
            if (!Settings.Enable)
            {
                return;
            }
            var currentArea = GameController.Area.CurrentArea;
            bool areaChanged = _lastArea != currentArea.Name;
            var player = GameController.Player.GetComponent<Player>();
            double playerLevel = player.Level + Math.Round(GetExpPct(player.Level, player.XP), 2);
            if (Settings.Debug) LogMessage($"Player Level: {playerLevel}");

            if (Settings.Debug) LogMessage($"Current Area: {currentArea.Name}");
            if (Settings.Debug) LogMessage($"Current Area Level: {currentArea.RealLevel}");
            if (Settings.Debug) LogMessage($"Last Area: {_lastArea}");
            foreach (var iArea in Areas)
            {
                //LogMessage(iArea.ToString(), 5);
            }
            Area area = Areas.Find(area => area.Name == currentArea.Name && area.Level == currentArea.RealLevel);
            if (area != null)
            {
                double levelWarning = area.LevelWarning;
                double diffLevel = playerLevel - levelWarning;
                string signal = diffLevel >= 0 ? "+" : "";
                string auxDiffLevel = $"{area.Name} => {levelWarning} ({signal}{diffLevel.ToString("F2")})  ";
                LogMessage(auxDiffLevel);


                //if (Settings.Debug) LogMessage(area.ToString()); 
                //if (Settings.Debug) LogMessage("levelWarning " + levelWarning.ToString("F2"));



                // reset timer if area is changed
                if (areaChanged)
                {
                    _lastArea = currentArea.Name;
                    if (Settings.Debug) LogMessage("Area changed, skipping processing");
                    _timeLastPlayed = DateTime.UnixEpoch;
                }



                // no need to process if we haven't waited long enough already
                if ((DateTime.Now - _timeLastPlayed).TotalSeconds < Settings.AudioDelay.Value)
                {
                    if (Settings.Debug) LogMessage("Audio Delay not hit yet, skipping processing");
                    return;
                }




                if (levelWarning <= playerLevel)
                {
                    _soundController.PlaySound("stopkilling");           
                    _timeLastPlayed = DateTime.Now;
                    if (Settings.Debug) LogMessage("Stop killing!");


                }
                else if (area.Level > player.Level && (int)(ExpPct(player.Level, area.Level) * 100) < Settings.ExpPenaltyWarning)
                //else if (levelWarning > playerLevel)
                {
                    _soundController.PlaySound("keepkilling");
                    _timeLastPlayed = DateTime.Now;
                    if (Settings.Debug) LogMessage("Kill more!");
                }
            }
        }

    }
}