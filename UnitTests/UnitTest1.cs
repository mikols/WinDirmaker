using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using WpfDirMaker;

namespace UnitTests
{
    [TestFixture]
    public class UnitTestCore
    {

        [Test]
        public void GivenDottedStr_WhenA_ExpectTrue()
        {
            MyIO myIO = new MyIO();

            Interpreter mInterpreter = new Interpreter();
            var instr = "Hejsan.AAA.17.11.01.Nisse.Johansson.ANd.Tjerna.PerAAAsson.888..mors.kors.AAA[asdf]";
            var outStr = "Nisse Johansson + Tjerna PerAAAsson - Hejsan171101 - MorsKors[888]";
            var result = mInterpreter.InterpretDottedString(instr, 2);
            //Assert.IsTrue(result == outStr);
            NUnit.Framework.Assert.IsTrue(instr == result);
        }

        [TestCase("apa")]
        public void TestMethod2(string apa)
        {
            var instr = "MorsningKorsning.17.11.01.KnaAaAsen.JohAAAnsson.ANd.Tjerna.888..HeyBaberibba.AAA[asdf]";
            var outStr = "apa";
            NUnit.Framework.Assert.IsTrue(outStr == "apa");
        }

        //[TestMethod]
        //public void TestMethod3()
        //{
        //    var instr3 = "hejsanhoppsan.AAA.17.11.01.KLaAackis.Johansson.ANd.Tjerna.PerAAAsson.888..mors.kors.AAA[asdf]";
        //    var outStr = "apa";
        //    Assert.IsTrue(outStr == "apa");
        //}
    }
}
