using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using AutoMapper;
using DiaryProject.Events;
using DiaryProject.Models;
using DiaryProject.Service;
using DiaryProject.Service.Local;
using DiaryProject.Service.Web;
using DiaryProject.Shared.Dtos;
using DiaryProject.Utils;

namespace DiaryProject.ViewModels;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class LoginViewModel : NavigationModel
{
    private readonly IUserService _userService;
    private readonly IMemoLocalRepository _memoRepository;
    private readonly IMapper _mapper;
    private readonly TimerService _timerService;

    #region BoundProperties

    private string _userName = "";
    private bool _userHintEnabled = true;
    private string _password = "";
    private bool _passwordHintEnabled = true;
    
    public string UserName
    {
        get => _userName;
        set
        {
            if (value.Contains((char) 0x0d))
            {
                FailureStatus = "*不合规的用户名";
                return;
            }
            if (string.IsNullOrEmpty(value)) _userHintEnabled = true;
            _userName = value;
            RaisePropertyChanged(nameof(UserHintVisible));
        }
    }
    
    public string Password 
    { 
        get => _password;
        set
        {
            if (value.Contains((char) 0x0d))
            {
                FailureStatus = "*不合规的密码";
                return;
            }
            if (string.IsNullOrEmpty(value)) _passwordHintEnabled = true;
            _password = value;
            RaisePropertyChanged(nameof(PasswordHintVisible));
        }
    }
    public string FailureStatus { get; set; }

    public Visibility UserHintVisible => _userHintEnabled ? Visibility.Visible : Visibility.Hidden;
    public Visibility PasswordHintVisible => _passwordHintEnabled ? Visibility.Visible : Visibility.Hidden;
    public DelegateCommand LoginCommand { get; private init; }
    public DelegateCommand RegisterCommand { get; private set; }
    public DelegateCommand LocalModeCommand { get; private init; }
    public DelegateCommand<string> GotFocusCommand { get; private init; }

    #endregion

    public LoginViewModel(IUserService userService, IEventAggregator aggregator, IMemoLocalRepository memoRepository, IMapper mapper, TimerService timerService) : base(aggregator)
    {
        _userService = userService;
        _memoRepository = memoRepository;
        _mapper = mapper;
        _timerService = timerService;

        UserName = "";
        Password = "";
        FailureStatus = "";
        
        LoginCommand = new DelegateCommand(Login);
        RegisterCommand = new DelegateCommand(Register);
        LocalModeCommand = new DelegateCommand(() =>
        {
            App.IsUserRegistered = false;
            App.IsSynchronizing = false;
            Aggregator.UpdateUserStatus(UserOperation.LocalMode, string.Empty);
            Initialize();
        });
        GotFocusCommand = new DelegateCommand<string>(t =>
        {
            switch (t)
            {
                case "UserName":
                    _userHintEnabled = false;
                    RaisePropertyChanged(nameof(UserHintVisible));
                    break;
                case "Password":
                    _passwordHintEnabled = false;
                    RaisePropertyChanged(nameof(PasswordHintVisible));
                    break;
            }
            FailureStatus = string.Empty;
            RaisePropertyChanged(nameof(FailureStatus));
        });
    }

    #region PrivateMethods

    private async void Login()
    {
        Aggregator.UpdateLoadingStatus(true);
        
        var loginResult = await _userService.LoginAsync(UserName, Password);
        if (!loginResult.Status)
        {
            FailureStatus = loginResult.Connected ? "*用户名或密码错误" : "*未连接到服务器，请考虑使用本地模式";
            RaisePropertyChanged(nameof(FailureStatus));
            Aggregator.UpdateLoadingStatus(false);
            return;
        }
        FailureStatus = string.Empty;
        RaisePropertyChanged(nameof(FailureStatus));
        
        App.IsUserRegistered = true;
        App.IsSynchronizing = true;
        App.UserToken = loginResult.Result;
        
        Debug.Assert(loginResult.Result != null, "loginResult.Result != null");
        Aggregator.UpdateUserStatus(UserOperation.SuccessfullyLogin, loginResult.Result);
        
        Initialize();
        
        Aggregator.UpdateLoadingStatus(false);
    }

    private async void Register()
    {
        Aggregator.UpdateLoadingStatus(true);

        var loginResult = await _userService.RegisterAsync(new UserDto { UserName = UserName, Password = Password });
        if (!loginResult.Status)
        {
            FailureStatus = loginResult.Connected ? "*用户名已存在" : "*未连接到服务器，请考虑使用本地模式";
            RaisePropertyChanged(nameof(FailureStatus));
            Aggregator.UpdateLoadingStatus(false);
            return;
        }
        FailureStatus = string.Empty;
        RaisePropertyChanged(nameof(FailureStatus));
        
        App.IsUserRegistered = true;
        App.IsSynchronizing = true;
        App.UserToken = loginResult.Result;
        
        Debug.Assert(loginResult.Result != null, "loginResult.Result != null");
        Aggregator.UpdateUserStatus(UserOperation.SuccessfullyLogin, loginResult.Result);
        
        Initialize();
        
        Aggregator.UpdateLoadingStatus(false);
    }

    private async void Initialize()
    {
        /* TODO:Configure local database */
        var query = (await _memoRepository.GetAllAsync()).Result;
        if (query == null) return;
        var waitList = (from memo in query select _mapper.Map<MemoRecord>(memo)).ToArray();
        if (waitList.Length == 0) return;
        
        foreach (var memo in waitList)
        {
            _timerService.RegisterToTimers(memo);
        }
    }
    
    #endregion
}