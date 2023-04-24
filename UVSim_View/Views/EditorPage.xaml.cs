using CommunityToolkit.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UVSim.ViewModel;

namespace UVSim_View
{
    public partial class EditorPage : ContentPage
    {
        private readonly MasterViewModel _masterViewModel;
        
        private readonly CancellationTokenSource _tokenSource = new();

        ProgramsManagementViewModel ProgramsViewModel { get => _masterViewModel.ProgramsManagementViewModel; }

        public EditorPage(MasterViewModel viewModel)
        {
            InitializeComponent();

            _masterViewModel = viewModel;

            BindingContext = viewModel;
        }

        private void DockOptionsButton_Tapped(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                RadioButton rbutton = button.Parent.Parent as RadioButton;
                rbutton.IsChecked = true;
            }
        }

        private async void SimModeButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(SimPage));
        }

        #region TAP_RECOGNIZERS
        private void ProgramDoubleClick_Edit(object sender, TappedEventArgs e)
        {
            //Label label = sender as Label;

            ProgramsViewModel.TryAddSetEditingProgram(e.Parameter as UVSim.Program, editingProgramsCollection);
        }
        #endregion

        #region TOOLBAR_MENU_CLICKED
        async void ToolBarMenuFlyout_New(object sender, EventArgs e)
        {
            try
            {
                string programName = await Shell.Current.CurrentPage.DisplayPromptAsync("Create New Program", "Enter program name");

                if(programName != null)
                {
                    _masterViewModel.SimPackage.NewProgram(programName);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Error on Open",
                    $"The requested open operation fialed with the following exeption\n{ex.Message}", "OK");
            }
        }

        async void ToolBarMenuFlyout_Open(object sender, EventArgs e)
        {
            try
            {
                var results = await FilePicker.Default.PickMultipleAsync();

                if (results != null)
                {
                    List<FileInfo> infos = new(results.Count());

                    foreach (FileResult result in results)
                    {
                        infos.Add(new(result.FullPath));
                    }

                    Task<List<FileInfo>> task = _masterViewModel.SimPackage.OpenPrograms(infos.ToArray(), _tokenSource.Token);

                    await task;

                    if (task.Result.Count > 0)
                    {
                        string failedFiles = "";
                        foreach (FileInfo info in task.Result)
                        {
                            failedFiles += info.Name + ",";
                        }

                        await Shell.Current.CurrentPage.DisplayAlert("Failed Selections", "Some of your selected files failed to load\n" + failedFiles, "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Error on Open",
                    $"The requested open operation fialed with the following exeption\n{ex.Message}", "OK");
            }
        }
        #endregion

        #region PROGRAM_FLYOUT_MENU_CLICKED
        public void ProgramExplorerMenuFlyout_Open(object sender, EventArgs e)
        {
            MenuFlyoutItem openItem = sender as MenuFlyoutItem;

            ProgramsViewModel.TryAddSetEditingProgram(openItem.CommandParameter as UVSim.Program, editingProgramsCollection);
        }

        public async void ProgramExplorerMenuFlyout_Save(object sender, EventArgs e)
        {
            MenuFlyoutItem openItem = sender as MenuFlyoutItem;
            UVSim.Program program = openItem.CommandParameter as UVSim.Program;

            try
            {
                Task<bool> task = program.Assembly != null ? _masterViewModel.SimPackage.SaveProgramAssemblyPair(program) : _masterViewModel.SimPackage.SaveProgram(program);

                await task;

                if (!task.Result)
                    await Shell.Current.CurrentPage.DisplayAlert("Save Failed", $"The program {program.ProgramName} could not be saved, though no exeptions were thrown", "OK");
            }
            catch (InvalidOperationException opEx)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Save Failed with Error!",
                    $"The program {program.ProgramName} could not be saved. Operation threw the following...\n{opEx.Message}", "OK");

                var result = await FolderPicker.Default.PickAsync(Directory.GetCurrentDirectory(), _tokenSource.Token);

                FileInfo folder = new(result.Folder.Path);

                try
                {
                    Task<bool> task = program.Assembly != null ? _masterViewModel.SimPackage.SaveProgramAssemblyPairTo(program, folder, folder) : _masterViewModel.SimPackage.SaveProgramTo(program, folder);

                    await task;
                    
                    if (!task.Result)
                        await Shell.Current.CurrentPage.DisplayAlert("Save Failed", $"The program {program.ProgramName} could not be saved, though no exeptions were thrown", "OK");
                }
                catch (Exception ex)
                {
                    await Shell.Current.CurrentPage.DisplayAlert("Save Failed with Error!",
                    $"The program {program.ProgramName} could not be saved. Operation threw the following...\n{ex.Message}", "OK");
                }
            }
        }

        public async void ProgramExplorerMenuFlyout_Build(object sender, EventArgs e)
        {
            MenuFlyoutItem openItem = sender as MenuFlyoutItem;
            UVSim.Program program = openItem.CommandParameter as UVSim.Program;

            try
            {
                Task<bool> task = _masterViewModel.SimPackage.TryBuildProgram(program);

                await task;

                if (!task.Result)
                    await Shell.Current.CurrentPage.DisplayAlert("Build Failed", $"The program {program.ProgramName} could not be built, though no exeptions were thrown", "OK");
            }
            catch (ArgumentException argEx)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Build Failed with Error!",
                    $"The program {program.ProgramName} could not be built. Operation threw the following...\n{argEx.Message}", "OK");
            }
        }

        public void ProgramExplorerMenuFlyout_Remove(object sender, EventArgs e)
        {
            MenuFlyoutItem openItem = sender as MenuFlyoutItem;
            UVSim.Program program = openItem.CommandParameter as UVSim.Program;

            if(program.Assembly != null)
                _masterViewModel.AssembliesManagementViewModel.Assemblies.Remove(program.Assembly);

            ProgramsViewModel.Programs.Remove(program);

            ProgramsViewModel.TryRemoveEditingProgram(program, editingProgramsCollection);
        }
        #endregion

        #region ASSEMBLY_FLYOUT_MENU_CLICKED
        private void AssemblyRun_Clicked(object sender, EventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            UVSim.Assembly_FixedSize assembly = item.CommandParameter as UVSim.Assembly_FixedSize;

            try
            {
                _masterViewModel.SimPackage.LoadAssembly(assembly);

                _masterViewModel.SimPackage.RunSimulator();
            }
            catch(Exception ex)
            {
                Shell.Current.CurrentPage.DisplayAlert("Execution Error!", $"The program threw the following error...\n{ex.Message}", "OK");
            }
        }

        private async void AssemblySave_Clicked(object sender, EventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            UVSim.Assembly_FixedSize assembly = item.CommandParameter as UVSim.Assembly_FixedSize;

            try
            {
                Task<bool> task = _masterViewModel.SimPackage.SaveAssembly(assembly);

                await task;

                if (!task.Result)
                    await Shell.Current.CurrentPage.DisplayAlert("Save Failed", $"The assembly {assembly.AssemblyName} could not be saved, though no exeptions were thrown", "OK");
            }
            catch (InvalidOperationException)
            {
                var result = await FolderPicker.Default.PickAsync(Directory.GetCurrentDirectory(), _tokenSource.Token);

                FileInfo folder = new(result.Folder.Path);

                try
                {
                    Task<bool> task = _masterViewModel.SimPackage.SaveAssemblyTo(assembly, folder);

                    await task;

                    if (!task.Result)
                        await Shell.Current.CurrentPage.DisplayAlert("Save Failed", $"The assembly {assembly.AssemblyName} could not be saved, though no exeptions were thrown", "OK");
                }
                catch (Exception ex)
                {
                    await Shell.Current.CurrentPage.DisplayAlert("Save Failed with Error!",
                    $"The program {assembly.AssemblyName} could not be saved. Operation threw the following...\n{ex.Message}", "OK");
                }
            }
        }
        #endregion

        #region COLLECTION_COMMAND
        private void EditingProgramsCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProgramsViewModel.EditingProgram = e.CurrentSelection[0] as UVSim.Program;
        }
        #endregion
    }
}