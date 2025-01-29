client.OnPacket<PositionPacket>()
    .Subscribe(p =>
    {
        if (p is PositionPacket.V768_769 v768_)
        {
            Console.WriteLine(v768_.Dx);
        }
    });