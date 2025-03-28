using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration; 
            _eventProcessor = eventProcessor;

            InitializeRabbitMQ();
        }


        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory() {HostName = _configuration["RabbitMQHost"], Port= int.Parse(_configuration["RabbitMQPort"])};

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel(); 
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _queueName,
                                exchange:"trigger",
                                routingKey: "");

            Console.WriteLine($"INFO --> Listening on the message bus! {_queueName.ToString()}");
            
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (ModuleHandle, ea) =>
            {
                 Console.WriteLine($"Event received!!!!!!");

                 var body = ea.Body;
                 var notifMessage = Encoding.UTF8.GetString(body.ToArray());

                 _eventProcessor.ProcessEvent(notifMessage);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs eventArgs)
        {
            Console.WriteLine($"INFO --> RabbitMQ Connection Shutdown: {eventArgs.ReplyCode} - {eventArgs.ReplyText}");
        }

        private void Dispose()
        {
            Console.WriteLine($"INFO --> Message bus Disposed");

            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
            
        }

    }
}