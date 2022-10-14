// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using MauiB2C.Views;

namespace MauiB2C;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("mainview", typeof(MainView));
        Routing.RegisterRoute("scopeview", typeof(ScopeView));
    }
}
