using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.DiagnosticsTool.Core.Categories;
using Sitecore.DiagnosticsTool.Core.Tests;
using Sitecore.DiagnosticsTool.Tests.ECM.Helpers;

namespace Sitecore.DiagnosticsTool.Tests.ECM
{
  public abstract class EcmTest : Test
  {
    public override IEnumerable<Category> Categories { get; } = new[] { Category.Ecm };
    protected virtual bool IsEcmVersionActual(EcmVersion ecmVersion)
    {
      return ecmVersion != null;
    }
    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      if (IsEcmVersionActual(EcmHelper.GetEcmVersion(data)))
      {
        if (data.SitecoreInfo.IsAnalyticsEnabled)
        {
          DoProcess(data, output);
        }
        else
        {
          output.Error("EXM cannot run if Analytics is disabled");
        }
      }
      else
      {
        output.Debug("The test is not actual for current EXM version");
      }
    }

    public abstract void DoProcess(ITestResourceContext data, ITestOutputContext output);

  }
}
