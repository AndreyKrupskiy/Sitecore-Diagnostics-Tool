namespace Sitecore.DiagnosticsTool.Tests.ECM
{
  using Sitecore.DiagnosticsTool.Tests.ECM.Helpers;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public class ExmSqlScriptTest : EcmTest
  {
    [NotNull]
    protected readonly string[] ProcedureNames =
    {
      "Add_VisitsByMessage",
      "Add_AutomationStatesStatisticsByMessage",
      "Add_AutomationStatesStatisticsByAbnMessage",
      "Add_LandingPages",
      "Add_AbnVisitsByMessage",
      "Ensure_LandingPageDetails"
    };

    [NotNull]
    protected readonly string[] TableNames =
    {
      "LandingPageDetails",
      "Fact_LandingPages",
      "Fact_VisitsByMessage",
      "Fact_AbnVisitsByMessage",
      "Fact_AutomationStatesStatisticsByMessage",
      "Fact_AutomationStatesStatisticsByAbnMessage"
    };

    public override string Name { get; } = "Check if EXM SQL script was run against reporting database";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Ecm };

    protected override bool IsActual(IReadOnlyCollection<ServerRole> roles)
    {
      // this test is valid only for authoring or reporting instances
      return roles.Contains(ServerRole.ContentManagement) || roles.Contains(ServerRole.Reporting);
    }

    protected override bool IsEcmVersionActual(EcmVersion ecmVersion)
    {
      return base.IsEcmVersionActual(ecmVersion) && ecmVersion.MajorMinorInt >= 30 && ecmVersion.MajorMinorInt <= 32;
    }

    public override void DoProcess(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var name = "reporting";
      var reporting = data.Databases.Sql[name];
      if (reporting == null)
      {
        var message = $"Cannot check the Reporting database. The {name} connection string is not presented in the ConnectionStrings.config file";
        output.Warning(message);
        return;
      }

      var schema = reporting.Schema;

      var sb = new StringBuilder();

      // check tables
      foreach (var tableName in TableNames)
      {
        if (!schema.Tables.ContainsKey(tableName))
        {
          sb.AppendFormat("\r\n- {0}.Tables.dbo.{1}", name, tableName);
        }
      }

      foreach (var procedureName in ProcedureNames)
      {
        if (!schema.StoredProcedures.ContainsKey(procedureName))
        {
          sb.AppendFormat("\r\n- {0}.Programmability.Stored Procedures.dbo.{1}", name, procedureName);
        }
      }

      if (sb.Length > 0)
      {
        var message = $"One or several objects are missing in the reporting database. This may happen if EXM SQL script was not run or ended with error. Please refer to EXM installation guide for more details.:{sb}";
        output.Error(message);
      }
    }
  }
}