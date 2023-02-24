using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new("amqps://owqngobg:blIdj-tp5MdEGI55DGHK77IO6REX6hxx@stingray.rmq.cloudamqp.com/owqngobg");

using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();


channel.ExchangeDeclare(exchange: "header-exchange-example", type: ExchangeType.Headers);


for (int i = 0; i < 100; i++)
{
    await Task.Delay(200);
    byte[] message = Encoding.UTF8.GetBytes($"Merhaba {i}");
    Console.WriteLine("Lütfen Header value'sunnu giriniz.");
    string value = Console.ReadLine();

    IBasicProperties basicProperties = channel.CreateBasicProperties();
    basicProperties.Headers = new Dictionary <string,object>
    {
        ["no"] = value
    };

    channel.BasicPublish(exchange: "header-exchange-example", routingKey: string.Empty, body: message, basicProperties: basicProperties);

} 

Console.Read();