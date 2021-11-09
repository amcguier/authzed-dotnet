using Authzed.Api.V0;
using Grpc.Net.Client;

namespace AuthZedClient.V0
{
    public class Context
    {
        private readonly string _address;

        public Context(string address)
        {
            _address = address;
        }

        private GrpcChannel GetChannel()
        {
            return GrpcChannel.ForAddress((_address));
        }

        public ACLService.ACLServiceClient GetAclClient()
        {
            return new ACLService.ACLServiceClient(GetChannel());
        }

        public DeveloperService.DeveloperServiceClient GetDeveloperClient()
        {
            return new DeveloperService.DeveloperServiceClient(GetChannel());
        }

        public NamespaceService.NamespaceServiceClient GetNamespaceClient()
        {
            return new NamespaceService.NamespaceServiceClient(GetChannel());
        }

        public WatchService.WatchServiceClient GetWatchClient()
        {
            return new WatchService.WatchServiceClient(GetChannel());
        }

    }
}