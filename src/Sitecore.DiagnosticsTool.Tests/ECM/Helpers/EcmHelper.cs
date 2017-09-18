namespace Sitecore.DiagnosticsTool.Tests.ECM.Helpers
{
  using System.Linq;
  using System.Xml;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Tests;

  internal static class EcmHelper
  {
    [NotNull]
    public static string GetAttributeValue([NotNull] XmlDocument configuration, [NotNull] string nodeXPath, [NotNull] string attributeName)
    {
      Assert.ArgumentNotNull(configuration, nameof(configuration));
      Assert.ArgumentNotNull(nodeXPath, nameof(nodeXPath));
      Assert.ArgumentNotNull(attributeName, nameof(attributeName));

      return configuration.SelectElements(nodeXPath).LastOrDefault().With(x => x.Attributes[attributeName].With(a => a.Value) ?? x.InnerText);
    }

    public static EcmVersion GetEcmVersion([NotNull] ITestResourceContext data)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      AssemblyFile assembly;
      if (!data.SitecoreInfo.Assemblies.TryGetValue("Sitecore.EmailCampaign.dll", out assembly) || assembly == null)
      {
        return null;
      }

      var productVersion = assembly.ProductVersion;
      Assert.IsNotNullOrEmpty(productVersion, "productVersion");

      return new EcmVersion(productVersion);
    }

    [NotNull]
    public static string WebsiteRootPath([NotNull] ITestResourceContext data)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      return data.WebServer.CurrentSite.WebRoot.FullName;
    }
  }
}