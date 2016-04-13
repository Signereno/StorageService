Signere.no StorageService
==============

Storage service running on local server. 

Supports creation of separate accounts for storing files.

The solution requires .NET, IIS and an HTTPS certificate.

This service is created by Signere.no for customers that want to host their files on their own servers/servers they control, as an alternative to using our standard cloud storage.

The service is secured with HTTPS and a signature of the URL used to call the server.

### Move files when signed
There is a companion service, Signere-StorageServer-FileMover, which is a Windows Service that can be installed alongside the Storage Service. This will move files as soon as the Signere.no PAdES file is saved, to a folder specified in the settings of the service. It will also at a given interval move all files which are not signed based on creation time. In the settings it is possible to specify the number of minutes before unsigned files are moved.
