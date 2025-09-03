using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
	public class RefreshToken
	{
		[Key]
		public int Id { get; set; }
		public string Token { get; set; } = string.Empty;
		public string UserId { get; set; } = string.Empty;
		public DateTime ExpiryDate { get; set; }
		public bool IsUsed { get; set; }
		public bool IsRevoked { get; set; }
	}
}
