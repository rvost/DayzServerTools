namespace DayzServerTools.Application.Services;

public interface IDispatcherService
{
    void BeginInvoke(Action callback);
}
