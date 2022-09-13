﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BaoXia.Utils.PinYin
{
        public struct ChinesePinYinInfo
        {

                ////////////////////////////////////////////////
                // @自身属性
                ////////////////////////////////////////////////

                #region 自身属性

                public int ChineseCharacterIndex { get; set; }

                public string ChineseCharacter { get; set; }

                public string PinYin { get; set; }

                public string PinYinWithSound { get; set; }

                #endregion


                ////////////////////////////////////////////////
                // @类方法
                ////////////////////////////////////////////////

                #region 类方法

                /// <summary>
                /// 字符串比较规则：首先，按长度由短到长，其次，按字符数值由小到大。
                /// </summary>
                /// <param name="charsA">指定的字符数组A。</param>
                /// <param name="charsB">指定的字符数组B。</param>
                /// <returns>返回值小于“0”时，指定的字符数组A应在指定的字符数组B之前，
                /// 返回值大于“0”时，指定的字符数组A应在指定的字符数组B之后，
                /// 返回值等于“0”时，指定的字符数组A和指定的字符数组B应当在同一未知。</returns>
                public static int CompareChars(char[] charsA, char[] charsB)
                {
                        if (charsA == charsB)
                        {
                                return 0;
                        }

                        var charsALength = charsA?.Length ?? 0;
                        var charsBLength = charsB?.Length ?? 0;

                        if (charsALength > charsBLength)
                        {
                                return 1;
                        }
                        else if (charsALength < charsBLength)
                        {
                                return -1;
                        }
                        else if (charsA != null
                                && charsB != null)
                        {
                                for (var charIndex = 0;
                                        charIndex < charsALength;
                                        charIndex++)
                                {
                                        var charA = charsA[charIndex];
                                        var charB = charsB[charIndex];
                                        if (charA > charB)
                                        {
                                                return 1;
                                        }
                                        else if (charA < charB)
                                        {
                                                return -1;
                                        }
                                }
                        }
                        return 0;
                }

                public static ChinesePinYinInfo? GetPinYinInfoWithChineseCharacter(
                        string objectChineseCharacter,
                        //
                        char charsEndSymbol,
                        //
                        string allPinYins,
                        int pinYinUnitLength,
                        string allPinYinWithSounds,
                        int pinYinWithSoundUnitLength,

                        //
                        string allChineseChars,
                        int chineseCharUnitLength,
                        //
                        short[] allChineseCharacterPinYinIndexes,
                        short[] allChineseCharacterPinYinWithSoundIndexes)
                {
                        if (string.IsNullOrEmpty(objectChineseCharacter))
                        {
                                return null;
                        }
                        var objectChineseCharacterLength = objectChineseCharacter.Length;
                        if (objectChineseCharacterLength == 1)
                        {
                                // “ASCII”码快速返回：
                                var objectChineseCharacterFirst = objectChineseCharacter[0];
                                if (objectChineseCharacterFirst >= 0
                                        && objectChineseCharacterFirst <= 255)
                                {
                                        // 0~31及127(共33个)是控制字符或通信专用字符
                                        if (objectChineseCharacterFirst >= 32
                                                && objectChineseCharacterFirst != 127)
                                        {
                                                return new ChinesePinYinInfo(
                                                        -1,
                                                        objectChineseCharacter,
                                                        objectChineseCharacter,
                                                        objectChineseCharacter);
                                        }
                                        return null;
                                }
                        }



                        var searchRangeBeginUnitIndex = 0;
                        var searchRangeEndUnitIndex = allChineseChars.Length / chineseCharUnitLength;
                        var chineseCharUnitIndexMatched = -1;
                        while (searchRangeEndUnitIndex > searchRangeBeginUnitIndex)
                        {
                                var searchRangeLength
                                        = searchRangeEndUnitIndex - searchRangeBeginUnitIndex;
                                var searchShotUnitIndex
                                        = searchRangeBeginUnitIndex
                                        + searchRangeLength / 2;

                                var compareResultOfChineseCharacterToObject = 0;
                                var chineseCharUnitShotBeginIndex = chineseCharUnitLength * searchShotUnitIndex;
                                var chineseCharLength = 0;
                                for (var chineseCharIndex = 0;
                                       chineseCharIndex < chineseCharUnitLength;
                                       chineseCharIndex++)
                                {
                                        var chineseChar = allChineseChars[chineseCharUnitShotBeginIndex + chineseCharIndex];
                                        if (chineseChar == charsEndSymbol)
                                        {
                                                break;
                                        }
                                        // !!!
                                        chineseCharLength++;
                                        // !!!
                                }

                                if (chineseCharLength > objectChineseCharacterLength)
                                {
                                        compareResultOfChineseCharacterToObject = 1;
                                }
                                else if (chineseCharLength < objectChineseCharacterLength)
                                {
                                        compareResultOfChineseCharacterToObject = -1;
                                }
                                else // if (chineseCharLength == objectChineseCharacterLength)
                                {
                                        for (var chineseCharIndex = 0;
                                        chineseCharIndex < chineseCharLength;
                                        chineseCharIndex++)
                                        {
                                                var chineseChar = allChineseChars[chineseCharUnitShotBeginIndex + chineseCharIndex];
                                                var objectChineseCharacterChar = objectChineseCharacter[chineseCharIndex];
                                                if (chineseChar != objectChineseCharacterChar)
                                                {
                                                        compareResultOfChineseCharacterToObject
                                                                = chineseChar < objectChineseCharacterChar
                                                                ? -1
                                                                : 1;
                                                        break;
                                                }
                                        }
                                }

                                if (compareResultOfChineseCharacterToObject == 0)
                                {
                                        // !!!
                                        chineseCharUnitIndexMatched = searchShotUnitIndex;
                                        // !!!
                                        break;
                                }
                                else if (searchRangeLength == 1)
                                {
                                        break;
                                }
                                else if (compareResultOfChineseCharacterToObject < 0)
                                {
                                        searchRangeBeginUnitIndex = searchShotUnitIndex;
                                        // searchRangeEndUnitIndex = searchRangeEndIndex;
                                }
                                else if (compareResultOfChineseCharacterToObject > 0)
                                {
                                        // searchRangeBeginUnitIndex = searchRangeBeginIndex;
                                        searchRangeEndUnitIndex = searchShotUnitIndex;
                                }
                        }
                        if (chineseCharUnitIndexMatched < 0)
                        {
                                return null;
                        }



                        string? chineseCharPinYin = null;
                        if (chineseCharUnitIndexMatched
                                < allChineseCharacterPinYinIndexes.Length)
                        {
                                var chineseCharPinYinIndex = allChineseCharacterPinYinIndexes[chineseCharUnitIndexMatched];
                                if (chineseCharPinYinIndex >= 0)
                                {
                                        var chineseCharPinYinCharIndex = pinYinUnitLength * chineseCharPinYinIndex;
                                        if (chineseCharPinYinCharIndex < allPinYins.Length)
                                        {
                                                chineseCharPinYin = allPinYins.Substring(
                                                        chineseCharPinYinCharIndex,
                                                        pinYinUnitLength)
                                                        .TrimEnd();
                                        }
                                }
                        }



                        string? chineseCharPinYinWithSound = null;
                        if (chineseCharUnitIndexMatched
                                < allChineseCharacterPinYinWithSoundIndexes.Length)
                        {
                                var chineseCharPinYinWithSoundIndex = allChineseCharacterPinYinWithSoundIndexes[chineseCharUnitIndexMatched];
                                if (chineseCharPinYinWithSoundIndex >= 0)
                                {
                                        var chineseCharPinYinWithSoundCharIndex = pinYinWithSoundUnitLength * chineseCharPinYinWithSoundIndex;
                                        if (chineseCharPinYinWithSoundCharIndex < allPinYinWithSounds.Length)
                                        {
                                                chineseCharPinYinWithSound = allPinYinWithSounds.Substring(
                                                        chineseCharPinYinWithSoundCharIndex,
                                                        pinYinWithSoundUnitLength)
                                                        .TrimEnd();
                                        }
                                }
                        }


                        var chinesePinYinInfo = new ChinesePinYinInfo(
                                chineseCharUnitIndexMatched,
                                objectChineseCharacter,
                                chineseCharPinYin!,
                                chineseCharPinYinWithSound!);
                        { }
                        return chinesePinYinInfo;
                }

                /// <summary>
                /// 获取指定汉字的常用拼音信息。
                /// </summary>
                /// <param name="objectChineseCharacter">指定的汉字字符，生僻字可能占用2个字符。</param>
                /// <returns>获取指定汉字的常用拼音信息。</returns>
                public static ChinesePinYinInfo? GetPinYinInfoWithChineseCharacter(
                        string objectChineseCharacter)
                {
                        return ChinesePinYinInfo.GetPinYinInfoWithChineseCharacter(
                                objectChineseCharacter,
                                //
                                ChinesePinYinInfes_01.CharsEndSymbol,
                                //
                                ChinesePinYinInfes_01.AllPinYins,
                                ChinesePinYinInfes_01.PinYinUnitLength,
                                ChinesePinYinInfes_01.AllPinYinWithSounds,
                                ChinesePinYinInfes_01.PinYinWithSoundUnitLength,
                                //
                                ChinesePinYinInfes_01.AllChineseCharacters,
                                ChinesePinYinInfes_01.ChineseCharacterUnitLength,
                                //
                                ChinesePinYinInfes_01.AllChineseCharacterPinYinIndexes,
                                ChinesePinYinInfes_01.AllChineseCharacterPinYinWithSoundIndexes);
                }

                public static List<ChinesePinYinInfo>? GetPinYinInfesWithChineseCharacter(
                        string objectChineseCharacter)
                {
                        List<ChinesePinYinInfo>? pinYinInfes = null;
                        {
                                var pinYinInfo_01 = ChinesePinYinInfo.GetPinYinInfoWithChineseCharacter(
                                        objectChineseCharacter,
                                        //
                                        ChinesePinYinInfes_01.CharsEndSymbol,
                                        //
                                        ChinesePinYinInfes_01.AllPinYins,
                                        ChinesePinYinInfes_01.PinYinUnitLength,
                                        ChinesePinYinInfes_01.AllPinYinWithSounds,
                                        ChinesePinYinInfes_01.PinYinWithSoundUnitLength,
                                        //
                                        ChinesePinYinInfes_01.AllChineseCharacters,
                                        ChinesePinYinInfes_01.ChineseCharacterUnitLength,
                                        //
                                        ChinesePinYinInfes_01.AllChineseCharacterPinYinIndexes,
                                        ChinesePinYinInfes_01.AllChineseCharacterPinYinWithSoundIndexes);
                                if (pinYinInfo_01 != null)
                                {
                                        if (pinYinInfes == null)
                                        {
                                                pinYinInfes = new List<ChinesePinYinInfo>();
                                        }
                                        pinYinInfes.Add(pinYinInfo_01.Value);
                                }

                                var pinYinInfo_02 = ChinesePinYinInfo.GetPinYinInfoWithChineseCharacter(
                                        objectChineseCharacter,
                                        //
                                        ChinesePinYinInfes_02.CharsEndSymbol,
                                        //
                                        ChinesePinYinInfes_02.AllPinYins,
                                        ChinesePinYinInfes_02.PinYinUnitLength,
                                        ChinesePinYinInfes_02.AllPinYinWithSounds,
                                        ChinesePinYinInfes_02.PinYinWithSoundUnitLength,
                                        //
                                        ChinesePinYinInfes_02.AllChineseCharacters,
                                        ChinesePinYinInfes_02.ChineseCharacterUnitLength,
                                        //
                                        ChinesePinYinInfes_02.AllChineseCharacterPinYinIndexes,
                                        ChinesePinYinInfes_02.AllChineseCharacterPinYinWithSoundIndexes);
                                if (pinYinInfo_02 != null)
                                {
                                        if (pinYinInfes == null)
                                        {
                                                pinYinInfes = new List<ChinesePinYinInfo>();
                                        }
                                        pinYinInfes.Add(pinYinInfo_02.Value);
                                }

                                var pinYinInfo_03 = ChinesePinYinInfo.GetPinYinInfoWithChineseCharacter(
                                        objectChineseCharacter,
                                        //
                                        ChinesePinYinInfes_03.CharsEndSymbol,
                                        //
                                        ChinesePinYinInfes_03.AllPinYins,
                                        ChinesePinYinInfes_03.PinYinUnitLength,
                                        ChinesePinYinInfes_03.AllPinYinWithSounds,
                                        ChinesePinYinInfes_03.PinYinWithSoundUnitLength,
                                        //
                                        ChinesePinYinInfes_03.AllChineseCharacters,
                                        ChinesePinYinInfes_03.ChineseCharacterUnitLength,
                                        //
                                        ChinesePinYinInfes_03.AllChineseCharacterPinYinIndexes,
                                        ChinesePinYinInfes_03.AllChineseCharacterPinYinWithSoundIndexes);
                                if (pinYinInfo_03 != null)
                                {
                                        if (pinYinInfes == null)
                                        {
                                                pinYinInfes = new List<ChinesePinYinInfo>();
                                        }
                                        pinYinInfes.Add(pinYinInfo_03.Value);
                                }

                                var pinYinInfo_04 = ChinesePinYinInfo.GetPinYinInfoWithChineseCharacter(
                                        objectChineseCharacter,
                                        //
                                        ChinesePinYinInfes_04.CharsEndSymbol,
                                        //
                                        ChinesePinYinInfes_04.AllPinYins,
                                        ChinesePinYinInfes_04.PinYinUnitLength,
                                        ChinesePinYinInfes_04.AllPinYinWithSounds,
                                        ChinesePinYinInfes_04.PinYinWithSoundUnitLength,
                                        //
                                        ChinesePinYinInfes_04.AllChineseCharacters,
                                        ChinesePinYinInfes_04.ChineseCharacterUnitLength,
                                        //
                                        ChinesePinYinInfes_04.AllChineseCharacterPinYinIndexes,
                                        ChinesePinYinInfes_04.AllChineseCharacterPinYinWithSoundIndexes);
                                if (pinYinInfo_04 != null)
                                {
                                        if (pinYinInfes == null)
                                        {
                                                pinYinInfes = new List<ChinesePinYinInfo>();
                                        }
                                        pinYinInfes.Add(pinYinInfo_04.Value);
                                }

                                var pinYinInfo_05 = ChinesePinYinInfo.GetPinYinInfoWithChineseCharacter(
                                        objectChineseCharacter,
                                        //
                                        ChinesePinYinInfes_05.CharsEndSymbol,
                                        //
                                        ChinesePinYinInfes_05.AllPinYins,
                                        ChinesePinYinInfes_05.PinYinUnitLength,
                                        ChinesePinYinInfes_05.AllPinYinWithSounds,
                                        ChinesePinYinInfes_05.PinYinWithSoundUnitLength,
                                        //
                                        ChinesePinYinInfes_05.AllChineseCharacters,
                                        ChinesePinYinInfes_05.ChineseCharacterUnitLength,
                                        //
                                        ChinesePinYinInfes_05.AllChineseCharacterPinYinIndexes,
                                        ChinesePinYinInfes_05.AllChineseCharacterPinYinWithSoundIndexes);
                                if (pinYinInfo_05 != null)
                                {
                                        if (pinYinInfes == null)
                                        {
                                                pinYinInfes = new List<ChinesePinYinInfo>();
                                        }
                                        pinYinInfes.Add(pinYinInfo_05.Value);
                                }

                                var pinYinInfo_06 = ChinesePinYinInfo.GetPinYinInfoWithChineseCharacter(
                                        objectChineseCharacter,
                                        //
                                        ChinesePinYinInfes_06.CharsEndSymbol,
                                        //
                                        ChinesePinYinInfes_06.AllPinYins,
                                        ChinesePinYinInfes_06.PinYinUnitLength,
                                        ChinesePinYinInfes_06.AllPinYinWithSounds,
                                        ChinesePinYinInfes_06.PinYinWithSoundUnitLength,
                                        //
                                        ChinesePinYinInfes_06.AllChineseCharacters,
                                        ChinesePinYinInfes_06.ChineseCharacterUnitLength,
                                        //
                                        ChinesePinYinInfes_06.AllChineseCharacterPinYinIndexes,
                                        ChinesePinYinInfes_06.AllChineseCharacterPinYinWithSoundIndexes);
                                if (pinYinInfo_06 != null)
                                {
                                        if (pinYinInfes == null)
                                        {
                                                pinYinInfes = new List<ChinesePinYinInfo>();
                                        }
                                        pinYinInfes.Add(pinYinInfo_06.Value);
                                }

                                var pinYinInfo_07 = ChinesePinYinInfo.GetPinYinInfoWithChineseCharacter(
                                        objectChineseCharacter,
                                        //
                                        ChinesePinYinInfes_07.CharsEndSymbol,
                                        //
                                        ChinesePinYinInfes_07.AllPinYins,
                                        ChinesePinYinInfes_07.PinYinUnitLength,
                                        ChinesePinYinInfes_07.AllPinYinWithSounds,
                                        ChinesePinYinInfes_07.PinYinWithSoundUnitLength,
                                        //
                                        ChinesePinYinInfes_07.AllChineseCharacters,
                                        ChinesePinYinInfes_07.ChineseCharacterUnitLength,
                                        //
                                        ChinesePinYinInfes_07.AllChineseCharacterPinYinIndexes,
                                        ChinesePinYinInfes_07.AllChineseCharacterPinYinWithSoundIndexes);
                                if (pinYinInfo_07 != null)
                                {
                                        if (pinYinInfes == null)
                                        {
                                                pinYinInfes = new List<ChinesePinYinInfo>();
                                        }
                                        pinYinInfes.Add(pinYinInfo_07.Value);
                                }

                                var pinYinInfo_08 = ChinesePinYinInfo.GetPinYinInfoWithChineseCharacter(
                                        objectChineseCharacter,
                                        //
                                        ChinesePinYinInfes_08.CharsEndSymbol,
                                        //
                                        ChinesePinYinInfes_08.AllPinYins,
                                        ChinesePinYinInfes_08.PinYinUnitLength,
                                        ChinesePinYinInfes_08.AllPinYinWithSounds,
                                        ChinesePinYinInfes_08.PinYinWithSoundUnitLength,
                                        //
                                        ChinesePinYinInfes_08.AllChineseCharacters,
                                        ChinesePinYinInfes_08.ChineseCharacterUnitLength,
                                        //
                                        ChinesePinYinInfes_08.AllChineseCharacterPinYinIndexes,
                                        ChinesePinYinInfes_08.AllChineseCharacterPinYinWithSoundIndexes);
                                if (pinYinInfo_08 != null)
                                {
                                        if (pinYinInfes == null)
                                        {
                                                pinYinInfes = new List<ChinesePinYinInfo>();
                                        }
                                        pinYinInfes.Add(pinYinInfo_08.Value);
                                }

                                var pinYinInfo_09 = ChinesePinYinInfo.GetPinYinInfoWithChineseCharacter(
                                        objectChineseCharacter,
                                        //
                                        ChinesePinYinInfes_09.CharsEndSymbol,
                                        //
                                        ChinesePinYinInfes_09.AllPinYins,
                                        ChinesePinYinInfes_09.PinYinUnitLength,
                                        ChinesePinYinInfes_09.AllPinYinWithSounds,
                                        ChinesePinYinInfes_09.PinYinWithSoundUnitLength,
                                        //
                                        ChinesePinYinInfes_09.AllChineseCharacters,
                                        ChinesePinYinInfes_09.ChineseCharacterUnitLength,
                                        //
                                        ChinesePinYinInfes_09.AllChineseCharacterPinYinIndexes,
                                        ChinesePinYinInfes_09.AllChineseCharacterPinYinWithSoundIndexes);
                                if (pinYinInfo_09 != null)
                                {
                                        if (pinYinInfes == null)
                                        {
                                                pinYinInfes = new List<ChinesePinYinInfo>();
                                        }
                                        pinYinInfes.Add(pinYinInfo_09.Value);
                                }

                                var pinYinInfo_10 = ChinesePinYinInfo.GetPinYinInfoWithChineseCharacter(
                                        objectChineseCharacter,
                                        //
                                        ChinesePinYinInfes_10.CharsEndSymbol,
                                        //
                                        ChinesePinYinInfes_10.AllPinYins,
                                        ChinesePinYinInfes_10.PinYinUnitLength,
                                        ChinesePinYinInfes_10.AllPinYinWithSounds,
                                        ChinesePinYinInfes_10.PinYinWithSoundUnitLength,
                                        //
                                        ChinesePinYinInfes_10.AllChineseCharacters,
                                        ChinesePinYinInfes_10.ChineseCharacterUnitLength,
                                        //
                                        ChinesePinYinInfes_10.AllChineseCharacterPinYinIndexes,
                                        ChinesePinYinInfes_10.AllChineseCharacterPinYinWithSoundIndexes);
                                if (pinYinInfo_10 != null)
                                {
                                        if (pinYinInfes == null)
                                        {
                                                pinYinInfes = new List<ChinesePinYinInfo>();
                                        }
                                        pinYinInfes.Add(pinYinInfo_10.Value);
                                }

                                var pinYinInfo_11 = ChinesePinYinInfo.GetPinYinInfoWithChineseCharacter(
                                        objectChineseCharacter,
                                        //
                                        ChinesePinYinInfes_11.CharsEndSymbol,
                                        //
                                        ChinesePinYinInfes_11.AllPinYins,
                                        ChinesePinYinInfes_11.PinYinUnitLength,
                                        ChinesePinYinInfes_11.AllPinYinWithSounds,
                                        ChinesePinYinInfes_11.PinYinWithSoundUnitLength,
                                        //
                                        ChinesePinYinInfes_11.AllChineseCharacters,
                                        ChinesePinYinInfes_11.ChineseCharacterUnitLength,
                                        //
                                        ChinesePinYinInfes_11.AllChineseCharacterPinYinIndexes,
                                        ChinesePinYinInfes_11.AllChineseCharacterPinYinWithSoundIndexes);
                                if (pinYinInfo_11 != null)
                                {
                                        if (pinYinInfes == null)
                                        {
                                                pinYinInfes = new List<ChinesePinYinInfo>();
                                        }
                                        pinYinInfes.Add(pinYinInfo_11.Value);
                                }
                        }
                        return pinYinInfes;
                }

                #endregion


                ////////////////////////////////////////////////
                // @自身实现
                ////////////////////////////////////////////////

                #region 自身实现

                public ChinesePinYinInfo(
                        int chineseCharacterIndex,
                        string chineseCharacter,
                        string pinYin,
                        string pinYinWithSound)
                {
                        this.ChineseCharacterIndex = chineseCharacterIndex;
                        this.ChineseCharacter = chineseCharacter;
                        this.PinYin = pinYin;
                        this.PinYinWithSound = pinYinWithSound;
                }

                #endregion

        }
}
