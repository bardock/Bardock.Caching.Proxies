:: nuget spec
set /p version=Version number:
nuget pack Bardock.Caching.Proxies.csproj -Prop Configuration=Debug -Symbols -IncludeReferencedProjects -Version %version%
nuget push Bardock.Caching.Proxies.%version%.nupkg
pause;