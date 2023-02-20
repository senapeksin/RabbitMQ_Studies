using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new("amqps://owqngobg:blIdj-tp5MdEGI55DGHK77IO6REX6hxx@stingray.rmq.cloudamqp.com/owqngobg");

using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();


channel.ExchangeDeclare(exchange: "topic-exchange-example", type: ExchangeType.Topic);

Console.Write("Dinlenecek Topic formatını belirtiniz : ");
string topicName = Console.ReadLine();

// Kuyruk oluşturduk.
string queueName = channel.QueueDeclare().QueueName; // RabbitMq 'nun random kuyruk ismini kullanmayı tercih ettik.

// Oluşturduğumuz exchange ile kuyruğu bind etmiş olduk.
channel.QueueBind(queue: queueName, exchange: "topic-exchange-example", routingKey: topicName);

EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

consumer.Received += (sender, e) =>
{
    string message = Encoding.UTF8.GetString(e.Body.Span);
    Console.WriteLine(message);
};

Console.Read();