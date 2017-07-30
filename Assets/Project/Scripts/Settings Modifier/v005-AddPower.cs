using OmiyaGames.Settings;

namespace LudumDare39
{
    /// <summary>
    /// Adds Options settings to <see cref="GameSettings"/>.
    /// </summary>
    public class AddPower : SettingsVersionGeneratorDecorator
    {
        public const ushort AppVersion = 5;
        public const int DefaultMaxEnergy = 7;
        public const int DefaultGameId = -1;
        public const string MaxEnergyField = "MaxEnergy";

        public override ushort Version
        {
            get
            {
                return AppVersion;
            }
        }

        protected override string[] GetKeysToRemove()
        {
            // Do nothing!
            return null;
        }

        public static int MaxEnergy
        {
            get
            {
                return UnityEngine.RemoteSettings.GetInt(MaxEnergyField, DefaultMaxEnergy);
            }
        }

        int GetEnergy(ISettingsRecorder settings, string key, int recordedVersion, int latestVersion, int defaultValue)
        {
            return settings.GetInt(key, MaxEnergy);
        }

        protected override IGenerator[] GetNewGenerators()
        {
            return new IGenerator[]
            {
                new StoredIntGenerator("Current Energy", DefaultMaxEnergy)
                {
                    TooltipDocumentation = new string[]
                    {
                        "Current energy on the dreaded energy meter."
                    },
                    Converter = GetEnergy
                },
                new StoredIntGenerator("Last Max Energy", DefaultMaxEnergy)
                {
                    TooltipDocumentation = new string[]
                    {
                        "The max energy when we last played (this can potentially change)."
                    },
                    Converter = GetEnergy
                },
                new StoredStringGenerator("Player Name", "")
                {
                    TooltipDocumentation = new string[]
                    {
                        "Stored name."
                    }
                },
                new StoredIntGenerator("Last Game ID", DefaultGameId)
                {
                    TooltipDocumentation = new string[]
                    {
                        "Every time the golf ball makes it into the hole, the server game ID will go up by one.  This is the ID that the player last played.  If different from the server, proceed to recover energy."
                    }
                },
            };
        }
    }
}
