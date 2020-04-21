using ServiceStack;

namespace WebApplication1
{
	[Route("/contact/{PersonID}", "GET")]
	public class ContactRequest
	{
		public long PersonID { get; set; }
	}
}
