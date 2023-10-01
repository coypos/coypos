using System.Diagnostics;
using CoyposServer.Utils.Extensions;

namespace CoyposServer.Utils;

public static class Log
{
    public static string LocalCache = "";
    
    private static string NORMAL      = "\x1b[39m";
    private static string RED         = "\x1b[91m";
    private static string GREEN       = "\x1b[92m";
    private static string YELLOW      = "\x1b[93m";
    private static string BLUE        = "\x1b[94m";
    private static string MAGENTA     = "\x1b[95m";
    private static string CYAN        = "\x1b[96m";
    private static string GREY        = "\x1b[97m";
    private static string BOLD        = "\x1b[1m";
    private static string NOBOLD      = "\x1b[22m";
    private static string UNDERLINE   = "\x1b[4m";
    private static string NOUNDERLINE = "\x1b[24m";
    private static string REVERSE     = "\x1b[7m";
    private static string NOREVERSE   = "\x1b[27m";
    
    public static void Msg(string message, string context = "CoyPos") =>
        PrintMessage(false, $"{CYAN}Msg{NORMAL}", $"{CYAN}{message}{NORMAL}", context);

    public static void Wrn(string message, string context = "CoyPos") =>
        PrintMessage(false, $"{YELLOW}Wrn{NORMAL}", $"{YELLOW}{message}{NORMAL}", context);

    public static void Err(string message, string context = "CoyPos") =>
        PrintMessage(false, $"{RED}Err{NORMAL}", $"{RED}{message}{NORMAL}", context);

    public static void DebugMsg(string message, string context = "CoyPos")
        => PrintMessage(true, $"{CYAN}DebugMsg{NORMAL}", $"{CYAN}{message}{NORMAL}", context);

    public static void DebugWrn(string message, string context = "CoyPos") =>
        PrintMessage(true, $"{YELLOW}DebugWrn{NORMAL}", $"{YELLOW}{message}{NORMAL}", context);

    public static void DebugErr(string message, string context = "CoyPos") =>
        PrintMessage(true, $"{RED}DebugErr{NORMAL}", $"{RED}{message}{NORMAL}", context);

    private static void PrintMessage(bool debug, string type, string message, string context = "CoyPos")
    {
        var messageString =
            $"[{type}] [{String.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.UtcNow)}] [{context}] {message}";
        if (debug)
            Debug.WriteLine(messageString);
        else 
            Console.WriteLine(messageString);
        SaveEntry(messageString);
    }

    private static StreamWriter? _logFileStream = null;
    private static DateTime _today;
    private static void SaveEntry(string message)
    {
        if (_logFileStream is null || _today != DateTime.Today)
        {
            Directory.CreateDirectory("/var/lib/coypos/logs");
            _logFileStream = new StreamWriter($"/var/lib/coypos/logs/{DateTime.UtcNow:yyyy-MM-dd}.log", true);
            _today = DateTime.Today;
        }
        
        message = message.RemoveAllOccurrences(NORMAL, RED, GREEN, YELLOW, BLUE, MAGENTA, CYAN, GREY, BOLD, NOBOLD, UNDERLINE,
            NOUNDERLINE, REVERSE, NOREVERSE);
        LocalCache += message += "\n";
        _logFileStream.WriteLine(message);
    }

    public static void Dispose()
    {
        _logFileStream?.Close();
    }
}