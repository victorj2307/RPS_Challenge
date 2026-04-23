using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RPS.Challenge.Core.Validation;

namespace RPS.Challenge.Core.Tests {
    [TestClass]
    public class TournamentPayloadValidatorTests {
        [TestMethod]
        public void EmptyRootArray_IsInvalid() {
            string err;
            Assert.IsFalse(TournamentPayloadValidator.TryValidateSingleChampionshipJson(Encoding.UTF8.GetBytes("[]"), out err));
            StringAssert.Contains(err, "empty");
        }

        [TestMethod]
        public void TrailingSecondArray_IsInvalid() {
            string err;
            string twoDocuments = "[[[\"A\",\"R\"],[\"B\",\"S\"]]] []";
            Assert.IsFalse(TournamentPayloadValidator.TryValidateSingleChampionshipJson(Encoding.UTF8.GetBytes(twoDocuments), out err));
            StringAssert.Contains(err, "exactly one");
        }

        [TestMethod]
        public void RootObject_IsInvalid() {
            string err;
            Assert.IsFalse(TournamentPayloadValidator.TryValidateSingleChampionshipJson(Encoding.UTF8.GetBytes("{}"), out err));
            StringAssert.Contains(err, "array");
        }

        [TestMethod]
        public void ValidMinimalBracket_Passes() {
            string json = "[[[\"A\",\"R\"],[\"B\",\"S\"]]]";
            string err;
            string normalized;
            Assert.IsTrue(TournamentPayloadValidator.TryValidateSingleChampionshipJson(Encoding.UTF8.GetBytes(json), out normalized, out err));
            Assert.IsFalse(string.IsNullOrEmpty(normalized));
        }
    }
}
