namespace Sitecore.DiagnosticsTool.Tests.ECM
{
  using System.Collections.Generic;
  using System.Linq;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.Tests.ECM.Helpers;

  public class CheckConnectionString : EcmTest
  {
    protected const string EcmDispatch = "ecm.dispatch";
    protected const string ExmDispatch = "exm.dispatch";
    protected const string ExmMaster = "exm.master";
    protected const string ExmWeb = "exm.web";
    protected const string ExmCryptoKey = "EXM.CryptographicKey";
    protected const string ExmAuthKey = "EXM.AuthenticationKey";
    public override string Name { get; } = "Presence of ecm.dispatch connection string";

    protected override bool IsEcmVersionActual(EcmVersion ecmVersion)
    {
      return base.IsEcmVersionActual(ecmVersion) && ecmVersion.MajorMinorInt >= 22;
    }

    public override void DoProcess(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));
      CheckEncryptionConnectionStrings(data, output);
      var ecmVersion = EcmHelper.GetEcmVersion(data);
      if (ecmVersion == null)
      {
        return;
      }

      if (ecmVersion.Major == 3 && ecmVersion.Minor >= 3)
      {
        CheckSqlConnectionString(data, output, ExmMaster);
        CheckSqlConnectionString(data, output, ExmWeb);
      }

      if (ecmVersion.Major == 3 && ecmVersion.Minor >= 1 && ecmVersion.Minor <= 2)
      {
        CheckSqlConnectionString(data, output, ExmDispatch);
      }

      if (ecmVersion.Major == 2 && ecmVersion.Minor == 2 || ecmVersion.Major == 3 && ecmVersion.Minor == 0)
      {
        CheckMongoConnectionString(data, output, EcmDispatch);
      }
    }

    protected void CheckMongoConnectionString(ITestResourceContext data, ITestOutputContext output, string connectionStringName)
    {
      if (!data.Databases.Mongo.DatabaseNames.Contains(connectionStringName))
      {
        output.Error(GetErrorMessage(connectionStringName));
      }
    }

    protected void CheckSqlConnectionString(ITestResourceContext data, ITestOutputContext output, string connectionStringName)
    {
      if (!data.Databases.Sql.DatabaseNames.Contains(connectionStringName))
      {
        output.Error(GetErrorMessage(connectionStringName));
      }
    }

    protected void CheckEncryptionConnectionStrings(ITestResourceContext data, ITestOutputContext output)
    {
      string cryptoKey = data.SitecoreInfo.GetConnectionString(ExmCryptoKey);
      if (cryptoKey == null)
        output.Error(GetErrorMessage(ExmCryptoKey));

      string authKey = data.SitecoreInfo.GetConnectionString(ExmAuthKey);
      if (authKey == null)
        output.Error(GetErrorMessage(ExmAuthKey));
    }

    [NotNull]
    protected string GetErrorMessage([NotNull] string connectionString)
    {
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      return $"The necessary '{connectionString}' connection string is missing. Please review the module installation guide for details.";
    }
  }
}