using CoreBlow.WindowsNode;
using Xunit;

namespace CoreBlow.WindowsNode.Tests;

public sealed class NodeStatusTests
{
    [Fact]
    public void RunningNodeCanOpenDashboardAndStudio()
    {
        var status = NodeStatus.Normalize(" Running ", 1);
        Assert.True(status.CanOpenDashboard);
        Assert.True(status.CanOpenStudio);
        Assert.True(status.IsPortBound);
        Assert.Equal(NodeStatus.DefaultGatewayPort, status.Port);
    }

    [Fact]
    public void ActiveConnectionsNeverGoesNegative()
    {
        Assert.Equal(0, NodeStatus.Normalize("stopped", -2).ActiveConnections);
    }

    [Fact]
    public void DefaultGatewayPortIs18789()
    {
        Assert.Equal(18789, NodeStatus.DefaultGatewayPort);
        Assert.Equal("/health", NodeStatus.HealthEndpoint);
        Assert.Equal("/api/corehub/desktop/state", NodeStatus.DesktopStateEndpoint);
        Assert.Equal("/api/corehub/desktop/quick-launch", NodeStatus.QuickLaunchEndpoint);
    }

    [Fact]
    public void GatewayHealthCalculatesCorrectly()
    {
        var healthy = new GatewayHealthStatus(
            IsReachable: true,
            Port: 18789,
            Version: "2.0.0",
            RouterTier: "CODING",
            ActiveSubagents: 4,
            VoiceRtf: 0.18
        );
        Assert.True(healthy.IsHealthy);

        var unhealthyRtf = healthy with { VoiceRtf = 0.55 };
        Assert.False(unhealthyRtf.IsHealthy);

        var wrongPort = healthy with { Port = 8080 };
        Assert.False(wrongPort.IsHealthy);
    }

    [Fact]
    public void TrayStatusSummaryContainsBrandAndPort()
    {
        var status = NodeStatus.Normalize("running", 2);
        Assert.Contains("CoreBlow Windows Node v2.0.0", status.TrayStatusSummary);
        Assert.Contains("18789", status.TrayStatusSummary);
    }
}
