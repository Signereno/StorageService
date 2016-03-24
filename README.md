StorageService
==============

Cloud storage service for running on local server. 

Support creation of seperate accounts for storing files.

The solution requires .NET and IIS and a HTTPS certificate.

This service is created for Signere.no for customers that want to host there own documents on their own servers og servers they controll.

The service is secured by HTTPS and a signature of the url used to call the server.

### Move files when signed
Theres a companion service Signere-StorageServer-FileMover witch is a Windows Service that can be installed alongside the Storage service. This will move files a soon as the Signere.no pades file is saved to a folder set inn the settings of the service. It will also at interval move all files witch are not signed based on creation time. In the settings it's possible to set the number of minutes before unsigned files are moved.
