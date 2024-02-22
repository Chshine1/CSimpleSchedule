using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using DiaryProject.Events;
using DiaryProject.Models;
using DiaryProject.Service;
using DiaryProject.Service.Local;
using DiaryProject.Service.Web;
using DiaryProject.Shared.Contact;
using DiaryProject.Shared.Dtos;
using DiaryProject.Shared.Parameters;
using DiaryProject.Utils;

namespace DiaryProject.ViewModels;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class LoginViewModel : NavigationModel
{
    private readonly IUserService _userService;
    private readonly IMemoLocalRepository _memoRepository;
    private readonly TimerService _timerService;
    private readonly IMapper _mapper;
    private readonly IMemoService _memoService;
    
    public string UserName { get; set; }
    public string Password { get; set; }

    public DelegateCommand LoginCommand { get; private init; }

    public DelegateCommand LocalModeCommand { get; private init; }

    public LoginViewModel(IMemoLocalRepository memoRepository, TimerService timerService, IMapper mapper, IRegionManager regionManager, IUserService userService, IEventAggregator aggregator, IMemoService memoService) : base(aggregator)
    {
        _memoRepository = memoRepository;
        _timerService = timerService;
        _mapper = mapper;
        _userService = userService;
        _memoService = memoService;
        UserName = "";
        Password = "";
        LoginCommand = new DelegateCommand(Login);
        LocalModeCommand = new DelegateCommand(() =>
        {
            App.IsUserRegistered = false;
            Aggregator.ChangeUserStatus(UserOperation.LocalMode, string.Empty);
            RegisterTimers(_memoRepository, _timerService, _mapper);
        });
    }

    private async void Login()
    {
        Aggregator.UpdateLoadingStatus(true);
        var loginResult = await _userService.LoginAsync(UserName, Password);
        if (!loginResult.Status)
        {
            Aggregator.UpdateLoadingStatus(false);
            return;
        }
        App.IsUserRegistered = true;
        App.UserToken = loginResult.Result;
        var sycThread = new Thread(SynchronizingWithServer);
        sycThread.Start();
        Aggregator.ChangeUserStatus(UserOperation.SuccessfullyLogin, loginResult.Result);
        RegisterTimers(_memoRepository, _timerService, _mapper);
        Aggregator.UpdateLoadingStatus(false);
    }

    private async void RegisterTimers(IMemoLocalRepository memoRepository, TimerService timerService, IMapper mapper)
    {
        var query = (await memoRepository.GetAllAsync()).Result;
        var waitList = (from memo in query select mapper.Map<MemoRecord>(memo)).ToArray();
        if (waitList.Length == 0) return;
        
        foreach (var memo in waitList)
        {
            timerService.RegisterToTimers(memo);
        }
    }

    private async void SynchronizingWithServer()
    {
        var logs = await _memoRepository.GetLogs();
        foreach (var log in logs)
        {
            try
            {
                switch (log.Operation)
                {
                    case 0:
                        await _memoService.AddAsync((await _memoRepository.GetFirstOrDefaultAsync(log.EntityId)).Result);
                        break;
                    case 1:
                        await _memoService.UpdateAsync((await _memoRepository.GetFirstOrDefaultAsync(log.EntityId)).Result);
                        break;
                    case 2:
                        await _memoService.DeleteAsync(log.EntityId);
                        break;
                    case 3:
                        if (!log.Exists)
                        {
                            var m = await _memoService.AddAsync(new MemoDto());
                            _memoService.DeleteAsync(m.Result.Id);
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(log.Operation), log.Operation, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}