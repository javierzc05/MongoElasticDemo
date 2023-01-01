using Microsoft.AspNetCore.Identity;


namespace MongoDBExample.DataAccess
{
    public class MongoUserIdentity : IdentityUser
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public int Age { get; set; }
    }
}