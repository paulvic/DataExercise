using NUnit.Framework;
using System;
using System.IO;

namespace TopN.Tests
{
    public class UnitTests
    {
        [TestFixture]
        class TopNUnitTests
        {
            private string assemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            [Test]
            public void IsPathValidated()
            {
                FileSorter sorter = new FileSorter();
                FileNotFoundException ex = Assert.Throws<FileNotFoundException>(
                    delegate { sorter.TopN(GetFullTestDataPath("1.txt"), 1); });
            }

            [Test]
            public void IsNValidated()
            {
                FileSorter sorter = new FileSorter();
                ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(
                    delegate { sorter.TopN(@"", 0); });
                Assert.That(ex.ParamName, Is.EqualTo("n must be greater than 0"));
            }

            [Test]
            public void Test_SmallFile_Distinct_Top1()
            {                
                using (var testStreamWriter = new TestStreamWriter(new MemoryStream()))
                {
                    FileSorter sorter = new FileSorter();
                    sorter.TopN(GetFullTestDataPath("Valid_10Numbers_Distinct.txt"), 1, testStreamWriter);
                    int[] expected = new int[] { 9 };
                    int[] actual = testStreamWriter.Data;
                    Assert.AreEqual(expected, actual);
                }
            }

            [Test]
            public void Test_SmallFile_Distinct_Top3()
            {                
                using (var testStreamWriter = new TestStreamWriter(new MemoryStream()))
                {
                    FileSorter sorter = new FileSorter();
                    sorter.TopN(GetFullTestDataPath("Valid_10Numbers_Distinct.txt"), 3, testStreamWriter);
                    int[] expected = new int[] { 9, 8, 7 };
                    int[] actual = testStreamWriter.Data;
                    Assert.AreEqual(expected, actual);
                }
            }

            [Test]
            public void Test_SmallFile_Distinct_Top5()
            {                
                using (var testStreamWriter = new TestStreamWriter(new MemoryStream()))
                {
                    FileSorter sorter = new FileSorter();
                    sorter.TopN(GetFullTestDataPath("Valid_10Numbers_Distinct.txt"), 5, testStreamWriter);
                    int[] expected = new int[] { 9, 8, 7, 6, 5 };
                    int[] actual = testStreamWriter.Data;
                    Assert.AreEqual(expected, actual);
                }
            }

            [Test]
            public void Test_SmallFile_Duplicates_Top3()
            {
                using (var testStreamWriter = new TestStreamWriter(new MemoryStream()))
                {
                    FileSorter sorter = new FileSorter();
                    sorter.TopN(GetFullTestDataPath("Valid_10Numbers_Duplicates.txt"), 3, testStreamWriter);
                    int[] expected = new int[] { 8, 8, 6 };
                    int[] actual = testStreamWriter.Data;
                    Assert.AreEqual(expected, actual);
                }
            }
            
            [Test]
            public void Test_SmallFile_Negative_Top10()
            {                
                using (var testStreamWriter = new TestStreamWriter(new MemoryStream()))
                {
                    FileSorter sorter = new FileSorter();
                    sorter.TopN(GetFullTestDataPath("Valid_10Numbers_Negative.txt"), 10, testStreamWriter);
                    int[] expected = new int[] { 9, 8, 6, 5, 4, 3, 2, 0, -1, -7 };
                    int[] actual = testStreamWriter.Data;
                    Assert.AreEqual(expected, actual);
                }
            }

            [Test]
            public void Test_SmallFile_Distinct_Top14()
            {
                // Asking for the top 14 numbers, but file only contains 10 numbers                
                using (var testStreamWriter = new TestStreamWriter(new MemoryStream()))
                {
                    FileSorter sorter = new FileSorter();
                    int resultsReturned = sorter.TopN(
                        GetFullTestDataPath("Valid_10Numbers_Distinct.txt"), 14, testStreamWriter);
                    int[] expected = new int[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
                    int[] actual = testStreamWriter.Data;
                    Assert.AreEqual(expected, actual);
                    Assert.AreEqual(10, resultsReturned);
                }
            }

            [Test]
            public void Test_MediumFile_Duplicates_Top4()
            {
                using (var testStreamWriter = new TestStreamWriter(new MemoryStream()))
                {
                    FileSorter sorter = new FileSorter();
                    int resultsReturned = sorter.TopN(
                        GetFullTestDataPath("Valid_1000000Numbers_Duplicates.txt"), 4, testStreamWriter);
                    int[] expected = new int[] { 2147483422, 2147483421, 2147483421, 2147483420 };
                    int[] actual = testStreamWriter.Data;
                    Assert.AreEqual(expected, actual);
                }
            }
            
            [Test]
            public void Test_Invalid_NonNumbers_Top1()
            {
                FileSorter sorter = new FileSorter();
                FormatException ex = Assert.Throws<FormatException>(
                    delegate { sorter.TopN(GetFullTestDataPath("Invalid_NonNumbers.txt"), 1); });
            }

            private string GetFullTestDataPath(string testFile)
            {
                return String.Format($"{assemblyPath}\\TestData\\{testFile}");
            }
        }
    }
}
