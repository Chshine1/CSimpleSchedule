namespace DiaryProject.Events;

public enum UserOperation
{
    SuccessfullyLogin,
    ExitAccount,
    LocalMode
}

public class AccountEvent : PubSubEvent<UserOperation>;