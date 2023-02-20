using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new("amqps://owqngobg:blIdj-tp5MdEGI55DGHK77IO6REX6hxx@stingray.rmq.cloudamqp.com/owqngobg");

using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();


channel.ExchangeDeclare(exchange: "topic-exchange-example", type: ExchangeType.Topic);


for (int i = 0; i < 100; i++)
{
    await Task.Delay(200); // her 20 ms'de bir bu işlemi gerçekleştirelim.
    byte[] message = Encoding.UTF8.GetBytes($"Merhaba  {i}");
    Console.Write("Mesajın gönderileceği Topic formatını belirtiniz : ");
    string topic = Console.ReadLine();
    channel.BasicPublish(exchange: "topic-exchange-example", routingKey: topic, body: message);
}
Console.Read();
