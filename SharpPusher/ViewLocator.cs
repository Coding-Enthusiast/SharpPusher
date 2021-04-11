// SharpPusher
// Copyright (c) 2017 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Avalonia.Controls;
using Avalonia.Controls.Templates;
using SharpPusher.ViewModels;
using System;

namespace SharpPusher
{
    public class ViewLocator : IDataTemplate
    {
        public bool SupportsRecycling => false;

        public IControl Build(object data)
        {
            if (data is null)
            {
                return new TextBlock { Text = "Unexpected Null ViewModel was received." };
            }
            else
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.
                string name = data.GetType().FullName.Replace("ViewModel", "View");
                Type type = Type.GetType(name);

                if (type is not null)
                {
                    return (Control)Activator.CreateInstance(type);
                }
                else
                {
                    return new TextBlock { Text = "Not Found: " + name };
                }
            }
        }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8603 // Possible null reference return.
        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}
