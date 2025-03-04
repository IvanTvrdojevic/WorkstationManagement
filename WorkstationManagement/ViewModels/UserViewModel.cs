using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using WorkstationManagement.Data;
using WorkstationManagement.Models;
using WorkstationManagement.Utils;

namespace WorkstationManagement.ViewModels;

public partial class UserViewModel : ViewModelBase{
    //=======================================================================================================================================================
    // DISPLAY USER INFO
    // Observable properties used to display First Name, Last Name and all of the users work positins
    // _currentUser set in the constructor to the _navigationService.CurrentUser 
    [ObservableProperty]
    private User _currentUser;

    [ObservableProperty]
    private string _firstName;

    [ObservableProperty]
    private string _lastName;

    [ObservableProperty]
    private ObservableCollection<WorkPosition> _workPositions;

    //=======================================================================================================================================================
    // NAVIGATION SERVICE   
    // Used for navigation
    // WorkstationManagement/Utils/NavigationService.cs
    private NavigationService _navigationService;
    //-------------------------------------------------------------------------------------------------------------------------------------------------------

    //=======================================================================================================================================================
    //  CONSTRUCTOR
    //=======================================================================================================================================================
    public UserViewModel(NavigationService navigationService){
        _navigationService = navigationService;
        _currentUser = _navigationService.CurrentUser;
        LoadFromDB();
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    //=======================================================================================================================================================
    // LOGOUT
    //=======================================================================================================================================================
    [RelayCommand]
    public void LogoutBtnClick()
    {
        _navigationService.NavigateTo<LoginViewModel>(null);
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    //=======================================================================================================================================================
    //  LOAD FROM DATABASE
    //=======================================================================================================================================================  
    //
    // Gets all of work positions for the CurrentUser using UserWorkPosition.UserId
    public void LoadFromDB()
    {
        using(var db = new WorkstationManagementContext())
        {
            WorkPositions = new ObservableCollection<WorkPosition>([.. db.UserWorkPositions.Include(uwp => uwp.WorkPosition)
                                                                                       .Where(uwp => uwp.UserId == CurrentUser.Id)
                                                                                       .Select(uwp => uwp.WorkPosition)]);
        }
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
}