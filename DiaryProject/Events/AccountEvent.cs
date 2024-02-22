namespace DiaryProject.Events;

public enum UserOperation
{
    SuccessfullyLogin,
    ExitAccount,
    LocalMode
    
}

public class AccountModel
{
    public UserOperation Operation { get; set; }
    public string Token { get; set; }
}

public class AccountEvent : PubSubEvent<AccountModel>;