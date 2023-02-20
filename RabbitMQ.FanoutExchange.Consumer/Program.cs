using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new("amqps://owqngobg:blIdj-tp5MdEGI55DGHK77IO6REX6hxx@stingray.rmq.cloudamqp.com/owqngobg");

using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();


// Fanout Exchange'e bind olmuş kuyrukları oluşturacağız.

// İlk olarak exchange'i yine tanımlamamız lazım. Publisher'da tanımladığımız gibi birebir aynı olması lazım.

channel.ExchangeDeclare(exchange: "fanout-exchange-example", type: ExchangeType.Fanout);

// Kuyruklarımızı belirlememiz lazım. Ve bu exchange' e bind etmemiz gerekecek.
Console.Write("Kuyruk adını giriniz : ");
string queueName = Console.ReadLine();

// Kuyruk oluşturduk.
channel.QueueDeclare(queue: queueName, exclusive: false);

// Oluşturduğumuz exchange ile kuyruğu bind etmiş olduk.
channel.QueueBind(queue: queueName, exchange: "fanout-exchange-example", routingKey: String.Empty);

EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

consumer.Received += (sender, e) =>
{
    string message = Encoding.UTF8.GetString(e.Body.Span);
    Console.WriteLine(message);
};

Console.Read();