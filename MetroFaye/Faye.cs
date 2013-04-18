using System;

namespace MetroFaye {
    public static class Faye {
        public static MessageHandler CreateClient(Uri address) {
            var handler = new WebsocketHandler { Address = address };
            return handler;
        }
    }
}
