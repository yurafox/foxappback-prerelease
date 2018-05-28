using System;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Wsds.DAL.Entities.Communication;

namespace WsdsMQReceiver
{
    class MQReceiver
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest", Port = 5672 };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "orders_queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    string objectType = ea.BasicProperties.Type;
                    Type t = Type.GetType(objectType);

                    object rawObject = JsonConvert.DeserializeObject(message, t);
                    if (rawObject.GetType() == typeof(ClientOrderMQ))
                    {
                        ClientOrderMQ order = rawObject as ClientOrderMQ;
                        Console.WriteLine("order ID: {0}", order.id);
                        Console.WriteLine("order: {0}", message);
                    }

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                channel.BasicConsume(queue: "orders_queue",
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
                
            }
        }
    }
}
