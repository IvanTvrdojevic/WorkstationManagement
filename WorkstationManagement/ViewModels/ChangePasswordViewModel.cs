using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using WorkstationManagement.Data;
using WorkstationManagement.Utils;

namespace WorkstationManagement.ViewModels;

public partial class ChangePasswordViewModel : ViewModelBase{
    //=======================================================================================================================================================
    // LOGIN
    // Observable properties used to check old password and create a new one
    [ObservableProperty]
    private string _currentPassword = "";
    [ObservableProperty]
    private string _newPassword = "";
    [ObservableProperty]
    private string _confirmedPassword = "";
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
    public ChangePasswordViewModel(NavigationService navigationService, UserSessionService userSessionService, WorkstationManagementContext dbContext){
        _navigationService = navigationService;
        _userSessionService = userSessionService;
        _dbContext = dbContext;
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    //=======================================================================================================================================================
    //  CHANGE PASSWORD
    //=======================================================================================================================================================
    //
    // Confirm that the user knows the temp passowrd and create a new one
    [RelayCommand]
    public void OnChangePasswordBtnClick(){
        // Implemented in WorkstationManagement.Utils.Helpers.cs
        string hashedCurrentPassword = Helper.ComputeSha256Hash(CurrentPassword);

        var user =  _dbContext.Users.Include(u => u.Role).FirstOrDefault(u => u.Username == _userSessionService.CurrentUser.Username && u.Password == hashedCurrentPassword);
        if(user != null)
        {
            bool passwordStrong = false;
            // Messages for weak password are handled by Helper.CheckStrenght, no need for else statement for passwordStrong
            (Message, passwordStrong) = Helper.CheckStrength(NewPassword);
            if(passwordStrong)
            {
                if(NewPassword != CurrentPassword)
                {
                    if(NewPassword == ConfirmedPassword)
                    {
                        user.Password = Helper.ComputeSha256Hash(NewPassword);
                        user.ChangePwNeeded = false;
                        _dbContext.SaveChanges();

                        if (user.Role.RoleName == "User")
                            _navigationService.NavigateTo<UserViewModel>();
                        else if (user.Role.RoleName == "Admin")
                            _navigationService.NavigateTo<AdminViewModel>();
                    }
                    else
                        Message = "Password confirmation incorrect";
                }
                else 
                    Message = "New Password has to be different";
            }
        }
        else
            Message = "Invalid password.";
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
}