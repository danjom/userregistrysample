using Microsoft.EntityFrameworkCore;
using SampleApp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace SampleApp.ViewModels
{
    public class AppState
    {
        private const int MaxStringLength = 50;

        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(MaxStringLength, ErrorMessage = "Email cannot exceed 50 characters.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(MaxStringLength, ErrorMessage = "Phone number cannot exceed 50 characters.")]
        public string? PhoneNumber { get; set; }

        public ObservableCollection<ContactInformation> SubmittedRecords { get; } = new ObservableCollection<ContactInformation>();
        public ContactInformation? SelectedRecord { get; set; }

        public void SubmitForm()
        {
            var validationResults = ValidateForm();
            if (validationResults.Count() > 0)
            {
                DisplayValidationErrors(validationResults);
                return;
            }

            var contactInfo = new ContactInformation { Name = Name!, Email = Email!, PhoneNumber = PhoneNumber! };
            SubmittedRecords.Add(contactInfo);

            using (var db = new AppDbContext())
            {
                db.EnsureDatabaseCreated();

                var userEntity = new ContactInformation { Name = Name!, Email = Email!, PhoneNumber = PhoneNumber! };
                db.Users.Add(userEntity);
                db.SaveChanges();
            }

            var message = $"Form Submitted!\n{contactInfo}";
            App.Current.MainPage.DisplayAlert("Form Submitted", message, "OK");

            UpdateSubmittedRecords();
        }

        public void UpdateSelectedRecord()
        {
            if (SelectedRecord == null)
                return;

            var validationResults = ValidateForm();
            if (validationResults.Count() > 0)
            {
                DisplayValidationErrors(validationResults);
                return;
            }

            using (var db = new AppDbContext())
            {
                db.Database.Migrate();

                var userEntity = new ContactInformation
                {
                    Id = SelectedRecord.Id,
                    Name = Name,
                    Email = Email,
                    PhoneNumber = PhoneNumber
                };

                db.Users.Update(userEntity);
                db.SaveChanges();
            }

            App.Current.MainPage.DisplayAlert("Record Updated", "The selected record has been updated.", "OK");

            UpdateSubmittedRecords();
        }

        public void UpdateSubmittedRecords()
        {
            using (var db = new AppDbContext())
            {
                var entities = db.Users.ToList();
                SubmittedRecords.Clear();

                foreach (var entity in entities)
                {
                    var contactInfo = new ContactInformation
                    {
                        Id = entity.Id,
                        Name = entity.Name,
                        Email = entity.Email,
                        PhoneNumber = entity.PhoneNumber
                    };
                    SubmittedRecords.Add(contactInfo);
                }
            }
        }

        private IEnumerable<ValidationResult> ValidateForm()
        {
            var validationResults = new List<ValidationResult>();

            var context = new ValidationContext(this, serviceProvider: null, items: null);
            Validator.TryValidateObject(this, context, validationResults, validateAllProperties: true);

            return validationResults;
        }

        private void DisplayValidationErrors(IEnumerable<ValidationResult> validationResults)
        {
            var errorMessages = validationResults.Select(vr => vr.ErrorMessage);
            var errorMessage = string.Join("\n", errorMessages);
            App.Current.MainPage.DisplayAlert("Validation Error", errorMessage, "OK");
        }
    }
}
