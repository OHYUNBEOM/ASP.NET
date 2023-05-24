using aspnet02_boardapp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace aspnet02_boardapp
{
    public class Program
    {
        //ASP.NET ������ ���� ���� �ʱ�ȭ
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //Data���� / ApplicationDbContext�� ����ϰڴٴ� ���� �߰�
            builder.Services.AddDbContext<ApplicationDbContext>(options=>options.UseMySql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
                ));
            //ASP.NET ������ ���� ���� ����
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // ��й�ȣ ��å ���� ����
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;//������ �ʿ俩��
                options.Password.RequireLowercase = false;//�ҹ��� �ʿ俩��
                options.Password.RequireUppercase = false;//�빮�� �ʿ俩��
                options.Password.RequireNonAlphanumeric = false;//Ư������ �ʿ俩��
                options.Password.RequiredLength = 4;//�ּ� �н����� ���� ��
                options.Password.RequiredUniqueChars = 0;//��ȣ �������� ����
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();//�����߰�
            app.UseAuthorization();//����

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}