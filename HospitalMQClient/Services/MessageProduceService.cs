
using Microsoft.Extensions.Logging;
using HospitalCommon;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;

namespace HospitalMQClient.Services
{
    public class MessageProduceService
    {
        private ILogger<MessageProduceService> _logger;
        private IConnection? _connection;
        private IChannel? _channel;
        

        public MessageProduceService(ILogger<MessageProduceService> logger)
        {
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken token)
        {
            try
            {
                await ConnectAsync("localhost", "", "");
            } catch
            {
                MessageBox.Show("Не удалось подключиться автоматически, введите данные в разделе Подключение");
            }
        }

        public async Task ConnectAsync(string hostname, string username, string password)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = hostname,
                    UserName = username,
                    Password = password
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.QueueDeclareAsync("hospital_queue", false, false, false);
                _logger.LogInformation("Канал был создан и очередь была обьявлена");
            } catch (Exception e)
            {
                _logger.LogCritical($"Не удалось подключиться: {e}");
                throw;
            }
        }

        public async Task SendPatientAsync(Patient patient)
        {
            if (_channel is not { IsOpen: true })
            {
                _logger.LogWarning("Попытка отправить сообщение, но канал закрыт");
                throw new InvalidOperationException("Канал RabbitMQ не открыт");
            }

            try
            {
                var json = JsonSerializer.Serialize(patient);
                var body = Encoding.UTF8.GetBytes(json);

                await _channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: "hospital_queue",
                    body: body);

                _logger.LogInformation($"Заказ {json} отправлен в hospital_queue!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при публикации сообщения");
                throw;
            }
        }

        public void Disconnect()
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
