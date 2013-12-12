using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS2TV1
{
    class SMSMessage
    {
        public string Nickname
        {
            get;
            set;
        }
        public string Sender
        {
            get;
            set;
        }
        public string Time
        {
            get;
            set;
        }
        public string Message
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Time + "\t" + Nickname + "(" + Sender + ")" + ": " + Message;
        }
    }
}
