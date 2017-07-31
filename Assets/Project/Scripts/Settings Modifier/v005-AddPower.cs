using System;
using UnityEngine;
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

        [Flags]
        public enum TutorialFlags
        {
            // Remember to add new flags at the end of the enum!
            None = 0,
            LowEnergy = 1 << 1,
        }

        class ClampEnergy : PropertyStoredSettingsGenerator<int>.ValueProcessor
        {
            public int Process(int value)
            {
                return Mathf.Clamp(value, 0, MaxEnergy);
            }
        }

        public override ushort Version
        {
            get
            {
                return AppVersion;
            }
        }

        public static int MaxEnergy
        {
            get
            {
                return RemoteSettings.GetInt(MaxEnergyField, DefaultMaxEnergy);
            }
        }

        public static bool HaveSeenTutorial(TutorialFlags setting, TutorialFlags tutorialInQuestion)
        {
            return (setting & tutorialInQuestion) != 0;
        }

        protected override string[] GetKeysToRemove()
        {
            // Do nothing!
            return null;
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
                    Converter = GetEnergy,
                    Processor = new ClampEnergy()
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
                new StoredEnumGenerator<TutorialFlags>("Seen Tutorial", TutorialFlags.None)
                {
                    TooltipDocumentation = new string[]
                    {
                        "Flag indicating which tutorials have been seen."
                    }
                },
            };
        }
    }
}
