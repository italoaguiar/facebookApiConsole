using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookIDE.Model
{
    public class ParameterCollection: ObservableCollection<APIParameter>
    {
        public ParameterCollection() : base()
        {

        }

        public ParameterCollection(IEnumerable<APIParameter> i): base(i)
        {

        }

        public new void Add(APIParameter item)
        {
            item.PropertyChanged += (s, a) => ItemChanged?.Invoke(this, new EventArgs());
            base.Add(item);
        }

        public APIParameter this[string name] => Items.FirstOrDefault(x=> x.Name == name);

        public event EventHandler ItemChanged;
    }
}
