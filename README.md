# UVSim2.0
UVU Simulator for Basic Machine Language

The 2.0 version aims to create a .net MAUI based GUI on top of the UVSIM model

The UVSIM model allows for the creation of machine language specifications that can be digested by the application
to allow a user to create programs via a defined assembly language (specific to the machine language), assemble those
programs, and run them

App publication install is difficult at best. An issue with building MAUI projects for windows has resulted in major issues.

Best case senerio, Run the Install.ps1 powershell script in the install package folder. This may not work and require the following

The publishing of the application produces a MSIX insaller package, which is signed with a personal certificate.
This requires the installing user to veryify the certificate before install is posible.

Instructions can be found here[https://learn.microsoft.com/en-us/dotnet/maui/windows/deployment/publish-cli?view=net-maui-7.0#installing-the-app]
Even with the certificate accepted and recognized the build may be faulty, as has been seen in testing.

Install will fail with error saying included package already exists.
In this case the install is not worth the time it would take to configure as these packages are added as part of the MAUI build process.

Run the application through Visual Studio 2022


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
