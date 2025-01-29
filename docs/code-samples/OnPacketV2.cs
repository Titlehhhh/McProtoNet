client.OnPacket<PositionPacket.V768_769>()
    .Subscribe(p =>
    {
        Console.WriteLine(p.Dy);
    });