﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using SQLite;
using System.Diagnostics;

namespace Packet_Sender_Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrafficPage : ContentPage
    {


        private ObservableCollection<Packet> _trafficpackets;
        private SQLiteAsyncConnection _connection;
        private bool _logtraffice;

        public TrafficPage()
        {
            InitializeComponent();
            _logtraffice = SettingsPage.LogTraffic;
            _trafficpackets =  new ObservableCollection<Packet>(new List<Packet>());

            trafficListView.ItemsSource = _trafficpackets;
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

            MessagingCenter.Subscribe<PacketsPage, Packet>(this, Events.NEW_TRAFFIC_PACKET, OnNewTrafficPacketAsync);
            MessagingCenter.Subscribe<SettingsPage, bool>(this, Events.TRAFFIC_TOGGLED, OnTrafficToggled);

        }

        private void OnTrafficToggled(SettingsPage source, bool toggled)
        {
            _logtraffice = SettingsPage.LogTraffic;
        }


        private void OnNewTrafficPacketAsync(PacketsPage source, Packet newpkt)
        {
            if (_logtraffice) {
                Debug.WriteLine("TP:Packet from has " + newpkt.fromip);
                if(_trafficpackets.Count == 0) {
                    _trafficpackets = new ObservableCollection<Packet>(new List<Packet>());
                    _trafficpackets.Add(newpkt); //(newpkt);
                    Debug.WriteLine("TP:Packet added");
                    trafficListView.ItemsSource = _trafficpackets;
                    Debug.WriteLine("TP:Packet added done");
                } else {
                    _trafficpackets.Insert(0, newpkt); //(newpkt);
                }
            }
            Debug.WriteLine("TP:Finished OnNewTrafficPacketAsync");
        }

        private void clearButton_Clicked(object sender, EventArgs e)
        {
            _trafficpackets.Clear();

        }

        private async void saveButton_ClickedAsync(object sender, EventArgs e)
        {
            var savepacket = trafficListView.SelectedItem as Packet;

            if (savepacket == null)
            {
                Debug.WriteLine("TP:saveButton_Clicked with null");
                return;

            }
            savepacket.name = string.Empty;
            if(savepacket.toip == "You") {
                savepacket.toip = savepacket.fromip;
                savepacket.toport = savepacket.fromport;
            }

            Debug.WriteLine("TP:saveButton_Clicked " + savepacket.name + " " + savepacket.method);
            //Navigation.PushAsync((new PacketEditPage()));
            await Navigation.PushModalAsync(new PacketEditPage(savepacket));



        }

        private void trafficListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }
    }
}