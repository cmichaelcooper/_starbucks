using System;

namespace solutions.starbucks.Model.Pocos
{
    public class OrderNotes
    {
        public int NoteID { get; set; }
        public Guid OrderID { get; set; }
        public string AddedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public string Details { get; set; }
    }
}