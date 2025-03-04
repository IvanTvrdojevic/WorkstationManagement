using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using WorkstationManagement.Data;
using WorkstationManagement.Models;
using WorkstationManagement.Utils;
using System;

namespace WorkstationManagement.ViewModels;

public partial class AdminViewModel : ViewModelBase{
    //=======================================================================================================================================================
    // ADD USER FORM
    // Observable properties binded to the input fields of the add user form
    [ObservableProperty]
    private string _newUsername;
    [ObservableProperty]
    private string _newUserFirstName;
    [ObservableProperty]
    private string _newUserLastName;
    [ObservableProperty]
    private string _newUserRoleName;
    [ObservableProperty]
    private string _newUserPassword;
    [ObservableProperty]
    private ObservableCollection<string> _roles;

    // RoleId is set by picking User or Admin in ComboBox
    // set in SetRoleIdFromRoleName() function
    private int _newUserRoleId;
    // User object created from input fields
    // CreateNewUserObjectFromInputs()
    private User _newUserObject;
    //-------------------------------------------------------------------------------------------------------------------------------------------------------

    //=======================================================================================================================================================
    // ADD WORK POSITION FORM
    // Observable properties binded to the input fields of the add work position form
    [ObservableProperty]
    private string _newWorkPositionName;
    [ObservableProperty]
    private string _newWorkPositionDesc;

    // WorkPosition object created from input fields
    // CreateNewWorkPositionObjectFromInputs()
    private WorkPosition _newWorkPositionObject;

    //-------------------------------------------------------------------------------------------------------------------------------------------------------

    //=======================================================================================================================================================
    // DISPLAY WORK POSITIONS
    // Collections used to get and display work positions from the database 
    // Only visible in combo boxes when assigning or changing user work position
    [ObservableProperty]
    private ObservableCollection<string> _workPositionNamesFromDb;
    [ObservableProperty]
    private ObservableCollection<string> _workPositionsToShow;
    //-------------------------------------------------------------------------------------------------------------------------------------------------------

    //=======================================================================================================================================================
    // SEARCH 
    // Observable property used to implement search logic
    // Implemented in OnSearchedUsernameChanged() function
    [ObservableProperty]
    private string _searchedUsername;
    //-------------------------------------------------------------------------------------------------------------------------------------------------------

    //=======================================================================================================================================================
    // DISPLAY USERS
    // Collections used to get and display users from the database
    // Left side of admin view
    [ObservableProperty]
    private ObservableCollection<User> _usersFromDB;
    [ObservableProperty]
    private ObservableCollection<User> _usersToShow;
    //-------------------------------------------------------------------------------------------------------------------------------------------------------

    //=======================================================================================================================================================
    // DISPLAY USER WORK POSITIONS
    // Collections used to get and display user work positions from the database
    // Right side of admin view
    [ObservableProperty]
    private ObservableCollection<UserWorkPosition> _userWorkPositionFromDB;
    [ObservableProperty]
    private ObservableCollection<UserWorkPosition> _userWorkPositionsToShow;
    //-------------------------------------------------------------------------------------------------------------------------------------------------------

    //=======================================================================================================================================================
    // ADD USER WORK POSITION
    // Observable properties and private fields used to add user work position
    [ObservableProperty]
    private string _newUserWorkPositionProductName;

    // Observable property used to get the id of the work position from its name
    [ObservableProperty]
    private string _newWorkPositionNameForUserWorkPosition;

    // Set in CreateNewUserWorkPositionObject() function
    private int _newUserWorkPositionWorkPositionId;

    // Set in OnAssignWorkPositionBtnCLick() function
    private int _newUserWorkPositionUserId;

    // UserWorkPosition object created from input fields
    // CreateNewUserWorkPositionObjectFromInputs()
    private UserWorkPosition _newUserWorkPositionObject;
    //-------------------------------------------------------------------------------------------------------------------------------------------------------
    
    //=======================================================================================================================================================
    // CHANGE USER WORK POSITION
    // Observable properties and fields uset to change the user work position
    // Of a selected user
    [ObservableProperty]
    private string _changedUserWorkPositionProductName;
    [ObservableProperty]
    private string _changedUserWorkPositionName;

    //Set in OnChangeBtnClick() function
    private int _userWorkPositionToChangeId;
    //-------------------------------------------------------------------------------------------------------------------------------------------------------

    //=======================================================================================================================================================
    // ERROR MESSAGES
    // Observable properties used to display error messages
    [ObservableProperty]
    private string _addUserErrorMessage;
    [ObservableProperty]
    private string _addWorkPositionErrorMessage;
    //-------------------------------------------------------------------------------------------------------------------------------------------------------

    //=======================================================================================================================================================
    // POPUPS
    // Observable properties used to display popups
    [ObservableProperty]
    private bool _isDeletePopupVisible;
    [ObservableProperty]
    private bool _isAssignWorkPositionPopupVisible;
    [ObservableProperty]
    private bool _isChagnePopUpVisible;
    //-------------------------------------------------------------------------------------------------------------------------------------------------------
    
    //=======================================================================================================================================================
    // NAVIGATION SERVICE   
    // Used for navigation
    // WorkstationManagement/Utils/NavigationService.cs
    private NavigationService _navigationService;
    //-------------------------------------------------------------------------------------------------------------------------------------------------------

    //=======================================================================================================================================================
    // DELETE USER
    // Used to delete user
    // Set in OnDeleteBtnClick() function
    private User _userToRemove;
    //-------------------------------------------------------------------------------------------------------------------------------------------------------

    //=======================================================================================================================================================
    //  CONSTRUCTOR
    //=======================================================================================================================================================
    public AdminViewModel(NavigationService navigationService){
        _navigationService = navigationService;
        Roles = new ObservableCollection<string> { "User", "Admin" };
        LoadFromDB();
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    //=======================================================================================================================================================
    //  LOAD FROM DATABASE
    //=======================================================================================================================================================
    //
    // Loads Users, WorkPositions and UserWorkPositions from database
    // UsersToShow and UserWorkPositionToShow are used because when using search not all data
    // from the database will be displayed, only the data that is matching the search string
    // this way there is no need to read from the database after every search, just reset UsersToShow
    private void LoadFromDB(){
        using (var db = new WorkstationManagementContext())
            {
                UsersFromDB = new ObservableCollection<User>(db.Users.Include(u => u.Role).ToList());
                UsersToShow = UsersFromDB;

                WorkPositionNamesFromDb = new ObservableCollection<string>(db.WorkPositions.Select(wp => wp.Name).ToList());

                UserWorkPositionFromDB = new ObservableCollection<UserWorkPosition>(db.UserWorkPositions.Include(uwp => uwp.User).Include(uwp => uwp.WorkPosition).ToList());
                UserWorkPositionsToShow = UserWorkPositionFromDB;
            }
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    //=======================================================================================================================================================
    //  ADD AND DELETE USER
    //=======================================================================================================================================================
    //
    // Adds new User to the database
    // Reads from database after update
    // UsersFromDB.Add(_newUserObject); would remove the need to read from DB after saving
    // but the Role name is not showing in UI since the database resolves which RoleID
    // corresponds to which Role 
    // UsersFromDB is just a ObservableCollection of Users so it can not select a Role from the
    // User.RoleID
    [RelayCommand]
    public void OnAddUserBtnClick(){
        if(ValidateInputsUser())
        {
            CreateNewUserObjectFromInputs();
            using(var db = new WorkstationManagementContext())
            {
                db.Users.Add(_newUserObject);
                db.SaveChanges();
                UsersFromDB = new ObservableCollection<User>(db.Users.Include(u => u.Role).ToList());
                UsersToShow = UsersFromDB;
            }
            ClearInputFieldsUser();
            AddUserErrorMessage = "";
        }
    }

    // Opens a popup
    [RelayCommand]
    public void OnDeleteBtnClick(User userToRemove)
    {
        IsDeletePopupVisible = true;
        _userToRemove = userToRemove;
    }

    // Deletes selected User from the database
    [RelayCommand]
    public void OnConfirmDeleteBtnClick()
    {
        using(var db = new WorkstationManagementContext())
            {
                // Also removes UserWorkPosition with the UserId of the _userToRemove from the UserWorkPositions table
                db.Users.Remove(_userToRemove);          
                db.SaveChanges();
                UsersFromDB.Remove(_userToRemove);
                UsersToShow = UsersFromDB;
                UserWorkPositionFromDB = new ObservableCollection<UserWorkPosition>(db.UserWorkPositions.Include(uwp => uwp.User).Include(uwp => uwp.WorkPosition).ToList());
                UserWorkPositionsToShow = UserWorkPositionFromDB;
            }
        OnSearchedUsernameChanged(SearchedUsername);
        IsDeletePopupVisible = false;
    }

    // Close popup
    [RelayCommand]
    public void OnCancelDeleteBtnClick()
    {
        IsDeletePopupVisible = false;
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    //=======================================================================================================================================================
    // ADD WORK POSITION
    //=======================================================================================================================================================
    //
    // Adds work position to the database
    // After adding saves Names of all work posotions from database to WorkPositionNamesFromDb
    // WorkPositionNamesFromDb are displayed in a combobox inside a popup that opens when  OnAssignWorkPositionBtnCLick() is called
    [RelayCommand]
    public void OnAddWorkPositionBtnClick()
    {
        if(ValidateInputsWorkPosition())
        {
            CreateNewWorkPositionObjectFromInputs();
            using(var db = new WorkstationManagementContext())
            {
                db.WorkPositions.Add(_newWorkPositionObject);
                db.SaveChanges();
                WorkPositionNamesFromDb = new ObservableCollection<string>(db.WorkPositions.Select(wp => wp.Name).ToList());
            }
            ClearInputFieldsWorkPosition();
            AddWorkPositionErrorMessage = "New Work position created";
        }
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    //=======================================================================================================================================================
    // ADD USER WORK POSITION
    //=======================================================================================================================================================
    //
    // Opens a popup
    [RelayCommand]
    public void OnAssignWorkPositionBtnCLick(User UserToAssign)
    {
        _newUserWorkPositionUserId = UserToAssign.Id;
        IsAssignWorkPositionPopupVisible = true;
    }

    // Adds UserWorkPosition to database
    // After adding saves all UserWorkPositions from database to UserWorkPositionFromDB
    [RelayCommand]
    public void OnConfirmAssignWorkPositionBtnClick()
    {
        if(string.IsNullOrEmpty(NewWorkPositionNameForUserWorkPosition) || string.IsNullOrEmpty(NewUserWorkPositionProductName))
            IsAssignWorkPositionPopupVisible = true;
        else
        {
            // Has to be outside of db scope since it opens its own db connection
            // Logic of the function could also be implemented inside of this db scope  if needed
            // This way is just more readable
            CreateNewUserWorkPositionObjectFromInputs();
            using(var db = new WorkstationManagementContext()) 
            {
                var userWorkPosition =  db.UserWorkPositions.FirstOrDefault(uwp => uwp.UserId == _newUserWorkPositionUserId &&
                                                                            uwp.WorkPositionId == _newUserWorkPositionWorkPositionId); 
                if(userWorkPosition == null )
                {
                    db.UserWorkPositions.Add(_newUserWorkPositionObject);
                    db.SaveChanges();
                    UserWorkPositionFromDB = new ObservableCollection<UserWorkPosition>(db.UserWorkPositions.Include(uwp => uwp.User).Include(uwp => uwp.WorkPosition).ToList());
                    UserWorkPositionsToShow = UserWorkPositionFromDB;
                    IsAssignWorkPositionPopupVisible = false;
                    ClearInputFieldsUserWorkPosition();
                }
            }
        }
    }

    // Closes popup
    [RelayCommand]
    public void OnCancelAssignWorkPositionBtnClick()
    {
        IsAssignWorkPositionPopupVisible = false;
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    //=======================================================================================================================================================
    // CHANGE USER WORK POSITION
    //=======================================================================================================================================================
    //
    // Gets the UserWorkPosition as a CommandParameter and sets _userWorkPositionToChangeId to the Id of the passed userWorkPositionToChange
    // Opens a popup
    [RelayCommand]
    public void OnChangeBtnClick(UserWorkPosition userWorkPositionToChange)
    {
        IsChagnePopUpVisible = true;
        _userWorkPositionToChangeId = userWorkPositionToChange.Id;
    }

    // 
    [RelayCommand]
    public void OnConfirmChangeBtnClick()
    {
        if(string.IsNullOrEmpty(ChangedUserWorkPositionProductName) && string.IsNullOrEmpty(ChangedUserWorkPositionName))
            IsChagnePopUpVisible = true;
        else
        {
            using(var db = new WorkstationManagementContext())
            {
                var UserWorkPositionToChange = db.UserWorkPositions.FirstOrDefault(uwp => uwp.Id == _userWorkPositionToChangeId);
                UserWorkPositionToChange.Date = DateTime.Now;
                UserWorkPositionToChange.ProductName = ChangedUserWorkPositionProductName;  
                UserWorkPositionToChange.WorkPositionId = db.WorkPositions.Where(wp => wp.Name == ChangedUserWorkPositionName).Select(wp => wp.Id).FirstOrDefault();
                db.SaveChanges();
                UserWorkPositionFromDB = new ObservableCollection<UserWorkPosition>(db.UserWorkPositions.Include(uwp => uwp.User).Include(uwp => uwp.WorkPosition).ToList());
                UserWorkPositionsToShow = UserWorkPositionFromDB;
            }
            IsChagnePopUpVisible = false;
        }
    }

    // Closes popup
    [RelayCommand]
    public void OnCancelChangeBtnClick()
    {
        IsChagnePopUpVisible = false;
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
    // SEARCH
    //=======================================================================================================================================================
    //
    // Search logic using LevensteinDistanceRatio -> WorkstationManagement.Utils.Helpers.cs
    // Check if Username FirstName or LastName are similar to the value in the search bar
    // if any are similar add the user to UsersToShowS if the search bar is empty reset
    // the UsersToShow to UsersFromDb
    partial void OnSearchedUsernameChanged(string value)
    {
        if(!string.IsNullOrEmpty(value)){
            UsersToShow = [];
            foreach(User user in UsersFromDB)
            {
                bool similarUsername = Helper.ApproximatelyEquals(user.Username, value, 0.8);
                bool similarFirstName = Helper.ApproximatelyEquals(user.FirstName, value, 0.8); 
                bool similarLastName = Helper.ApproximatelyEquals(user.LastName, value, 0.8);

                if(similarUsername || similarFirstName || similarLastName)
                {
                    UsersToShow.Add(user);
                }
            }
        }
        else
            UsersToShow = UsersFromDB;
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    //=======================================================================================================================================================
    // VALIDATIONS
    //=======================================================================================================================================================
    private bool ValidateInputsUser()
    {
        if (string.IsNullOrEmpty(NewUserFirstName))
        {
            AddUserErrorMessage = "First name is empty";
            return false;
        }
        if (string.IsNullOrEmpty(NewUserLastName))
        {
            AddUserErrorMessage = "Last name is empty";
            return false;
        }
        if (string.IsNullOrEmpty(NewUsername))
        {
            AddUserErrorMessage = "Username is empty";
            return false;
        }
        if (string.IsNullOrEmpty(NewUserRoleName))
        {
            AddUserErrorMessage = "Role is empty";
            return false;
        }
        bool PasswordGood;
        // WorkstationManagement.Utils.Helpers.cs
        (AddUserErrorMessage, PasswordGood) = Helper.CheckStrength(NewUserPassword);
                bool UsernameUnique = CheckUsernameIsUnique();
        if (PasswordGood && UsernameUnique)
            return true;
        else
            return false;
    }

    private bool ValidateInputsWorkPosition()
    {
        if (string.IsNullOrEmpty(NewWorkPositionName))
        {
            AddWorkPositionErrorMessage = "Work position name is emty";
            return false;
        }
        if (string.IsNullOrEmpty(NewWorkPositionDesc))
        {
            AddWorkPositionErrorMessage = "Work position description is empty";
            return false;
        }
        bool WorkpositionUnique = CheckWorkpositionIsUnique();
        if(WorkpositionUnique)
            return true;
        else
        return false;
    }

    private bool CheckUsernameIsUnique()
    {
        using (var db = new WorkstationManagementContext())
        {
            var user =  db.Users.FirstOrDefault(u => u.Username == NewUsername);
            if(user == null)
            {
                return true;
            }
            else
            {
                AddUserErrorMessage = "Username is in use";
                return false;
            }
        }
    }

    private bool CheckWorkpositionIsUnique()
    {
        using (var db = new WorkstationManagementContext())
        {
            var workPosition =  db.WorkPositions.FirstOrDefault(wp => wp.Name == NewWorkPositionName);
            if(workPosition == null)
            {
                return true;
            }
            else
            {
                AddWorkPositionErrorMessage = "Work position already exists";
                return false;
            }
        }
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    //=======================================================================================================================================================
    // CREATE OBJECT FROM INPUTS
    //=======================================================================================================================================================
    private void CreateNewUserObjectFromInputs()
    {
        if (NewUserRoleName == "User")
            _newUserRoleId = 2;
        else 
            _newUserRoleId = 1;
        _newUserObject = new User
        {
            FirstName = NewUserFirstName,
            LastName = NewUserLastName,
            Username = NewUsername,
            // Implemented in WorkstationManagement.Utils.Helpers.cs
            Password = Helper.ComputeSha256Hash(NewUserPassword),
            RoleId = _newUserRoleId
        };
    }

    private void CreateNewWorkPositionObjectFromInputs()
    {
        _newWorkPositionObject = new WorkPosition
        {
            Name = NewWorkPositionName,
            Description = NewWorkPositionDesc
        };
    }

     private void CreateNewUserWorkPositionObjectFromInputs()
    {
        using (var db = new WorkstationManagementContext())
        {
            _newUserWorkPositionWorkPositionId = db.WorkPositions.Where(wp => wp.Name == NewWorkPositionNameForUserWorkPosition).Select(wp => wp.Id).FirstOrDefault();  
        }

        _newUserWorkPositionObject = new UserWorkPosition
        {
            ProductName = NewUserWorkPositionProductName,
            Date = DateTime.Now,
            UserId = _newUserWorkPositionUserId,
            WorkPositionId = _newUserWorkPositionWorkPositionId
        };
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    //=======================================================================================================================================================
    // CLEAR INPUTS
    //=======================================================================================================================================================
    private void ClearInputFieldsUser()
    {
        NewUserFirstName = "";
        NewUserLastName = "";
        NewUsername = "";
        NewUserPassword = "";
        NewUserRoleName = "";
    }

    private void ClearInputFieldsWorkPosition()
    {
        NewWorkPositionName = "";
        NewWorkPositionDesc = "";
    }

    private void ClearInputFieldsUserWorkPosition()
    {
        NewUserWorkPositionProductName = "";
        NewWorkPositionNameForUserWorkPosition = "";
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
}