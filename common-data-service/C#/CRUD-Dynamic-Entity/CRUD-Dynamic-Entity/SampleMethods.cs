using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;

namespace PowerApps.Samples
{
    partial class SampleProgram
    {

     public static Guid CreateEntity(IOrganizationService service, Entity entity) {
            CreateRequest req = new CreateRequest() { Target = entity };
            CreateResponse resp = (CreateResponse)service.Execute(req);
            return resp.id;
        }
    }
}
