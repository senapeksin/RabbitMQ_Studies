using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new("amqps://owqngobg:blIdj-tp5MdEGI55DGHK77IO6REX6hxx@stingray.rmq.cloudamqp.com/owqngobg");


using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

// 1. Adım : Publisher'da ki exchane ile birebir aynı isim ve type'a sahip bir exchange tanımlanmalıdır.

channel.ExchangeDeclare(exchange: "direct-exchange-example", type: ExchangeType.Direct);

// 2.Adım : Publisher tarafından routing key'de bulunan değerdeki kuyruğa gönderilen mesajları kendi oluşturduğumuz kuyruğa yönkendirerek tüketmemiz gerekmektedir.
// Bunun için öncelikle bir kuyruk oluşturulmalıdır. 

// Öncelikle bir kuyruk oluşturmamız lazım ve routing Key'de belirtilmiş olan kuyruk ismine karşılık gelen mesajların bizim oluşturacağımız bu kuyruğa yönlendirmemiz gerekecektir.
// Ancak o zaman consumer ilgili mesajları tüketecektir.

string queueName = channel.QueueDeclare().QueueName;  // Boş bırakıldığında RabbitMQ random bir isim verir. İsterseniz kendinizde kuyruk ismi girebilirsiniz.

// Şimdi oluşturduğumuz bu queue'ya publisher uygulaması tarafından routing key' e gönderilen mesajları yönlendireceğim.
// Bunu binding mekanizmasını kullanarak yapacağoz.

// 3. Adım : 

channel.QueueBind(queue: queueName, exchange: "direct-exchange-example", routingKey: "direct-queue-example");

EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);


consumer.Received += (sender, args) =>
{
    string message = Encoding.UTF8.GetString(args.Body.Span);
    Console.WriteLine(message);
};


Console.Read();