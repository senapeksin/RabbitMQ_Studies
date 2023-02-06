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
factory.Uri = new("");


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

//channel.BasicConsume(queue: "example-queue", autoAck: false, consumer: consumer);

// BasicConsume(queue, autoAck , consumer);
// BasicConsume("hangi kuyruğu dinleyeceğim?", "Mesaji aldıktan sonra o mesajı kuyruktan fiziksel olarak sileyim mi silmeyeyim mi?", "consumer");

// MESSAGE ACKNOWLEDGEMENT NEDİR? 
// RabbitMQ, tüketiciye gönderdiği mesajı başarılı bir şekilde işlesin veya işlemesein hemen kuyruktan silinmesi üzere işaretler. (Default olarak böyle bir davrasına sahiptir.)
// Tüketicinin kuyruktan aldığı mesajları işlemeleri sürecinde herhangi bir kesinti ya da problem durumunda ilgili mesajı tam olarak işlenemeyeceği için esasında görev tamamlanmamış olacaktır.
// Bu tarz durumlara istinaden mesaj başarıyla işlendiyse eğer kuyruktan silinmesi için tüketiciden RabbitMQ'nun uyarılması gerekmektedir.
// Bir mesaj işlenmeden consumer problem yaşarsa bu mesajın sağlıklı bir şekilde işlenebilmesi için başka bir consumer tarafından işlenebilmesi gerekmektedir.
// Aksi taktirde veri kaybı yaşanacaktır. Bu tarz durumlar için message acknowledgement şarttır diyebiliriz.

// MESSAGE ACKNOWLEDGEMET YAPILANDIRILMASI
// RabbitMQ üzerinde mesaj onaylama sürecini aktifleştirebilmek için consumer uygulamasında "BasicConsume" metodundaki 'autoAck' parametresini false değerine getirebiliriz.
// Böylece RabbitMq'da varsayılan olarak mesajların kuyruktan silinme davranışı değiştirilecek ve consumer'dan onay beklenecektir.
// Consumer işlemini yaptıktan sonra RabbitMQ'ya nasıl bildiride bulunacak? 
// "BasicAck" metodunu çağırarak bunu gerçekleştirecek. İlk parametrede e.DeliveryTag gönderiyoruz. Bu hangi mesaj ise ona dait unique değeri temsil ediyor.
// Bu deliveryTag' e karşılık olan mesajın başarılı bir şekilde sonuçlandığını RabbitMQ 'e bildirmiş oluyoruz.
// İkinci parametremiz olan "multiple" ise birden fazla mesaja dair onay bildirisi gönderir.
// Eğer true değer verilirse DeliveryTag değerine sahip olan bu mesajla birlikte bundan önceki mesajlarında işlendiğini onaylar. 
// False verilirse sadece bu mesaj için onay bildirisinde bulunacaktır.  

// Bu uygulamada mesajların kuyruktan silinip, silinmeyeceğine dair karar consumerdaki mesajın handle edilmesine bağlı olsun. 
// Bunun için BasicConsume metodunda autoAck parametresini false yaparak konfigurasyonumuza başlıyoruz.
// autoAck: false diyerek artık burada consume edilecek mesajın consumer tarafından bildirim gelmediği sürece RabbitMQ tarafından kuyruktan silinmesini engelliyoruz.

// Şimdi bizim Received olarak elde ettiğimiz mesajı başarılı bir şekilde işledikten sonra RabbitMQ'ya silmesi gerektiğinin talimatını vermemiz gerekmektedir.
// Bunun içinde BasicAck fonksiyonunu çağırarak bildiri yapabiliriz.

channel.BasicConsume(queue: "example-queue", autoAck: false, consumer: consumer);

// Consumer'ın ne zaman mesaj geldi ise onu yakalayabilmek için bunu received etmesi lazım. Received etmek bir delegate olacağı için metod bağlamamız lazım.

consumer.Received += (sender, e) =>
{
    // Kuyruğa gelen mesajın  işlendiği yerdir.
    // e.Body : Kuyruktaki mesajın verisini bütünsel olarak getirecektir.
    // Eğer mesajın içerisindeki byte veriyi elde etmek istiyorsak bunun için Span veya ToArray metodlarını kullanabiliriz
    // e.Body.Span veya e.Body.ToArray() : Kuyruktaki mesajın byte verisini getirecektir.
    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
    channel.BasicAck(deliveryTag:e.DeliveryTag,multiple:false); //multiple:false diyerek sadece bu mesaja dair bildiride bulunacağımı ifade ediyorum.

    // BasicAck(deliveryTag: Bildirimde bulunacağımız mesaja dair unique bir değerdir.Bu mesajın bildirisinde bulunacağımı ifade ediyorum , multiple: false diyerek sadece bu mesaja dair bildiride bulunacağımı ifade ediyorum. );
};

Console.Read();