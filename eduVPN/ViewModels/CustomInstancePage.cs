﻿/*
    eduVPN - End-user friendly VPN

    Copyright: 2017, The Commons Conservancy eduVPN Programme
    SPDX-License-Identifier: GPL-3.0+
*/

using Prism.Commands;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace eduVPN.ViewModels
{
    /// <summary>
    /// Custom instance source entry wizard page
    /// </summary>
    public class CustomInstancePage : ConnectWizardPage
    {
        #region Properties

        /// <summary>
        /// Instance host name
        /// </summary>
        public string Hostname
        {
            get { return _host_name; }
            set { SetProperty(ref _host_name, value); }
        }
        private string _host_name;

        /// <summary>
        /// Authorize other instance command
        /// </summary>
        public DelegateCommand SelectCustomInstance
        {
            get
            {
                if (_select_custom_instance == null)
                {
                    bool TryParseUri(string input, out Uri output)
                    {
                        try
                        {
                            // Convert hostname to https://hostname.
                            output = new UriBuilder("https", input).Uri;
                            return true;
                        }
                        catch
                        {
                            output = null;
                            return false;
                        }
                    };

                    _select_custom_instance = new DelegateCommand(
                        // execute
                        async () =>
                        {
                            Parent.ChangeTaskCount(+1);
                            try
                            {
                                TryParseUri(Hostname, out var uri);
                                var selected_instance = new Models.InstanceInfo(uri);
                                selected_instance.RequestAuthorization += Parent.Instance_RequestAuthorization;

                                // Trigger initial authorization request.
                                var authorization_task = new Task(() => selected_instance.GetAccessToken(Window.Abort.Token), Window.Abort.Token, TaskCreationOptions.LongRunning);
                                authorization_task.Start();
                                await authorization_task;

                                // Set authentication instance.
                                Parent.AuthenticatingInstance = selected_instance;

                                // Go to (instance and) profile selection page.
                                Parent.CurrentPage = Parent.ConnectingProfileSelectPage;
                            }
                            catch (Exception ex) { Parent.Error = ex; }
                            finally { Parent.ChangeTaskCount(-1); }
                        },

                        // canExecute
                        () => TryParseUri(Hostname, out var uri));

                    // Setup canExecute refreshing.
                    PropertyChanged += (object sender, PropertyChangedEventArgs e) => { if (e.PropertyName == nameof(Hostname)) _select_custom_instance.RaiseCanExecuteChanged(); };
                }

                return _select_custom_instance;
            }
        }
        private DelegateCommand _select_custom_instance;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a view model.
        /// </summary>
        public CustomInstancePage(ConnectWizard parent) :
            base(parent)
        {
        }

        #endregion

        #region Methods

        protected override void DoNavigateBack()
        {
            base.DoNavigateBack();

            Parent.CurrentPage = Parent.InstanceSourceSelectPage;
        }

        protected override bool CanNavigateBack()
        {
            return true;
        }

        #endregion
    }
}
