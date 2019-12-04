using FacebookIDE.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FacebookIDE
{
    public class RequestTemplateSelector:DataTemplateSelector
    {
        public DataTemplate SingleRequestTemplate { get; set; }
        public DataTemplate CodeRequestTemplate { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(((StoredRequest)item).Type == RequestType.HTTP)
            {
                return SingleRequestTemplate;
            }
            else
            {
                return CodeRequestTemplate;
            }
        }
    }
}
