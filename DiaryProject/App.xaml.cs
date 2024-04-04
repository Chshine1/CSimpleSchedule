using System.Diagnostics.CodeAnalysis;
using System.Windows;
using AutoMapper;
using DiaryProject.Service;
using DiaryProject.Service.Local;
using DiaryProject.Service.Web;
using DiaryProject.Utils;
using DiaryProject.ViewModels;
using DiaryProject.Views;
using DryIoc;
using Prism.DryIoc;

namespace DiaryProject;

[SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
public partial class App : PrismApplication
{
    public static bool IsUserRegistered { get; set; }

    public static bool IsSynchronizing { get; set; }

    public static string? UserToken { get; set; }

    public static bool IsHoverVisible { get; set; }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.GetContainer()
            .Register<HttpRestClient>(made: Parameters.Of.Type<string>(serviceKey: "webUrl"));
        containerRegistry.RegisterInstance("https://localhost:44319/", "webUrl");

        var mapperConfig = new MapperConfiguration(c => c.AddProfile(new ClientMapperProfile()));
        containerRegistry.RegisterInstance(mapperConfig.CreateMapper());
        
        containerRegistry.RegisterSingleton<TimerService>();
        
        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        containerRegistry.GetContainer().Register<IMemoLocalRepository,MemoLocalRepository>(made: Parameters.Of.Type<string>(serviceKey: "localFolder"));
        containerRegistry.RegisterInstance(path + @"\CSimpleSchedule\visitor", "localFolder");
        containerRegistry.Register<IMemoService, MemoService>();
        containerRegistry.Register<IUserService, UserService>();
        containerRegistry.GetContainer()
            .Register<FileCopyService>(made: Parameters.Of.Type<string>(serviceKey: "localFolder"));
        
        containerRegistry.RegisterForNavigation<LoginView, LoginViewModel>();
        containerRegistry.RegisterForNavigation<CalendarView, CalendarViewModel>();
        containerRegistry.RegisterForNavigation<MemoEditorView, MemoEditorViewModel>();
        containerRegistry.RegisterForNavigation<UserView, UserViewModel>();
        containerRegistry.RegisterForNavigation<HoverSettingsView, HoverSettingsViewModel>();
    }

    protected override Window CreateShell()
    {
        return Container.Resolve<MainView>();
    }
}