using SupermarketSystem.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SupermarketSystem.Repository
{
    public class ProductRepository
    {
        //Data Source=DESKTOP-UT5QPFM;Initial Catalog=marketDB;Integrated Security=True

        public List<ProductModel> viewProduct()
        {
            List<ProductModel> listProduct = new List<ProductModel>();

            SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-UT5QPFM;Initial Catalog=marketDB;Integrated Security=True");
            SqlCommand command = new SqlCommand();
            SqlDataReader reader;

            string query = "SELECT * FROM Product";

            command.Connection = connection;
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = query;

            connection.Open();

            reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ProductModel product = new ProductModel();
                    product.productID = Convert.ToInt32(reader["productID"].ToString());
                    product.productName = reader["productName"].ToString();
                    product.productPrice = Convert.ToInt32(reader["productPrice"].ToString());
                    product.productQty = Convert.ToInt32(reader["productQty"].ToString());

                    listProduct.Add(product);
                }
            }

            reader.Close();
            connection.Close();
            command.Dispose();

            return listProduct;
        }

        public void insertProduct(ProductModel product)
        { 
            SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-UT5QPFM;Initial Catalog=marketDB;Integrated Security=True");
            SqlCommand command = new SqlCommand();

            string query = "INSERT INTO Product VALUES (@productID, @productName, @productPrice, @productQty)";

            command.Parameters.Add("@productID", SqlDbType.Int).Value = product.productID;
            command.Parameters.Add("@productName", SqlDbType.VarChar, 20).Value = product.productName;
            command.Parameters.Add("@productPrice", SqlDbType.Int).Value = product.productPrice;
            command.Parameters.Add("@productQty", SqlDbType.Int).Value = product.productQty;

            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = query;

            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();
            command.Dispose();
        }

        public void updateProduct(ProductModel product)
        {
            SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-UT5QPFM;Initial Catalog=marketDB;Integrated Security=True");
            SqlCommand command = new SqlCommand();

            string query = "UPDATE Product SET productName = @productName, productPrice = @productPrice, productQty = @productQty WHERE productID = @productID";
            command.Parameters.Add("@productID", SqlDbType.Int).Value = product.productID;
            command.Parameters.Add("@productName", SqlDbType.VarChar, 20).Value = product.productName;
            command.Parameters.Add("@productPrice", SqlDbType.Int).Value = product.productPrice;
            command.Parameters.Add("@productQty", SqlDbType.Int).Value = product.productQty;

            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = query;

            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();
            command.Dispose();
        }

        public void deleteProduct(int productID)
        {
            SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-UT5QPFM;Initial Catalog=marketDB;Integrated Security=True");
            SqlCommand command = new SqlCommand();

            string query = "DELETE FROM Product WHERE productID = @productID";
            command.Parameters.Add("@productID", SqlDbType.Int).Value = productID;

            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = query;

            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();
            command.Dispose();
        }

    }

    
}
