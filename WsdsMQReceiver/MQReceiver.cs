﻿using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace WsdsMQReceiver
{
    class MQReceiver
    {
        private const string WorkQueue = "WorkQueue";
        private readonly IConfigurationRoot _configuration;
        private readonly HttpClient _client;

        public MQReceiver() {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            _configuration = builder.Build();            
            var url = $"{_configuration["Baseurl:host"]}/api/";
            Console.WriteLine(url);
            _client = new HttpClient
            {
                BaseAddress = new Uri(url)
            };
        }

        private IConnection GetConnection() {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["Rabbit:hostname"],
                UserName = _configuration["Rabbit:username"],
                Password = _configuration["Rabbit:password"],
                Port = Int32.Parse(_configuration["Rabbit:port"])
            };

            return factory.CreateConnection();
        }

        private void ReceiveMessage() {
            using (var connection = GetConnection())
            using (var channel = connection.CreateModel())
            {
                Console.WriteLine(" [*] Waiting for messages.");
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    ReceiveHandler(channel, ea); 
                };
                channel.BasicConsume(queue: WorkQueue,
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
                var message = Encoding.UTF8.GetString(ea.Body);
                Console.WriteLine("order: {0}", message);

                var order = JObject.Parse(message);
                CreateOrder(order);
                Console.WriteLine("Order comlete");
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception e) {
                Console.WriteLine("Exception. Error Message: {0}", e.Message);
                Console.WriteLine("Exception. Stack Trace: {0}", e.StackTrace);
                channel.BasicNack(deliveryTag: ea.DeliveryTag, requeue: !ea.Redelivered, multiple: false);
            }
        }

        private void CreateOrder(JObject order) {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var stringContent = new StringContent(order.ToString(), Encoding.UTF8, "application/json");
            var result = _client.PostAsync("saleRmm/NewSaleRmm", stringContent).Result;
            Console.WriteLine($"Status code: {result.StatusCode}");
            if (result.StatusCode != HttpStatusCode.OK)
            {
                var resultContent = result.Content.ReadAsStringAsync();
                throw new Exception($"Http request error: {resultContent}");
            }
        }

        static void Main(string[] args) {
            var mqReceiver = new MQReceiver();
            mqReceiver.ReceiveMessage();
        }
    }
}