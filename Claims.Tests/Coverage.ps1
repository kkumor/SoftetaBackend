Remove-Item TestResults\* -Recurse -Force

dotnet tool install -g coverlet.console;
dotnet tool install -g dotnet-reportgenerator-globaltool;

dotnet test --collect:"XPlat Code Coverage" /p:CollectCoverage=true /p:IncludeTestAssembly=true /p:CoverletOutputFormat=opencover

$initialDirectory = Get-Location;
$testResultsDir = "TestResults";
$directories = Get-ChildItem $testResultsDir -Recurse -Directory;
foreach ($dir in $directories) {
    $xmlFiles = Get-ChildItem $dir.FullName -Filter "coverage.cobertura.xml";
    if ($xmlFiles) {
        Set-Location -Path $dir.FullName;
        reportgenerator -reports:'**/coverage.cobertura.xml' -targetdir:'../../CoverageReports' -reporttypes:'Html' -filefilters:-**\Migrations*;
        Set-Location -Path "../";
    }
}

Set-Location -Path $initialDirectory;
