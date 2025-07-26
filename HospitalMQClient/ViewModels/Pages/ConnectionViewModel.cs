
using HospitalMQClient.Services;

namespace HospitalMQClient.ViewModels.Pages
{
    public partial class ConnectionViewModel : ObservableObject
    {
        private MessageProduceService _produceService;

        public ConnectionViewModel(MessageProduceService produceService)
        {
            _produceService = produceService;
        }

        [ObservableProperty]
        private string _hostname = "localhost";
        [ObservableProperty]
        private string _username = "";
        [ObservableProperty]
        private string _password = "";

        [RelayCommand]
        private void OnConnect()
        {
            Task.Run(async () =>
            {
                try
                {
                    await _produceService.ConnectAsync(Hostname, Username, Password);
                    MessageBox.Show($"Подключено!");
                } catch (Exception e)
                {
                    MessageBox.Show($"Не удалось подключиться: {e.Message}");
                }
            });
        }
    }
}
