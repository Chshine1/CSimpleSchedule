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

    public static string? UserToken { get; set; }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Register HttpRestClient, to be injected to the services
        containerRegistry.GetContainer()
            .Register<HttpRestClient>(made: Parameters.Of.Type<string>(serviceKey: "webUrl"));
        containerRegistry.RegisterInstance("https://localhost:44319/", "webUrl");

        var mapperConfig = new MapperConfiguration(c => c.AddProfile(new ClientMapperProfile()));
        containerRegistry.RegisterInstance(mapperConfig.CreateMapper());
        
        containerRegistry.RegisterSingleton<TimerService>();
        
        containerRegistry.GetContainer().Register<IMemoLocalRepository,MemoLocalRepository>(made: Parameters.Of.Type<string>(serviceKey: "localFolder"));
        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        containerRegistry.RegisterInstance(path + @"\CSimpleSchedule\visitor\save.db", "localFolder");
        containerRegistry.Register<IMemoService, MemoService>();
        containerRegistry.Register<IUserService, UserService>();
        
        containerRegistry.RegisterForNavigation<LoginView, LoginViewModel>();
        containerRegistry.RegisterForNavigation<CalendarView, CalendarViewModel>();
        containerRegistry.RegisterForNavigation<MemoEditorView, MemoEditorViewModel>();
        containerRegistry.RegisterForNavigation<UserView, UserViewModel>();
    }

    protected override Window CreateShell()
    {
        Exit += (_, _) =>
        {
            if (!IsUserRegistered) return;
            var repository = Container.Resolve<IMemoLocalRepository>();
            repository.DropLogsDatabase();
        };
        return Container.Resolve<MainView>();
    }
}