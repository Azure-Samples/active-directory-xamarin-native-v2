// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MauiAppWithBroker.Views;

namespace MauiAppWithBroker
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("mainview", typeof(MainView));
            Routing.RegisterRoute("userview", typeof(UserView));
        }
    }
}