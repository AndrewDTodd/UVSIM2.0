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
