using DreamCheeky.Driver;

using var bigRedButton = new BigRedButton();

bigRedButton.LidClosed += (sender, args) => Console.WriteLine("Kapak kapandı");
bigRedButton.LidOpen += (sender, args) => Console.WriteLine("Kapak açıldı");
bigRedButton.ButtonPressed += (sender, args) => Console.WriteLine("Butona basıldı");


bigRedButton.Start();

Console.WriteLine("Kapatmak için ENTER tuşuna basabilirsin...");
Console.ReadLine();

bigRedButton.Stop();