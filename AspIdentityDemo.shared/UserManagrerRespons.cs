

namespace AspNetIdentityDemo.shared
{
    public class UserManagrerRespons
    {
        public string Message { get; set; }
        public bool IsSuccessful { get; set; }
        public IEnumerable<string> Error { get; set; }
        public DateTime?  ExpireDate { get; set; }
    }
}
