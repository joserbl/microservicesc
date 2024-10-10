using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQHost"], Port = int.Parse(_configuration["RabbitMQPort"])};

            try 
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel(); 

                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("INFO --> Connected to message bus");



            }
            catch(Exception ex)
            {
                Console.WriteLine($"EXCEPTION --> Couldn't connect to the message bus: ", ex.Message);
            }
        }

        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);

            if(_connection.IsOpen)
            {
                Console.WriteLine("INFO --> RabbitMQ Connection Open, sending message...");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("INFO --> RabbitMQ Connection Closed, not sending message...");
            }

        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "trigger", 
                                    routingKey: "",
                                    basicProperties: null,
                                    body: body);

            Console.WriteLine($"INFO --> Message sent!!!! uwu: {message}");
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

        public void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs eventArgs)
        {
            Console.WriteLine($"INFO --> RabbitMQ Connection Shutdown: {eventArgs.ReplyCode} - {eventArgs.ReplyText}");
        }

    }

}
