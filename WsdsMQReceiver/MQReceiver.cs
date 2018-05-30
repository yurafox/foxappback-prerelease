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
using Oracle.ManagedDataAccess.Client;

namespace WsdsMQReceiver
{
    class MQReceiver
    {
        private static readonly String QUEUE_NAME = "orders_queue";

        private IConfigurationRoot configuration;

        public MQReceiver() {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            this.configuration = builder.Build();
        }

        private IConnection getConnection() {
            var factory = new ConnectionFactory()
            {
                HostName = configuration["Rabbit:hostname"],
                UserName = configuration["Rabbit:username"],
                Password = configuration["Rabbit:password"],
                Port = Int32.Parse(configuration["Rabbit:port"])
            };

            return factory.CreateConnection();
        }

        public void receiveMessage() {
            using (var connection = this.getConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: QUEUE_NAME,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    ReceiveHandler(channel, ea); 
                };

                channel.BasicConsume(queue: QUEUE_NAME,
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private void ReceiveHandler(IModel channel, BasicDeliverEventArgs ea)
        {
            try
            {
                string message = Encoding.UTF8.GetString(ea.Body);
                Console.WriteLine("order: {0}", message);

                Type t = Type.GetType(ea.BasicProperties.Type);
                object rawObject = JsonConvert.DeserializeObject(message, t);                
                if (rawObject.GetType() != typeof(ClientOrderMQ)) {
                    Console.WriteLine("Wrong received object type. {0}, {1}", rawObject.GetType());
                    return;
                }

                ClientOrderMQ order = rawObject as ClientOrderMQ;
                createOrder(order);
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception e) {
                Console.WriteLine("Exception. Error Message: {0}", e.Message);
                Console.WriteLine("Exception. Stack Trace: {0}", e.StackTrace);
            }
        }

        private void createOrder(ClientOrderMQ order) {
            Console.WriteLine("order ID: {0}", order.id);
            String ConnString = this.configuration["DBConnectionStrings:MainDataConnection"];
            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand("begin Pkg_Order.CreateOrderInRmm(:id); end;", con))
            {   
                cmd.Parameters.Add(new OracleParameter("id", order.id));
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();                
            };
        }

        static void Main(string[] args) {
            MQReceiver mqReceiver = new MQReceiver();
            mqReceiver.receiveMessage();
        }
    }
}