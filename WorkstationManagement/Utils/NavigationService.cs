using System;
using Microsoft.Extensions.DependencyInjection;
using WorkstationManagement.Models;
using WorkstationManagement.ViewModels;

namespace WorkstationManagement.Utils;

public class NavigationService{
    private ViewModelBase  _currentViewModel;
    public User? CurrentUser;
    private readonly IServiceProvider _serviceProvider;
    public ViewModelBase CurrentViewModel{
        get => _currentViewModel;
        set{
            _currentViewModel = value;
            OnCurretnViewModelChange();
        }
    }

    public NavigationService(IServiceProvider serviceProvider){
        _serviceProvider = serviceProvider;
    }

    public event Action? CurrentViewModelChanged;
    private void OnCurretnViewModelChange(){
        CurrentViewModelChanged?.Invoke();
    }

    private ViewModelBase BuildViewModel(Type viewModelType){
        return (ViewModelBase)_serviceProvider.GetRequiredService(viewModelType);
    }
    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase{
        CurrentViewModel = BuildViewModel(typeof(TViewModel));
    }

    public void  NavigateTo<TViewModel>(User? currentUser) where TViewModel : ViewModelBase{
        CurrentUser = currentUser;
        CurrentViewModel = BuildViewModel(typeof(TViewModel));
    }
}