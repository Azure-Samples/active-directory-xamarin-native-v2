// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace MauiAppBasic
{
    internal class Program : MauiApplication
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        static void Main(string[] args)
        {
            throw new PlatformNotSupportedException("Tizen platform is not supported!");
        }
    }
}