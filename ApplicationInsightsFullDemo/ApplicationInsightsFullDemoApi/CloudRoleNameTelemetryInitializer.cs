using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace WebApiEFSqlServer
{
    public class CloudRoleNameTelemetryInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            // set custom role name here
            telemetry.Context.Cloud.RoleName = "WebApiSqlServer";
        }
    }
}
