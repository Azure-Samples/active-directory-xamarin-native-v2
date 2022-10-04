# MAUI Sample Apps
This file is readme for MAUI sample apps. **It currently supports only iOS, Android, and Windows platforms.**.  
A preview package has been published on NuGet. [Microsoft.Identity.Client 4.47.0]( https://www.nuget.org/packages/Microsoft.Identity.Client/4.47.0)  


## Prerequisites
To build and run the branch, it will require:
- Visual Studio 2022 Release 17.3.5
- Mac with **XCode 13.3** to compile iOS Apps. Note: Support for XCode 14.0 for MAUI is in RC.

# Getting started
- Clone this repository  
`git clone https://github.com/Azure-Samples/active-directory-xamarin-native-v2.git`  
> Given that the name of the sample is pretty long, and so are the name of the referenced NuGet packages, you might want to clone it in a folder close to the root of your hard drive, to avoid file size limitations on Windows.
- Open Visual Studio 2022
- Open the solution  
`/MAUI/MauiApps.sln`
- It contains the following projects:

    - MauiAppBasic  
    This shows how to perform authentication with no broker. It has the common pattern of Acquire Token Silent (ATS) + Acquire Token Interactive (ATI). Note: Android does not support the embedded browser.
    - MauiAppWithBroker  
    This shows how to perform authentication with broker. It has the common pattern of ATS + ATI.
    - MauiAppB2C  
    This shows how to perform authentication with OAuth providers such as Google, Facebook, Microsoft Personal account. It has the common pattern of ATS + ATI.

### MauiAppBasic
This performs basic authentication using ATS + ATI flow. There is no broker involved. 
The repro steps are:
1. ATS + ATI
2. ATS
3. Logout followed by ATS + ATI


The results are as follows:

<div style="margin-left: auto;
            margin-right: auto;
            width: 30%">

| Platform | Status |
| ----------- | ----------- |
| iOS (System) | **Works** |
| iOS (Embedded) | **Works** |
| Android (System) | **Works** |
| Android (Embedded) | **Works** |
| WinUI3 | **Works** |
</div>

### MauiAppBroker
This performs basic authentication using ATS + ATI flow using broker.  
The repro steps are:
1. ATS + ATI
2. ATS
3. Logout followed by ATS + ATI


The results are as follows:

<div style="margin-left: auto;
            margin-right: auto;
            width: 30%">

| Platform | Status |
| ----------- | ----------- |
| iOS | **Works** |
| Android | **Works** |
| WinUI3 | **Works** |
</div>

### MauiAppB2C
This performs basic authentication using ATS + ATI flow using external identity providers.  
The repro steps are:
1. ATS + ATI
2. ATS
3. Logout followed by ATS + ATI


The results are as follows:

<div style="margin-left: auto;
            margin-right: auto;
            width: 30%">

| Platform | Status |
| ----------- | ----------- |
| iOS | **Works** |
| Android | **Works** |
| WinUI3 | **Works** |
</div>
