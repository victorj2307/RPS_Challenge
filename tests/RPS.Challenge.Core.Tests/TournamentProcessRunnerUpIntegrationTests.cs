using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RPS.Challenge.Core.Processes;

namespace RPS.Challenge.Core.Tests {
    [TestClass]
    public class TournamentProcessRunnerUpIntegrationTests {
        [TestMethod]
        public void TwoPlayerFinal_LoserIsSecondPlace() {
            // Paper vs Scissors -> Scissors beats Paper -> Dave wins, Armando second.
            byte[] bytes = Encoding.UTF8.GetBytes("[[[\"Armando\",\"P\"],[\"Dave\",\"S\"]]]");
            var result = TournamentProcess.ProcessTournamentDetailed(bytes);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Dave", result.WinnerName);
            Assert.AreEqual("Armando", result.SecondPlaceName);
            Assert.IsFalse(string.IsNullOrEmpty(result.SecondPlaceStrategy));
        }
    }
}
