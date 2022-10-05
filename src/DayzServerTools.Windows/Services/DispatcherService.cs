using System;

using DayzServerTools.Application.Services;

namespace DayzServerTools.Windows.Services;

internal class DispatcherService: IDispatcherService
{
    public void BeginInvoke(Action callback) 
        => System.Windows.Application.Current.Dispatcher.BeginInvoke(callback);
}
