using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Consumer.Consumer
{
    public sealed class BasicConsumer<TMessage> : IConsumer<TMessage>, IDisposable where TMessage : IMessage
    {
        private readonly ConsumerConfiguration _consumerConfiguration;
        private IConnection _connection;
        private IModel _channel;

        public BasicConsumer(ConsumerConfiguration consumerConfiguration)
        {
            _consumerConfiguration = consumerConfiguration;
            InitRabbitMQ();
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }

        private void InitRabbitMQ()
        {

            var factory = new ConnectionFactory()
            {
                HostName = _consumerConfiguration.HostName,
                UserName = _consumerConfiguration.UserName,
                Password = _consumerConfiguration.Password,
                Port = _consumerConfiguration.Port,
                VirtualHost = _consumerConfiguration.VirtualHost
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            var queueName = _consumerConfiguration.QueueName;

            _channel.QueueDeclare(queue: queueName, false, false, false, null);

            Console.WriteLine(" [*] Waiting for logs.");

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, eventArgs) =>
            {
                TMessage message;
                try
                {
                    byte[] body = eventArgs.Body.ToArray();
                    var serializedMessage = Encoding.UTF8.GetString(body);
                    message = JsonConvert.DeserializeObject<TMessage>(serializedMessage);
                    Console.WriteLine($"Message received: {serializedMessage}.");
                    //handleEvent
                    _channel.BasicAck(eventArgs.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error on handle message: {ex}.");
                    _channel.BasicNack(eventArgs.DeliveryTag, false, true);
                    throw;
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);  
        }
    }
}
