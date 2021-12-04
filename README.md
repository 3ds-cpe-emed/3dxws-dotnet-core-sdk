# 3DEXPERIENCE .Net Core SDK 
Welcome to the Open Source Initiative for a .NET SDK for the 3DEXPERIENCE Web Services. This has been setup by the Dassault Systèmes CPE Emed team with the idea to empower our partners with the tools and techniques to rapidly master the usage of 3DEXPERIENCE Web Services. We are looking for Partners who which to contribute to the extension of this repository.


## Getting Started
 - The repository of samples that exercise many of the functionality wrapped by this SDK can be found [here](https://github.com/3ds-cpe-emed/3dxws-dotnet-samples).
 - The repository of samples focused on the Enterprise Integration Framework (EI) can be found [here](https://github.com/3ds-cpe-emed/3dxws.dotnet.event.samples)
 - A growing number of projects is being complemented with an equivalent tests folder that also shows how to exercise the classes in the SDK.

## Projects

Please note that this is working in progress and the coverage of all the available 3DEXPERIENCE resources is only partial.

| 3DSpace Web Services | Project |
| ------------ | ------- | 
| CAD Design Integration | ds.delmia.dsdxcad |
| Collaboration Lifecycle | ds.enovia.dslc |
| Collaboration Lifecycle | ds.enovia.dslc.changeaction |
| Derived Outputs| ds.delmia.dsdo |
| Document| ds.delmia.document |
| Engineering | ds.enovia.dseng |
| Enterprise Integration Framework | ds.enovia.eif |
| IP Classification | ds.enovia.dslib |
| IP Configuration | ds.enovia.dscfg |
| Manufacturing Item | ds.delmia.dsmfg |
| Manufacturing Process | ds.delmia.dsmfg |
| Portfolio | ds.enovia.dsplf |

## Builds
- Built using Visual Studio 2019 (16.11.7)

- Nuget compiled binaries -

[ds.authentication](https://www.nuget.org/packages/ds.authentication/)
[ds.authentication.ui.win](https://www.nuget.org/packages/ds.authentication.ui.win/)
[ds.delmia.dsmfg](https://www.nuget.org/packages/ds.delmia.dsmfg/)
[ds.enovia](https://www.nuget.org/packages/ds.enovia/)
[ds.enovia.common](https://www.nuget.org/packages/ds.enovia.common/)
[ds.enovia.document](https://www.nuget.org/packages/ds.enovia.document/)
[ds.enovia.dscfg](https://www.nuget.org/packages/ds.enovia.dscfg/)
[ds.enovia.dsdo](https://www.nuget.org/packages/ds.enovia.dsdo/)
[ds.enovia.dseng](https://www.nuget.org/packages/ds.enovia.dseng/)
[ds.enovia.dslc](https://www.nuget.org/packages/ds.enovia.dslc/)
[ds.enovia.dslc.changeaction](https://www.nuget.org/packages/ds.enovia.dslc.changeaction/)
[ds.enovia.dslib](https://www.nuget.org/packages/ds.enovia.dslib/)
[ds.enovia.dspfl](https://www.nuget.org/packages/ds.enovia.dspfl/)
[ds.enovia.dsxcad](https://www.nuget.org/packages/ds.enovia.dsxcad/)
[ds.enovia.eif](https://www.nuget.org/packages/ds.enovia.eif/)

### Dependencies

For the most the projects depend on the following:

- [.NET Standard](https://www.nuget.org/packages/NETStandard.Library) 2.0.3 or later
- [System.Text.Json](https://www.nuget.org/packages/System.Text.Json)  5.0.2
- [System.Net.Http.Json](https://www.nuget.org/packages/System.Net.Http.Json) 5.0.0

Individual projects also have inter-dependencies as the picture below shows.

![Dependency model for the ds.enovia.dseng library!](/docs/media/ds.enovia.dseng.dependencies.png)


## Need Help?
- For reference documentation visit the [3DEXPERIENCE Cloud Web Services documentation](https://media.3ds.com/support/documentation/developer/Cloud/en/DSDoc.htm?show=CAAiamREST/CAATciamRESTToc.htm) - requires a free 3DEXPERIENCE ID
