using DreamCheeky.Driver.Enums;
using HidLibrary;

namespace DreamCheeky.Driver;

class Device : IDisposable
{
    private readonly byte[] statusCommand = { 0, 0, 0, 0, 0, 0, 0, 0, 2 };

    private readonly int vendorId = 0x1D34;
    private readonly int productId = 0x000D;
    private readonly HidDevice? device;
    public bool IsOpen { get; private set; }

    public Device()
    {
        device = HidDevices.Enumerate(vendorId, productId).FirstOrDefault();

        if (device == null)
            throw new InvalidOperationException("Device not found");
    }

    public void Open()
    {
        if (device == null)
            throw new InvalidOperationException("Device not found");

        device.OpenDevice();
        IsOpen = true;
    }

    public void Close()
    {
        if (device == null)
            throw new InvalidOperationException("Device not found");

        try
        {
            device.CloseDevice();
            IsOpen = false;
        }
        catch
        {
            IsOpen = false;
        }
        
    }

    public DeviceStatus GetStatus()
    {
        try
        {
            if (device == null)
            {
                return DeviceStatus.Errored;
            }

            if (!device.Write(statusCommand, 100))
            {
                return DeviceStatus.Errored;
            }

            HidDeviceData data = device.Read(100);

            if (data.Status != HidDeviceData.ReadStatus.Success)
            {
                return DeviceStatus.Errored;
            }

            return (DeviceStatus)data.Data[1];
        }
        catch
        {
            return DeviceStatus.Unknown;
        }
    }

    public void Dispose()
    {
        Close();
    }
}
