﻿/*
AccountSettings.xaml.cs
Copyright (C) 2015  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using BelledonneCommunications.Linphone.Native;
using Linphone.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Linphone.Views
{
    /// <summary>
    /// Page used to display SIP account settings
    /// </summary>
    public partial class AccountSettings : Page
    {
        private SIPAccountSettingsManager _settings = new SIPAccountSettingsManager();
      
        private bool saveSettingsOnLeave = true;
        private bool linphoneAccount = false;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public AccountSettings()
        {
            this.InitializeComponent();

            _settings.Load();
            Username.Text = _settings.Username;
            UserId.Text = _settings.UserId;
            Password.Password = _settings.Password;
            Domain.Text = _settings.Domain;
            Proxy.Text = _settings.Proxy;
            OutboundProxy.IsEnabled = (bool) _settings.OutboundProxy;
            DisplayName.Text = _settings.DisplayName;
            Expires.Text = _settings.Expires;

            List<string> transports = new List<string>
            {
                ResourceLoader.GetForCurrentView().GetString("TransportUDP"),
                ResourceLoader.GetForCurrentView().GetString("TransportTCP"),
                ResourceLoader.GetForCurrentView().GetString("TransportTLS")
            };
            Transport.ItemsSource = transports;
            Transport.SelectedItem = _settings.Transports;

            AVPF.IsEnabled = true;
        }

        private void Save()
        {
            if (Domain.Text.Contains(":"))
            {
                if (Proxy.Text.Length == 0)
                {
                    Proxy.Text = Domain.Text;
                }
                Domain.Text = Domain.Text.Split(':')[0];
            }

            _settings.Username = Username.Text;
            _settings.UserId = UserId.Text;
            _settings.Password = Password.Password;
            _settings.Domain = Domain.Text;
            _settings.Proxy = Proxy.Text;
            _settings.OutboundProxy = OutboundProxy.IsEnabled;
            _settings.DisplayName = DisplayName.Text;
            _settings.Transports = Transport.SelectedItem.ToString();
            _settings.Expires = Expires.Text;
            _settings.AVPF = AVPF.IsEnabled;
            _settings.Save();

            if (linphoneAccount)
            {
                NetworkSettingsManager networkSettings = new NetworkSettingsManager();
                networkSettings.Load();
                networkSettings.MEncryption = networkSettings.EnumToMediaEncryption[MediaEncryption.SRTP];
                networkSettings.FWPolicy = networkSettings.EnumToFirewallPolicy[FirewallPolicy.UseIce];
                networkSettings.StunServer = "stun.linphone.org";
                networkSettings.Save();
            }
        }

        /// <summary>
        /// Method called when the user is navigation away from this page
        /// </summary>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (saveSettingsOnLeave)
            {
                Save();
                Debug.WriteLine("SAVE ");
            }
            base.OnNavigatingFrom(e);
        }

        private void cancel_Click_1(object sender, RoutedEventArgs e)
        {
            saveSettingsOnLeave = false;
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }

        }

        private void save_Click_1(object sender, RoutedEventArgs e)
        {
            saveSettingsOnLeave = true;
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void linphone_Click_1(object sender, RoutedEventArgs e)
        {
            Domain.Text = "sip.linphone.org";
            Transport.SelectedItem = ResourceLoader.GetForCurrentView().GetString("TransportTLS");
            Proxy.Text = "sip.linphone.org:5223";
            OutboundProxy.IsEnabled = true;
            Expires.Text = "28800";
            AVPF.IsEnabled = true;
            linphoneAccount = true;
        }
    }
}