using Bogus;
using HospitalCommon;
using HospitalMQClient.Services;

namespace HospitalMQClient.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        private MessageProduceService _produceService;

        public DashboardViewModel(MessageProduceService produceService)
        {
            _produceService = produceService;
        }

        [ObservableProperty]
        private string _name = "";
        [ObservableProperty]
        private int _age;
        [ObservableProperty]
        private string _doctorName = "";

        [RelayCommand]
        private void OnGenerateFields()
        {
            var faker = new Faker("ru");
            Name = $"{faker.Name.FirstName()} {faker.Name.LastName()}";
            DoctorName = $"{faker.Name.FirstName()} {faker.Name.LastName()}";
            Age = faker.Random.Int(20, 70);
        }

        [RelayCommand]
        private void OnSendMessage()
        {
            Task.Run(async () =>
            {
                try
                {
                    await _produceService.SendPatientAsync(new Patient()
                    {
                        AttendingDoctor = new()
                        {
                            Name = DoctorName
                        },
                        Name = Name,
                        Age = Age
                    });
                    MessageBox.Show("Успешно отправлено!");
                } catch (Exception e)
                {
                    MessageBox.Show($"Не удалось отправить: {e.Message}");
                }
            });
        }
    }
}
