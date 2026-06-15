using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoList.Domain.Entites
{
    public class TaskApp : Entity
    {
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime ExpiredData { get; set; }
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
