// SharpPusher
// Copyright (c) 2017 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SharpPusher.ViewModels;
using SharpPusher.Views;

namespace SharpPusher
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow()
                {
                    DataContext = new MainWindowViewModel()
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
