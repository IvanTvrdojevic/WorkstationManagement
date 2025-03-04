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
    // NAVIGATION SERVICE   
    // Used for navigation
    // WorkstationManagement/Utils/NavigationService.cs
    private NavigationService _navigationService;
    //-------------------------------------------------------------------------------------------------------------------------------------------------------

    //=======================================================================================================================================================
    //  CONSTRUCTOR
    //=======================================================================================================================================================
    public LoginViewModel(NavigationService navigationService, CurrentUser currentUser){
        _navigationService = navigationService;
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
        using (var db = new WorkstationManagementContext())
        {
            var user =  db.Users.Include(u => u.Role).FirstOrDefault(u => u.Username == Username && u.Password == hashedPassword);
            if(user != null)
            {
                if (user.Role.RoleName == "User")
                {
                    db.Dispose();
                    _navigationService.NavigateTo<UserViewModel>(user);
                }   
                else if (user.Role.RoleName == "Admin")
                {
                    db.Dispose();
                    _navigationService.NavigateTo<AdminViewModel>(user);
                }
            }
            else
            {
                Message = "Invalid username or password.";
            }
        }
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
}