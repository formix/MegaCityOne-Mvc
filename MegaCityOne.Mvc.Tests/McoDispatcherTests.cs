using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MegaCityOne.Mvc.Tests
{
    [TestClass]
    public class McoDispatcherTests
    {
        public McoDispatcherTests()
        {
            McoDispatcher.Current.Summon += McoDispatcherCurrent_Summon;
        }

        void McoDispatcherCurrent_Summon(object source, JudgeSummonEventArgs e)
        {
            JudgeDredd judge = new JudgeDredd();
            judge.Laws.Add("CanTestWithoutContext", (principal, arguments) => true);
            e.Respondent = judge;
        }


        [TestMethod]
        public void TestAdviseWithoutHttpContext()
        {
            var result = McoDispatcher.Advise("CanTestWithoutContext");
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void TestEnforceWithoutHttpContext()
        {
            McoDispatcher.Enforce("CanTestWithoutContext");
        }
    }
}
