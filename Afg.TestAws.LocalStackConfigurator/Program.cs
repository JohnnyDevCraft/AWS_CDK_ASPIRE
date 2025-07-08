using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Ardalis.GuardClauses;

var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
var shell = isWindows ? "cmd.exe" : Environment.GetEnvironmentVariable("SHELL") ?? "/bin/zsh";
var shellConfig = "~/.zshrc";
List<string> envCommands = [];
List<string> commands = [];

if (!isWindows)
{
    switch (shell)
    {
        case "/bin/bash":
            shellConfig = "~/.bashrc";
            break;
        case "/bin/zsh":
            shellConfig = "~/.zshrc";
            break;
        default:
            throw new InvalidOperationException($"Unsupported shell: {shell}");
    }
}

//SetDefaults();
SetNodeVersion("22");
SetLocalStackSetup();
RunCommands();
return;

void SetDefaults()
{
    if (!isWindows)
    {
        Console.WriteLine("Setting up environment for shell: " + shell);
        // Ensure the shell config is sourced
        commands.Add($"source {shellConfig}");
    } else 
    {
        Console.WriteLine("Setting up environment for shell: " + shell);
        // For Windows, we need to use the 'call' command to source the config
        commands.Add($"/c {shellConfig}");
    }
}

void SetNodeVersion(string version)
{
    // Ensure NVM is loaded and set the Node.js version
    Console.WriteLine($"Setting Node.js version to {version}");
    commands.Add($"nvm use {version}");
}

void SetLocalStackSetup()
{
    Console.WriteLine("Setting up LocalStack configuration...");
    // Use CDK Local to bootstrap LocalStack
    commands.Add("cdklocal bootstrap aws://000000000000/us-east-1" + "  --bootstrap-bucket-name cdktoolkit-deploys" + "  --qualifier hnb659fds");
}

void RunCommands()
{
    Console.WriteLine("Running commands...");
    Guard.Against.NullOrEmpty(shell, nameof(shell), "Shell executable must be set before running commands.");
    var psi = new ProcessStartInfo
    {
        FileName = shell,
        Arguments = $"-i -c \"{string.Join(" && ", commands)}\"",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    psi.Environment.Add("AWS_ACCESS_KEY", "test");
    psi.Environment.Add("AWS_SECRET_ACCESS_KEY", "test");
    psi.Environment.Add("AWS_REGION", "us-east-1");
    
    Console.WriteLine("Process Start Info: {0}", psi.Arguments);

    using var proc = Process.Start(psi);
    Guard.Against.Null(proc, nameof(proc), "Process cannot be null.");

    Console.WriteLine("Process started successfully. Waiting for output...");
    var stdout = proc.StandardOutput.ReadToEnd();
    var stderr = proc.StandardError.ReadToEnd();
    proc.WaitForExit();
    
    Console.WriteLine("Process completed. Output received.");

    Console.WriteLine("== OUT ==");
    Console.WriteLine(stdout);
    if (!string.IsNullOrEmpty(stderr))
    {
        Console.WriteLine("== ERR ==");
        Console.WriteLine(stderr);
    }

    Console.WriteLine($"Exit Code: {proc.ExitCode}");
}


