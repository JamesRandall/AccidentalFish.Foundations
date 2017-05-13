Param(
	[switch]$pushLocal,
	[switch]$pushNuget,
	[switch]$cleanup
)

if (Test-Path -Path nuget-powershell) 
{
	rmdir nuget-powershell -Recurse
}
if (Test-Path -Path nuget-cmdline) 
{
	rmdir nuget-cmdline -Recurse
}

rm .\Source\AccidentalFish.Foundations.Policies\bin\debug\*.nupkg
rm .\Source\AccidentalFish.Foundations.Resources.Abstractions\bin\debug\*.nupkg
rm .\Source\AccidentalFish.Foundations.Resources.Azure\bin\debug\*.nupkg
rm .\Source\AccidentalFish.Foundations.Runtime\bin\debug\*.nupkg
rm .\Source\AccidentalFish.Foundations.Runtime.HostableComponents\bin\debug\*.nupkg
rm .\Source\AccidentalFish.Foundations.Threading\bin\debug\*.nupkg

msbuild

if ($pushLocal)
{
	cp .\Source\AccidentalFish.Foundations.Policies\bin\debug\*.nupkg \MicroserviceAnalyticPackageRepository
	cp .\Source\AccidentalFish.Foundations.Resources.Abstractions\bin\debug\*.nupkg \MicroserviceAnalyticPackageRepository
	cp .\Source\AccidentalFish.Foundations.Resources.Azure\bin\debug\*.nupkg \MicroserviceAnalyticPackageRepository
	cp .\Source\AccidentalFish.Foundations.Runtime\bin\debug\*.nupkg \MicroserviceAnalyticPackageRepository
	cp .\Source\AccidentalFish.Foundations.Runtime.HostableComponents\bin\debug\*.nupkg \MicroserviceAnalyticPackageRepository
	cp .\Source\AccidentalFish.Foundations.Threading\bin\debug\*.nupkg \MicroserviceAnalyticPackageRepository
}

if ($pushNuget)
{
	.\nuget push *.nupkg -source nuget.org
}

if ($cleanup)
{
	rmdir nuget-powershell -Recurse
	rm *.nupkg
}
