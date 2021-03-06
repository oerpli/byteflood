﻿/* 
    ByteFlood - A BitTorrent client.
    Copyright (C) 2014 Burak Öztunç

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;
using System.Net.Sockets;
using System.Diagnostics;
using System.Reflection;

namespace ByteFlood
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ByteFlood.Settings Settings = new Settings();

        ResourceDictionary themerd = new ResourceDictionary();

        public static string[] to_add;

        protected override void OnStartup(StartupEventArgs e)
        {
            Environment.CurrentDirectory =
                new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent.FullName;
            if (e.Args.Length != 0)
            {
                try
                {
                    foreach (string str in e.Args)
                    {
                        TcpClient tcp = new TcpClient();
                        tcp.Connect("127.0.0.1", 65432);
                        NetworkStream ns = tcp.GetStream();
                        StreamWriter sw = new StreamWriter(ns);
                        sw.WriteLine(str);
                        sw.Flush();
                        tcp.Close();
                    }
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    to_add = e.Args;
                }
            }
            else
                to_add = new string[0];

            base.OnStartup(e);

            Settings = Settings.Load("./config.xml");

            LoadTheme(Settings.Theme);
        }


        private static Dictionary<Theme, string> theme_mapping = new Dictionary<Theme, string>() 
        {
            {Theme.Aero, "PresentationFramework.Aero;V3.0.0.0;31bf3856ad364e35;component/themes/Aero.normalcolor.xaml"},
            {Theme.Aero2, "PresentationFramework.Aero2;V3.0.0.0;31bf3856ad364e35;component/themes/Aero2.normalcolor.xaml"},
            {Theme.Classic, "PresentationFramework.Classic;V3.0.0.0;31bf3856ad364e35;component/themes/classic.xaml"},
            {Theme.Luna, "PresentationFramework.Luna;V3.0.0.0;31bf3856ad364e35;component/themes/Luna.normalcolor.xaml"},
            {Theme.Royale, "PresentationFramework.Royale;V3.0.0.0;31bf3856ad364e35;component/themes/Royale.normalcolor.xaml"}
        };

        public void LoadTheme(Theme t)
        {
            if (Utility.IsWindows8OrNewer)
            {
                themerd.Source = new Uri(theme_mapping[t], UriKind.Relative);
            }
            else
            {
                if (t == Theme.Aero2)
                {
                    //Aero2 is for windows 8 and newer
                    themerd.Source = new Uri(theme_mapping[Theme.Aero], UriKind.Relative);
                }
                else
                {
                    themerd.Source = new Uri(theme_mapping[t], UriKind.Relative);
                }
            }

            if (!this.Resources.MergedDictionaries.Contains(themerd))
            {
                Resources.MergedDictionaries.Add(themerd);
            }

            Settings.Theme = t;
        }
    }
}
