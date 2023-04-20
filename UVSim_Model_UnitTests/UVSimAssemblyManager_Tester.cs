using UVSim;

namespace UVSimLogic_UnitTests
{
    [TestClass]
    public class UVSimAssemblyManager_Tester
    {
        [TestMethod]
        public void CreateProgram_ProgramTooBig()
        {
            BasicMLAssemblyManager _testManagerObject = new();

            Assert.ThrowsException<System.ArgumentException>(() => _testManagerObject.CreateAssembly(new string[101]));
        }

        [TestMethod]
        public void CreateProgram_ProgramEmpty()
        {
            BasicMLAssemblyManager _testManagerObject = new();

            Assert.ThrowsException<System.ArgumentException>(() => _testManagerObject.CreateAssembly(Array.Empty<string>()));
        }

        [TestMethod]
        public void CreateProgram_InvalidInput()
        {
            BasicMLAssemblyManager _testManagerObject = new();

            Assert.ThrowsException<System.ArgumentException>(() => _testManagerObject.CreateAssembly(new string[] { "999999" }));
        }

        [TestMethod]
        public void CreateProgram_ValidInput()
        {
            BasicMLAssemblyManager _testManagerObject = new();

            string program = "+1107,+2007,+3106,+2107,+4108,+4000,#+0003,#+0001,+4300";

            Assert.IsTrue(_testManagerObject.CreateAssembly(program.Split(',')));
            Assert.AreEqual(1, _testManagerObject.LoadedAssembliesCount);
        }

        [TestMethod]
        public void DeleteProgram_CollectionEmpty()
        {
            BasicMLAssemblyManager _testManagerObject = new();

            Assert.ThrowsException<System.InvalidOperationException>(() => _testManagerObject.DeleteProgram(0));
        }

        [TestMethod]
        public void DeleteProgram_BadIndex()
        {
            BasicMLAssemblyManager _testManagerObject = new();

            string program = "+1107,+2007,+3106,+2107,+4108,+4000,#+0003,#+0001,+4300";

            _testManagerObject.CreateAssembly(program.Split(','));

            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => _testManagerObject.DeleteProgram(1));
        }

        [TestMethod]
        public void DeleteProgram_GoodIndex()
        {
            BasicMLAssemblyManager _testManagerObject = new();

            string program = "+1107,+2007,+3106,+2107,+4108,+4000,#+0003,#+0001,+4300";

            _testManagerObject.CreateAssembly(program.Split(','));

            _testManagerObject.DeleteProgram(0);

            Assert.AreEqual(0, _testManagerObject.LoadedAssembliesCount);
        }

        [TestMethod]
        public void SerializePrograms_InvalidFilePath()
        {
            BasicMLAssemblyManager _testManagerObject = new();

            Assert.ThrowsException<System.ArgumentNullException>(() => _testManagerObject.SerializeAssemblies(new int[] { 0 }, ""));
        }

        [TestMethod]
        public void SerializePrograms_CollectionEmpty()
        {
            BasicMLAssemblyManager _testManagerObject = new();

            Assert.ThrowsException<System.InvalidOperationException>(() => _testManagerObject.SerializeAssemblies(new int[] { 0 }, "../"));
        }

        [TestMethod]
        public void SerializePrograms_Sucess()
        {
            BasicMLAssemblyManager _testManagerObject = new();

            string program = "+1107,+2007,+3106,+2107,+4108,+4000,#+0003,#+0001,+4300";

            _testManagerObject.CreateAssembly(program.Split(','));

            string directory = "../../../";
            string fileName = "TestSerialize01";

            _testManagerObject.SerializeAssemblies(new int[] { 0 }, directory, new string[] { fileName });

            byte[] buffer = new byte[2];

            using (BinaryReader reader = new(new FileStream(directory + "\\" + fileName + ".BML", FileMode.Open)))
            {
                reader.Read(buffer,0,2);
            }

            Assert.AreEqual(_testManagerObject[0][0], BitConverter.ToInt16(buffer,0));
        }

        [TestMethod]
        public void DeserializePrograms_EmptyFilePath()
        {
            BasicMLAssemblyManager _testManagerObject = new();

            Assert.ThrowsException<System.ArgumentException>(() => _testManagerObject.DeserializeAssemblies(new string[] { "" }));
        }

        [TestMethod]
        public void DeserializePrograms_Success()
        {
            BasicMLAssemblyManager _testManagerObject = new();

            string program = "+1107,+2007,+3106,+2107,+4108,+4000,#+0003,#+0001,+4300";

            _testManagerObject.CreateAssembly(program.Split(','));

            string directory = "../../../";
            string fileName = "TestSerialize01";

            _testManagerObject.SerializeAssemblies(new int[] { 0 }, directory, new string[] { fileName });

            Int16 code = _testManagerObject[0][0];

            _testManagerObject.DeleteProgram(0);

            Assert.AreEqual(0, _testManagerObject.LoadedAssembliesCount);

            _testManagerObject.DeserializeAssemblies(new string[] { directory + "\\" + fileName + ".BML" });

            Assert.AreEqual(1, _testManagerObject.LoadedAssembliesCount);

            Assert.AreEqual(code, _testManagerObject[0][0]);
        }
    }
}
