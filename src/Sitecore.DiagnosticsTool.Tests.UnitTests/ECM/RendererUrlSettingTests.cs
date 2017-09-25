namespace Sitecore.DiagnosticsTool.Tests.UnitTests.ECM
{
  using System.Xml;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.ECM;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;
  using Xunit;

  public class RendererUrlSettingTests : RendererUrlSetting
  {
    [Fact]
    public void TestPassed()
    {
      var sitecoreConfiguration = new SitecoreInstance
      {
        ServerRoles = new[] { ServerRole.ContentManagement },
        Configuration = new XmlDocument().Create("/configuration/sitecore/settings/setting[@name='ECM.RendererUrl' and @value='http://sitecore.net']"),
        Version = new SitecoreVersion(7, 2, 2, 140526),
        Assemblies = new[] { new AssemblyFile("Sitecore.EmailCampaign.dll", "1.3.3.4334", "1.3.3 rev. 130212") } // ExM 1.3.3 has only 1 assembly - that's why it was chosen
      };

      UnitTestContext
        .Create(this)
        .AddResource(sitecoreConfiguration)
        .Process(this)
        .Done();
    }

    [Fact]
    public void TestFailed()
    {
      var sitecoreConfiguration = new SitecoreInstance
      {
        ServerRoles = new[] { ServerRole.ContentManagement },
        Configuration = new XmlDocument().Create("/configuration/sitecore/settings"),
        Version = new SitecoreVersion(7, 2, 2, 140526),
        Assemblies = new[] { new AssemblyFile("Sitecore.EmailCampaign.dll", "1.3.3.4334", "1.3.3 rev. 130212") } // ExM 1.3.3 has only 1 assembly - that's why it was chosen
      };

      UnitTestContext
        .Create(this)
        .AddResource(sitecoreConfiguration)
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, string.Format(ErrorFormatWithMessage, KbNumber, ErrorMessage, KbName), Link))
        .Done();
    }
  }
}