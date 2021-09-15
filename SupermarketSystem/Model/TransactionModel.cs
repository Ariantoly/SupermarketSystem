using System;
using System.Collections.Generic;
using System.Text;

namespace SupermarketSystem.Model
{
    public class TransactionModel
    {
        public int transactionID { get; set; }
        public List<ProductModel> listProduct { get; set; }
        public string paymentMethod { get; set; }
    }
}
