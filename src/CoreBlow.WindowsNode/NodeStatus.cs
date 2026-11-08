namespace CoreBlow.WindowsNode;

public sealed record NodeStatus(string State, int ActiveConnections, int Port = NodeStatus.DefaultGatewayPort)
{
    public const int DefaultGatewayPort = 18789;
    public const string HealthEndpoint = "/health";
    public const string DesktopStateEndpoint = "/api/corehub/desktop/state";
    public const string QuickLaunchEndpoint = "/api/corehub/desktop/quick-launch";

    public bool CanOpenDashboard => State is "running" or "degraded";
    public bool CanOpenStudio => State is "running" or "degraded";
    public bool IsPortBound => Port == DefaultGatewayPort;

    public GatewayHealthStatus GatewayHealth { get; init; } = GatewayHealthStatus.Default;

    public string TrayStatusSummary =>
        $"CoreBlow Windows Node v2.0.0 | State: {State} | Port: {Port} | Gateway: {(GatewayHealth.IsReachable ? "ONLINE" : "STANDBY")}";

    public static NodeStatus Normalize(string state, int activeConnections, int port = DefaultGatewayPort)
    {
        return new NodeStatus(
            state.Trim().ToLowerInvariant(),
            Math.Max(0, activeConnections),
            port > 0 ? port : DefaultGatewayPort
        );
    }
}

public sealed record GatewayHealthStatus(
    bool IsReachable,
    int Port,
    string Version,
    string RouterTier,
    int ActiveSubagents,
    double VoiceRtf
)
{
    public bool IsHealthy => IsReachable && Port == NodeStatus.DefaultGatewayPort && VoiceRtf <= 0.40;

    public static GatewayHealthStatus Default => new(
        IsReachable: false,
        Port: NodeStatus.DefaultGatewayPort,
        Version: "2.0.0",
        RouterTier: "CODING",
        ActiveSubagents: 0,
        VoiceRtf: 0.18
    );
}
