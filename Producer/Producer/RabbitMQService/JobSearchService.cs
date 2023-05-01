using System.Text;
using System.Text.Json;
using Producer.Dto;
using RabbitMQ.Client;

namespace Producer.RabbitMQService
{
    public class JobSearchService: IJobSearchService
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public JobSearchService(IConfiguration configuration)
        {
            _configuration = configuration;
            Console.WriteLine($"{_configuration["RabbitMQ:RabbitMQHost"]} {_configuration["RabbitMQ:RabbitMQPort"]}");
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQ:RabbitMQPort"])
            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: "get_me_hired_exchange", type: ExchangeType.Direct);

                Console.WriteLine("--> Connected to MessageBus");
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {e.Message}");
            }
        }
        public void PublishMessage(JobQueryRequest request)
        {
            var message = JsonSerializer.Serialize(request);
            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ Connection Open, sending message...");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ connections closed, not sending");
            }
        }

        public void Dispose()
        {
            Console.WriteLine("MessageBus Disposed");
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "get_me_hired_exchange", routingKey: "jobs_search", basicProperties: null, body: body);
            Console.WriteLine($"--> We have sent {message}");
        }
    }
}
