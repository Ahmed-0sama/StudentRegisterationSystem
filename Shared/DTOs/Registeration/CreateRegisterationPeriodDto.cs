using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Registeration
{
	public class CreateRegisterationPeriodDto
	{
		[Required(ErrorMessage = "Period name is required")]
		[StringLength(100)]
		public string SemesterName { get; set; }
		[Required(ErrorMessage = "Start date is required")]
		public DateTime StartDate { get; set; }
		[Required(ErrorMessage = "End date is required")]
		public DateTime EndDate { get; set; }
	}
}
