using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WPFShopOrderOverview
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IQueryable<customer> customerList;

        public IQueryable<order> orderList;
        public MainWindow()
        {
            InitializeComponent();
            var context = new ShopEntities();

            // gets all cust from database set which represent all data in cust table. 
            customerList = from cust in context.customers select cust;

            // populate combobox with customer data.  
            custBox.ItemsSource = customerList.ToList();
            // value of customer_fname will be displayed in combox  
            custBox.DisplayMemberPath = "customer_fname";
            custBox.SelectedValuePath = "customer_id";


            // skal ikke bruges alligevel: 
            orderDetailsView.SelectedValuePath = "product_id";

        }


        private void CustBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // empty order details when changing cust 
            orderDetailsView.ItemsSource = null;

            // gets value from chosen item in combobox which matches the id of the cust chosen.  
            var selectedCust = custBox.SelectedValue;
            using (var context = new ShopEntities())
            {
                // gets cust chosen in combobox picked from customerList                 
                var empChosen = customerList.FirstOrDefault(x => x.customer_id.Equals((int)selectedCust));

                // populate textboxes with cust data   
                txtFirst.Text = empChosen.customer_fname.ToString();
                txtLast.Text = empChosen.customer_lname.ToString();
                txtMail.Text = empChosen.customer_email.ToString();

                // gets cust orders where custid match cust chosen in combobox 
                orderList = context.orders.Where(o => o.customer_id == (int)selectedCust);

                listOrders.Items.Clear();

                // Adds all orders of cust to Listbox   
                foreach (order ord in orderList)
                {
                    listOrders.Items.Add(ord.order_id);
                }


            }
        }

        private void updateOrderDetails(int orderID)
        {
            using (var context = new ShopEntities())
            {
                // joiner de forskellige database set sammen, for at få fat i de properties jeg skal bruge.  
                var ordDetails = from ordDet in context.orderDetails
                                 join ord in context.orders on ordDet.order_id equals ord.order_id
                                 join prod in context.products on ordDet.product_id equals prod.product_id
                                 where ordDet.order_id == orderID
                                 select new { ordDet.quantity, prod.product_name, ordDet.combined_price, ordDet.order_detail_entry_ID, prod.product_id, ord.order_total_price };

                // populater min ListView med dataet 
                orderDetailsView.ItemsSource = ordDetails.ToList();
            }
        }

        private void ListOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // when changing cust in combobox the list of orders selected value is set to null and this 
            // method gets called and causes a error, if this statement doesnt stop it.   
            if (listOrders.SelectedValue != null)
            {
                int orderChosen = (int)listOrders.SelectedValue;

                updateOrderDetails(orderChosen);
            }
        }
        // hej 

        // bare slet følgende  // suus

        //private void updateProductInfo(int orderID) 
        //{
        //    using (var context = new ShopEntities())
        //    {
        //        // joiner de forskellige database set sammen, for at få fat i de properties jeg skal bruge.  
        //        var ordDetails = from ordDet in context.orderDetails
        //                         join ord in context.orders on ordDet.order_id equals ord.order_id
        //                         join prod in context.products on ordDet.product_id equals prod.product_id
        //                         where ordDet.order_id == orderID
        //                         select new { ordDet.quantity, prod.product_name, ordDet.combined_price, ordDet.order_detail_entry_ID, prod.product_id, ord.order_total_price };

        //        // populater min ListView med dataet 
        //        orderDetailsView.ItemsSource = ordDetails.ToList();
        //    }
        //}

        //private void OrderDetailsView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (orderDetailsView.SelectedValue != null)
        //    {
        //        int productChosen = (int)orderDetailsView.SelectedValue;
        //    }
        //}  
    }
}
