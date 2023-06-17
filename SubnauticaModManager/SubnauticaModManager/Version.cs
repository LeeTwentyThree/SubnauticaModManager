using Semver;
using System;

namespace SubnauticaModManager;

public sealed class Version :
    IComparable, IComparable<Version>, IComparable<SemVersion>, IComparable<System.Version>,
    IEquatable<Version>, IEquatable<SemVersion>, IEquatable<System.Version>
{
    /// <summary>
    /// A reserved prerelease tag used for converting System.Version format versions to Semver format versions.
    /// </summary>
    private const string reservedPrereleaseTag = "Subnautica-Mod-Manager-f052bcef-bb99-4b03-81f3-bbce37f72f6a";
    private const SemVersionStyles semVersionStyles = SemVersionStyles.AllowWhitespace | SemVersionStyles.AllowV | SemVersionStyles.OptionalMinorPatch;

    public System.Version AssemblyVersion;
    public SemVersion Semver;

    public static Version Zero => new("0.0.0");

    public Version(string version)
    {
        if (System.Version.TryParse(version, out AssemblyVersion))
        {   // if version is in System.Version format, convert to Semver format for use when performing version comparisons
            SemVersion.TryParse($"{AssemblyVersion.Major}.{AssemblyVersion.Minor}.{Math.Max(AssemblyVersion.Build, 0)}-{reservedPrereleaseTag}.{Math.Max(AssemblyVersion.Revision, 0)}", semVersionStyles, out Semver);
        }
        else if (!SemVersion.TryParse(version, semVersionStyles, out Semver))
        {   // otherwise, simply parse as Semver format and throw if failed
            throw new ArgumentException($"Invalid version string: {version}");
        }
    }

    public Version(System.Version version) : this(version.ToString()) { }

    public static bool TryParse(string version, out Version result)
    {
        try
        {
            result = new Version(version);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    public int CompareTo(SemVersion semver) => (Semver.Prerelease.StartsWith(reservedPrereleaseTag), semver.Prerelease.StartsWith(reservedPrereleaseTag)) switch
    {
        (true, false) => Semver.WithoutPrerelease().ComparePrecedenceTo(semver),
        (false, true) => Semver.ComparePrecedenceTo(semver.WithoutPrerelease()),
        _ => Semver.ComparePrecedenceTo(semver)
    };
    public int CompareTo(Version other) => CompareTo(other.Semver);
    public int CompareTo(System.Version assemblyVersion) => CompareTo(new Version(assemblyVersion.ToString()).Semver);
    public int CompareTo(object obj) => obj switch
    {
        Version v => CompareTo(v),
        SemVersion semver => CompareTo(semver),
        System.Version assemblyVersion => CompareTo(assemblyVersion),
        _ => throw new ArgumentException($"Cannot compare {GetType().Name} to {obj.GetType().Name}")
    };

    public bool Equals(SemVersion semver) => (Semver.Prerelease.StartsWith(reservedPrereleaseTag), semver.Prerelease.StartsWith(reservedPrereleaseTag)) switch
    {
        (true, false) => Semver.WithoutPrerelease().PrecedenceEquals(semver),
        (false, true) => Semver.PrecedenceEquals(semver.WithoutPrerelease()),
        _ => Semver.PrecedenceEquals(semver)
    };
    public bool Equals(Version other) => Equals(other.Semver);
    public bool Equals(System.Version assemblyVersion) => Equals(new Version(assemblyVersion.ToString()).Semver);
    public override bool Equals(object obj) => obj switch
    {
        _ when ReferenceEquals(this, obj) => true,
        null => false,
        Version v => Equals(v),
        SemVersion semver => Equals(semver),
        System.Version assemblyVersion => Equals(assemblyVersion),
        _ => false,
    };

    public override int GetHashCode() => Semver.GetHashCode();

    public override string ToString() => AssemblyVersion switch
    {
        System.Version v => v.ToString(),
        _ => Semver.ToString()
    };

    public static bool operator ==(Version left, Version right) => left.Equals(right);
    public static bool operator !=(Version left, Version right) => !left.Equals(right);
    public static bool operator >(Version left, Version right) => left.CompareTo(right) > 0;
    public static bool operator >=(Version left, Version right) => left.CompareTo(right) >= 0;
    public static bool operator <(Version left, Version right) => left.CompareTo(right) < 0;
    public static bool operator <=(Version left, Version right) => left.CompareTo(right) <= 0;
}