using PostSharp.Patterns.Model;

namespace PostSharpNotifyChangedProject
{
    [NotifyPropertyChanged]
    public class User
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Mail { get; set; }
    }
}
