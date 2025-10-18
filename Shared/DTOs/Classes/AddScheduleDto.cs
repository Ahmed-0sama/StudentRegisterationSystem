using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Classes
{
	public class AddScheduleDto
	{
		public string DayOfWeek { get; set; }      
		public TimeSpan StartTime { get; set; }    
		public TimeSpan EndTime { get; set; }     
	}
}
