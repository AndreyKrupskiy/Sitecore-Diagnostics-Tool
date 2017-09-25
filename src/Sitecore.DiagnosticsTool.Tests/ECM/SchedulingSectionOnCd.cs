namespace Sitecore.DiagnosticsTool.Tests.ECM
{
  using System.Collections.Generic;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public class SchedulingSectionOnCd : EcmTest
  {
    public override IEnumerable<Category> Categories { get; } = new[] { Category.Ecm };

    public override IEnumerable<ServerRole> ServerRoles => new[] { ServerRole.ContentDelivery };

    public override string Name { get; } = "ECM scheduling section must be disabled on CD";

    [NotNull]
    protected string ErrorMessage => "ECM agents found in scheduling section. These agents should be disabled on CD.";

    public override void DoProcess(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      if (
        data.SitecoreInfo.Configuration.SelectSingleNode("/configuration/sitecore/scheduling/agent[@hint='ECM']") != null ||
        data.SitecoreInfo.Configuration.SelectSingleNode("/configuration/sitecore/scheduling/agent[@hint='EXM tasks']") != null ||
        data.SitecoreInfo.Configuration.SelectSingleNode("/configuration/sitecore/scheduling/agent[@hint='EXM instance task']") != null)
      {
        output.Error(ErrorMessage);
      }
    }
  }
}