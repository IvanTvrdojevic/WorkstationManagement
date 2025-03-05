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
    // SERVICES 
    // WorkstationManagement/Utils/NavigationService.cs
    // For navigation
    private NavigationService _navigationService;

    // For user session
    private UserSessionService _userSessionService;

    // For database
    private WorkstationManagementContext _dbContext;
    //-------------------------------------------------------------------------------------------------------------------------------------------------------

    //=======================================================================================================================================================
    //  CONSTRUCTOR
    //=======================================================================================================================================================
    public UserViewModel(NavigationService navigationService, UserSessionService userSessionService, WorkstationManagementContext dbContext){
        _navigationService = navigationService;
        _userSessionService = userSessionService;
        CurrentUser = userSessionService.CurrentUser;
        _dbContext = dbContext;
        LoadFromDB();
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    //=======================================================================================================================================================
    // LOGOUT
    //=======================================================================================================================================================
    [RelayCommand]
    public void LogoutBtnClick()
    {
        _userSessionService.CurrentUser = null;
        _navigationService.NavigateTo<LoginViewModel>();
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    //=======================================================================================================================================================
    //  LOAD FROM DATABASE
    //=======================================================================================================================================================  
    //
    // Gets all of work positions for the CurrentUser using UserWorkPosition.UserId
    public void LoadFromDB()
    {

        WorkPositions = new ObservableCollection<WorkPosition>([.. _dbContext.UserWorkPositions.Include(uwp => uwp.WorkPosition)
                                                                                               .Where(uwp => uwp.UserId == _userSessionService.CurrentUser.Id)
                                                                                               .Select(uwp => uwp.WorkPosition)]);
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
}