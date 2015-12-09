using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using NV.Midi;
using File = System.IO.File;

namespace NV.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly MainWindow _window;
        public App()
        {
            var nanoKontrol = new NanoKontrol2();
            nanoKontrol.Start();

            _window = new MainWindow(nanoKontrol);

            var shell = new WshShellClass();
            var shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\NanoVolume.lnk";

            var trayIcon = new NotifyIcon();

            var menuItemExit = new MenuItem
            {
                Index = 1,
                Text = @"Exit"
            };

            var menuItemStartup = new MenuItem()
            {
                Index = 0,
                Text = @"Start on Boot",
                Checked = File.Exists(shortcutPath)
            };

            var menuItemShowWindow = new MenuItem()
            {
                Index = 2,
                Text = @"Show Window"
            };

            menuItemStartup.Click += (sender, args) =>
            {
                if (File.Exists(shortcutPath))
                {
                    File.Delete(shortcutPath);
                    menuItemStartup.Checked = false;
                }
                else
                {
                    var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                    shortcut.TargetPath = Assembly.GetEntryAssembly().Location;
                    shortcut.Description = @"Startup for NanoVolume";
                    shortcut.WorkingDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    shortcut.Save();
                    menuItemStartup.Checked = true;
                }
            };

            menuItemExit.Click += (sender, args) =>
            {
                nanoKontrol.Stop();
                trayIcon.Visible = false;
                Environment.Exit(0);
            };

            menuItemShowWindow.Click += (sender, args) =>
            {
                _window.Show();
            };

            trayIcon.ContextMenu = new ContextMenu()
            {
                MenuItems =
                {
                    menuItemShowWindow,
                    menuItemStartup,
                    menuItemExit
                }
            };

            trayIcon.DoubleClick += (sender, args) =>
            {
                _window.Show();
            };

            trayIcon.Text = @"NanoVolume";
            trayIcon.Icon = new Icon("app.ico");
            trayIcon.Visible = true;
        }
    }
}
