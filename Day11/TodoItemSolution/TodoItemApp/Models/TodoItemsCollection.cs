using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace TodoItemApp.Models
{
    public class TodoItemsCollection : ObservableCollection<TodoItem>
    {
        public void CopyForm(IEnumerable<TodoItem> todoItems)
        {
            this.Items.Clear();//초기화
            foreach (TodoItem item in todoItems)
            {
                this.Items.Add(item);//데이터 추가
            }
            //데이터 변경을 알림
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
