namespace KanBan.Model
{
    public class KanbanDetail
    {
        public int KanbanDetailsID { get; set; }
        public int KanbanID { get; set; }
        public int UsersID { get; set; }
        public string? Comments { get; set; }
    }
}
