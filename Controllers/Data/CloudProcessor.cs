using Cloudsdale.Models.Json;

namespace Cloudsdale.Controllers.Data {
    public sealed class CloudProcessor {
        private readonly Cloud _cloud;

        internal CloudProcessor(Cloud cloud) {
            _cloud = cloud;
            MessageProcessor = new MessageProcessor();
            UserProcessor = new UserProcessor();
            DropProcessor = new DropProcessor(cloud);
        }

        public MessageProcessor MessageProcessor { get; private set; }
        public UserProcessor UserProcessor { get; private set; }
        public DropProcessor DropProcessor { get; private set; }
    }
}
