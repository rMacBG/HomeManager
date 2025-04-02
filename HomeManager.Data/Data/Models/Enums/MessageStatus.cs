using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeManager.Data.Data.Models.Enums
{
    public enum MessageStatus
    {
        Pending = 0,
        Sent,
        Delivered,
        Seen,
        Failed
    }
}
