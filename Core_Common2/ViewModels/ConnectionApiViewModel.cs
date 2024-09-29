namespace Core_Common2.ViewModels
{
    public class ConnectionApiViewModel
    {
        public int Id { get; set; }

        public string name { get; set; }

        public ConnectionApiObj ConnectionApiObj { get; set; }
    }

    public class ConnectionApiObj
    {
        public DateTime DateTime { get; set; }
        public bool boo { get; set; }
    }
}
