using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new("amqps://owqngobg:blIdj-tp5MdEGI55DGHK77IO6REX6hxx@stingray.rmq.cloudamqp.com/owqngobg");


using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "direct-exchange-example", type: ExchangeType.Direct);

// Bu exchange üzerinden herhangi bir mesajı gönderiyorsam yanni publish ediyorsam
// bu exchange'in davranısı Direct olduğundan dolayı RabbitMQ routing key 'e bakacaktır.
// Routing Key 'e karşılık gelen queue hangisi ise bu mesajı oraya gönderecektir.

while (true)
{
    Console.WriteLine("Mesaj: ");
    string message = Console.ReadLine();
    byte[] byteMessage = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchange: "direct-exchange-example", routingKey: "direct-queue-example", body: byteMessage);  
}
 
Console.Read();


// BİRDEN FAZLA KUYRUĞUN KULLANILDIĞI SENARYOLARDA İSTEDİĞİNİZ KUYRUĞA  HEDEF MESAJ GÖNDERMEK İSTİYORSANIZ DIRECT EXCHANGE DAVRANISINI KULLANABİLİRSİNİZ.