using CommunityToolkit.Mvvm.ComponentModel;
using WorkstationManagement.Utils;

namespace WorkstationManagement.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{   
    //=======================================================================================================================================================
    // CURRENTVIEWMODEL
    // Used to switch ViewModel
    [ObservableProperty]
    public ViewModelBase? _currentViewModel;
    
    //=======================================================================================================================================================
    // NAVIGATION SERVICE   
    // Used for navigation
    // WorkstationManagement/Utils/NavigationService.cs
    private NavigationService _navigationService;
    //-------------------------------------------------------------------------------------------------------------------------------------------------------

    //=======================================================================================================================================================
    //  CONSTRUCTOR
    //=======================================================================================================================================================
    public MainWindowViewModel(NavigationService navigationService){
        _navigationService = navigationService;
        // Loose binding of the _navigationService object with the CurrentViewModel field
        // Every time navigationService.CurrentViewModel is changed the MainWindowViewModel.CurrentViewModel is set to that value
        _navigationService.CurrentViewModelChanged += ChangeCurrentViewModel;
        _navigationService.NavigateTo<LoginViewModel>();
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    
    private void ChangeCurrentViewModel(){
        CurrentViewModel = _navigationService.CurrentViewModel;
    }
}