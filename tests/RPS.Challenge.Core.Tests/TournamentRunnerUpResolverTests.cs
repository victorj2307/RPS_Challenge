using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RPS.Challenge.Core.Entities;
using RPS.Challenge.Core.Processes;

namespace RPS.Challenge.Core.Tests {
    [TestClass]
    public class TournamentRunnerUpResolverTests {
        [TestMethod]
        public void FinalRound_RealMatchAfterBye_PicksLoserOfRealMatch() {
            var result = new TournamentResult {
                IsSuccess = true,
                Rounds = new List<RoundResult> {
                    new RoundResult {
                        RoundNumber = 1,
                        Matches = new List<MatchResult>()
                    },
                    new RoundResult {
                        RoundNumber = 2,
                        Matches = new List<MatchResult> {
                            new MatchResult { IsBye = true, LoserName = string.Empty },
                            new MatchResult {
                                IsBye = false,
                                LoserName = "Runner",
                                LoserStrategy = "Paper"
                            }
                        }
                    }
                }
            };

            string name;
            string strategy;
            Assert.IsTrue(TournamentRunnerUpResolver.TryGetRunnerUp(result, out name, out strategy));
            Assert.AreEqual("Runner", name);
            Assert.AreEqual("Paper", strategy);
        }
    }
}
