using System.Collections.Generic;
using System.Text;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace WebApplication1
{
	public interface IDatabase
	{
		void CreateTablesAndTestData(IDbConnectionFactory dbFactory);

		List<string> ExampleData(IDbConnectionFactory dbFactory);

		List<string> RelationsData(IDbConnectionFactory dbFactory);

		List<string> SpecificRelationsData(IDbConnectionFactory dbFactory, long personID);

		List<string> ContactData(IDbConnectionFactory dbFactory, long personID);
	}

	public class Database : IDatabase
	{
		public void CreateTablesAndTestData(IDbConnectionFactory dbFactory)
		{
			using (var db = dbFactory.Open())
			{
				db.ExecuteNonQuery("CREATE TABLE People (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT NOT NULL);");
				db.ExecuteNonQuery("CREATE TABLE Enterprises(id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT NOT NULL);");
				db.ExecuteNonQuery("INSERT INTO People (name) VALUES ('Willifred Manford');");
				db.ExecuteNonQuery("INSERT INTO People (name) VALUES ('Ola Nordmann');");
				db.ExecuteNonQuery("INSERT INTO People (name) VALUES ('Kari Nordkvinne');");
				db.ExecuteNonQuery("INSERT INTO People (name) VALUES ('Kjell Fjellblåst');");
				db.ExecuteNonQuery("INSERT INTO People (name) VALUES ('Oskar Hvinesen');");
				db.ExecuteNonQuery("INSERT INTO People (name) VALUES ('Petter Putling');");
				db.ExecuteNonQuery("INSERT INTO People (name) VALUES ('Siri Sorrysen');");
				db.ExecuteNonQuery("INSERT INTO Enterprises (name) VALUES ('ACME Inc');");
				db.ExecuteNonQuery("INSERT INTO Enterprises (name) VALUES ('Generic Enterprises');");
				db.ExecuteNonQuery("INSERT INTO Enterprises (name) VALUES ('BnL');");

				// Add relations, hardcoded ID's (cardinal sin)
				db.DropAndCreateTable<PersonEnterprise>();
				db.Insert(
					new PersonEnterprise { id = 1, PersonId = 1, EnterpriseID = 1 },
					new PersonEnterprise { id = 2, PersonId = 2, EnterpriseID = 2 },
					new PersonEnterprise { id = 3, PersonId = 3, EnterpriseID = 3 }
					);

				// Add contacts, hardcoded ID's (still a sin)
				db.DropAndCreateTable<Contact>();
				db.Insert(
					new Contact { id = 1, PersonId = 1, ContactId = 2},

					new Contact { id = 2, PersonId = 2, ContactId = 3 },
					new Contact { id = 3, PersonId = 2, ContactId = 4 },

					new Contact { id = 4, PersonId = 3, ContactId = 5 },
					new Contact { id = 5, PersonId = 3, ContactId = 6 },
					new Contact { id = 6, PersonId = 3, ContactId = 7 },

					new Contact { id = 7, PersonId = 7, ContactId = 1 },
					new Contact { id = 8, PersonId = 7, ContactId = 2 },
					new Contact { id = 9, PersonId = 7, ContactId = 3 },
					new Contact { id = 10, PersonId = 7, ContactId = 4 },
					new Contact { id = 11, PersonId = 7, ContactId = 5 },
					new Contact { id = 12, PersonId = 7, ContactId = 6 }

					);
			}
		}

		public List<string> ExampleData(IDbConnectionFactory dbFactory)
		{
			using (var db = dbFactory.Open())
			{
				//return db.Select<string>("SELECT name FROM people");

				// Testing queries
				var query = db.From<People>();
				var result = db.Select(query);
				List<string> ret = new List<string>();
				foreach (var p in result)
				{
					ret.Add(p.ToString());
				}

				return ret;
			}
		}

		public List<string> RelationsData(IDbConnectionFactory dbFactory)
		{
			using (var db = dbFactory.Open())
			{

				var query = db.SelectMulti<PersonEnterprise, People, Enterprises>(db.From<PersonEnterprise>()
					.Join<People>((pe, p) => pe.PersonId == p.id)
					.Join<Enterprises>((pe, ent) => pe.EnterpriseID == ent.id)
					);

				List<string> ret = new List<string>();
				foreach (var ep in query)
				{
					ret.Add($"{ep.Item2.name} - {ep.Item3.name}");
				}
				return ret;
			}
		}

		public List<string> SpecificRelationsData(IDbConnectionFactory dbFactory, long personID)
		{
			using (var db = dbFactory.Open())
			{
				var query = db.SelectMulti<PersonEnterprise, People, Enterprises>(db.From<PersonEnterprise>()
					.Join<People>((pe, p) => pe.PersonId == p.id)
					.Join<Enterprises>((pe, ent) => pe.EnterpriseID == ent.id)
					.Where<People>(p => p.id == personID)
					);

				List<string> ret = new List<string>();
				foreach (var ep in query)
				{
					ret.Add($"{ep.Item2.name} - {ep.Item3.name}");
				}
				return ret;
			}
		}


		public List<string> ContactData(IDbConnectionFactory dbFactory, long personID)
		{
			using (var db = dbFactory.Open())
			{
				// get person we are looking at
				var q = db.From<People>().Where(p => p.id == personID).Take(1);
				var mainPerson = db.Select(q);
				var query = db.SelectMulti<Contact, People>(db.From<Contact>()
					.Join<People>((cont, p) => cont.ContactId == p.id)
					.Where<Contact>(c => c.PersonId == personID)
					);

				List<string> ret = new List<string>();

				// make sure we found the person we are looking at, and only one
				if(mainPerson.Count == 1)
				{
					ret.Add($"Contacts for '{mainPerson[0].name}'");
					int i = 1;
					foreach (var ep in query)
					{
						ret.Add($"Contact {i} - {ep.Item2.name}");
						i++;
					}

				}
				return ret;
			}
		}

	}
}
