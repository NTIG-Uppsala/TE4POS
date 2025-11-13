using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TE4POS
{
    public class Products : INotifyPropertyChanged
    {
        public Products()
        {
            _allProducts = new ObservableCollection<Product>()
            {
                new Product() {id = 1, name = "kaffe", price = 49, amount = 1},
                new Product() {id = 2, name = "wawa", price = 1, amount = 0}
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<Product> _allProducts;

        public ObservableCollection<Product> AllProducts
        {
            get { return _allProducts; }
            set
            {
                _allProducts = value;
                OnPropertyChanged(nameof(AllProducts));
            }
        }
    }
}
