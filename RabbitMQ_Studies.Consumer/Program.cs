using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;



/*
 *  CONSUMER UYGULAMASI İŞLEM SIRASI
 * 
 * 1-) BAĞLANTI OLUŞTURMA
 * 
 * 2-) BAĞLANTI AKTİFLEŞTİRME VE KANAL AÇMA
 * 
 * 3-) QUEUE OLUŞTURMA
 * 
 * 4-) QUEUE'A MESAJ OKUMA
 *
 **/


// Öncelikle Factory sınıfını oluşturalım. 
// Bağlantı Oluşturma
ConnectionFactory factory = new();
factory.Uri = new("amqps://owqngobg:blIdj-tp5MdEGI55DGHK77IO6REX6hxx@stingray.rmq.cloudamqp.com/owqngobg");


// Bağlantı Aktifleştirme ve Kanal Açma

using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();


// Queue Tanımlama
// Publisher üzerinde queue oluşturduk neden consumer da queue oluşturuyoruz?
// RabbitMQ sunucusunu kullanırken publisher ve consumer aynı queue kullanılacaksa aynı queue kullanacaksa her ikisinde de oluşturulması gerekmektedir.
// Publisher'daki ile birebir aynı yapılandırmada tanımlanmalıdır!
channel.QueueDeclare(queue: "example-queue", exclusive: false);

// Queue' a Mesaj Okuma
// Kuyruktan mesaj okuyabilmek için bu channel üzerinde bir event operasyonu yapmamız lazım. Bunun için EventingBasicConsumer sınıfını kullanacağız.
EventingBasicConsumer consumer = new(channel);

// Bu consumer isimli instance'ın üzerinde yani bu channel'da bildireceğim kuyrukta bir mesaj olursa onu receive edeceğiz.(işleyeceğiz, consume edeceğiz, tüketeceğiz gibi terimleri kullanılabilir)

channel.BasicConsume(queue: "example-queue", autoAck: false, consumer: consumer);

// BasicConsume(queue, autoAck , consumer);
// BasicConsume("hangi kuyruğu dinleyeceğim?", "Mesaji aldıktan sonra o mesajı kuyruktan fiziksel olarak sileyim mi silmeyeyim mi?", "consumer");


// Consumer'ın ne zaman mesaj geldi ise onu yakalayabilmek için bunu received etmesi lazım. Received etmek bir delegate olacağı için metod bağlamamız lazım.

consumer.Received += (sender, e) =>
{
    // Kuyruğa gelen mesajın  işlendiği yerdir.
    // e.Body : Kuyruktaki mesajın verisini bütünsel olarak getirecektir.
    // Eğer mesajın içerisindeki byte veriyi elde etmek istiyorsak bunun için Span veya ToArray metodlarını kullanabiliriz
    // e.Body.Span veya e.Body.ToArray() : Kuyruktaki mesajın byte verisini getirecektir.
    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
};

Console.Read();