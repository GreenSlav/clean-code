using DotNetEnv;

namespace Infrastructure.Services;

public class EnvService
{
    public EnvService(string envFilePath = ".env")
    {
        Load(envFilePath);
    }

    private void Load(string envFilePath)
    {
        if (File.Exists(envFilePath))
        {
            Env.Load(envFilePath);
        }
        else
        {
            throw new FileNotFoundException($"Environment file '{envFilePath}' not found.");
        }
    }

    public string GetVariable(string key, string defaultValue = null!)
    {
        return Environment.GetEnvironmentVariable(key) ?? defaultValue;
    }
}
