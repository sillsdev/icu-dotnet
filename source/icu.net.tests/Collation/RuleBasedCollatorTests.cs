// Copyright (c) 2013 SIL International
// This software is licensed under the MIT license (http://opensource.org/licenses/MIT)
using System;
using System.Collections.Generic;
using System.Globalization;
using Icu.Collation;
using NUnit.Framework;

namespace Icu.Tests.Collation
{
	[TestFixture]
	public class RuleBasedCollatorTests
	{
		private readonly string SerbianRules = "& C < č <<< Č < ć <<< Ć";
		// with UCA:            after Tailoring:
		// --------------       ----------------
		// CUKIĆ RADOJICA       CUKIĆ RADOJICA
		// ČUKIĆ SLOBODAN       CUKIĆ SVETOZAR
		// CUKIĆ SVETOZAR       CURIĆ MILOŠ
		// ČUKIĆ ZORAN          CVRKALJ ÐURO
		// CURIĆ MILOŠ          ČUKIĆ SLOBODAN
		// ĆURIĆ MILOŠ          ČUKIĆ ZORAN
		// CVRKALJ ÐURO         ĆURIĆ MILOŠ

		private readonly string DanishRules = "&V <<< w <<< W";

		//UCA 	&V <<< w <<< W
		//---   ---
		//va    va
		//Va    Va
		//VA    VA
		//vb    wa
		//Vb    Wa
		//VB    WA
		//vz    vb
		//Vz    Vb
		//VZ    VB
		//wa    wb
		//Wa    Wb
		//WA    WB
		//wb    vz
		//Wb    Vz
		//WB    VZ
		//wz    wz
		//Wz    Wz
		//WZ    WZ

		[Test]
		public void Construct_EmptyRules_UCAcollator()
		{
			Assert.IsNotNull(new RuleBasedCollator(string.Empty));
		}

		[Test]
		public void Construct_Rules_Okay()
		{
			new RuleBasedCollator(SerbianRules);
		}

		[Test]
		[ExpectedException(typeof(ApplicationException))]
		public void Construct_SyntaxErrorInRules_Throws()
		{
			string badRules = "& C < č <<<< Č < ć <<< Ć";
			new RuleBasedCollator(badRules);
		}

		[Test]
		public void Clone()
		{
			RuleBasedCollator danishCollator = new RuleBasedCollator(DanishRules);
			RuleBasedCollator danishCollator2 = (RuleBasedCollator)danishCollator.Clone();
			Assert.AreEqual(-1, danishCollator2.Compare("wa", "vb"));
		}

		[Test]
		public void Compare()
		{
			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty);
			Assert.AreEqual(-1, ucaCollator.Compare("ČUKIĆ SLOBODAN", "CUKIĆ SVETOZAR"));

			RuleBasedCollator serbianCollator = new RuleBasedCollator(SerbianRules);
			Assert.AreEqual(1, serbianCollator.Compare("ČUKIĆ SLOBODAN", "CUKIĆ SVETOZAR"));
		}

		[Test]
		public void Compare_String1IsNull_Less()
		{
			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty);
			Assert.AreEqual(-1, ucaCollator.Compare(null, "a"));

		}

		[Test]
		public void Compare_String2IsNull_Greater()
		{
			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty);
			Assert.AreEqual(1, ucaCollator.Compare("a", null));

		}

		[Test]
		public void Compare_BothStringsNull_Equal()
		{
			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty);
			Assert.AreEqual(0, ucaCollator.Compare(null, null));
		}

		[Test]
		public void GetSortKey()
		{
			RuleBasedCollator serbianCollator = new RuleBasedCollator(SerbianRules);
			SortKey sortKeyČUKIĆ = serbianCollator.GetSortKey("ČUKIĆ SLOBODAN");
			SortKey sortKeyCUKIĆ = serbianCollator.GetSortKey("CUKIĆ SVETOZAR");
			Assert.AreEqual(1, SortKey.Compare(sortKeyČUKIĆ, sortKeyCUKIĆ));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetSortKey_Null()
		{
			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty);
			ucaCollator.GetSortKey(null);
		}

		[Test]
		public void GetSortKey_emptyString()
		{
			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty);
			SortKey key = ucaCollator.GetSortKey(string.Empty);
			Assert.IsNotNull(key);
			Assert.IsNotNull(key.KeyData);
		}

		[Test]
		public void AlternateHandling_Shifted()
		{
			/*  The Alternate attribute is used to control the handling of the so-called 
             * variable characters in the UCA: whitespace, punctuation and symbols. If 
             * Alternate is set to Non-Ignorable (N), then differences among these 
             * characters are of the same importance as differences among letters. 
             * If Alternate is set to Shifted (S), then these characters are of only 
             * minor importance. The Shifted value is often used in combination with 
             * Strength set to Quaternary. In such a case, white-space, punctuation, 
             * and symbols are considered when comparing strings, but only if all other 
             * aspects of the strings (base letters, accents, and case) are identical. 
             * If Alternate is not set to Shifted, then there is no difference between 
             * a Strength of 3 and a Strength of 4.
              Example:
                  S=3, A=N di Silva < Di Silva < diSilva < U.S.A. < USA
                  S=3, A=S di Silva = diSilva < Di Silva  < U.S.A. = USA
                  S=4, A=S di Silva < diSilva < Di Silva < U.S.A. < USA
             */
			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty, CollationStrength.Tertiary);
			ucaCollator.AlternateHandling = AlternateHandling.Shifted;
			Assert.AreEqual(AlternateHandling.Shifted, ucaCollator.AlternateHandling);
			Assert.AreEqual(0, ucaCollator.Compare("di Silva", "diSilva"));
			Assert.AreEqual(-1, ucaCollator.Compare("diSilva", "Di Silva"));
			Assert.AreEqual(0, ucaCollator.Compare("U.S.A.", "USA"));

			ucaCollator.Strength = CollationStrength.Quaternary;
			Assert.AreEqual(-1, ucaCollator.Compare("di Silva", "diSilva"));
			Assert.AreEqual(-1, ucaCollator.Compare("diSilva", "Di Silva"));
			Assert.AreEqual(-1, ucaCollator.Compare("U.S.A.", "USA"));

		}

		[Test]
		public void AlternateHandling_NonIgnorable()
		{
			//  S=3, A=N di Silva < Di Silva < diSilva < U.S.A. < USA
			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty, CollationStrength.Tertiary);
			Assert.AreEqual(AlternateHandling.NonIgnorable, ucaCollator.AlternateHandling);

			Assert.AreEqual(-1, ucaCollator.Compare("di Silva", "Di Silva"));
			Assert.AreEqual(-1, ucaCollator.Compare("Di Silva", "diSilva"));
			Assert.AreEqual(-1, ucaCollator.Compare("U.S.A.", "USA"));
		}

		[Test]
		public void CaseFirst_Lowerfirst()
		{
			/* The Case_First attribute is used to control whether uppercase letters 
             * come before lowercase letters or vice versa, in the absence of other 
             * differences in the strings. The possible values are Uppercase_First 
             * (U) and Lowercase_First (L), plus the standard Default and Off. There 
             * is almost no difference between the Off and Lowercase_First options in 
             * terms of results, so typically users will not use Lowercase_First: only 
             * Off or Uppercase_First. (People interested in the detailed differences 
             * between X and L should consult the Collation Customization ).
             *   Specifying either L or U won't affect string comparison performance, 
             * but will affect the sort key length.
                Example: 
                    C=X or C=L "china" < "China" < "denmark" < "Denmark"
                    C=U "China" < "china" < "Denmark" < "denmark"
             */
			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty);
			ucaCollator.CaseFirst = CaseFirst.LowerFirst;
			Assert.AreEqual(CaseFirst.LowerFirst, ucaCollator.CaseFirst);

			Assert.AreEqual(-1, ucaCollator.Compare("china", "China"));
			Assert.AreEqual(-1, ucaCollator.Compare("China", "denmark"));
			Assert.AreEqual(-1, ucaCollator.Compare("denmark", "Denmark"));

		}

		[Test]
		public void CaseFirst_Off()
		{
			//                    C=X or C=L "china" < "China" < "denmark" < "Denmark"
			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty);
			Assert.AreEqual(CaseFirst.Off, ucaCollator.CaseFirst);

			Assert.AreEqual(-1, ucaCollator.Compare("china", "China"));
			Assert.AreEqual(-1, ucaCollator.Compare("China", "denmark"));
			Assert.AreEqual(-1, ucaCollator.Compare("denmark", "Denmark"));

		}

		[Test]
		public void CaseFirst_Upperfirst()
		{
			//                  C=U "China" < "china" < "Denmark" < "denmark"
			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty);
			ucaCollator.CaseFirst = CaseFirst.UpperFirst;
			Assert.AreEqual(CaseFirst.UpperFirst, ucaCollator.CaseFirst);

			Assert.AreEqual(-1, ucaCollator.Compare("China", "china"));
			Assert.AreEqual(-1, ucaCollator.Compare("china", "Denmark"));
			Assert.AreEqual(-1, ucaCollator.Compare("Denmark", "denmark"));

		}

		[Test]
		public void CaseLevel_Off()
		{
			/*The Case_Level attribute is used when ignoring accents but not case. In 
             * such a situation, set Strength to be Primary, and Case_Level to be On. 
             * In most locales, this setting is Off by default. There is a small string 
             * comparison performance and sort key impact if this attribute is set to be On.
                Example:
                S=1, E=X role = Role = rôle
                S=1, E=O role = rôle <  Role*/

			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty, CollationStrength.Primary);
			Assert.AreEqual(CaseLevel.Off, ucaCollator.CaseLevel);

			Assert.AreEqual(0, ucaCollator.Compare("role", "Role"));
			Assert.AreEqual(0, ucaCollator.Compare("role", "rôle"));
		}

		[Test]
		public void CaseLevel_On()
		{
			//S=1, E=O role = rôle <  Role

			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty, CollationStrength.Primary);
			ucaCollator.CaseLevel = CaseLevel.On;
			Assert.AreEqual(CaseLevel.On, ucaCollator.CaseLevel);

			Assert.AreEqual(0, ucaCollator.Compare("role", "rôle"));
			Assert.AreEqual(-1, ucaCollator.Compare("rôle", "Role"));

		}

		[Test]
		public void FrenchCollation_Off()
		{

			/*The French sort strings with different accents from the back of the 
             * string. This attribute is automatically set to On for the French 
             * locales and a few others. Users normally would not need to explicitly 
             * set this attribute. There is a string comparison performance cost 
             * when it is set On, but sort key length is unaffected.
                Example:
                F=X cote < coté < côte < côté
                F=O cote < côte < coté < côté
             */
			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty);
			Assert.AreEqual(FrenchCollation.Off, ucaCollator.FrenchCollation);

			Assert.AreEqual(-1, ucaCollator.Compare("cote", "coté"));
			Assert.AreEqual(-1, ucaCollator.Compare("coté", "côte"));
			Assert.AreEqual(-1, ucaCollator.Compare("côte", "côté"));
		}

		[Test]
		public void FrenchCollation_On()
		{
			//                F=O cote < côte < coté < côté
			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty);
			ucaCollator.FrenchCollation = FrenchCollation.On;
			Assert.AreEqual(FrenchCollation.On, ucaCollator.FrenchCollation);

			Assert.AreEqual(-1, ucaCollator.Compare("cote", "côte"));
			Assert.AreEqual(-1, ucaCollator.Compare("côte", "coté"));
			Assert.AreEqual(-1, ucaCollator.Compare("coté", "côté"));
		}

		[Test]
		[Category("ICU50 Deprecated")]
		public void HiraganaQuarternary_Off()
		{
			/* Compatibility with JIS x 4061 requires the introduction of an 
             * additional level to distinguish Hiragana and Katakana characters. 
             * If compatibility with that standard is required, then this attribute 
             * should be set On, and the strength set to Quaternary. This will affect
             * sort key length and string comparison string comparison performance.
                Example:
                H=X, S=4 きゅう = キュウ < きゆう = キユウ
                H=O, S=4 きゅう < キュウ < きゆう < キユウ        
             */
			Collator jaCollator = CreateJaCollator();
			jaCollator.HiraganaQuaternary = HiraganaQuaternary.Off;
			jaCollator.Strength = CollationStrength.Quaternary;
			Assert.AreEqual(HiraganaQuaternary.Off, jaCollator.HiraganaQuaternary);

			Assert.AreEqual(0, jaCollator.Compare("きゅう", "キュウ"));
			Assert.AreEqual(-1, jaCollator.Compare("キュウ", "きゆう"));
			Assert.AreEqual(0, jaCollator.Compare("きゆう", "キユウ"));

		}

		private static Collator CreateJaCollator()
		{
			try
			{
				return RuleBasedCollator.Create("ja");
			}
			catch
			{
				return new RuleBasedCollator(JaRules);
			}
		}

		[Test]
		public void HiraganaQuarternary_On()
		{
			//    H=O, S=4 きゅう < キュウ < きゆう < キユウ        

			Collator jaCollator = CreateJaCollator();
			jaCollator.Strength = CollationStrength.Quaternary;
			Assert.AreEqual(HiraganaQuaternary.On, jaCollator.HiraganaQuaternary);
			Assert.AreEqual(CollationStrength.Quaternary, jaCollator.Strength);

			Assert.AreEqual(-1, jaCollator.Compare("きゅう", "キュウ"));
			Assert.AreEqual(-1, jaCollator.Compare("キュウ", "きゆう"));
			Assert.AreEqual(-1, jaCollator.Compare("きゆう", "キユウ"));

		}

		[Test]
		public void NormalizationMode_Off()
		{
			/*The Normalization setting determines whether text is thoroughly 
             * normalized or not in comparison. Even if the setting is off (which 
             * is the default for many locales), text as represented in common usage 
             * will compare correctly (for details, see UTN #5 ). Only if the accent 
             * marks are in non-canonical order will there be a problem. If the 
             * setting is On, then the best results are guaranteed for all possible 
             * text input.There is a medium string comparison performance cost if 
             * this attribute is On, depending on the frequency of sequences that 
             * require normalization. There is no significant effect on sort key 
             * length.If the input text is known to be in NFD or NFKD normalization 
             * forms, there is no need to enable this Normalization option.
                Example:
                N=X ä = a + ◌̈ < ä + ◌̣ < ạ + ◌̈
                N=O ä = a + ◌̈ < ä + ◌̣ = ạ + ◌̈
             */

			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty, NormalizationMode.Off, CollationStrength.Default);
			Assert.AreEqual(NormalizationMode.Off, ucaCollator.NormalizationMode);

			Assert.AreEqual(0, ucaCollator.Compare("ä", "a\u0308"));
			Assert.AreEqual(-1, ucaCollator.Compare("a\u0308", "ä\u0323"));
			Assert.AreEqual(-1, ucaCollator.Compare("ä\u0323", "ạ\u0308"));
		}

		[Test]
		public void NormalizationMode_On()
		{
			//  N=O ä = a + ◌̈ < ä + ◌̣ = ạ + ◌̈

			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty, NormalizationMode.On, CollationStrength.Default);
			Assert.AreEqual(NormalizationMode.On, ucaCollator.NormalizationMode);

			Assert.AreEqual(0, ucaCollator.Compare("ä", "a\u0308"));
			Assert.AreEqual(-1, ucaCollator.Compare("a\u0308", "ä\u0323"));
			Assert.AreEqual(0, ucaCollator.Compare("ä\u0323", "ạ\u0308"));
		}

		[Test]
		public void NumericCollation_Off()
		{
			//  1 < 10 < 2 < 20
			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty);
			Assert.AreEqual(NumericCollation.Off, ucaCollator.NumericCollation);

			Assert.AreEqual(-1, ucaCollator.Compare("1", "10"));
			Assert.AreEqual(-1, ucaCollator.Compare("10", "2"));
			Assert.AreEqual(-1, ucaCollator.Compare("2", "20"));
		}

		[Test]
		public void NumericCollation_On()
		{
			//  1 < 2 < 10 < 20
			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty);
			ucaCollator.NumericCollation = NumericCollation.On;
			Assert.AreEqual(NumericCollation.On, ucaCollator.NumericCollation);

			Assert.AreEqual(-1, ucaCollator.Compare("1", "2"));
			Assert.AreEqual(-1, ucaCollator.Compare("2", "10"));
			Assert.AreEqual(-1, ucaCollator.Compare("10", "20"));

		}

		[Test]
		public void Strength_Primary()
		{
			/*The Strength attribute determines whether accents or case are taken
             * into account when collating or matching text. ( (In writing systems 
             * without case or accents, it controls similarly important features).  
             * The default strength setting usually does not need to be changed for 
             * collating (sorting), but often needs to be changed when matching 
             * (e.g. SELECT). The possible values include Default (D), Primary 
             * (1), Secondary (2), Tertiary (3), Quaternary (4), and Identical (I).
             * 
             * For example, people may choose to ignore accents or ignore accents and 
             * case when searching for text.
             * 
             * Almost all characters are distinguished by the first three levels, and 
             * in most locales the default value is thus Tertiary. However, if 
             * Alternate is set to be Shifted, then the Quaternary strength (4) 
             * can be used to break ties among whitespace, punctuation, and symbols 
             * that would otherwise be ignored. If very fine distinctions among 
             * characters are required, then the Identical strength (I) can be 
             * used (for example, Identical Strength distinguishes between the 
             * Mathematical Bold Small A and the Mathematical Italic Small A. For 
             * more examples, look at the cells with white backgrounds in the 
             * collation charts). However, using levels higher than Tertiary - 
             * the Identical strength - result in significantly longer sort keys, 
             * and slower string comparison performance for equal strings.
                Example:
                S=1 role = Role = rôle
                S=2 role = Role < rôle
                S=3 role < Role < rôle*/
			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty, CollationStrength.Primary);
			Assert.AreEqual(CollationStrength.Primary, ucaCollator.Strength);

			Assert.AreEqual(0, ucaCollator.Compare("role", "Role"));
			Assert.AreEqual(0, ucaCollator.Compare("Role", "rôle"));
		}

		[Test]
		public void Strength_Secondary()
		{
			//                S=2 role = Role < rôle
			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty, CollationStrength.Secondary);
			Assert.AreEqual(CollationStrength.Secondary, ucaCollator.Strength);

			Assert.AreEqual(0, ucaCollator.Compare("role", "Role"));
			Assert.AreEqual(-1, ucaCollator.Compare("Role", "rôle"));
		}

		[Test]
		public void Strength_Tertiary()
		{
			//   S=3 role < Role < rôle
			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty, CollationStrength.Tertiary);
			Assert.AreEqual(CollationStrength.Tertiary, ucaCollator.Strength);

			Assert.AreEqual(-1, ucaCollator.Compare("role", "Role"));
			Assert.AreEqual(-1, ucaCollator.Compare("Role", "rôle"));

		}

		[Test]
		public void Strength_Quaternary()
		{
			// A=S  S=4 ab < a c < a-c < ac
			RuleBasedCollator ucaCollator = new RuleBasedCollator(string.Empty, CollationStrength.Quaternary);
			ucaCollator.AlternateHandling = AlternateHandling.Shifted;
			Assert.AreEqual(CollationStrength.Quaternary, ucaCollator.Strength);

			Assert.AreEqual(-1, ucaCollator.Compare("ab", "a c"));
			Assert.AreEqual(-1, ucaCollator.Compare("a c", "a-c"));
			Assert.AreEqual(-1, ucaCollator.Compare("a-c", "ac"));
		}

		[Test]
		public void GetAvailableLocales_ReturnsList()
		{
			IList<string> locales = RuleBasedCollator.GetAvailableCollationLocales();
			Assert.IsNotNull(locales);
		}

		private const string JaRules = 
                " [strength 3 ] [hiraganaQ on ]" +
			"&ヽ=ヽ=ゝ" +
			"&[before 3]ァ<<<ァ|ー=ｧ|ー=ぁ|ー=ア|ー=ｱ|ー=あ|ー=カ|ー=ｶ|ー=か|ー=ガ|ー=が|ー=サ|ー=ｻ|ー=さ" +
			"|ー=ザ|ー=ざ|ー=タ|ー=ﾀ|ー=た|ー=ダ|ー=だ|ー=ナ|ー=ﾅ|ー=な|ー=ハ|ー=ﾊ|ー=は|ー=バ|ー=ば|ー=パ|ー=ぱ" +
			"|ー=マ|ー=ﾏ|ー=ま|ー=ャ|ー=ｬ|ー=ゃ|ー=ヤ|ー=ﾔ|ー=や|ー=ラ|ー=ﾗ|ー=ら|ー=ヮ|ー=ゎ|ー=ワ|ー=ﾜ|ー=わ" +
			"|ー=ヵ|ー=ヷ|ー" +
			"&[before 3]ィ<<<ィ|ー=ｨ|ー=ぃ|ー=イ|ー=ｲ|ー=い|ー=キ|ー=ｷ|ー=き|ー=ギ|ー=ぎ|ー=シ|ー=ｼ|ー=し" +
			"|ー=ジ|ー=じ|ー=チ|ー=ﾁ|ー=ち|ー=ヂ|ー=ぢ|ー=ニ|ー=ﾆ|ー=に|ー=ヒ|ー=ﾋ|ー=ひ|ー=ビ|ー=び|ー=ピ|ー=ぴ" +
			"|ー=ミ|ー=ﾐ|ー=み|ー=リ|ー=ﾘ|ー=り|ー=ヰ|ー=ゐ|ー=ヸ|ー" +
			"&[before 3]ゥ<<<ゥ|ー=ｩ|ー=ぅ|ー=ウ|ー=ｳ|ー=う|ー=ク|ー=ｸ|ー=く|ー=グ|ー=ぐ|ー=ス|ー=ｽ|ー=す" +
			"|ー=ズ|ー=ず|ー=ッ|ー=ｯ|ー=っ|ー=ツ|ー=ﾂ|ー=つ|ー=ヅ|ー=づ|ー=ヌ|ー=ﾇ|ー=ぬ|ー=フ|ー=ﾌ|ー=ふ|ー=ブ" +
			"|ー=ぶ|ー=プ|ー=ぷ|ー=ム|ー=ﾑ|ー=む|ー=ュ|ー=ｭ|ー=ゅ|ー=ユ|ー=ﾕ|ー=ゆ|ー=ル|ー=ﾙ|ー=る|ー=ヴ|ー=ゔ" +
			"|ー" +
			"&[before 3]ェ<<<ェ|ー=ｪ|ー=ぇ|ー=エ|ー=ｴ|ー=え|ー=ケ|ー=ｹ|ー=け|ー=ゲ|ー=げ|ー=セ|ー=ｾ|ー=せ" +
			"|ー=ゼ|ー=ぜ|ー=テ|ー=ﾃ|ー=て|ー=デ|ー=で|ー=ネ|ー=ﾈ|ー=ね|ー=ヘ|ー=ﾍ|ー=へ|ー=ベ|ー=べ|ー=ペ|ー=ぺ" +
			"|ー=メ|ー=ﾒ|ー=め|ー=レ|ー=ﾚ|ー=れ|ー=ヱ|ー=ゑ|ー=ヶ|ー=ヹ|ー" +
			"&[before 3]ォ<<<ォ|ー=ｫ|ー=ぉ|ー=オ|ー=ｵ|ー=お|ー=コ|ー=ｺ|ー=こ|ー=ゴ|ー=ご|ー=ソ|ー=ｿ|ー=そ" +
			"|ー=ゾ|ー=ぞ|ー=ト|ー=ﾄ|ー=と|ー=ド|ー=ど|ー=ノ|ー=ﾉ|ー=の|ー=ホ|ー=ﾎ|ー=ほ|ー=ボ|ー=ぼ|ー=ポ|ー=ぽ" +
			"|ー=モ|ー=ﾓ|ー=も|ー=ョ|ー=ｮ|ー=ょ|ー=ヨ|ー=ﾖ|ー=よ|ー=ロ|ー=ﾛ|ー=ろ|ー=ヲ|ー=ｦ|ー=を|ー=ヺ|ー" +
			"&[before 3]ア<<<ア|ヽ=ｱ|ヽ=あ|ゝ=ァ|ヽ=ｧ|ヽ=ぁ|ゝ" +
			"&[before 3]イ<<<イ|ヽ=ｲ|ヽ=い|ゝ=ィ|ヽ=ｨ|ヽ=ぃ|ゝ" +
			"&[before 3]ウ<<<ウ|ヽ=ｳ|ヽ=う|ゝ=ゥ|ヽ=ｩ|ヽ=ぅ|ゝ=ヴ|ヽ=ゔ|ゝ=ウ|ヾ/゙=ｳ|ヾ/゙=う|ゞ/゙=ゥ|ヾ" +
			"/゙=ｩ|ヾ/゙=ぅ|ゞ/゙=ヴ|ヾ/゙=ゔ|ゞ/゙" +
			"&[before 3]エ<<<エ|ヽ=ｴ|ヽ=え|ゝ=ェ|ヽ=ｪ|ヽ=ぇ|ゝ" +
			"&[before 3]オ<<<オ|ヽ=ｵ|ヽ=お|ゝ=ォ|ヽ=ｫ|ヽ=ぉ|ゝ" +
			"&[before 3]カ<<<カ|ヽ=ｶ|ヽ=か|ゝ=ヵ|ヽ" +
			"&[before 3]ガ<<<ガ|ヽ=が|ゝ" +
			"&[before 3]キ<<<キ|ヽ=ｷ|ヽ=き|ゝ=ギ|ヽ=ぎ|ゝ=キ|ヾ/゙=ｷ|ヾ/゙=き|ゞ/゙=ギ|ヾ/゙=ぎ|ゞ/゙" +
			"&[before 3]ク<<<ク|ヽ=ｸ|ヽ=く|ゝ=グ|ヽ=ぐ|ゝ=ク|ヾ/゙=ｸ|ヾ/゙=く|ゞ/゙=グ|ヾ/゙=ぐ|ゞ/゙" +
			"&[before 3]ケ<<<ケ|ヽ=ｹ|ヽ=け|ゝ=ヶ|ヽ" +
			"&[before 3]ゲ<<<ゲ|ヽ=げ|ゝ" +
			"&[before 3]コ<<<コ|ヽ=ｺ|ヽ=こ|ゝ=ゴ|ヽ=ご|ゝ=コ|ヾ/゙=ｺ|ヾ/゙=こ|ゞ/゙=ゴ|ヾ/゙=ご|ゞ/゙" +
			"&[before 3]サ<<<サ|ヽ=ｻ|ヽ=さ|ゝ=ザ|ヽ=ざ|ゝ=サ|ヾ/゙=ｻ|ヾ/゙=さ|ゞ/゙=ザ|ヾ/゙=ざ|ゞ/゙" +
			"&[before 3]シ<<<シ|ヽ=ｼ|ヽ=し|ゝ=ジ|ヽ=じ|ゝ=シ|ヾ/゙=ｼ|ヾ/゙=し|ゞ/゙=ジ|ヾ/゙=じ|ゞ/゙" +
			"&[before 3]ス<<<ス|ヽ=ｽ|ヽ=す|ゝ=ズ|ヽ=ず|ゝ=ス|ヾ/゙=ｽ|ヾ/゙=す|ゞ/゙=ズ|ヾ/゙=ず|ゞ/゙" +
			"&[before 3]セ<<<セ|ヽ=ｾ|ヽ=せ|ゝ=ゼ|ヽ=ぜ|ゝ=セ|ヾ/゙=ｾ|ヾ/゙=せ|ゞ/゙=ゼ|ヾ/゙=ぜ|ゞ/゙" +
			"&[before 3]ソ<<<ソ|ヽ=ｿ|ヽ=そ|ゝ=ゾ|ヽ=ぞ|ゝ=ソ|ヾ/゙=ｿ|ヾ/゙=そ|ゞ/゙=ゾ|ヾ/゙=ぞ|ゞ/゙" +
			"&[before 3]タ<<<タ|ヽ=ﾀ|ヽ=た|ゝ=ダ|ヽ=だ|ゝ=タ|ヾ/゙=ﾀ|ヾ/゙=た|ゞ/゙=ダ|ヾ/゙=だ|ゞ/゙" +
			"&[before 3]チ<<<チ|ヽ=ﾁ|ヽ=ち|ゝ=ヂ|ヽ=ぢ|ゝ=チ|ヾ/゙=ﾁ|ヾ/゙=ち|ゞ/゙=ヂ|ヾ/゙=ぢ|ゞ/゙" +
			"&[before 3]ツ<<<ツ|ヽ=ﾂ|ヽ=つ|ゝ=ッ|ヽ=ｯ|ヽ=っ|ゝ=ヅ|ヽ=づ|ゝ=ツ|ヾ/゙=ﾂ|ヾ/゙=つ|ゞ/゙=ヅ|ヾ" +
			"/゙=づ|ゞ/゙=ツ|ヽ=ﾂ|ヽ=つ|ゝ=ッ|ヾ/゙=ｯ|ヾ/゙=っ|ゞ/゙=ツ|ヾ/゙=ﾂ|ヾ/゙=つ|ゞ/゙" +
			"&[before 3]テ<<<テ|ヽ=ﾃ|ヽ=て|ゝ=デ|ヽ=で|ゝ=テ|ヾ/゙=ﾃ|ヾ/゙=て|ゞ/゙=デ|ヾ/゙=で|ゞ/゙" +
			"&[before 3]ト<<<ト|ヽ=ﾄ|ヽ=と|ゝ=ド|ヽ=ど|ゝ=ト|ヾ/゙=ﾄ|ヾ/゙=と|ゞ/゙=ド|ヾ/゙=ど|ゞ/゙" +
			"&[before 3]ナ<<<ナ|ヽ=ﾅ|ヽ=な|ゝ" +
			"&[before 3]ニ<<<ニ|ヽ=ﾆ|ヽ=に|ゝ" +
			"&[before 3]ヌ<<<ヌ|ヽ=ﾇ|ヽ=ぬ|ゝ" +
			"&[before 3]ネ<<<ネ|ヽ=ﾈ|ヽ=ね|ゝ" +
			"&[before 3]ノ<<<ノ|ヽ=ﾉ|ヽ=の|ゝ" +
			"&[before 3]ハ<<<ハ|ヽ=ﾊ|ヽ=は|ゝ=バ|ヽ=ば|ゝ=ハ|ヾ/゙=ﾊ|ヾ/゙=は|ゞ/゙=バ|ヾ/゙=ば|ゞ/゙=パ|ヽ" +
			"=ぱ|ゝ=パ|ヾ/゙=ぱ|ゞ/゙" +
			"&[before 3]ヒ<<<ヒ|ヽ=ﾋ|ヽ=ひ|ゝ=ビ|ヽ=び|ゝ=ヒ|ヾ/゙=ﾋ|ヾ/゙=ひ|ゞ/゙=ビ|ヾ/゙=び|ゞ/゙=ピ|ヽ" +
			"=ぴ|ゝ=ピ|ヾ/゙=ぴ|ゞ/゙" +
			"&[before 3]フ<<<フ|ヽ=ﾌ|ヽ=ふ|ゝ=ブ|ヽ=ぶ|ゝ=フ|ヾ/゙=ﾌ|ヾ/゙=ふ|ゞ/゙=ブ|ヾ/゙=ぶ|ゞ/゙=プ|ヽ" +
			"=ぷ|ゝ=プ|ヾ/゙=ぷ|ゞ/゙" +
			"&[before 3]ヘ<<<ヘ|ヽ=ﾍ|ヽ=へ|ゝ=ベ|ヽ=べ|ゝ=ヘ|ヾ/゙=ﾍ|ヾ/゙=へ|ゞ/゙=ベ|ヾ/゙=べ|ゞ/゙=ペ|ヽ" +
			"=ぺ|ゝ=ペ|ヾ/゙=ぺ|ゞ/゙" +
			"&[before 3]ホ<<<ホ|ヽ=ﾎ|ヽ=ほ|ゝ=ボ|ヽ=ぼ|ゝ=ホ|ヾ/゙=ﾎ|ヾ/゙=ほ|ゞ/゙=ボ|ヾ/゙=ぼ|ゞ/゙=ポ|ヽ" +
			"=ぽ|ゝ=ポ|ヾ/゙=ぽ|ゞ/゙" +
			"&[before 3]マ<<<マ|ヽ=ﾏ|ヽ=ま|ゝ" +
			"&[before 3]ミ<<<ミ|ヽ=ﾐ|ヽ=み|ゝ" +
			"&[before 3]ム<<<ム|ヽ=ﾑ|ヽ=む|ゝ" +
			"&[before 3]メ<<<メ|ヽ=ﾒ|ヽ=め|ゝ" +
			"&[before 3]モ<<<モ|ヽ=ﾓ|ヽ=も|ゝ" +
			"&[before 3]ヤ<<<ヤ|ヽ=ﾔ|ヽ=や|ゝ=ャ|ヽ=ｬ|ヽ=ゃ|ゝ" +
			"&[before 3]ユ<<<ユ|ヽ=ﾕ|ヽ=ゆ|ゝ=ュ|ヽ=ｭ|ヽ=ゅ|ゝ" +
			"&[before 3]ヨ<<<ヨ|ヽ=ﾖ|ヽ=よ|ゝ=ョ|ヽ=ｮ|ヽ=ょ|ゝ" +
			"&[before 3]ラ<<<ラ|ヽ=ﾗ|ヽ=ら|ゝ" +
			"&[before 3]リ<<<リ|ヽ=ﾘ|ヽ=り|ゝ" +
			"&[before 3]ル<<<ル|ヽ=ﾙ|ヽ=る|ゝ" +
			"&[before 3]レ<<<レ|ヽ=ﾚ|ヽ=れ|ゝ" +
			"&[before 3]ロ<<<ロ|ヽ=ﾛ|ヽ=ろ|ゝ" +
			"&[before 3]ワ<<<ワ|ヽ=ﾜ|ヽ=わ|ゝ=ヮ|ヽ=ゎ|ゝ=ヷ|ヽ=ワ|ヾ/゙=ﾜ|ヾ/゙=わ|ゞ/゙=ヷ|ヾ/゙=ヮ|ヾ/゙" +
			"=ゎ|ゞ/゙" +
			"&[before 3]ヰ<<<ヰ|ヽ=ゐ|ゝ=ヸ|ヽ=ヰ|ヾ/゙=ゐ|ゞ/゙=ヸ|ヾ/゙" +
			"&[before 3]ヱ<<<ヱ|ヽ=ゑ|ゝ=ヹ|ヽ=ヱ|ヾ/゙=ゑ|ゞ/゙=ヹ|ヾ/゙" +
			"&[before 3]ヲ<<<ヲ|ヽ=ｦ|ヽ=を|ゝ=ヺ|ヽ=ヲ|ヾ/゙=ｦ|ヾ/゙=を|ゞ/゙=ヺ|ヾ/゙" +
			"&[before 3]ン<<<ン|ヽ=ﾝ|ヽ=ん|ゝ" +
			"&ァ=ァ=ぁ=ｧ" +
			"&ア=ア=あ=ｱ" +
			"&ィ=ィ=ぃ=ｨ" +
			"&イ=イ=い=ｲ" +
			"&ゥ=ゥ=ぅ=ｩ" +
			"&ウ=ウ=う=ｳ" +
			"&ェ=ェ=ぇ=ｪ" +
			"&エ=エ=え=ｴ" +
			"&ォ=ォ=ぉ=ｫ" +
			"&オ=オ=お=ｵ" +
			"&カ=カ=か=ｶ" +
			"&キ=キ=き=ｷ" +
			"&ク=ク=く=ｸ" +
			"&ケ=ケ=け=ｹ" +
			"&コ=コ=こ=ｺ" +
			"&サ=サ=さ=ｻ" +
			"&シ=シ=し=ｼ" +
			"&ス=ス=す=ｽ" +
			"&セ=セ=せ=ｾ" +
			"&ソ=ソ=そ=ｿ" +
			"&タ=タ=た=ﾀ" +
			"&チ=チ=ち=ﾁ" +
			"&ッ=ッ=っ=ｯ" +
			"&ツ=ツ=つ=ﾂ" +
			"&テ=テ=て=ﾃ" +
			"&ト=ト=と=ﾄ" +
			"&ナ=ナ=な=ﾅ" +
			"&ニ=ニ=に=ﾆ" +
			"&ヌ=ヌ=ぬ=ﾇ" +
			"&ネ=ネ=ね=ﾈ" +
			"&ノ=ノ=の=ﾉ" +
			"&ハ=ハ=は=ﾊ" +
			"&ヒ=ヒ=ひ=ﾋ" +
			"&フ=フ=ふ=ﾌ" +
			"&ヘ=ヘ=へ=ﾍ" +
			"&ホ=ホ=ほ=ﾎ" +
			"&マ=マ=ま=ﾏ" +
			"&ミ=ミ=み=ﾐ" +
			"&ム=ム=む=ﾑ" +
			"&メ=メ=め=ﾒ" +
			"&モ=モ=も=ﾓ" +
			"&ャ=ャ=ゃ=ｬ" +
			"&ヤ=ヤ=や=ﾔ" +
			"&ュ=ュ=ゅ=ｭ" +
			"&ユ=ユ=ゆ=ﾕ" +
			"&ョ=ョ=ょ=ｮ" +
			"&ヨ=ヨ=よ=ﾖ" +
			"&ラ=ラ=ら=ﾗ" +
			"&リ=リ=り=ﾘ" +
			"&ル=ル=る=ﾙ" +
			"&レ=レ=れ=ﾚ" +
			"&ロ=ロ=ろ=ﾛ" +
			"&ヮ=ヮ=ゎ" +
			"&ワ=ワ=わ=ﾜ" +
			"&ヰ=ヰ=ゐ" +
			"&ヱ=ヱ=ゑ" +
			"&ヲ=ヲ=を=ｦ" +
			"&ン=ン=ん=ﾝ" +
			"&ヵ=ヵ" +
			"&ヶ=ヶ" +
			"&ー=ー" +
			"&゙=゙" +
			"&゚=゚" +
			"&'\u0020'='\u3000'=￣";
	}
}
