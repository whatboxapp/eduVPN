﻿/*
    eduVPN - End-user friendly VPN

    Copyright: 2017, The Commons Conservancy eduVPN Programme
    SPDX-License-Identifier: GPL-3.0+
*/

using Prism.Commands;
using System;
using System.Windows.Input;

namespace eduVPN.ViewModels
{
    /// <summary>
    /// Instance selection base wizard page
    /// </summary>
    public class AuthenticatingInstanceSelectPage : ConnectWizardPage
    {
        #region Properties

        /// <summary>
        /// Selected instance
        /// </summary>
        /// <remarks><c>null</c> if none selected.</remarks>
        public Models.InstanceInfo SelectedInstance
        {
            get { return _selected_instance; }
            set
            {
                _selected_instance = value;
                RaisePropertyChanged();
                ((DelegateCommandBase)AuthorizeSelectedInstance).RaiseCanExecuteChanged();
            }
        }
        private Models.InstanceInfo _selected_instance;

        /// <summary>
        /// Authorize selected instance command
        /// </summary>
        public ICommand AuthorizeSelectedInstance
        {
            get
            {
                lock (_authorize_instance_lock)
                {
                    if (_authorize_instance == null)
                    {
                        _authorize_instance = new DelegateCommand(
                            // execute
                            () =>
                            {
                                Parent.Error = null;
                                Parent.ChangeTaskCount(+1);
                                try
                                {
                                    // Save selected instance.
                                    Parent.Configuration.AuthenticatingInstance = SelectedInstance;

                                    if (Parent.InstanceSource is Models.LocalInstanceSourceInfo)
                                    {
                                        // Connecting instance will be the same as authenticating.
                                        Parent.Configuration.ConnectingInstance = Parent.Configuration.AuthenticatingInstance;
                                    }
                                    else if (Parent.InstanceSource is Models.DistributedInstanceSourceInfo)
                                    {
                                        // Connecting instance will not (necessarry) be the same as authenticating.
                                        Parent.Configuration.ConnectingInstance = null;
                                    }
                                    else
                                        throw new NotImplementedException();

                                    Parent.CurrentPage = Parent.ProfileSelectPage;
                                }
                                catch (Exception ex) { Parent.Error = ex; }
                                finally { Parent.ChangeTaskCount(-1); }
                            },

                            // canExecute
                            () => SelectedInstance != null);
                    }

                    return _authorize_instance;
                }
            }
        }
        private ICommand _authorize_instance;
        private object _authorize_instance_lock = new object();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs an instance selection wizard page.
        /// </summary>
        /// <param name="parent">The page parent</param>
        public AuthenticatingInstanceSelectPage(ConnectWizard parent) :
            base(parent)
        {
        }

        #endregion

        #region Methods

        public override void OnActivate()
        {
            base.OnActivate();

            // Reset selected instance, to prevent automatic continuation to
            // CustomInstance/Authorization page.
            SelectedInstance = null;
        }

        protected override void DoNavigateBack()
        {
            if (Array.IndexOf(Parent.InstanceSources, Parent.InstanceSource) >= 0)
                Parent.CurrentPage = Parent.InstanceSourceSelectPage;
            else
                Parent.CurrentPage = Parent.CustomInstancePage;
        }

        protected override bool CanNavigateBack()
        {
            return true;
        }

        #endregion
    }
}