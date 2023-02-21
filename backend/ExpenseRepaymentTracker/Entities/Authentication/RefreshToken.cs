using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Entities.Authentication
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public string? Token { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}