namespace CloudsdaleLib {
    /// <summary>
    /// All the endpoints you'll ever need for v1
    /// </summary>
    public static class Endpoints {
        /// <summary>
        /// The base URI for all cloudsdale endpoints
        /// </summary>
        public const string Base = "https://www.cloudsdale.org/";
        /// <summary>
        /// The base URI for asset requests
        /// </summary>
        public const string AssetBase = "https://avatar-cloudsdale.netdna-ssl.com";
        /// <summary>
        /// Version of the API used in API requests
        /// </summary>
        public const string ApiVersion = "v1";
        /// <summary>
        /// Fully qualified URI for API requests
        /// </summary>
        public const string Api = Base + ApiVersion;
        /// <summary>
        /// Internal token for v1 oAuth provider requests
        /// </summary>
        public const string InternalToken = "$2a$10$7Pfcv89Q9c/9WMAk6ySfhu";

        /// <summary>
        /// Endpoint to retrieve sessions
        /// </summary>
        public const string Session = Api + "/sessions";
        /// <summary>
        /// Endpoint to retrieve user objects
        /// </summary>
        public const string User = Api + "/users/[:id]";

        /// <summary>
        /// Endpoint to retrieve cloud objects
        /// </summary>
        public const string Cloud = Api + "/clouds/[:id]";
        /// <summary>
        /// Endpoint to retrieve the recent messages sent in a cloud
        /// </summary>
        public const string CloudMessages = Cloud + "/chat/messages";
        /// <summary>
        /// Endpoint to the list of users in a cloud, along with their statuses
        /// </summary>
        public const string CloudUsers = Cloud + "/users";
        /// <summary>
        /// A list of users in a cloud who are not offline
        /// </summary>
        public const string CloudOnlineUsers = CloudUsers + "/online";
        /// <summary>
        /// A list of active bans for a given potential offender in a cloud
        /// </summary>
        public const string CloudUserBans = Cloud + "/bans?offender_id=[:offender_id]";

        /// <summary>
        /// The asset for an avatar of a given ID of a resource Type at the given Size.
        /// Use the following replacements: [:type], [:id], [:size]
        /// </summary>
        public const string Avatar = AssetBase + "/[:type]/[:id].png?s=[:size]";

        /// <summary>
        /// The websocket address to connect to the Push server at
        /// </summary>
        public const string PushAddress = "wss://push.cloudsdale.org/push";
    }
}
