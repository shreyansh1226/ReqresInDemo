using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReqresInApiClient.Exceptions;
using ReqresInApiClient.Extensions;
using ReqresInApiClient.Options;
using ReqresInApiClient.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReqresInDemo.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddReqresInClient(context.Configuration);
                })
                .Build();

            var service = host.Services.GetRequiredService<IReqresInService>();

            try
            {
                var users = await service.GetAllUsersAsync();
                foreach (var userList in users)
                {
                    System.Console.WriteLine($"ID: {userList.Id}");
                    System.Console.WriteLine($"Name: {userList.First_Name} {userList.Last_Name}");
                    System.Console.WriteLine($"Email: {userList.Email}");
                    System.Console.WriteLine($"Avatar: {userList.Avatar}");
                    System.Console.WriteLine(); 
                }
                System.Console.WriteLine();

                var user = await service.GetUserByIdAsync(2);
                System.Console.WriteLine($"User 2 details:");
                System.Console.WriteLine($"- Name: {user.First_Name} {user.Last_Name}");
                System.Console.WriteLine($"- Email: {user.Email}");
                System.Console.WriteLine($"- Avatar: {user.Avatar}");
                System.Console.WriteLine();

                var user999 = await service.GetUserByIdAsync(999); 
            }
            catch (UserNotFoundException ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }
}