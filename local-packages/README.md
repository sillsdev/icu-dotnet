# Local NuGet packages

Drop `.nupkg` files here so restore can find packages that are not (yet) on nuget.org.

The repo `NuGet.config` registers this folder as the `local-packages` source.

## Add / update Icu4c.Android.Fw.Lib

```powershell
Copy-Item path\to\Icu4c.Android.Fw.Lib.*.nupkg .\local-packages\
dotnet restore source\icu.net.android.tests\icu.net.android.tests.csproj -p:IcuDotNetIncludeAndroid=true
```

`.nupkg` files in this folder are gitignored.
