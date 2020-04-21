using System.Collections.Generic;
using ServiceStack;
using ServiceStack.Data;

namespace WebApplication1
{
	public class Endpoint : Service
	{
		private readonly IDatabase _database;
		private readonly IDbConnectionFactory _dbFactory;

		public Endpoint(IDatabase database, IDbConnectionFactory dbFactory)
		{
			_database = database;
			_dbFactory = dbFactory;
		}

		//Example endpoint
		public List<string> Get(DocumentRequest request)
		{
			var result = _database.ExampleData(_dbFactory);
			return result;
		}

		// Relation endpoint
		// If we have multi-route for the same class, it seems we need to check on an invalid ID
		// so create a new request for specific relation
		public List<string> Get(RelationRequest request)
		{
			var result = _database.RelationsData(_dbFactory);
			return result;
		}

		public List<string> Get(SpecificRelationRequest request)
		{
			var result = _database.SpecificRelationsData(_dbFactory, request.PersonID);
			return result;
		}

		public List<string> Get(ContactRequest request)
		{
			var result = _database.ContactData(_dbFactory, request.PersonID);
			return result;
		}
	}
}
