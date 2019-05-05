using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using WpfDirMaker;

namespace UnitTests
{
    [TestClass]
    public class UnitTestCore
    {
        private MyIO myIO;
        private Interpreter mInterpreter;

        [TestInitialize]
        public void setUpTests()
        {
            myIO = new MyIO();
            mInterpreter = new Interpreter(myIO);
            myIO.RemoveShitString = "Mors,KORS,asdf,dfghdhdgh,.AAA,MP4-KTR,APA,Kaka";
        }

        [TestMethod]
        public void GivenDottedStr_When1NameAndItIsOnly1Name_ThenExpectTrue()
        {
            var lastFullNameHasNumberOfNames = 1;
            var instr = "Hejsan.AAA.17.11.01.Tjerna.888..mors.kors.AAA[asdf]";
            var expected = "Tjerna - Hejsan171101 - MorsKors[888]";
            var result = mInterpreter.InterpretDottedString(instr, lastFullNameHasNumberOfNames);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GivenDottedStr_When1NameAndItIsFirstPlusLastname_ThenExpectTrue()
        {
            var lastFullNameHasNumberOfNames = 2;
            var instr = "Hejsan.AAA.17.11.01.Tjerna.PerAAAsson.888..mors.kors.AAA[asdf]";
            var expected = "Tjerna PerAAAsson - Hejsan171101 - MorsKors[888]";
            var result = mInterpreter.InterpretDottedString(instr, lastFullNameHasNumberOfNames);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GivenDottedStr_When1NameAndItIsFirstAndMiddlePlusLastname_ThenExpectTrue()
        {
            var lastFullNameHasNumberOfNames = 3;
            var instr = "Hejsan.AAA.17.11.01.Tjerna.von.PerAAAsson.888..mors.kors.AAA[asdf]";
            var expected = "Tjerna Von PerAAAsson - Hejsan171101 - MorsKors[888]";
            var result = mInterpreter.InterpretDottedString(instr, lastFullNameHasNumberOfNames);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GivenDottedStr_When2NamesAndLastNameIsFirstPlusLastname_ThenExpectTrue()
        {
            var lastFullNameHasNumberOfNames = 2;
            var instr = "Hejsan.AAA.17.11.01.Nisse.Johansson.ANd.Tjerna.PerAAAsson.888..mors.kors.AAA[asdf]";
            var expected = "Nisse Johansson + Tjerna PerAAAsson - Hejsan171101 - MorsKors[888]";
            var result = mInterpreter.InterpretDottedString(instr, lastFullNameHasNumberOfNames);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GivenDottedStr_When3NamesAndLastNameIsMessi_ThenExpectTrue()
        {
            var lastFullNameHasNumberOfNames = 1;
            var instr = "Hejsan.AAA.17.11.01.Nisse.Johansson.ANd.Tjerna.PerAAAsson.and.Messi.888..mors.kors.AAA[asdf]";
            var expected = "Nisse Johansson + Tjerna PerAAAsson + Messi - Hejsan171101 - MorsKors[888]";
            var result = mInterpreter.InterpretDottedString(instr, lastFullNameHasNumberOfNames);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GivenDottedStr_When3NamesAndLastNameIsFredAsp_ThenExpectTrue()
        {
            var lastFullNameHasNumberOfNames = 2;
            var instr = "Hejsan.AAA.17.11.01.Nisse.Johansson.ANd.Tjerna.PerAAAsson.and.Fred.Asp.888..mors.kors.AAA[asdf]";
            var expected = "Nisse Johansson + Tjerna PerAAAsson + Fred Asp - Hejsan171101 - MorsKors[888]";
            var result = mInterpreter.InterpretDottedString(instr, lastFullNameHasNumberOfNames);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GivenDottedStr_When3NamesAndFirstNameIsMessiAndLastNameIsFredAspAndMessi_ThenExpectTrue()
        {
            var lastFullNameHasNumberOfNames = 2;
            var instr = "Hejsan.AAA.17.11.01.Messi.ANd.Tjerna.PerAAAsson.and.Fred.Asp.888..mors.kors.AAA[asdf]";
            var expected = "Messi + Tjerna PerAAAsson + Fred Asp - Hejsan171101 - MorsKors[888]";
            var result = mInterpreter.InterpretDottedString(instr, lastFullNameHasNumberOfNames);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GivenDottedStr_When3NamesAndLastNameIsFrankDeBoor_ThenExpectTrue()
        {
            var lastFullNameHasNumberOfNames = 3;
            var instr = "Hejsan.AAA.17.11.01.Nisse.Johansson.ANd.Tjerna.PerAAAsson.and.Frank.De.Boor.888..mors.kors.tjenare.hejsan.hello.AAA[asdf]";
            var expected = "Nisse Johansson + Tjerna PerAAAsson + Frank De Boor - Hejsan171101 - MorsKorsTjenareHejsanHello[888]";
            var result = mInterpreter.InterpretDottedString(instr, lastFullNameHasNumberOfNames);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GivenStrwithSpecialChars_When1NamesAndFullName_ThenExpectTrue()
        {
            var lastFullNameHasNumberOfNames = 3;
            var instr = "Hejsan.AAA.(171101)-Nisse.Johansson.ANd.Tjerna.PerAAAsson.and.Frank.De.Boor.888..mors.kors.[asdf]";
            var expected = "Nisse Johansson + Tjerna PerAAAsson + Frank De Boor - Hejsan171101 - MorsKors[888]";
            var result = mInterpreter.InterpretDottedString(instr, lastFullNameHasNumberOfNames);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GivenDottedStrWithMP4KTR_When2NamesAndLastNameIsFirstPlusLastname_ThenExpectTrue()
        {
            var lastFullNameHasNumberOfNames = 2;
            var instr = "Hejsan.AAA.17.11.01.Nisse.Johansson.ANd.Tjerna.PerAAAsson.888..mors.kors.AAA.MP4-KTR[asdf]";
            var expected = "Nisse Johansson + Tjerna PerAAAsson - Hejsan171101 - MorsKors[888]";
            var result = mInterpreter.InterpretDottedString(instr, lastFullNameHasNumberOfNames);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [TestCase("apa")]
        public void SanityTest(string apa)
        {
            NUnit.Framework.Assert.IsTrue(apa == "apa");
        }


    }
}
