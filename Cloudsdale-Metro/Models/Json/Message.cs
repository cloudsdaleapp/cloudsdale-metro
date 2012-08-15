using System;
using System.Collections.Generic;
using System.ComponentModel;
using Cloudsdale.Controllers.Data;
using Newtonsoft.Json;

namespace Cloudsdale.Models.Json {
    [JsonObject(MemberSerialization.OptIn)]
    public partial class Message : CloudsdaleItem, INotifyPropertyChanged, IComparable<Message> {
        public Message() {
            _subMessages = new List<Message> {
                this
            };
        }

        [JsonProperty("timestamp")]
        public DateTime? TimeStamp { get; set; }
        [JsonProperty("content")]
        public string Content;
        [JsonProperty("user")]
        public User User {
            get { return _user; }
            set { _user = UserProcessor.RegisterData(value); }
        }

        public DateTime EndTime {
            get { return _subMessages[_subMessages.Count - 1].TimeStamp ?? new DateTime(0); }
        }

        private User _user;
        [JsonProperty("topic")]
        public Topic Topic;

    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Topic : CloudsdaleItem {
        [JsonProperty("type")]
        public string Type;
    }
}
