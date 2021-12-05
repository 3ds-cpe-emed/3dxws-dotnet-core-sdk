# 3DEXPERIENCE .Net Core SDK 
Welcome to the Open Source Initiative for a .NET SDK for the 3DEXPERIENCE Web Services. This has been setup by the Dassault Systèmes CPE Emed team with the idea to empower our partners with the tools and techniques to rapidly master the usage of 3DEXPERIENCE Web Services. We are looking for Partners who which to contribute to the extension of this repository.


## Getting Started
 - The repository of samples that exercise many of the functionality wrapped by this SDK can be found [here](https://github.com/3ds-cpe-emed/3dxws-dotnet-samples).
 - The repository of samples focused on the Enterprise Integration Framework (EI) can be found [here](https://github.com/3ds-cpe-emed/3dxws.dotnet.event.samples)
 - A growing number of projects is being complemented with an equivalent tests folder that also shows how to exercise the classes in the SDK.

## Projects


Please note that this is working in progress and the coverage of all the available 3DEXPERIENCE resources is only partial. In many cases very little at least for the moment.

| Project | Description | Web Services Scope | Nuget |  
| ------------ | ------- | ------------ | ------- | 
|[ds.authentication](/ds.authentication/)|Authentication protocols; CAS, Openness Agent (Cloud), Batch Service (OnPremise)| Authentication (3DPassport)|[ds.authentication](https://www.nuget.org/packages/ds.authentication/)|
|[ds.authentication.ui.win](/ds.authentication.ui.win/)|Helper User Interface dialogs for Authentication workflows| Authentication (3DPassport)|[ds.authentication.ui.win](https://www.nuget.org/packages/ds.authentication.ui.win/)|
|[ds.enovia.common](/ds.enovia.common/)|Common classes and methods shared by other projects|-|[ds.enovia.common](https://www.nuget.org/packages/ds.enovia.common/)|
|[ds.enovia](/ds.enovia/)|Common methods shared by project services|-|[ds.enovia](https://www.nuget.org/packages/ds.enovia/)|
|[ds.enovia.dsxcad](/ds.enovia.dsxcad/)|CAD Design Integration wrapper classes and methods|CAD Design Integration (3DSpace)|[ds.enovia](https://www.nuget.org/packages/ds.enovia.dsxcad/)|
|[ds.enovia.dslc](/ds.enovia.dslc/)|General Collaboration Lifecycle wrapper classes and methods except for Change Action|Collaboration Lifecycle (3DSpace)|[ds.enovia.dslc](https://www.nuget.org/packages/ds.enovia.dslc/)|
|[ds.enovia.dslc.changeaction/](/ds.enovia.dslc.changeaction)|Change Action related wrapper classes and methods|Collaboration Lifecycle (3DSpace)|[ds.enovia.dslc.changeaction](https://www.nuget.org/packages/ds.enovia.dslc.changeaction/)|
|[ds.enovia.dsdo](/ds.enovia.dsdo/)|Derived Outputs related wrapper classes and methods|Derived Outputs (3DSpace)|[ds.enovia.dsdo](https://www.nuget.org/packages/ds.enovia.dsdo/)|
|[ds.enovia.document](/ds.enovia.document/)|Documents related wrapper classes and methods|Document (3DSpace)|[ds.enovia.document](https://www.nuget.org/packages/ds.enovia.document/)|
|[ds.enovia.dseng](/ds.enovia.dseng/)|Engineering related wrapper classes and methods|Engineering (3DSpace)|[ds.enovia.dseng](https://www.nuget.org/packages/ds.enovia.dseng/)|
|[ds.enovia.eif](/ds.enovia.eif/)|Enterprise Integration Framework (EIF) related wrapper classes and methods|Enterprise Integration Framework|[ds.enovia.eif](https://www.nuget.org/packages/ds.enovia.eif/)|
|[ds.enovia.dslib](/ds.enovia.dslib/)|IP Classification related wrapper classes and methods|IP Classification (3DSpace)|[ds.enovia.dslib](https://www.nuget.org/packages/ds.enovia.dslib/)|
|[ds.enovia.dscfg](/ds.enovia.dscfg/)|IP Configuration related wrapper classes and methods|IP Configuration (3DSpace)|[ds.enovia.dscfg](https://www.nuget.org/packages/ds.enovia.dscfg/)|
|[ds.delmia.dsmfg](/ds.delmia.dsmfg/)|Manufacturing related wrapper classes and methods|Manufacturing Item and Manufacturing Process (3DSpace)|[ds.delmia.dsmfg](https://www.nuget.org/packages/ds.delmia.dsmfg/)|
|[ds.delmia.dspfl](/ds.delmia.dspfl/)|Portfolio related wrapper classes and methods|Portfolio (3DSpace)|[ds.delmia.dsmfg](https://www.nuget.org/packages/ds.delmia.dsmfg/)|

## Build
- Built and developed using Microsoft Visual Studio 2019 (16.11.7)

### Dependencies

For the most the projects depend on the following:

- [.NET Standard](https://www.nuget.org/packages/NETStandard.Library) 2.0.3 or later
- [System.Text.Json](https://www.nuget.org/packages/System.Text.Json)  5.0.2
- [System.Net.Http.Json](https://www.nuget.org/packages/System.Net.Http.Json) 5.0.0

Individual projects also have inter-dependencies as the picture below shows.

![Dependency model for the ds.enovia.dseng library!](/docs/media/ds.enovia.dseng.dependencies.png)

## Need Help?
- For reference documentation visit the [3DEXPERIENCE Cloud Web Services documentation](https://media.3ds.com/support/documentation/developer/Cloud/en/DSDoc.htm?show=CAAiamREST/CAATciamRESTToc.htm) - requires a free 3DEXPERIENCE ID
