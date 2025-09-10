﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
	public enum BookingStatus
	{
		Reserved,
		Active,
		Cancelled,
		PickedUp,
		Returned,
		Overdue
	}
}
