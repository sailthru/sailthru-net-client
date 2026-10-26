#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
framework="${1:?Usage: bash scripts/test-package.sh net8.0 (after a Release build)}"
[[ "$framework" =~ ^net[0-9]+\.[0-9]+$ ]] || { echo "Invalid test framework: $framework" >&2; exit 1; }
package_version="$(dotnet msbuild "$repo_root/Sailthru/Sailthru/Sailthru.csproj" -nologo -getProperty:Version)"
package="$repo_root/Sailthru/Sailthru/bin/Release/Sailthru.Client.$package_version.nupkg"
[[ -f "$package" ]] || { echo "Build the Release package first: $package" >&2; exit 1; }

temporary="$(mktemp -d -t sailthru-package-XXXXXXXX)"
trap 'rm -rf -- "$temporary"' EXIT
mkdir "$temporary/feed" "$temporary/consumer"
cp "$package" "$temporary/feed/"
cp "$repo_root"/test/package-smoke/* "$temporary/consumer/"

# Fresh package storage and source mapping ensure this exact local artifact is used.
export NUGET_PACKAGES="$temporary/packages"
cat > "$temporary/consumer/NuGet.Config" <<'XML'
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="built-package" value="../feed" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <packageSourceMapping>
    <packageSource key="built-package"><package pattern="Sailthru.Client" /></packageSource>
    <packageSource key="nuget.org"><package pattern="*" /></packageSource>
  </packageSourceMapping>
</configuration>
XML
dotnet restore "$temporary/consumer/PackageSmoke.csproj" \
  -p:ClientTestFramework="$framework" -p:ClientPackageVersion="$package_version" \
  --configfile "$temporary/consumer/NuGet.Config"
dotnet build "$temporary/consumer/PackageSmoke.csproj" --configuration Release --no-restore \
  -p:ClientTestFramework="$framework" -p:ClientPackageVersion="$package_version"
dotnet "$temporary/consumer/bin/Release/$framework/PackageSmoke.dll" \
  "$temporary/packages/sailthru.client/$package_version/lib/netstandard2.0"
