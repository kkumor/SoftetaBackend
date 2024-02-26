using System.Runtime.CompilerServices;
using Argon;

namespace Claims.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.AddExtraSettings(settings => settings.DefaultValueHandling = DefaultValueHandling.Populate);
        VerifierSettings.DontScrubDateTimes();
    }
}