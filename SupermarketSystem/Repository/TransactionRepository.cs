using SupermarketSystem.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SupermarketSystem.Repository
{
    class TransactionRepository
    {
        List<TransactionModel> listTransaction = new List<TransactionModel>();
        // Source=DESKTOP-UT5QPFM;Initial Catalog=marketDB;Integrated Security=True
        public List<TransactionModel> viewTransaction()
        {
            List<TransactionModel> listTransaction = new List<TransactionModel>();

            SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-UT5QPFM;Initial Catalog=marketDB;Integrated Security=True;MultipleActiveResultSets=True");
            SqlCommand command = new SqlCommand();
            SqlCommand command1 = new SqlCommand();
            SqlDataReader reader, reader1;

            string query = "SELECT transactionID, paymentMethod FROM HeaderTransaction";

            command.Connection = command1.Connection = connection;
            command.CommandType = command1.CommandType = CommandType.Text;
            command.CommandText = query;

            connection.Open();

            reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                command1.Parameters.Add("@transactionID", SqlDbType.Int);
                while (reader.Read())
                {
                    TransactionModel transaction = new TransactionModel();
                    List<ProductModel> listProduct = new List<ProductModel>();
                    command1.Parameters["@transactionID"].Value = transaction.transactionID = Convert.ToInt32(reader["transactionID"].ToString());
                    string query2 = "SELECT dtr.productID, productName, dtr.productQty, productPrice FROM DetailTransaction dtr JOIN Product p ON dtr.productID = p.productID WHERE transactionID = @transactionID";
                    command1.CommandText = query2;
                    reader1 = command1.ExecuteReader();
                    while (reader1.Read())
                    {
                        ProductModel product = new ProductModel
                        {
                            productID = Convert.ToInt32(reader1["productID"].ToString()),
                            productName = reader1["productName"].ToString(),
                            productPrice = Convert.ToInt32(reader1["productPrice"].ToString()),
                            productQty = Convert.ToInt32(reader1["productQty"].ToString())
                        };
                        listProduct.Add(product);
                    }
                    transaction.listProduct = listProduct; 
                    transaction.paymentMethod = reader["paymentMethod"].ToString();
                   
                    listTransaction.Add(transaction);

                    reader1.Close();
                }
            }

            command1.Dispose();

            reader.Close();
            connection.Close();
            command.Dispose();


            return listTransaction;
        }

        public void makeTransaction(TransactionModel transaction)
        {
            SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-UT5QPFM;Initial Catalog=marketDB;Integrated Security=True");
            SqlCommand command = new SqlCommand();

            string query = "INSERT INTO HeaderTransaction VALUES (@transactionID, @paymentMethod)";

            command.Parameters.Add("@paymentMethod", SqlDbType.VarChar, 10).Value = transaction.paymentMethod;
            command.Parameters.Add("@transactionID", SqlDbType.Int).Value = transaction.transactionID;

            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = query;

            connection.Open();

            command.ExecuteNonQuery();

            command.Parameters["@transactionID"].Value = transaction.transactionID;
            command.Parameters.Add("@productID", SqlDbType.Int);
            command.Parameters.Add("@productQty", SqlDbType.Int);

            foreach (ProductModel product in transaction.listProduct)
            {
                string query2 = "INSERT INTO DetailTransaction VALUES (@transactionID, @productID, @productQty)";

                command.Parameters["@productID"].Value = product.productID;
                command.Parameters["@productQty"].Value = product.productQty;

                command.CommandType = CommandType.Text;
                command.CommandText = query2;
                command.ExecuteNonQuery();
            }

            connection.Close();
            command.Dispose();
        }
    }
}
