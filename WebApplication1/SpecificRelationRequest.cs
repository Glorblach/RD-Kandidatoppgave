using ServiceStack;

namespace WebApplication1
{
	[Route("/relation/{PersonID}", "GET")]
	public class SpecificRelationRequest
	{
		public long PersonID { get; set; }
	}
}
