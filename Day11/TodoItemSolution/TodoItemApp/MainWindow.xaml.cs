using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TodoItemApp.Models;
using System.Net.Http.Headers;
using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics;

namespace TodoItemApp
{
    public class DivCode
    {
        public string Key { get; set; }
        public string Value { get; set; } 
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private List<DivCode> divCodes= new List<DivCode>();
        HttpClient client = new HttpClient();
        TodoItemsCollection todoItems = new TodoItemsCollection();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            divCodes.Add(new DivCode { Key = "True", Value = "1" });
            divCodes.Add(new DivCode { Key = "False", Value = "0" });
            CboIsComplete.ItemsSource=divCodes;
            CboIsComplete.DisplayMemberPath = "Key"; //콤보박스에 True/False 추가

            //yyyy-MM--dd HH:mm:ss 
            DtpTodoDate.Culture = new System.Globalization.CultureInfo("ko-KR");
            //RestAPI 기본 URI 호출
            client.BaseAddress = new Uri("https://localhost:7188/");//RestAPI 서버 기본 URL
            //헤더 설정
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            GetData();//데이터 로드 메서드 호출
        }

        private async void GetData()
        {
            //데이터그리드 ItemsSource에 todoItems 바인딩
            GrdTodoItems.ItemsSource = todoItems;
            //Api 호출 핵심
            try
            {
                HttpResponseMessage? response = await client.GetAsync("api/TodoItems");
                response.EnsureSuccessStatusCode();

                var items = await response.Content.ReadAsAsync<IEnumerable<TodoItem>>();
                todoItems.CopyForm(items);
            }
            catch (Newtonsoft.Json.JsonException jEx)
            {
                await (this.ShowMessageAsync("error", jEx.Message, MessageDialogStyle.Affirmative, new MetroDialogSettings()
                { AnimateShow = true, AnimateHide = true }));
            }
            catch (HttpRequestException ex)
            {
                await (this.ShowMessageAsync("error", ex.Message, MessageDialogStyle.Affirmative, new MetroDialogSettings()
                { AnimateShow = true, AnimateHide = true }));
            }
        }

        private async void BtnInsert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var todoItem = new TodoItem()
                {
                    Id = 0,
                    Title = TxtTitle.Text,
                    TodoDate = ((DateTime)DtpTodoDate.SelectedDateTime).ToString("yyyy-MM-dd HH:mm:ss"),
                    IsComplete = Int32.Parse((CboIsComplete.SelectedItem as DivCode).Value)
                };

                var response = await client.PostAsJsonAsync("api/TodoItems", todoItem);
                response.EnsureSuccessStatusCode();

                GetData();

                TxtId.Text = TxtTitle.Text = string.Empty;
                CboIsComplete.SelectedIndex = -1;
            }
            catch (Newtonsoft.Json.JsonException jEx)
            {
                await (this.ShowMessageAsync("error", jEx.Message, MessageDialogStyle.Affirmative, new MetroDialogSettings()
                { AnimateShow = true, AnimateHide = true }));
            }
            catch (HttpRequestException ex)
            {
                await (this.ShowMessageAsync("error", ex.Message, MessageDialogStyle.Affirmative, new MetroDialogSettings()
                { AnimateShow = true, AnimateHide = true }));
            }
        }

        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var todoItem = new TodoItem()
                {
                    Id = Int32.Parse(TxtId.Text),
                    Title = TxtTitle.Text,
                    TodoDate = ((DateTime)DtpTodoDate.SelectedDateTime).ToString("yyyy-MM-dd HH:mm:ss"),
                    IsComplete = Int32.Parse((CboIsComplete.SelectedItem as DivCode).Value)
                };
                //Update할때는 Put
                var response = await client.PutAsJsonAsync($"api/TodoItems/{todoItem.Id}", todoItem);
                response.EnsureSuccessStatusCode();

                GetData();

                TxtId.Text = TxtTitle.Text = string.Empty;
                CboIsComplete.SelectedIndex = -1;
            }
            catch (Newtonsoft.Json.JsonException jEx)
            {
                await(this.ShowMessageAsync("error", jEx.Message, MessageDialogStyle.Affirmative, new MetroDialogSettings()
                { AnimateShow = true, AnimateHide = true }));
            }
            catch (HttpRequestException ex)
            {
                await(this.ShowMessageAsync("error", ex.Message, MessageDialogStyle.Affirmative, new MetroDialogSettings()
                { AnimateShow = true, AnimateHide = true }));
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var Id = Int32.Parse(TxtId.Text);
                    
                //Update할때는 Put
                var response = await client.DeleteAsync($"api/TodoItems/{Id}");
                response.EnsureSuccessStatusCode();

                GetData();

                TxtId.Text = TxtTitle.Text = string.Empty;
                CboIsComplete.SelectedIndex = -1;
            }
            catch (Newtonsoft.Json.JsonException jEx)
            {
                await(this.ShowMessageAsync("error", jEx.Message, MessageDialogStyle.Affirmative, new MetroDialogSettings()
                { AnimateShow = true, AnimateHide = true }));
            }
            catch (HttpRequestException ex)
            {
                await(this.ShowMessageAsync("error", ex.Message, MessageDialogStyle.Affirmative, new MetroDialogSettings()
                { AnimateShow = true, AnimateHide = true }));
            }
        }

        private async void GrdTodoItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var Id = ((TodoItem)GrdTodoItems.SelectedItem).Id;
                HttpResponseMessage? response = await client.GetAsync($"api/TodoItems/{Id}");
                response.EnsureSuccessStatusCode();

                var item = await response.Content.ReadAsAsync<TodoItem>();
                TxtId.Text = item.Id.ToString();
                TxtTitle.Text = item.Title;
                DtpTodoDate.SelectedDateTime = DateTime.Parse(item.TodoDate);
                //1인 경우에는 0번을 인덱스를 할당 즉 True이면 콤보박스의 0번째(True)를 할당 , 그 외는 1번(False)
                CboIsComplete.SelectedIndex = item.IsComplete == 1 ? 0 : 1;
            }
            catch (Newtonsoft.Json.JsonException jEx)
            {
                await(this.ShowMessageAsync("error", jEx.Message, MessageDialogStyle.Affirmative, new MetroDialogSettings()
                { AnimateShow = true, AnimateHide = true }));
            }
            catch (HttpRequestException ex)
            {
                await(this.ShowMessageAsync("error", ex.Message, MessageDialogStyle.Affirmative, new MetroDialogSettings()
                { AnimateShow = true, AnimateHide = true }));
            }
            catch(Exception ex)
            {
                //아래 비동기 메시지 주석은 화면에 출력하지 않기 위해 Debug.WriteLine으로 뺀것
                //await(this.ShowMessageAsync("error", ex.Message, MessageDialogStyle.Affirmative, new MetroDialogSettings()
                //{ AnimateShow = true, AnimateHide = true }));
                Debug.WriteLine($"이외 예외 {ex.Message}");
            }
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            GetData();
        }
    }
}