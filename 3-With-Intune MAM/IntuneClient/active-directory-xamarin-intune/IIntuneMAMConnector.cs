// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace activedirectoryxamarinintune
{
    /// <summary>
    /// Interface for platform specific Intune implementation
    /// </summary>
    public interface IIntuneMAMConnector
    {
        /// <summary>
        /// Perform registration with MAM
        /// </summary>
        /// <param name="exProtection"></param>
        /// <returns></returns>
        Task DoMAMRegisterAsync(IntuneAppProtectionPolicyRequiredException exProtection);

        /// <summary>
        /// Unenrolls App from MAM
        /// </summary>
        void Unenroll();
    }
}
