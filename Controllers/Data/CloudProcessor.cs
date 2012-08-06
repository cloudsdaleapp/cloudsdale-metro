namespace Cloudsdale.Controllers.Data {
    public class CloudProcessor {
        internal CloudProcessor() {
            MessageProcessor = new MessageProcessor();
            UserProcessor = new UserProcessor();
        }

        public MessageProcessor MessageProcessor { get; private set; }
        public UserProcessor UserProcessor { get; private set; }
    }
}
