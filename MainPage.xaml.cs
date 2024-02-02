using SampleApp.Models;
using SampleApp.ViewModels;

namespace SampleApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new AppState();
        }

        private void SubmitForm(object sender, System.EventArgs e)
        {
            var appState = (AppState)BindingContext;
            appState.SubmitForm();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var appState = (AppState)BindingContext;
            appState.SelectedRecord = e.CurrentSelection.FirstOrDefault() as ContactInformation;
        }

        private void UpdateSelectedRecord(object sender, System.EventArgs e)
        {
            var appState = (AppState)BindingContext;
            appState.UpdateSelectedRecord();
        }
    }
}
