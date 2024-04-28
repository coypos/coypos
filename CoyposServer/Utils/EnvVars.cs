using CoyposServer.Utils.Extensions;

namespace CoyposServer.Utils;

public static class EnvVars
{
    public static string DatabaseUser => GetEnvVar("DB_USER");
    public static string DatabasePass => GetEnvVar("DB_PASS");
    public static string DatabaseHost => GetEnvVar("DB_HOST");
    public static string DatabasePort => GetEnvVar("DB_PORT");
    public static string ServerApiKey => GetEnvVar("SERVER_APIKEY");

    private static string GetEnvVar(string v)
    {
        var val = Environment.GetEnvironmentVariable(v);
        if (val.IsNullOrEmpty())
            throw new Exception($"Environment variable {v} not set or empty");
        return val!;
    }

    public static void VerifyEnvVars()
    {
        var env = "";
        try
        {
            Log.Msg($"✅ DB_USER => {DatabaseUser}", "EnvVars");
            Log.Msg($"✅ DB_PASS => {new string('*', DatabasePass.Length)}", "EnvVars");
            Log.Msg($"✅ DB_HOST => {DatabaseHost}", "EnvVars");
            Log.Msg($"✅ DB_PORT => {DatabasePort}", "EnvVars");
            Log.Msg($"✅ SERVER_APIKEY => {new string('*', ServerApiKey.Length)}", "EnvVars");
        }
        catch (Exception e)
        {
            throw new Exception("Failed to verify environment variables => " + e.Message);
        }
        Log.Msg("✅ All good!", "EnvVars");
    }
}