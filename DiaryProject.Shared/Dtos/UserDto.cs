namespace DiaryProject.Shared.Dtos;

public class UserDto : BaseDto
{
    private string _userName;
    private string _password;

    public string UserName
    {
        get => _userName;
        set
        {
            _userName = value;
            OnPropertyChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
        }
    }
}