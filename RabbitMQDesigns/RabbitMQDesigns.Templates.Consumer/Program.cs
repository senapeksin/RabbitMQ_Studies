using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new("amqps://owqngobg:blIdj-tp5MdEGI55DGHK77IO6REX6hxx@stingray.rmq.cloudamqp.com/owqngobg");


using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

#region P2P(Point-to-Point) Tasarımı

//string queueNmae = "example-p2p-queue";
//channel.QueueDeclare(queue: queueNmae, durable: false, exclusive: false, autoDelete: false);

//EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
//channel.BasicConsume(queue:queueNmae, autoAck:false, consumer:consumer);

//consumer.Received += (sender, e) => { Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span)); };

#endregion

#region Publish/Subscribe Tasarımı


//string exchangeName = "example-pub-sub-exchange";
//channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

//string queueName = channel.QueueDeclare().QueueName;
//channel.QueueBind(queue:queueName, exchange:exchangeName,routingKey:string.Empty);

//EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
//channel.BasicConsume(queue:queueName,autoAck:false,consumer:consumer); 

//consumer.Received += (sender, e) => {
//    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
//};

#endregion

#region Work Queue(İş kuyruğu) Tasarımı

//string queueNmae = "example-work-queue";
//channel.QueueDeclare(queue: queueNmae, durable: false, exclusive: false, autoDelete: false);


//EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
//channel.BasicConsume(queue: queueNmae, autoAck: true, consumer: consumer);

//// BasiQos fonksiyonu ile ölçeklendirme çalışması yapıyoruz.
//// Tüm consumerların sadece 1 tane mesajı işleyeceklerini ve toplam mesaj boyutu olarakta sınırsız mesaj alabileceğinin ayarlarını yapıyoruz.

//channel.BasicQos(prefetchCount: 1, prefetchSize: 0, global: false);

//consumer.Received += (sender, e) =>
//{
//    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
//};

#endregion

#region Request/Response Tasarımı


string requestQueueName = "example-reques-response-queue";
channel.QueueDeclare(queue: requestQueueName, durable: false, exclusive: false, autoDelete: false);


EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
channel.BasicConsume(queue: requestQueueName, autoAck: true, consumer: consumer);


consumer.Received += (sender, e) =>
{
    string message = Encoding.UTF8.GetString(e.Body.Span);
    Console.WriteLine(message);

    byte[] responseMessage = Encoding.UTF8.GetBytes($"İşlem tamamnlandı. :   { message }");
    IBasicProperties properties = channel.CreateBasicProperties();
    properties.CorrelationId = e.BasicProperties.CorrelationId;

    channel.BasicPublish(exchange:string.Empty, routingKey:e.BasicProperties.ReplyTo,basicProperties:properties,body:responseMessage);
};


#endregion