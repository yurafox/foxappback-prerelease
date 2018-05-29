using System;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Wsds.DAL.Entities.Communication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using System.IO;

namespace WsdsMQReceiver
{
    class MQReceiver
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();

            var factory = new ConnectionFactory() {
                HostName = configuration["Rabbit:hostname"],
                UserName = configuration["Rabbit:username"],
                Password = configuration["Rabbit:password"],
                Port = Int32.Parse(configuration["Rabbit:port"])
            };

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
                    try
                    {
                        string message = Encoding.UTF8.GetString(ea.Body);
                        Type t = Type.GetType(ea.BasicProperties.Type);
                        object rawObject = JsonConvert.DeserializeObject(message, t);
                        if (rawObject.GetType() == typeof(ClientOrderMQ))
                        {
                            ClientOrderMQ order = rawObject as ClientOrderMQ;
                            Console.WriteLine("order ID: {0}", order.id);
                            Console.WriteLine("order: {0}", message);
                        }
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception e) {
                        Console.WriteLine("Exception. Error Message: {0}", e.Message);
                        Console.WriteLine("Exception. Stack Trace: {0}", e.StackTrace);
                    }
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
