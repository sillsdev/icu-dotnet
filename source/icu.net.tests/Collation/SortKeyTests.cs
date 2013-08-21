using System;
using System.Globalization;
using Icu.Collation;
using NUnit.Framework;

namespace Icu.Tests.Collation
{
	[TestFixture]
	public class SortKeyTests
	{
		public const int Follows = 1;
		public const int Precedes = -1;
		public const int Same = 0;

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SortKey_nullKeyData_Throws()
		{
			Collator.CreateSortKey("hello", null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void SortKey_KeyDataLengthTooLarge_Throws()
		{
			byte[] keyData = new byte[] { 0xae, 0x1,0x20,0x1};
			Collator.CreateSortKey("hello", keyData, keyData.Length+1);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void SortKey_KeyDataLengthNegative_Throws()
		{
			byte[] keyData = new byte[] { 0xae, 0x1, 0x20,0x1 };
			Collator.CreateSortKey("hello", keyData, -1);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SortKey_nullOriginalString_Throws()
		{
			byte[] keyData = new byte[] { 0xae, 0x1, 0x20,0x1 };
			Collator.CreateSortKey(null, keyData);
		}

		[Test]
		public void ConstructSortKey()
		{
			byte[] keyData = new byte[] { 0xae, 0x1, 0x20,0x1 };
			Assert.IsNotNull(Collator.CreateSortKey("iou", keyData));
		}

		[Test]
		public void Compare_keyDataChanges_NotAffected()
		{
			byte[] keyData = new byte[] { 0xae, 0x1, 0x20,0x1 };
			SortKey sortKey1 = Collator.CreateSortKey("heo", keyData);
			keyData = new byte[] { 0xae, 0x1, 0x21,0x1 };
			SortKey sortKey2 = Collator.CreateSortKey("heol", keyData);
			Assert.AreEqual(Precedes, SortKey.Compare(sortKey1, sortKey2));
		}

		[Test]
		public void Compare_keyDataByteChanges_NotAffected()
		{
			byte[] keyData = new byte[] { 0xae, 0x1, 0x20, 0x1 };
			SortKey sortKey1 = Collator.CreateSortKey("heo", keyData);
			keyData[2] = 0x21;
			SortKey sortKey2 = Collator.CreateSortKey("hao", keyData);
			Assert.AreEqual(Precedes, SortKey.Compare(sortKey1, sortKey2));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Compare_bothnull_throws()
		{
			SortKey.Compare(null, null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Compare_firstnull_throws()
		{
			byte[] keyData = new byte[] { 0xae, 0x1, 0x20, 0x1 };
			SortKey sortKey = Collator.CreateSortKey("heo", keyData);
			SortKey.Compare(null, sortKey);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Compare_secondnull_throws()
		{
			byte[] keyData = new byte[] { 0xae, 0x1, 0x20, 0x1 };
			SortKey sortKey = Collator.CreateSortKey("heo", keyData);
			SortKey.Compare(sortKey, null);
		}

		[Test]
		public void Compare_keyDataSame_same()
		{
			byte[] keyData = new byte[] { 0xae, 0x1, 0x20, 0x1 };
			SortKey sortKey1 = Collator.CreateSortKey("heo", keyData);
			SortKey sortKey2 = Collator.CreateSortKey("heol", keyData);
			Assert.AreEqual(Same, SortKey.Compare(sortKey1, sortKey2));
		}

		[Test]
		public void Compare_SamePrefixSecondLonger_precedes()
		{
			byte[] keyData1 = new byte[] { 0xae, 0x1, 0x20,0x1 };
			SortKey sortKey1 = Collator.CreateSortKey("heo", keyData1);
			byte[] keyData2 = new byte[] { 0xae, 0x1, 0x20, 0x32, 0x1 };
			SortKey sortKey2 = Collator.CreateSortKey("heol", keyData2);
			Assert.AreEqual(Precedes, SortKey.Compare(sortKey1, sortKey2));
		}

		[Test]
		public void Compare_SamePrefixSecondShorter_follows()
		{
			byte[] keyData1 = new byte[] { 0xae, 0x1, 0x20, 0x30, 0x1 };
			SortKey sortKey1 = Collator.CreateSortKey("heol", keyData1);
			byte[] keyData2 = new byte[] { 0xae, 0x1, 0x20, 0x1 };
			SortKey sortKey2 = Collator.CreateSortKey("heo", keyData2);
			Assert.AreEqual(Follows, SortKey.Compare(sortKey1, sortKey2));
		}


		[Test]
		public void Compare_SecondGreater_precedes()
		{
			byte[] keyData = new byte[] { 0xae, 0x1, 0x20, 0x1 };
			SortKey sortKey1 = Collator.CreateSortKey("heol", keyData);
			keyData[0] = 0xaf;
			SortKey sortKey2 = Collator.CreateSortKey("heo", keyData);
			Assert.AreEqual(Precedes, SortKey.Compare(sortKey1, sortKey2));
		}

		[Test]
		public void Compare_SecondLesser_follows()
		{
			byte[] keyData = new byte[] { 0xae, 0x1, 0x20, 0x1 };
			SortKey sortKey1 = Collator.CreateSortKey("heol", keyData);
			keyData[0] = 0xad;
			SortKey sortKey2 = Collator.CreateSortKey("heo", keyData);
			Assert.AreEqual(Follows, SortKey.Compare(sortKey1, sortKey2));
		}

	}

}