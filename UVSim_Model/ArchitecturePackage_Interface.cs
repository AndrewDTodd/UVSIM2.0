using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UVSim
{
    /// <summary>
    /// This abstract class acts as the interface and default implementation for concrete types that wrap the necessary components of the UVSim Model creating a package of items that are needed to run an architecture simulation in this application
    /// </summary>
    public abstract partial class ArchitecturePackage_Interface : ObservableObject
    {
        #region FIELDS
        /// <summary>
        /// A reference to an <seealso cref="ArchitectureSim_Interface"/> concrete implementation
        /// </summary>
        [ObservableProperty]
        protected ArchitectureSim_Interface _architectureSim;

        /// <summary>
        /// A reference to an <seealso cref="AssembliesManagementFixedSize_Interface"/> concrete implementation
        /// </summary>
        [ObservableProperty]
        protected AssembliesManagementFixedSize_Interface _assemblyManager;

        /// <summary>
        /// A refernece to a <seealso cref="ProgramsManagement_Interface"/> concrete implementation
        /// </summary>
        [ObservableProperty]
        protected ProgramsManagement_Interface _programsManager;
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Construct and initialize the package by assigning concrete implementations to the packages fields
        /// </summary>
        /// <param name="architectureSim">The concrete <seealso cref="ArchitectureSim_Interface"/> to use in the package</param>
        /// <param name="assemblyManager">The concrete <seealso cref="AssembliesManagement_Interface"/> to use in the package</param>
        /// <param name="programManager">the concrete <seealso cref="ProgramsManagement_Interface"/> to use in the package</param>
        public ArchitecturePackage_Interface(ArchitectureSim_Interface architectureSim,
            ProgramsManagement_Interface programManager,
            AssembliesManagementFixedSize_Interface assemblyManager) =>
            (_architectureSim, _programsManager, _assemblyManager) = (architectureSim, programManager, assemblyManager);
        #endregion

        #region PROPERTIES

        #endregion

        #region METHODS
        /// <summary>
        /// Adds program objects to the collection and initializes them from the specified files
        /// </summary>
        /// <returns>A list of the FileInfo objects that couldn't be opened properly</returns>
        /// <param name="programFiles">FileInfo objects for the programs to be loaded</param>
        /// <param name="token">A CancelToken to signal to the operation to exit early</param>
        public virtual async Task<List<FileInfo>> OpenPrograms(FileInfo[] programFiles, CancellationToken token)
        {
            Task<List<FileInfo>> task = ProgramsManager.DeserializePrograms(programFiles, token);

            await task;

            return task.Result;
        }

        /// <summary>
        /// Create a new program with an optional name. If one is not provided a default one is generated
        /// </summary>
        /// <param name="programName">The name to give the new program</param>
        public virtual void NewProgram(string? programName = null)
        {
            programName ??= $"program{ProgramsManager.ProgramsCount}";

            if (!ProgramsManager.TryCreateNewProgram(programName))
                throw new InvalidOperationException($"The programs manager was unable to create a new program object with the name {programName}");
        }

        /// <summary>
        /// Save the program at the specified index to disk
        /// </summary>
        /// <param name="index">The index of the program to save</param>
        /// <returns>True if the program was saved successfully, false otherwise</returns>
        public virtual async Task<bool> SaveProgram(int index)
        {
            Task<bool> task = ProgramsManager[index].SerializeProgram();

            await task;

            return task.Result;
        }
        /// <summary>
        /// Save the specified program to disk
        /// </summary>
        /// <param name="program">The program to save</param>
        /// <returns>True if the program was saved successfully, false otherwise</returns>
        /// <exception cref="InvalidOperationException">Thrown if called when the program's file info isnt set</exception>
        public virtual async Task<bool> SaveProgram(Program program)
        {
            Task<bool> task = program.SerializeProgram();

            await task;

            return task.Result;
        }

        /// <summary>
        /// Save a set of programs to disk
        /// </summary>
        /// <param name="indexes">The indexes of the programs to save</param>
        /// <param name="token">A CancelToken to signal to the operation to exit early</param>
        /// <returns>A list of the programs that failed to be saved</returns>
        public virtual async Task<List<Program>> SavePrograms(int[] indexes, CancellationToken token)
        {
            Task<List<Program>> task = ProgramsManager.SerializePrograms(indexes, token);

            await task;

            return task.Result;
        }

        /// <summary>
        /// Save all of the programs in the collection to disk
        /// </summary>
        /// <param name="token">A CancelToken to signal to the operation to exit early</param>
        /// <returns>A list of the programs that failed to be saved</returns>
        public virtual async Task<List<Program>> SaveAllPrograms(CancellationToken token)
        {
            Task<List<Program>> task = ProgramsManager.SerializeAll(token);

            await task;

            return task.Result;
        }

        /// <summary>
        /// Save the program at the specified index to disk at the provided location
        /// </summary>
        /// <param name="index">The index of the program to save</param>
        /// <param name="info">Information concerning where the file is to be saved</param>
        /// <returns>True if the program is saved successfully, false otherwise</returns>
        public virtual async Task<bool> SaveProgramTo(int index, FileInfo info)
        {
            ProgramsManager[index].FileInfo = new($"{info.FullName}//{ProgramsManager[index].ProgramName}.{ProgramsManager[index].Extension}"); ;
            
            Task<bool> task = ProgramsManager[index].SerializeProgram();

            await task;

            return task.Result;
        }
        /// <summary>
        /// Save the specified program to disk at the location provided
        /// </summary>
        /// <param name="program">The program to save</param>
        /// <param name="info">Information concerning where the file is to be saved</param>
        /// <returns>True if the file was successfully saved, false otherwise</returns>
        public virtual async Task<bool> SaveProgramTo(Program program, FileInfo info)
        {
            program.FileInfo = new($"{info.FullName}//{program.ProgramName}.{program.Extension}");
            
            Task<bool> task = program.SerializeProgram();

            await task;

            return task.Result;
        }

        /// <summary>
        /// Create a copy of the program at the specified index and save it to disk
        /// </summary>
        /// <param name="index">The index of the program to save</param>
        /// <param name="newName">Optional name given to the copy, if not provided one is generated</param>
        /// <param name="info">Optional info about where to save the copy, if not provided the copy is saved adjacent to the original</param>
        /// <returns>True if a copy of the program was successfully saved, false otherwise</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual async Task<bool> SaveProgramAs(int index, string? newName = null, FileInfo? info = null)
        {
            if (ProgramsManager[index].FileInfo == null && info == null)
                throw new InvalidOperationException("Can't save a copy of the specified program. No file path specified");

            ProgramsManager.TryCreateNewProgram(newName ?? ProgramsManager[index].ProgramName + "_copy");

            Program program = ProgramsManager.Last;

            program.FileInfo = info;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            program.FileInfo ??= new FileInfo($"{ProgramsManager[index].FileInfo.DirectoryName}.{program.ProgramName}.{program.Extension}");
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            program.TrySetContent(ProgramsManager[index].Text);
            Task<bool> task = program.SerializeProgram();

            await task;

            return task.Result;
        }

        /// <summary>
        /// Adds assembly objects to the collection and initializes them from the specified files
        /// </summary>
        /// <param name="assemblyFiles">FileInfo objects for the assemblies to be loaded</param>
        /// <param name="token">A CancelToken to signal to the operation to exit early</param>
        /// <returns>A list of FileInfos that wern't opened successfully</returns>
        public virtual async Task<List<FileInfo>> OpenAssemblies(FileInfo[] assemblyFiles, CancellationToken token)
        {
            Task<List<FileInfo>> task = AssemblyManager.DeserializeAssemblies(assemblyFiles, token);

            await task;

            return task.Result;
        }

        /// <summary>
        /// Create a new assembly with an optional name. If one is not provided a default one is generated
        /// </summary>
        /// <param name="assemblyName">The name to give the new assembly</param>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual void NewAssembly(string? assemblyName = null)
        {
            assemblyName ??= $"assembly{AssemblyManager.LoadedAssembliesCount}";

            if (!AssemblyManager.TryCreateAssembly(assemblyName))
                throw new InvalidOperationException($"The assembly manager was unable to create a new assembly object with the name {assemblyName}");
        }

        /// <summary>
        /// Save the assembly at the specified index to disk
        /// </summary>
        /// <param name="index">The index of the assembly to save</param>
        /// <returns>True if the assembly is saved successfully, false otherwise</returns>
        public virtual async Task<bool> SaveAssembly(int index)
        {
            Task<bool> task = AssemblyManager[index].SerializeAssembly();

            await task;

            return task.Result;
        }
        /// <summary>
        /// Save the specified assembly to disk
        /// </summary>
        /// <param name="assembly">The assembly to save</param>
        /// <returns>True if the assembly is saved successfully, false otherwise</returns>
        public virtual async Task<bool> SaveAssembly(Assembly assembly)
        {
            Task<bool> task = assembly.SerializeAssembly();

            await task;

            return task.Result;
        }

        /// <summary>
        /// Save a set of assemblies to disk
        /// </summary>
        /// <param name="indexes">The indexes of the assemblies to save</param>
        /// <param name="token">A CancelToken to signal to the operation to exit early</param>
        /// <returns>A list of the assemblies that failed to be saved</returns>
        public virtual async Task<List<Assembly>> SaveAssemblies(int[] indexes, CancellationToken token)
        {
            Task<List<Assembly>> task = AssemblyManager.SerializeAssemblies(indexes, token);

            await task;

            return task.Result;
        }

        /// <summary>
        /// Save all of the assemblies in the collection to disk
        /// </summary>
        /// <param name="token">A CancelToken to signal to the operation to exit early</param>
        /// <returns>A list of the assemblies that failed to be saved</returns>
        public virtual async Task<List<Assembly>> SaveAllAssemblies(CancellationToken token)
        {
            Task<List<Assembly>> task = AssemblyManager.SerializeAll(token);

            await task;

            return task.Result;
        }

        /// <summary>
        /// Save the assembly at the specified index to disk at the provided location
        /// </summary>
        /// <param name="index">The index of the assembly to save</param>
        /// <param name="info">Information concerning where the file is to be saved</param>
        /// <returns>True if the assembly is saved successfully, false otherwise</returns>
        public virtual async Task<bool> SaveAssemblyTo(int index, FileInfo info)
        {
            AssemblyManager[index].FileInfo = new($"{info.FullName}//{AssemblyManager[index].AssemblyName}.{AssemblyManager[index].Extension}");

            Task<bool> task = AssemblyManager[index].SerializeAssembly();

            await task;

            return task.Result;
        }
        /// <summary>
        /// Save the specified assembly to disk at the location provided
        /// </summary>
        /// <param name="assembly">The assembly to save</param>
        /// <param name="info">Information concerning where the file is to be saved</param>
        /// <returns>True if the file was successfully saved, false otherwise</returns>
        public virtual async Task<bool> SaveAssemblyTo(Assembly assembly, FileInfo info)
        {
            assembly.FileInfo = new($"{info.FullName}//{assembly.AssemblyName}.{assembly.Extension}");

            Task<bool> task = assembly.SerializeAssembly();

            await task;

            return task.Result;
        }

        /// <summary>
        /// Create a copy of the assembly at the specified index and save it to disk
        /// </summary>
        /// <param name="index">The index of the assembly to save</param>
        /// <param name="newName">Optional name given to the copy, if not provided one is generated</param>
        /// <param name="info">Optional info about where to save the copy, if not provided the copy is saved adjacent to the original</param>
        /// <returns>True if a copy of the program was successfully saved, false otherwise</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual async Task<bool> SaveAssemblyAs(int index, string? newName = null, FileInfo? info = null)
        {
            if (AssemblyManager[index].FileInfo == null && info == null)
                throw new InvalidOperationException("Can't save a copy of the specified program. No file path specified");

            AssemblyManager.TryCreateAssembly(newName ?? AssemblyManager[index].AssemblyName + "_copy", AssemblyManager[index].Words);

            Assembly assembly = AssemblyManager.Last;

            assembly.FileInfo = info;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            assembly.FileInfo ??= new FileInfo($"{AssemblyManager[index].FileInfo.DirectoryName}.{assembly.AssemblyName}.{assembly.Extension}");
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            Task<bool> task = assembly.SerializeAssembly();

            await task;

            return task.Result;
        }

        /// <summary>
        /// Method to load multiple programs and assemblies from disk and automatically pair the assemblies to their associated program provided they have the same name
        /// </summary>
        /// <remarks>***This method is likely very expensive to call and contains multiple nested parallel statements***</remarks>
        /// <param name="infos">Set of FileInfos to try and load programs or assemblies from</param>
        /// <param name="token">A CancelToken to signal to the operation to exit early</param>
        /// <returns>A List of the FileInfos that were not opened</returns>
        public virtual async Task<List<FileInfo>> OpenProgramsAssemblys(FileInfo[] infos, CancellationToken token)
        {
            Task<List<FileInfo>> task = Task.Run(() =>
            {
                ProgramsManager.TryCreateNewProgram("newProgram");
                Program program = ProgramsManager.Last;
                string programExt = program.Extension;
                ProgramsManager.DeleteProgram(ProgramsManager.ProgramsCount - 1);

                AssemblyManager.TryCreateAssembly("newAssembly");
                Assembly assembly = AssemblyManager.Last;
                string assemblyExt = assembly.Extension;
                AssemblyManager.DeleteAssembly(AssemblyManager.LoadedAssembliesCount - 1);

                FileInfo[] programInfos = infos.Where(info => info.Extension == programExt).ToArray();
                FileInfo[] assemblyInfos = infos.Where(info => info.Extension == assemblyExt).ToArray();

                Task<List<FileInfo>> programTask = ProgramsManager.DeserializePrograms(programInfos, token);
                Task<List<FileInfo>> assemblyTask = AssemblyManager.DeserializeAssemblies(assemblyInfos, token);

                ParallelOptions outerOptions = new()
                {
                    CancellationToken = token
                };

                try
                {
                    Parallel.ForEach(ProgramsManager.Programs, outerOptions, (program) =>
                    {
                        CancellationTokenSource tokenSource = new();
                        ParallelOptions innerOptions = new()
                        {
                            CancellationToken = tokenSource.Token
                        };

                        try
                        {
                            Parallel.ForEach(AssemblyManager.LoadedAssemblies, innerOptions, (assembly) =>
                            {
                                if (assembly.AssemblyName == program.ProgramName)
                                {
                                    program.Assembly = assembly;

                                    tokenSource.Cancel();
                                }
                            });
                        }
                        finally
                        {
                            tokenSource.Dispose();
                        }
                    });
                }
                finally
                {
                }

                return programTask.Result.Concat(assemblyTask.Result).ToList();
            }, token);

            await task;

            return task.Result;
        }

        /// <summary>
        /// Create a new program and associated 
        /// </summary>
        /// <param name="programName"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual void NewProgramAssemblyPair(string? programName = null)
        {
            programName ??= $"program{ProgramsManager.ProgramsCount}";

            try
            {
                NewProgram(programName);
                NewAssembly(programName);

                ProgramsManager.Last.Assembly = AssemblyManager.Last;
            }
            catch (InvalidOperationException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Save a program and its associated assembly at the specified index to disk
        /// </summary>
        /// <param name="index">The index of the program to save</param>
        /// <returns>True if the program and assembly are saved successfully, false otherwise</returns>
        public virtual async Task<bool> SaveProgramAssemblyPair(int index)
        {
            if (ProgramsManager[index].Assembly != null)
            {
                Task<bool> programTask = ProgramsManager[index].SerializeProgram();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                Task<bool> assemblyTask = ProgramsManager[index].Assembly.SerializeAssembly();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                Task<bool[]> duo =  Task.WhenAll(programTask, assemblyTask);

                await duo;

                return duo.Result[0] && duo.Result[1];
            }
            return false;
        }
        /// <summary>
        /// Save the specified program and its associalted assembly to disk
        /// </summary>
        /// <param name="program">The program to save</param>
        /// <returns>True if the program and assembly are saved successfully, false otherwise</returns>
        /// <exception cref="InvalidOperationException">Thrown if the program or assembly has no save info set</exception>
        public virtual async Task<bool> SaveProgramAssemblyPair(Program program)
        {
            if (program.Assembly != null)
            {
                Task<bool> programTask = program.SerializeProgram();
                Task<bool> assemblyTask = program.Assembly.SerializeAssembly();

                Task<bool[]> duo = Task.WhenAll(programTask, assemblyTask);

                await duo;

                return duo.Result[0] && duo.Result[1];
            }
            else
                return false;
        }

        /// <summary>
        /// Save a set of programs and their associated assemblies to disk
        /// </summary>
        /// <param name="indexes">The indexes of the programs to save</param>
        /// <param name="token">A CancelToken to signal to the operation to exit early</param>
        /// <returns>A list of the programs that failed to be saved</returns>
        public virtual async Task<List<Program>> SaveProgramAssemblyPairs(int[] indexes, CancellationToken token)
        {
            List<Program> failedPrograms = new();

            ParallelOptions parallelOptions = new()
            {
                CancellationToken = token
            };

            await Parallel.ForEachAsync(indexes, parallelOptions, async (index, token) =>
            {
                token.ThrowIfCancellationRequested();

                Task<bool> task = SaveProgramAssemblyPair(index);

                await task;

                if (!task.Result)
                    failedPrograms.Add(ProgramsManager[index]);
            });

            return failedPrograms;
        }

        /// <summary>
        /// Save all of the programs with an associated assembly in the collection to disk
        /// </summary>
        /// <param name="token">A CancelToken to signal to the operation to exit early</param>
        /// <returns>A list of the programs that failed to be saved</returns>
        public virtual async Task<List<Program>> SaveAllProgramAssemblyPairs(CancellationToken token)
        {
            List<Program> failedPrograms = new();

            ParallelOptions parallelOptions = new()
            {
                CancellationToken = token
            };

            await Parallel.ForEachAsync(ProgramsManager.Programs, parallelOptions, async (program, token) =>
            {
                token.ThrowIfCancellationRequested();

                Task<bool> task = SaveProgramAssemblyPair(program);

                await task;

                if (!task.Result)
                    failedPrograms.Add(program);
            });

            return failedPrograms;
        }

        /// <summary>
        /// Save the program and its associated assembly at the specified index to disk at the provided locations
        /// </summary>
        /// <param name="index">The index of the program to save</param>
        /// <param name="programInfo">Information concerning where the program's file is to be saved</param>
        /// <param name="assemblyInfo">Information concerning where the assembly's file is to be saved</param>
        /// <returns>True if the program and assembly are saved successfully, false otherwise</returns>
        public virtual async Task<bool> SaveProgramAssemblyPairTo(int index, FileInfo programInfo, FileInfo assemblyInfo)
        {
            if (ProgramsManager[index].Assembly != null)
            {
                ProgramsManager[index].FileInfo = programInfo;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                ProgramsManager[index].Assembly.FileInfo = assemblyInfo;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                Task<bool> task = SaveProgramAssemblyPair(index);

                await task;

                return task.Result;
            }
            else
                return false;
        }
        /// <summary>
        /// Save the specified program and its associated assembly to disk at the provided locations
        /// </summary>
        /// <param name="program">The program to save</param>
        /// <param name="programInfo">Information concerning where the program's file is to be saved</param>
        /// <param name="assemblyInfo">Infomation concerning where the assembly's file is to be saved</param>
        /// <returns>True if the program and its assembly are saved successfully, false otherwise</returns>
        public virtual async Task<bool> SaveProgramAssemblyPairTo(Program program, FileInfo programInfo, FileInfo assemblyInfo)
        {
            if(program.Assembly != null)
            {
                program.FileInfo = programInfo;
                program.Assembly.FileInfo = assemblyInfo;

                Task<bool> task = SaveProgramAssemblyPair(program);
                
                await task;
                
                return task.Result;
            }
            else
                return false;
        }

        /// <summary>
        /// Create a copy of the program and its associated assembly at the specified index and save them to disk
        /// </summary>
        /// <param name="index">The index of the assembly to save</param>
        /// <param name="newName">Optional name given to the copy, if not provided one is generated</param>
        /// <param name="programInfo">Optional info about where to save the copy of the program, if not provided the copy is saved adjacent to the original</param>
        /// <param name="assemblyInfo">Optional info about where to save the copuy of the assembly, if not provided the copy is saved adjacent to the original</param>
        /// <returns>True if a copy of the program and its assembly where successfully saved, false otherwise</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual async Task<bool> SaveProgramAssemblyPairAs(int index, string? newName = null, FileInfo? programInfo = null, FileInfo? assemblyInfo = null)
        {
            if (ProgramsManager[index].FileInfo == null && programInfo == null)
                throw new InvalidOperationException("Can't save a copy of the specified program. No file path specified");

            if (ProgramsManager[index].Assembly != null)
            {
                ProgramsManager.TryCreateNewProgram(newName ?? ProgramsManager[index].ProgramName + "_copy");
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                AssemblyManager.TryCreateAssembly(newName ?? ProgramsManager[index].Assembly.AssemblyName + "_copy");
#pragma warning restore CS8602 // Dereference of a possibly null reference.

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (ProgramsManager[index].Assembly.FileInfo == null)
                {
                    ProgramsManager[index].Assembly.FileInfo = new FileInfo($"{ProgramsManager[index].FileInfo.DirectoryName}." +
                        $"{ProgramsManager[index].Assembly.AssemblyName}." +
                        $"{ProgramsManager[index].Assembly.Extension}");
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                Program program = ProgramsManager.Last;
                Assembly_FixedSize assembly = AssemblyManager.Last;

                program.FileInfo = programInfo;
                assembly.FileInfo = assemblyInfo;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                program.FileInfo ??= new FileInfo($"{ProgramsManager[index].FileInfo.DirectoryName}.{program.ProgramName}.{program.Extension}");
                assembly.FileInfo ??= new FileInfo($"{ProgramsManager[index].Assembly.FileInfo.DirectoryName}.{assembly.AssemblyName}.{assembly.Extension}");
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                program.Assembly = assembly;

                Task<bool> task = SaveProgramAssemblyPair(program);

                await task;

                return task.Result;
            }
            else
                return false;
        }

        /// <summary>
        /// Create a new assembly from the program at the specified index, or updates the program's associated assembly
        /// </summary>
        /// <param name="index">The index of the program to build</param>
        /// <returns>True if the operation succeeds, false otherwise</returns>
        public virtual async Task<bool> TryBuildProgram(int index)
        {
            Task<bool> task = Task.Run(() =>
            {
                Program program = ProgramsManager[index];

                if (program.Assembly == null)
                {
                    if (!AssemblyManager.TryCreateAssembly(program.ProgramName, program.Text.Split('\n')))
                        return false;

                    program.Assembly = AssemblyManager.Last;
                    program.Assembly.UpToDate = true;

                    return true;
                }
                else
                {
                    if (!program.Assembly.AssembleProgram(program.ProgramName, program.Text.Split('\n')))
                        return false;

                    program.Assembly.UpToDate = true;

                    return true;
                }
            });

            await task;

            return task.Result;
        }

        /// <summary>
        /// Create a new assembly from the provided program, or update the program's associated assembly
        /// </summary>
        /// <param name="program">The program object to build the assembly from</param>
        /// <returns>True if the operation succeeds, false otherwise</returns>
        /// <exception cref="ArgumentException">Will be thrown if the program is not valid and can not be built</exception>
        public virtual async Task<bool> TryBuildProgram(Program program)
        {
            Task<bool> task = Task.Run(() =>
            {
                if (program.Assembly == null)
                {
                    if (!AssemblyManager.TryCreateAssembly(program.ProgramName, program.Text.Split(new string[] { "\n", "\r", "\r\n" }, StringSplitOptions.None)))
                        return false;

                    program.Assembly = AssemblyManager.Last;
                    program.Assembly.UpToDate = true;

                    return true;
                }
                else
                {
                    if (!program.Assembly.AssembleProgram(program.ProgramName, program.Text.Split(new string[] { "\n", "\r", "\r\n" }, StringSplitOptions.None)))
                        return false;

                    program.Assembly.UpToDate = true;

                    return true;
                }
            });

            await task;

            /*if (program.Assembly == null)
            {
                if (!AssemblyManager.TryCreateAssembly(program.ProgramName, program.Text.Split(new string[] {"\n", "\r", "\r\n"}, StringSplitOptions.None)))
                    return false;

                program.Assembly = AssemblyManager.Last;
                program.Assembly.UpToDate = true;

                return true;
            }
            else
            {
                if (!program.Assembly.AssembleProgram(program.ProgramName, program.Text.Split(new string[] {"\n", "\r", "\r\n"}, StringSplitOptions.None)))
                    return false;

                program.Assembly.UpToDate = true;

                return true;
            }*/

            return task.Result;
        }

        /// <summary>
        /// Loads the assembly at the specified index into the simulator
        /// </summary>
        /// <param name="index">The index of the assembly to load</param>
        public virtual async void LoadAssembly(int index)
        {
            await Task.Run(() =>
            {
                Assembly assembly = AssemblyManager[index];

                ArchitectureSim.LoadProgram(assembly.Words);
            });
        }

        /// <summary>
        /// Loads the provided assembly into the simulator
        /// </summary>
        /// <param name="assembly">The assembly to load</param>
        public virtual void LoadAssembly(Assembly assembly)
        {
            /*
            await Task.Run(() =>
            {
                ArchitectureSim.LoadProgram(assembly.Words);
            });
            */
            ArchitectureSim.LoadProgram(assembly.Words);
        }

        /// <summary>
        /// Runs the simulator on the currently loaded program, if there is one
        /// </summary>
        public virtual  void RunSimulator()
        {
            /*
            await Task.Run(() =>
            {
                ArchitectureSim.RunProgram();
            });
            */
            ArchitectureSim.RunProgram();
        }

        /// <summary>
        /// Loads an assembly and then imediately executes it in the simulator
        /// </summary>
        /// <param name="index">The index of the assembly to load and run</param>
        public virtual async void ExecuteProgram(int index)
        {
            await Task.Run(() =>
            {
                LoadAssembly(index);
                RunSimulator();
            });
        }

        /// <summary>
        /// Loads an assembly and then imediately executes it in the simulator
        /// </summary>
        /// <param name="assembly">The assembly to load and run</param>
        public virtual async void ExecuteProgram(Assembly assembly)
        {
            await Task.Run(() => 
            { 
                LoadAssembly(assembly); 
                RunSimulator(); 
            });
        }
        #endregion
    }
}
