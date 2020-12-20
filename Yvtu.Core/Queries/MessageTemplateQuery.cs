using System;
using System.Collections.Generic;
using System.Text;
using Yvtu.Core.Entities;

namespace Yvtu.Core.Queries
{
    public class MessageTemplateQuery
    {
        public string QMessage { get; set; }

        public List<MessageTemplate> Results { get; set; }
    }
}
