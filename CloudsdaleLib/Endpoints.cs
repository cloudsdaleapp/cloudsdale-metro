namespace CloudsdaleLib {
    public static class Endpoints {
        public const string Base = "https://www.cloudsdale.org/";
        public const string AssetBase = "https://avatar-cloudsdale.netdna-ssl.com";
        public const string ApiVersion = "v1";
        public const string Api = Base + ApiVersion;
        public const string InternalToken = "$2a$10$7Pfcv89Q9c/9WMAk6ySfhu";

        public const string Session = Api + "/sessions";
        public const string User = Api + "/users/[:id]";

        public const string Cloud = Api + "/clouds/[:id]";
        public const string CloudMessages = Cloud + "/chat/messages";
        public const string CloudUsers = Cloud + "/users";
        public const string CloudOnlineUsers = CloudUsers + "/online";
        public const string CloudUserBans = Cloud + "/bans?offender_id=[:offender_id]";

        public const string Avatar = AssetBase + "/[:type]/[:id].png?s=[:size]";

        public const string PushAddress = "wss://push.cloudsdale.org/push";
    }
}
