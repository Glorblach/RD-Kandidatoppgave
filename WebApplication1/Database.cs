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
	}
}
