namespace Workflow_Document_Management_System_UI.DTOs
{
    public class AdminResponseDto
    {
        public int AdminId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string AccessLevel { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
