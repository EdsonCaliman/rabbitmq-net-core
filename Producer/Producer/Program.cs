using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace Producer
{
    class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "admin", Password = "admin", Port = 5672, VirtualHost = "test" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare("queueonetest", false, false, false, null);
                channel.QueueDeclare("queuetwotest", false, false, false, null);

                channel.ExchangeDeclare(exchange: "exchangetest", type: ExchangeType.Fanout);

                channel.QueueBind("queueonetest", "exchangetest", "");
                channel.QueueBind("queuetwotest", "exchangetest", "");

                var count = 0;
                while (true)
                {
                    var message = new Message() { Title = "Title", Description = "Description" };
                    var json = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(json);
                    channel.BasicPublish(exchange: "exchangetest",
                                         routingKey: "",
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine(" [x] Sent {0}", json);

                    Thread.Sleep(3000);
                }
            }
        }
    }
}
