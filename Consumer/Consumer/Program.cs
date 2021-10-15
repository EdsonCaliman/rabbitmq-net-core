using Consumer.Consumer;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Consumer
{
    class Program
    {
        public static void Main()
        {
            var config = new ConsumerConfiguration()
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "admin",
                Port = 5672,
                VirtualHost = "test",
                ExchangeName = "exchangetest",
                QueueName = "queueonetest"
            };

            var consumer = new BasicConsumer<Message>(config);

            Console.ReadLine();
        }
    }
}
