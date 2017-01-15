using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Topshelf;
using System.Timers;

namespace TopRabbit
{
    public class Rabbit
    {
        readonly Timer _timer;

        public Rabbit()
        {
            _timer = new Timer(1000) {AutoReset = true};
            _timer.Elapsed += (sender, eventArgs) => SendMessage();
        }

        public void Start()
        {
            SendMessage();
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            ReceiveMessages();
        }


        public void SendMessage()
        {
            var factory = new ConnectionFactory() {HostName = "localhost"};
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "hello",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    string message = "hello world!";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);
                }
            }
        }

        public void ReceiveMessages()
        {
            var factory = new ConnectionFactory {HostName = "localhost"};
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("hello",
                        false,
                        false,
                        false,
                        null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                    };
                    channel.BasicConsume("hello",
                        true,
                        consumer);
                }
            }
        }
    }

    public class Program
    {
        public static void Main()
        {
            HostFactory.Run(x =>
            {
                x.Service<Rabbit>(r =>
                {
                    r.ConstructUsing(name => new Rabbit());
                    r.WhenStarted(y => y.Start());
                    r.WhenStopped(y => y.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("very simple topshelf demo");
                x.SetDisplayName("toprabbit");
                x.SetServiceName("toprabbit");
            });
        }
    }
}