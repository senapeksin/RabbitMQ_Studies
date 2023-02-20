using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new("amqps://owqngobg:blIdj-tp5MdEGI55DGHK77IO6REX6hxx@stingray.rmq.cloudamqp.com/owqngobg");

using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();


channel.ExchangeDeclare(exchange: "fanout-exchange-example", type: ExchangeType.Fanout);


// Fanout Exchange: Herhangi bir kuyruğu ayırt etmeyeceğimizden dolayı routingKey boş bir değer verebiliriz.. Fanout Exchange, bu exchange' e bind olmuş kuyrukların hepsine isim gözetmeksizin mesajları ileteceği için routingKey boş olmalıdır.



for (int i = 0; i < 100; i++)
{
    await Task.Delay(200);
    byte[] message = Encoding.UTF8.GetBytes($"Merhaba  {i}");
    channel.BasicPublish(exchange: "fanout-exchange-example", routingKey: string.Empty, body: message);
}

Console.Read();
