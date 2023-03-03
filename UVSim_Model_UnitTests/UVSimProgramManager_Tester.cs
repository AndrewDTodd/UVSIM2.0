using UVSim;

namespace UVSimLogic_UnitTests
{
    [TestClass]
    public class UVSimProgramManager_Tester
    {
        [TestMethod]
        public void CreateProgram_ProgramTooBig()
        {
            UVSimProgramManager _testManagerObject = new();

            Assert.ThrowsException<System.ArgumentException>(() => _testManagerObject.CreateProgram(new string[101]));
        }

        [TestMethod]
        public void CreateProgram_ProgramEmpty()
        {
            UVSimProgramManager _testManagerObject = new();

            Assert.ThrowsException<System.ArgumentException>(() => _testManagerObject.CreateProgram(Array.Empty<string>()));
        }

        [TestMethod]
        public void CreateProgram_InvalidInput()
        {
            UVSimProgramManager _testManagerObject = new();

            Assert.ThrowsException<System.ArgumentException>(() => _testManagerObject.CreateProgram(new string[] { "999999" }));
        }

        [TestMethod]
        public void CreateProgram_ValidInput()
        {
            UVSimProgramManager _testManagerObject = new();

            string program = "+1107,+2007,+3106,+2107,+4108,+4000,#+0003,#+0001,+4300";

            Assert.IsTrue(_testManagerObject.CreateProgram(program.Split(',')));
            Assert.AreEqual(1, _testManagerObject.LoadedProgramsCount);
        }

        [TestMethod]
        public void DeleteProgram_CollectionEmpty()
        {
            UVSimProgramManager _testManagerObject = new();

            Assert.ThrowsException<System.InvalidOperationException>(() => _testManagerObject.DeleteProgram(0));
        }

        [TestMethod]
        public void DeleteProgram_BadIndex()
        {
            UVSimProgramManager _testManagerObject = new();

            string program = "+1107,+2007,+3106,+2107,+4108,+4000,#+0003,#+0001,+4300";

            _testManagerObject.CreateProgram(program.Split(','));

            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => _testManagerObject.DeleteProgram(1));
        }

        [TestMethod]
        public void DeleteProgram_GoodIndex()
        {
            UVSimProgramManager _testManagerObject = new();

            string program = "+1107,+2007,+3106,+2107,+4108,+4000,#+0003,#+0001,+4300";

            _testManagerObject.CreateProgram(program.Split(','));

            _testManagerObject.DeleteProgram(0);

            Assert.AreEqual(0, _testManagerObject.LoadedProgramsCount);
        }

        [TestMethod]
        public void SerializePrograms_InvalidFilePath()
        {
            UVSimProgramManager _testManagerObject = new();

            Assert.ThrowsException<System.ArgumentNullException>(() => _testManagerObject.SerializePrograms(new int[] { 0 }, ""));
        }

        [TestMethod]
        public void SerializePrograms_CollectionEmpty()
        {
            UVSimProgramManager _testManagerObject = new();

            Assert.ThrowsException<System.InvalidOperationException>(() => _testManagerObject.SerializePrograms(new int[] { 0 }, "../"));
        }

        [TestMethod]
        public void SerializePrograms_Sucess()
        {
            UVSimProgramManager _testManagerObject = new();

            string program = "+1107,+2007,+3106,+2107,+4108,+4000,#+0003,#+0001,+4300";

            _testManagerObject.CreateProgram(program.Split(','));

            string directory = "../../../";
            string fileName = "TestSerialize01";

            _testManagerObject.SerializePrograms(new int[] { 0 }, directory, new string[] { fileName });

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
            UVSimProgramManager _testManagerObject = new();

            Assert.ThrowsException<System.ArgumentException>(() => _testManagerObject.DeserializePrograms(new string[] { "" }));
        }

        [TestMethod]
        public void DeserializePrograms_Success()
        {
            UVSimProgramManager _testManagerObject = new();

            string program = "+1107,+2007,+3106,+2107,+4108,+4000,#+0003,#+0001,+4300";

            _testManagerObject.CreateProgram(program.Split(','));

            string directory = "../../../";
            string fileName = "TestSerialize01";

            _testManagerObject.SerializePrograms(new int[] { 0 }, directory, new string[] { fileName });

            Int16 code = _testManagerObject[0][0];

            _testManagerObject.DeleteProgram(0);

            Assert.AreEqual(0, _testManagerObject.LoadedProgramsCount);

            _testManagerObject.DeserializePrograms(new string[] { directory + "\\" + fileName + ".BML" });

            Assert.AreEqual(1, _testManagerObject.LoadedProgramsCount);

            Assert.AreEqual(code, _testManagerObject[0][0]);
        }
    }
}
