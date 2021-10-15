using Consumer.Consumer;

namespace Consumer
{
    public class Message : IMessage
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
