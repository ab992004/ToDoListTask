namespace ToDoListTask.Models
{
    public class ToDoTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DeadLine { get; set; }
        public string File { get; set; } = string.Empty;
    }
}
