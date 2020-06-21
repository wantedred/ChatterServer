namespace WebChatterServer.Models.Database.BindingModels
{
    public class UserStatus
    {
        public int Id { get; set; }
        
        public string ConnectionId { get; set; }
        
        public string Username { get; set; }
        
        public bool Status { get; set; }
    }
}