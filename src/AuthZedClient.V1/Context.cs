using Grpc.Net.Client;
using Authzed.Api.V1;

namespace AuthZedClient.V1 {
    public class Context {
        private readonly string _address;

        public Context(string address, string token){
            this._address = address;
        }

        private GrpcChannel GetChannel()
        {
           return GrpcChannel.ForAddress(_address);
        }

        public PermissionsService.PermissionsServiceClient GetPermissionClient()
        {
            return new PermissionsService.PermissionsServiceClient(GetChannel());
        }

        public SchemaService.SchemaServiceClient GetSchemaClient()
        {
            return new SchemaService.SchemaServiceClient(GetChannel());
        }

        public WatchService.WatchServiceClient GetWatchClient()
        {
            return new WatchService.WatchServiceClient(GetChannel());
        }

        public Authzed.Api.V1Alpha1.SchemaService.SchemaServiceClient GetAlphaSchemaClient()
        {
            return new Authzed.Api.V1Alpha1.SchemaService.SchemaServiceClient(GetChannel());
        }



    }


}