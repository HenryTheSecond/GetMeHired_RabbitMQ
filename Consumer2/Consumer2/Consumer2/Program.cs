using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Data.Common;
using System.Text;
using System.Threading.Channels;

var factory = new ConnectionFactory()
{
    HostName = "localhost",
    Port = 8888
};

var _connection = factory.CreateConnection();
var _channel = _connection.CreateModel(); _channel.ExchangeDeclare(exchange: "get_me_hired_exchange", type: ExchangeType.Direct);
_channel.QueueDeclare("get_me_hired_queue", false, false, false);
var _queueName = "get_me_hired_queue";
_channel.QueueBind(queue: "get_me_hired_queue", exchange: "get_me_hired_exchange", routingKey: "jobs_search");
Console.WriteLine("--> Listening on the Message Bus...");

var consumer = new EventingBasicConsumer(_channel);

consumer.Received += (ModuleHandle, ea) =>
{
    var body = ea.Body;
    var message = Encoding.UTF8.GetString(body.ToArray());
    Thread.Sleep(10*1000);
    Console.WriteLine($"--> Event Received: {message}");
    Console.WriteLine($"{ea.DeliveryTag}");
    _channel.BasicAck(ea.DeliveryTag, false);
};
_channel.BasicConsume(queue: "get_me_hired_queue", autoAck: false, consumer: consumer);

Console.ReadKey();