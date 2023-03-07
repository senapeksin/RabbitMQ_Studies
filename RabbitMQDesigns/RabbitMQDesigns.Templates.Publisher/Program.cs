using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new("amqps://owqngobg:blIdj-tp5MdEGI55DGHK77IO6REX6hxx@stingray.rmq.cloudamqp.com/owqngobg");


using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

#region P2P(Point-to-Point) Tasarımı

/*
  Bu tasarımda, bir publisher ilgili mesajı direkt bir kuyruğa gönderir ve bu mesaj kuyruğu işleyen bir consumer tarafından tüketilir.
  P2P tasarımında genellikle Direct exchange kullanılır.
 */

//string queueNmae = "example-p2p-queue";
//channel.QueueDeclare(queue:queueNmae, durable:false, exclusive:false, autoDelete:false);
//byte[] messsage = Encoding.UTF8.GetBytes("merhaba");  
//channel.BasicPublish(exchange:string.Empty, routingKey:queueNmae,body:messsage);

#endregion

#region Publish/Subscribe Tasarımı

/*
 Bu tasarımda publisher mesajı bir exchange' e gönderir ve böylece mesaj bu exchange'e bind edilmiş olan tüm kuyruklara yönlendirilir. Bu tasarım, bir mesajın birçok tüketici tarafından işlenmesi gerektiği durumlarda kullanışlıdır. 

Genellikle Fanout exchange kullanılır.
 */

//string exchangeName = "example-pub-sub-exchange";
//channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

//for (int i = 0; i < 100; i++)
//{
// await Task.Delay(200);
//    byte[] messsage = Encoding.UTF8.GetBytes("merhaba" + i);
//    channel.BasicPublish(exchange: exchangeName, routingKey: string.Empty, body: messsage); 
//}

#endregion

#region Work Queue(İş kuyruğu) Tasarımı
/*
 Bu tasarımda, publisher tarafından yayımlanmıs bir mesajın birden fazla consumer araasından yalnızca biri tarafından tüketilmesi amaçlanmaktadır. Böylece msajların işlenmesi sürecinde tüm consumer'lar aynı iş yüküne ve eşit görev dağılımına sahip olacaktırlar.
 
Genellikle direct exchange kullanılır.
 */


//string queueNmae = "example-work-queue";
//channel.QueueDeclare(queue: queueNmae, durable: false, exclusive: false, autoDelete: false);

//for (int i = 0; i < 100; i++)
//{
//    await Task.Delay(200);
//    byte[] messsage = Encoding.UTF8.GetBytes("merhaba"+i);
//    channel.BasicPublish(exchange: string.Empty, routingKey: queueNmae, body: messsage);
//}

#endregion

#region Request/Response Tasarımı

/*
 Bu tasarımda, publisher bir request yapar gibi kuyruğa mesaj gönderir ve bu mesajı tüketen consumer'dan sonuca dair başka kuyruktan bir yanıt/response bekler. Bu tarz senaryolar için oldukça uygun bir tasarımdır. 

Hem publisher'ın hem de consumer'ın kullanacağı 2 tane kuyruk olacak.
1- Request queue
2- Response queue olacaktır. 

 */

string requestQueueName = "example-reques-response-queue";
channel.QueueDeclare(queue: requestQueueName, durable: false, exclusive: false, autoDelete: false);

string responseQueueName = channel.QueueDeclare().QueueName; // random queue name verelim. İsimli de hitap edebiliriz.

string correlationId = Guid.NewGuid().ToString();

#region Request Mesajını oluşturma ve gönderme

IBasicProperties properties = channel.CreateBasicProperties();
properties.CorrelationId = correlationId;  //Göndereceğimiz mesajın korelasyon değerini taşıyacak.
properties.ReplyTo = responseQueueName; // Göndereceğimiz mesaj neticesinde beklenecek response değerinin hangi kuyruğa gönderileceğini ifade edecektir.

for (int i = 0; i < 10; i++)
{
    await Task.Delay(200);
    byte[] messsage = Encoding.UTF8.GetBytes("merhaba" + i);
    channel.BasicPublish(exchange: string.Empty, routingKey: requestQueueName, body: messsage, basicProperties: properties);
}
#endregion

#region Response  Kuyrugu Dinleme
EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(queue: responseQueueName, autoAck: true, consumer: consumer);

consumer.Received += (sender, e) =>
{
    if (e.BasicProperties.CorrelationId == correlationId)
    {

        Console.WriteLine($"Response:{ Encoding.UTF8.GetString(e.Body.Span)}");
    }
};


#endregion



#endregion


Console.Read();