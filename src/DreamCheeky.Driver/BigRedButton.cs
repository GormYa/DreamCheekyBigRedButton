using DreamCheeky.Driver.Enums;

namespace DreamCheeky.Driver;

public class BigRedButton : IDisposable
{
    private readonly Device device;
    private volatile bool terminated = false;
    private Thread thread;
    private TimeSpan PollingInterval { get; set; }
    private DeviceStatus lastStatus = DeviceStatus.Unknown;

    public BigRedButton()
    {
        PollingInterval = TimeSpan.FromMilliseconds(100);
        device = new Device();
    }

    public void Start()
    {
        thread = new Thread(ThreadCallback);
        thread.Start();
    }

    private void ThreadCallback()
    {
        while (!terminated)
        {
            var thread2 = new Thread(GetStatus);
            thread2.Start();
            thread2.Join(TimeSpan.FromMilliseconds(100));

            Thread.Sleep(100);
        }
    }

    private void GetStatus()
    {
        device.Open();
        var status = device.GetStatus();
        device.Close();

        switch (status)
        {
            case DeviceStatus.LidClosed when lastStatus == DeviceStatus.LidOpen:
                LidClosed?.Invoke(this, EventArgs.Empty);
                lastStatus = status;
                break;
            case DeviceStatus.ButtonPressed when lastStatus != DeviceStatus.ButtonPressed:
                ButtonPressed?.Invoke(this, EventArgs.Empty);
                lastStatus = status;
                break;
            case DeviceStatus.LidOpen when lastStatus == DeviceStatus.LidClosed:
                LidOpen?.Invoke(this, EventArgs.Empty);
                lastStatus = status;
                break;
            case DeviceStatus.Errored:
                break;
            default:
                lastStatus = status;
                break;
        }
    }

    public void Stop()
    {
        terminated = true;
        thread.Join(TimeSpan.FromSeconds(2));
        if (device.IsOpen)
        {
            device.Close();
        }
    }

    public void Dispose()
    {
        Stop();
    }

    public EventHandler LidOpen;
    public EventHandler LidClosed;
    public EventHandler ButtonPressed;
}
