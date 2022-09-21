using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace RabbitMQ.Subscriber
{
    internal class Program
    {
        static void Main1(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://jhsmvvbc:D9eZr9WK0GxjymoMamptheYXVLX4Iw5q@chimpanzee.rmq.cloudamqp.com/jhsmvvbc");

            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            // channel.QueueDeclare("hello-queue", true, false, false);

            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume("hello-queue", false, consumer);

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Console.WriteLine(message);

                channel.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }

        static void Main2(string[] args)//Fenout Exchange
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://jhsmvvbc:D9eZr9WK0GxjymoMamptheYXVLX4Iw5q@chimpanzee.rmq.cloudamqp.com/jhsmvvbc");

            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            var randomQueueName = channel.QueueDeclare().QueueName;



            channel.QueueBind(randomQueueName, "logs-fanout", "", null);

            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(randomQueueName, false, consumer);

            Console.WriteLine("Loglar  Dinleniyor");

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Console.WriteLine(message);

                channel.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }

        static void Main3(string[] args)//Direct Exchange
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://jhsmvvbc:D9eZr9WK0GxjymoMamptheYXVLX4Iw5q@chimpanzee.rmq.cloudamqp.com/jhsmvvbc");

            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            var queueName = "direct-queue-Critical";

            channel.BasicConsume(queueName, false, consumer);

            Console.WriteLine("Loglar  Dinleniyor");

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Console.WriteLine(message);

                //  File.AppendAllText("log-criticel.txt", message+"\n");

                channel.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }
        static void Main4(string[] args)//Topic Exchange
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://jhsmvvbc:D9eZr9WK0GxjymoMamptheYXVLX4Iw5q@chimpanzee.rmq.cloudamqp.com/jhsmvvbc");

            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            var queueName = channel.QueueDeclare().QueueName;

            var routeKey = "Information.#";

            channel.QueueBind(queueName, "logs-topic", routeKey);

            channel.BasicConsume(queueName, false, consumer);

            Console.WriteLine("Loglar  Dinleniyor");

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Console.WriteLine(message);

                //  File.AppendAllText("log-criticel.txt", message+"\n");

                channel.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }
        static void Main(string[] args)//Header Exchange
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://jhsmvvbc:D9eZr9WK0GxjymoMamptheYXVLX4Iw5q@chimpanzee.rmq.cloudamqp.com/jhsmvvbc");

            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            var queueName = channel.QueueDeclare().QueueName;

            Dictionary<string, object> headers = new Dictionary<string, object>();
            headers.Add("format", "pdf");
            headers.Add("shape", "a4");
            headers.Add("x-match", "any");

            channel.QueueBind(queueName, "header-exchange", string.Empty, headers);

            channel.BasicConsume(queueName, false, consumer);

            Console.WriteLine("Loglar  Dinleniyor");

            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Product product = JsonSerializer.Deserialize<Product>(message);

                Console.WriteLine($"Gelen Mesaj : {product.Id}-{product.Name}-{product.Price}-{product.Stock}");


                channel.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }
    }
}
