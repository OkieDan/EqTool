﻿using EQTool.Models;
using EQTool.ViewModels;
using System;
using System.Collections.Generic;

namespace EQTool.Services.Spells.Log
{
    public class DPSLogParse
    {
        private readonly string PointsOfDamage = " points of damage.";
        private readonly string PointOfDamage = " point of damage.";
        private readonly string WasHitByNonMelee = "was hit by non-melee for";

        private readonly ActivePlayer activePlayer;

        private readonly List<string> HitTypes = new List<string>()
        {
            " maul ",
            " mauls ",
            " strike ",
            " strikes ",
            " slice ",
            " slices ",
            " slash ",
            " slashes ",
            " crush ",
            " crushes ",
            " bite ",
            " bites ",
            " bash ",
            " bashes ",
            " kick ",
            " kicks ",
            " hit ",
            " hits ",
            " punch ",
            " punches ",
            " pierce ",
            " pierces ",
            " backstab ",
            " backstabs ",
            " claw ",
            " claws ",
            " gores "
        };

        public DPSLogParse(ActivePlayer activePlayer)
        {
            this.activePlayer = activePlayer;
        }

        public DPSParseMatch Match(string message, DateTime date)
        {
            var nonmelleindex = message.IndexOf(WasHitByNonMelee);
            if (nonmelleindex > -1)
            {
                message = message.Replace(PointsOfDamage, string.Empty);
                message = message.Replace(PointOfDamage, string.Empty);
                var nameoftarget = message.Substring(0, nonmelleindex).Trim();
                message = message.Replace(nameoftarget, string.Empty);
                var damagedone = message.Replace(WasHitByNonMelee, string.Empty).Trim();
                return new DPSParseMatch
                {
                    SourceName = "You",
                    DamageDone = int.Parse(damagedone),
                    TimeStamp = date,
                    TargetName = nameoftarget
                };
            }
            else if (message.EndsWith(PointsOfDamage) || message.EndsWith(PointOfDamage))
            {
                message = message.Replace(PointsOfDamage, string.Empty);
                message = message.Replace(PointOfDamage, string.Empty);
                var splits = message.Split(' ');
                var damagedone = splits[splits.Length - 1];
                var nameofattacker = string.Empty;
                var nameoftarget = string.Empty;
                var hittype = string.Empty;
                foreach (var item in HitTypes)
                {
                    var found = message.IndexOf(item);
                    if (found != -1)
                    {
                        hittype = item;
                        nameofattacker = message.Substring(0, found + 1).Trim();
                        nameoftarget = message.Replace(nameofattacker, string.Empty).Replace(item, string.Empty).Replace(damagedone, string.Empty).Replace(" for ", string.Empty).Trim();
                        break;
                    }
                }

                if (!string.IsNullOrWhiteSpace(nameofattacker))
                {
                    if (nameofattacker.ToLower() == "you" && activePlayer.Player != null && activePlayer.Player.PlayerClass == null)
                    {
                        if (hittype.Contains("backstab"))
                        {
                            activePlayer.Player.PlayerClass = PlayerClasses.Rogue;
                        }
                    }

                    return new DPSParseMatch
                    {
                        SourceName = nameofattacker,
                        DamageDone = int.Parse(damagedone),
                        TimeStamp = date,
                        TargetName = nameoftarget
                    };
                }
            }

            return null;
        }

        public enum PetLevel
        {
            Best,
            AboveAverage,
            Average,
            BelowAverage,
            Worst
        }

        public static PetLevel GetPetLevel(int hit, PlayerClasses playerClasses, int playerlevel)
        {
            if (playerClasses == PlayerClasses.Magician)
            {
                switch (hit)
                {
                    case 12 when playerlevel <= 4:
                    case 16 when playerlevel <= 8:
                    case 18 when playerlevel <= 12:
                    case 20 when playerlevel <= 16:
                    case 22 when playerlevel <= 20:
                    case 26 when playerlevel <= 24:
                    case 28 when playerlevel <= 29:
                    case 34 when playerlevel <= 34:
                    case 40 when playerlevel <= 39:
                    case 48 when playerlevel <= 44:
                    case 56 when playerlevel <= 49:
                    case 58 when playerlevel <= 51:
                    case 60 when playerlevel <= 57:
                        return PetLevel.Best;
                    case 11:
                        return PetLevel.AboveAverage;
                    case 10:
                        return PetLevel.Average;
                    case 9:
                        return PetLevel.BelowAverage;
                    case 8:
                        return PetLevel.Worst;
                }
            }

            return PetLevel.AboveAverage;
        }
    }
}
