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

      version = version
        .Replace("4.0.0", "3.4.0")
        .Replace("4.0.1", "3.4.1")
        .Replace("4.0.2", "3.4.2")
        .Replace("5.0.0", "3.5.0"); //product version hack for EXM 3.4 and newer

      var match = Parse(version);
      var groups = match.Groups;

      Major = int.Parse(groups[1].Value);
      Minor = int.Parse(groups[2].Value);
      Update = string.IsNullOrEmpty(groups[4].Value) ? 0 : int.Parse(groups[4].Value);
    }

    public int Major { get; }

    public string MajorMinor => $"{Major}.{Minor}";

    public int MajorMinorInt => int.Parse($"{Major}{Minor}");

    public int Minor { get; }

    public int Update { get; }

    public string MajorMinorUpdate => $"{Major}.{Minor}.{Update}";

    public int MajorMinorUpdateInt=>int.Parse($"{Major}{Minor}{Update}");

    public int Revision
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public string Hotfix
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public string Text => ToString();

    /// <summary>
    ///   Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </summary>
    /// <returns>
    ///   A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </returns>
    public override string ToString()
    {
      return MajorMinorUpdate;
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
  }
}