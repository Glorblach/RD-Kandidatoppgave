using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
	public class People
	{
		public long id { get; set; }
		public string name { get; set; }

		public override string ToString() => $"{id} - {name}";
	}

	public class Enterprises
	{
		public long id { get; set; }
		public string name { get; set; }
		public override string ToString() => $"{id} - {name}";
	}

	public class PersonEnterprise
	{
		public long id { get; set; }

		public long PersonId { get; set; } // PersonFK
		public long EnterpriseID { get; set; } // EnterpriseFK

		public override string ToString() => $"{id} - PersonID '{PersonId}' - EnterpriseID '{EnterpriseID}'";

	}

	public class Contact
	{
		public long id { get; set; }
		public long PersonId { get; set; }
		public long ContactId { get; set; }
	}

}
