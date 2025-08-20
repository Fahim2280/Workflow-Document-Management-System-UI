namespace Workflow_Document_Management_System_UI.DTOs
{
    public class AdminListResponseDto
    {
        public List<AdminResponseDto> Admins { get; set; }
        public int Count { get; set; }
        public string Message { get; set; }
    }
}
