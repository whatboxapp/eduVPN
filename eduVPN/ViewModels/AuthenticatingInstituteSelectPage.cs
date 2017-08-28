﻿/*
    eduVPN - End-user friendly VPN

    Copyright: 2017, The Commons Conservancy eduVPN Programme
    SPDX-License-Identifier: GPL-3.0+
*/

namespace eduVPN.ViewModels
{
    /// <summary>
    /// Institute selection wizard page
    /// </summary>
    public class AuthenticatingInstituteSelectPage : AuthenticatingInstanceSelectPage
    {
        #region Constructors

        /// <summary>
        /// Constructs an institute selection wizard page.
        /// </summary>
        /// <param name="parent">The page parent</param>
        public AuthenticatingInstituteSelectPage(ConnectWizard parent) :
            base(parent)
        {
        }

        #endregion
    }
}