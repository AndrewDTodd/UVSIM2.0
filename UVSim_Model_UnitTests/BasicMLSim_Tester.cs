using UVSim;

namespace UVSimLogic_UnitTests
{
    [TestClass]
    public class BasicMLSim_Tester
    {
        [TestMethod]
        public void LoadProgram_TestExceptions()
        {
            //create Unit Test on the _testCaseObject.LoadProgram(IList<short>) method here

            BasicMLSim _testCaseObject = new();

            List<Int16>? testList = null;

            Assert.ThrowsException<System.ArgumentNullException>(() => _testCaseObject.LoadProgram(testList));

            testList = new List<Int16>(101);
            testList.AddRange(new Int16[101]);

            Assert.ThrowsException<System.ArgumentException>(() => _testCaseObject.LoadProgram(testList));
        }
        [TestMethod]
        public void LoadProgram_TestValidInput()
        {
            BasicMLSim _testCaseObject = new();

            List<Int16>? testList = new(50);
            testList.AddRange(new Int16[50]);

            testList[0] = Int16.MaxValue;

            _testCaseObject.LoadProgram(testList);

            Assert.AreEqual(Int16.MaxValue, _testCaseObject.Memory[0]);
        }

        [TestMethod]
        public void RunProgram_NoInput()
        {
            //create Unit Test on the _testCaseObject.RunProgram() method here

            BasicMLSim _testCaseObject = new();

            Assert.ThrowsException<System.InvalidOperationException>(() => _testCaseObject.RunProgram());
        }

        [TestMethod]
        public void RunProgram_BadInput()
        {
            BasicMLSim _testCaseObject = new();
            BasicMLAssemblyManager _manager = new();

            _testCaseObject.LoadProgram(_manager.ParseProgram(new string[] {"9900"}).Words);

            Assert.ThrowsException<System.ArgumentException>(() => _testCaseObject.RunProgram());
        }

        [TestMethod]
        public void RunProgram_GoodInput()
        {
            BasicMLSim _testCaseObject = new();
            BasicMLAssemblyManager _manager = new();

            _testCaseObject.LoadProgram(_manager.ParseProgram(new string[] 
            {
                "2003", 
                "2121", 
                "4300", 
                "#+5150"
            }).Words);
            _testCaseObject.RunProgram();

            Assert.AreEqual(5150, _testCaseObject.Memory[21]);
        }
    }
}