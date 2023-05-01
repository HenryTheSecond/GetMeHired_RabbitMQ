using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer.AsyncDataService
{
    public class MessageBusSubcriber: BackgroundService
    {
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubcriber(IConfiguration configuration)
        {
            _configuration = configuration;
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQ:RabbitMQPort"])
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "get_me_hired_exchange", type: ExchangeType.Direct);
            _channel.QueueDeclare("get_me_hired_queue", false, false, false);
            _queueName = "get_me_hired_queue";
            _channel.QueueBind(queue: "get_me_hired_queue", exchange: "get_me_hired_exchange", routingKey: "jobs_search");
            Console.WriteLine("--> Listening on the Message Bus...");
        }

        public override void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
            base.Dispose();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            Console.WriteLine("1");
            consumer.Received += (ModuleHandle, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                Console.WriteLine($"--> Event Received: {message}");
            };
            _channel.BasicConsume(queue: "get_me_hired_queue", autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }
    }
}
