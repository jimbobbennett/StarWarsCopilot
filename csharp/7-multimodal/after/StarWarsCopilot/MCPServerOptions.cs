namespace StarWarsCopilot;

public static class MCPServerOptions
{
    public static string Name => "StarWarsMCPServer";
    public static string Command => "dotnet";
    public static List<string> Arguments => [
        "run",
        "--project",
        "<path>/StarWarsMCPServer.csproj"
    ];
}