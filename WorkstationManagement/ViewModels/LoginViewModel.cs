using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using WorkstationManagement.Data;
using WorkstationManagement.Utils;

namespace WorkstationManagement.ViewModels;

public partial class LoginViewModel : ViewModelBase{
    //=======================================================================================================================================================
    // LOGIN
    // Observable properties used to check login info and display error message
    [ObservableProperty]
    private string _username = "";
    [ObservableProperty]
    private string _password = "";
    [ObservableProperty]
    private string _message = "";

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
    public LoginViewModel(NavigationService navigationService, UserSessionService userSessionService, WorkstationManagementContext dbContext){
        _navigationService = navigationService;
        _userSessionService = userSessionService;
        _dbContext = dbContext;
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    //=======================================================================================================================================================
    //  LOGIN
    //=======================================================================================================================================================
    //
    // Check if user exists in database and navigate to corresponding ViewModel depending on the role of the user
    [RelayCommand]
    public void OnLoginBtnClick(){
        // Implemented in WorkstationManagement.Utils.Helpers.cs
        string hashedPassword = Helper.ComputeSha256Hash(Password);

        var user =  _dbContext.Users.Include(u => u.Role).FirstOrDefault(u => u.Username == Username && u.Password == hashedPassword);
        if(user != null)
        {
            _userSessionService.CurrentUser = user;
            if (user.Role.RoleName == "User")
            {
                _navigationService.NavigateTo<UserViewModel>();
            }   
            else if (user.Role.RoleName == "Admin")
            {
                _navigationService.NavigateTo<AdminViewModel>();
            }
        }
        else
        {
            Message = "Invalid username or password.";
        }
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
}