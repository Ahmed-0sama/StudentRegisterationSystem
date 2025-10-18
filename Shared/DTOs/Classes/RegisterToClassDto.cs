using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Classes
{
	public class RegisterToClassDto
	{
		public Guid ClassId { get; set; }
		public Guid RegistrationPeriodId { get; set; }
	}
}
