﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mantle.Messaging
{
    public interface ICanRenewLock
    {
        void RenewLock();
    }
}
