using QuickProxyNet;

namespace McProtoNet.Client.New;

public class MinecraftClientBuilder
{
    public MinecraftClient Build()
    {
        return new MinecraftClient(new MinecraftClientStartOptions()
        {
            ConnectTimeout = ConnectTimeout,
            Host = Host,
            Port = Port,
            Version = Version,
            Proxy = Proxy,
            ReadTimeout = ReadTimeout,
            WriteTimeout = WriteTimeout
        });
    }

    public MinecraftClientBuilder WithStartOptions(MinecraftClientStartOptions options)
    {
        StartOptions = options;
        return this;
    }

    public MinecraftClientStartOptions StartOptions { get; private set; }

    public MinecraftClientBuilder WithProxy(IProxyClient proxy)
    {
        Proxy = proxy;
        return this;
    }

    public IProxyClient? Proxy { get; private set; }

    public MinecraftClientBuilder WithReadTimeout(TimeSpan readTimeout)
    {
        ReadTimeout = readTimeout;
        return this;
    }

    public MinecraftClientBuilder WithWriteTimeout(TimeSpan writeTimeout)
    {
        WriteTimeout = writeTimeout;
        return this;
    }

    public TimeSpan ReadTimeout { get; private set; } = TimeSpan.FromSeconds(30);
    public TimeSpan WriteTimeout { get; private set; } = TimeSpan.FromSeconds(30);

    public MinecraftClientBuilder WithConnectTimeout(TimeSpan connectTimeout)
    {
        ConnectTimeout = connectTimeout;
        return this;
    }

    public TimeSpan ConnectTimeout { get; private set; } = TimeSpan.FromSeconds(3);

    public MinecraftClientBuilder WithVersion(int version)
    {
        Version = version;
        return this;
    }

    public int Version { get; private set; }

    public MinecraftClientBuilder WithHost(string host)
    {
        Host = host;
        return this;
    }

    public string Host { get; private set; }

    public MinecraftClientBuilder WithPort(int port)
    {
        Port = port;
        return this;
    }

    public int Port { get; private set; }
}