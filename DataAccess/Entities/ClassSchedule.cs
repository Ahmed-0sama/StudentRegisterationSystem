using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
	public class ClassSchedule
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		[ForeignKey("Class")]
		public Guid ClassId { get; set; }

		[Required, StringLength(10)]
		public string DayOfWeek { get; set; }

		public TimeSpan StartTime { get; set; }

		public TimeSpan EndTime { get; set; }

		public Class Class { get; set; }
	}
}
