using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace McProtoNet.Utils;
//public delegate void ServerFindedHandler(LanServer server);

public sealed class LanServerDetector
{
    //public event ServerFindedHandler ServerFinded;

    private const ushort Port = 4445;
    private const string Host = "224.0.2.60";


    private readonly Socket udpSocket;
    private EndPoint localEndPoint;
    private readonly IPEndPoint localIPEndPoint;

    public LanServerDetector(int ttl)
    {
        //IPHostEntry HosyEntry = Dns.GetHostEntry((Dns.GetHostName()));
        //if (HosyEntry.AddressList.Length > 0)
        //{
        //    foreach (IPAddress ip in HosyEntry.AddressList)
        //    {
        //        
        //        strIP = ip.ToString();
        //        cmbInterfaces.Items.Add(strIP);
        //    }
        //}     
        udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        localIPEndPoint = new IPEndPoint(IPAddress.Any, Port);
        localEndPoint = localIPEndPoint;

        //init Socket properties:
        udpSocket.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoDelay, 1);

        //allow for loopback testing 
        udpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);

        //extremly important to bind the Socket before joining multicast groups 
        udpSocket.Bind(localIPEndPoint);

        //set multicast flags, sending flags - TimeToLive (TTL) 
        // 0 - LAN 
        // 1 - Single Router Hop 
        // 2 - Two Router Hops... 
        udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, ttl);

        //join multicast group 
        udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
            new MulticastOption(IPAddress.Parse(Host), IPAddress.Broadcast));
    }


    public LanServerDetector() : this(0)
    {
    }

    public void StartReceiving()
    {
        
        
        
    }

    private Task StartReceivingAsync()
    {
        NetworkStream ns = new NetworkStream(udpSocket, true);
        StreamReader sr = new StreamReader(ns, Encoding.UTF8);
        while (true)
        {
            var line = sr.ReadLine();
            if (line == null) break;
            Notify(line, localIPEndPoint);
        }
        return Task.CompletedTask;
    }
    

    


    private void Notify(string data, EndPoint endPoint)
    {
        try
        {
            var ip = (IPEndPoint)endPoint;
            var motd = ParseMotd(data);
            var port = ushort.Parse(ParseAddress(data));
            //ServerFinded?.Invoke(new LanServer(ip.Address.ToString(), port, motd));
        }
        catch (Exception e)
        {
            Debug.WriteLine("Lan Server error: " + e);
        }
    }

   

    private static string ParseMotd(string line)
    {
        Debug.WriteLine("data: " + line);
        var n = line.IndexOf("[MOTD]");
        if (n < 0) return "missing no";
        var n2 = line.IndexOf("[/MOTD]", n + "[MOTD]".Length);
        if (n2 < n) return "missing no";

        return line.JavaSubStr(n + "[MOTD]".Length, n2);
    }


    private static string ParseAddress(string line)
    {
        var n = line.IndexOf("[/MOTD]");
        if (n < 0) return null;
        var n2 = line.IndexOf("[/MOTD]", n + "[/MOTD]".Length);
        if (n2 >= 0) return null;
        var n3 = line.IndexOf("[AD]", n + "[/MOTD]".Length);
        if (n3 < 0) return null;
        var n4 = line.IndexOf("[/AD]", n3 + "[AD]".Length);
        if (n4 < n3) return null;

        return line.JavaSubStr(n3 + "[AD]".Length, n4);
    }
}

internal static class JavaStringExt
{
    public static string JavaSubStr(this string self, int startIndex, int endIndex)
    {
        return self.Substring(startIndex, endIndex - startIndex);
    }

    public static string JavaSubStr(this string self, int startIndex)
    {
        return self.Substring(startIndex);
    }
}
