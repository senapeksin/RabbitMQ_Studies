﻿using RabbitMQ.Client;
using System.Text;

/*
 *  PUBLISHER UYGULAMASI İŞLEM SIRASI
 * 
 * 1-) BAĞLANTI OLUŞTURMA
 * 
 * 2-) BAĞLANTI AKTİFLEŞTİRME VE KANAL AÇMA
 * 
 * 3-) QUEUE OLUŞTURMA
 * 
 * 4-) QUEUE'A MESAJ GÖNDERME
 *
 **/


// Öncelikle Factory sınıfını oluşturalım. 
// Bağlantı Oluşturma

ConnectionFactory factory = new();
factory.Uri = new("");


// Bağlantı Aktifleştirme ve Kanal Açma

using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

// Queue Oluşturma

channel.QueueDeclare(queue: "example-queue", exclusive: false);

// Queue' a Mesaj Gönderme
// RabbitMQ kuyruga atacağı mesajları byte türünden kabul etmektedir. Haliyle mesajları bizim byte dönüşmemiz gerekecektir.

//byte[] message = Encoding.UTF8.GetBytes("Merhaba");
//channel.BasicPublish(exchange: "", routingKey: "example-queue", body: message);   //mesaj gönderilmesi

for (int i = 0;i < 100; i++)
{
    await Task.Delay(200);
    byte[] message = Encoding.UTF8.GetBytes($"Merhaba {i}");
    channel.BasicPublish(exchange: "", routingKey: "example-queue", body: message);   
}

// Exchange boş gönderilmesi demek default exchange kullan demektir. RabbitMQ 'nün default exchange türü Direct Exchange'dir.
// Routing key : Exchange'e bind edilmiş kuyruklardan hangisine mesaj göndereceğimizi belirleyen yöntemlerden biridir.
// Default exchange kullandığımız için  direct exchange'de routing key mesaj kuyrugunun ismine karşılık gelmektedir.
// Direct exhange kullandığımız için hangi queue kullanıyorsak o kuyrugun adını vermemiz gerekmektedir.

Console.Read();