using HospitalCommon;
using HospitalMQ.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HospitalMQ.Services
{
    public class StorageService
    {
        private ILogger<StorageService> _logger;
        private AppDbContext _db;

        public StorageService(ILogger<StorageService> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
            _db.Database.EnsureCreated();
            _logger.LogInformation("База данных создана");
        }

        public async Task SavePatientAsync(Patient patient)
        {
            _db.Patients.Add(patient);
            _logger.LogInformation($"Пациент {patient.Name} успешно добавлен!");
        }
    }
}
