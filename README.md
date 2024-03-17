# UVSim2.0
UVU Simulator for Basic Machine Language

The 2.0 version aims to create a .net MAUI based GUI on top of the UVSIM model

The UVSIM model allows for the creation of machine language specifications that can be digested by the application
to allow a user to create programs via a defined assembly language (specific to the machine language), assemble those
programs, and run them

MAUI finally lets us build to standalone exe. You can find an x64 Windows build under Releases


Application can be used as follows

Home screen of the application is an editor context page. Here a user can create new program files, or open existing ones from disk
  Right click on the empty space in the Programs Explorer. You will see a two item context menue. New Program, or Open Program.
  New program will create a new program
  Open program will have you select a file from disk
  
With a program in the Programs Explorer you can right click on that program to open a new context menue
  Open, will select the program and open it in the editor for text manipulation
  Save, will save the file to its existing location on disk or prompt the user to select a location to save the file
  Build, will attempt to assemble the program into an intermediate language assembly. After successfull completion of this operation an assembly object file will
  appear as a child of the expandable UI element representing the program that was assembled
  Remove, will remove the program from the applications interface. Effectively deleting it from memory
  
With an Assembly as a child of a program
  Run, will execute the intermediate assemblty on the virtual machine. The programs execution will reflect in the applications simulator recources views
  Save, will save the assemlby file to disk at its existing location or prompt the user to select a location to save to
  
Application context views
The application has two context views, Edit Mode, and Simulator Mode
These contexts can be selected by clicking either the "Edit Mode" button or the "Simulator Mode" button at the top center of the screen

When in Edit context the program explorer will display the loaded programs and associated assemblies as well as the editor window for text manipulation of selected program files
When in Simulator Mode the program will display the simulators Memory and Register resources. these resources reflect changes in the simulators state due to assemlby loading and execution
