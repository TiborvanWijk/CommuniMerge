﻿using CommuniMerge.Library.Enums;

namespace CommuniMerge.Library.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public DateTime TimeStamp { get; set; }
        public User SenderUser { get; set; }
        public string SenderId { get; set; }
        public string? ReceiverId { get; set; }
        public User? Receiver { get; set; }
        public int? GroupId { get; set; }
        public string? FilePath { get; set; }
        public FileType? FileType { get; set; }
        public Group? Group { get; set; }
    }
}
