// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MauiAppBasic.Views;

namespace MauiAppBasic
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            Routing.RegisterRoute("mainview", typeof(MainView));
            Routing.RegisterRoute("userview", typeof(UserView));
            InitializeComponent();
        }
    }
}