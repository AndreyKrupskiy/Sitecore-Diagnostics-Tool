namespace Sitecore.DiagnosticsTool.Tests.ECM.Helpers
{
  using System;
  using System.Text.RegularExpressions;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Extensions.StringExtensions;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Extensions;

  public class EcmVersion : IReleaseVersion
  {
    public EcmVersion(string version)
    {
      Assert.ArgumentNotNull(version, nameof(version));

      if (version.StartsWith("5.0")) //temporary fix for 3.5 assembly product version
      {
        Major = 3;
        Minor = 5;
        Revision = 170810;
        Hotfix = string.Empty;
      }
      else
      {
        version = version
          .Replace("4.0.0", "3.4.0")
          .Replace("4.0.1", "3.4.1")
          .Replace("4.0.2", "3.4.2");
        
        var match = Parse(version);
        var groups = match.Groups;

        Major = int.Parse(groups[1].Value);
        Minor = int.Parse(groups[2].Value);
        Revision = groups.Count >= 7 ? int.Parse(groups[6].Value.EmptyToNull() ?? "0") : 0;
        Hotfix = ParseHotfix(groups[7].Value);
      }
    }

    public int Major { get; }

    public string MajorMinor => $"{Major}.{Minor}";

    public int MajorMinorInt => int.Parse($"{Major}{Minor}");

    public int Minor { get; }

    public int Update
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public string MajorMinorUpdate
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public int MajorMinorUpdateInt
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public int Revision { get; }

    public string Hotfix { get; }

    public string Text => ToString();

    /// <summary>
    ///   Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </summary>
    /// <returns>
    ///   A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </returns>
    public override string ToString()
    {
      return $"{Major}.{Minor} rev. {Revision}{Hotfix.EmptyToNull().With(x => " " + x)}";
    }

    public override bool Equals(object obj)
    {
      var left = this;
      var right = obj as SitecoreVersion;
      if (right != null)
      {
        return string.Equals(left.ToString(), right.ToString(), StringComparison.OrdinalIgnoreCase);
      }

      return false;
    }

    public override int GetHashCode()
    {
      return Text.GetHashCode();
    }

    [NotNull]
    protected static Match Parse([NotNull] string productVersion)
    {
      Assert.ArgumentNotNull(productVersion, nameof(productVersion));

      var regex = new Regex(@"^(\d+)\.(\d+)(\.(\d))?( rev\. (\d\d\d\d\d\d))?(\s+[hH][oO][tT][fF][iI][xX]\s+\d\d\d\d\d\d-?\d*)?$");
      var match = regex.Match(productVersion);
      if (!match.Success)
      {
        throw new FormatException($"The \"{productVersion}\" value is not valid version");
      }

      return match;
    }

    [NotNull]
    private static string ParseHotfix([CanBeNull] string value)
    {
      if (string.IsNullOrEmpty(value))
      {
        return string.Empty;
      }

      var lower = value.Trim().ToLower();
      Assert.ArgumentCondition(!lower.Contains("hotfix-"), "value", "Hotfix-##### format is not supported");

      return lower.Replace("hotfix", "Hotfix");
    }
  }
}