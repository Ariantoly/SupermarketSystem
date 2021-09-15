using SupermarketSystem.Model;
using SupermarketSystem.Repository;
using System;
using System.Collections.Generic;

namespace SupermarketSystem
{
    class Program
    {
        ProductRepository productRepository = new ProductRepository();
        TransactionRepository transactionRepository = new TransactionRepository();
        int numOfProduct = 0, numOfTransaction = 0;
        public Program()
        {
            int roleChoice = -1, menuChoice = -1;

            numOfProduct = productRepository.viewProduct().Count;
            numOfTransaction = getNumOfTransaction();

            do
            {
                Console.Clear();
                Console.WriteLine("Supermarket System");
                Console.WriteLine("==================");
                Console.WriteLine("1. Login as User");
                Console.WriteLine("2. Login as Admin");
                Console.WriteLine("3. Exit");
                Console.Write("Choice: ");
                roleChoice = Convert.ToInt32(Console.ReadLine());

                if(roleChoice == 1)
                {
                    do
                    {
                        Console.Clear();
                        Console.WriteLine("1. View Product");
                        Console.WriteLine("2. Buy Product");
                        Console.WriteLine("3. Exit");
                        Console.Write("Choice: ");
                        menuChoice = Convert.ToInt32(Console.ReadLine());

                        if(menuChoice == 1)
                        {
                            viewProduct();
                        }
                        else if(menuChoice == 2)
                        {
                            if(numOfProduct > 0) 
                                buyProduct();
                        }
                    } while (menuChoice != 3);
                }
                else if(roleChoice == 2)
                {
                    do
                    {
                        Console.Clear();
                        Console.WriteLine("1. Insert Product");
                        Console.WriteLine("2. Update Product");
                        Console.WriteLine("3. Delete Product");
                        Console.WriteLine("4. View Product");
                        Console.WriteLine("5. View Transaction");
                        Console.WriteLine("6. Exit");
                        Console.Write("Choice: ");
                        menuChoice = Convert.ToInt32(Console.ReadLine());

                        if (menuChoice == 1)
                        {
                            insertProduct();
                        }
                        else if (menuChoice == 2)
                        {
                            if (numOfProduct > 0) updateProduct();
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("No product available!");
                                Console.Write("\nPress enter to continue...");
                                Console.ReadKey();
                            }
                        }
                        else if (menuChoice == 3)
                        {
                            if (numOfProduct > 0) deleteProduct();
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("No product available!");
                                Console.Write("\nPress enter to continue...");
                                Console.ReadKey();
                            }
                        }
                        else if (menuChoice == 4)
                        {
                            viewProduct();
                        }
                        else if (menuChoice == 5)
                        {
                            if(numOfTransaction > 0) viewTransaction();
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("No transaction!");
                                Console.Write("\nPress enter to continue...");
                                Console.ReadKey();
                            }
                        }
                    } while (menuChoice != 6);
                }

            } while (roleChoice != 3);
            
        }

        void viewProduct()
        {
            Console.Clear();

            List<ProductModel> listProduct = productRepository.viewProduct();

            if(listProduct.Count == 0)
            {
                Console.WriteLine("No product available!");
                Console.Write("\nPress enter to continue...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("View Product");
                Console.WriteLine("============");
                foreach(ProductModel product in listProduct)
                {
                    Console.WriteLine("Product ID           : " + product.productID);
                    Console.WriteLine("Product Name         : " + product.productName);
                    Console.WriteLine("Product Quantity     : " + product.productQty);
                    Console.WriteLine("Product Price        : " + product.productPrice);
                    Console.WriteLine("");
                }
                Console.Write("Press enter to continue...");
                Console.ReadKey();
            }
        }

        void buyProduct()
        {
            Console.Clear();

            List<ProductModel> listProductAvailable = productRepository.viewProduct();
            List<ProductModel> listProduct = new List<ProductModel>();
            TransactionModel transaction = new TransactionModel();
            int productID, productQty;
            string continueFlag, paymentMethod;
            int total, idx;
            Boolean flag;

            total = 0;

            Console.WriteLine("Buy Product");
            Console.WriteLine("===========");

            do
            {
                ProductModel product = new ProductModel();
                do
                {
                    do
                    {
                        Console.Write("Input Product ID [1 - {0}]: ", getLastProductID());
                        productID = Convert.ToInt32(Console.ReadLine());
                    } while (productID < 1 || productID > getLastProductID());

                    flag = searchItem(productID);

                    if (!flag) Console.WriteLine("Item has been deleted");
                } while (!flag);

                do
                {
                    idx = indexOfItem(productID);
                    Console.Write("Input Product Quantity [1 - {0}]: ", listProductAvailable[idx].productQty);
                    productQty = Convert.ToInt32(Console.ReadLine());
                } while (productQty < 1 || productQty > listProductAvailable[idx].productQty);

                product.productID = productID;
                product.productName = listProductAvailable[idx].productName;
                product.productQty = productQty;
                product.productPrice = listProductAvailable[idx].productPrice;

                listProduct.Add(product);

                total += (listProductAvailable[idx].productPrice * productQty);

                do
                {
                    Console.Write("Do you want to add another product ? [Yes | No]: ");
                    continueFlag = Console.ReadLine();
                } while (!continueFlag.Equals("Yes") && !continueFlag.Equals("No"));

            } while (continueFlag.Equals("Yes"));

            do
            {
                Console.Write("Choose payment method [Cash | Credit]: ");
                paymentMethod = Console.ReadLine();
            } while (!paymentMethod.Equals("Cash") && !paymentMethod.Equals("Credit"));

            transaction.transactionID = ++numOfTransaction;
            transaction.listProduct = listProduct;
            transaction.paymentMethod = paymentMethod;

            transactionRepository.makeTransaction(transaction);

            foreach (ProductModel product1 in listProduct)
            {
                ProductModel updatedProduct = new ProductModel();
                idx = indexOfItem(product1.productID);

                updatedProduct.productID = product1.productID;
                updatedProduct.productName = product1.productName;
                updatedProduct.productPrice = product1.productPrice;
                updatedProduct.productQty = (listProductAvailable[idx].productQty - product1.productQty);
                
                productRepository.updateProduct(updatedProduct);
            }

            listProductAvailable.Clear();

            Console.WriteLine("");
            Console.WriteLine("Rp {0} Successfully paid by Cash!", total);
            Console.Write("Press enter to continue...");
            Console.ReadKey();
            
        }

        void insertProduct()
        {
            Console.Clear();

            ProductModel product = new ProductModel();
            int productID, productPrice, productQty;
            string productName;

            Console.WriteLine("Insert Product");
            Console.WriteLine("==============");

            productID = getLastProductID() + 1;

            do
            {
                Console.Write("Input Product Name [Length between 5-20]: ");
                productName = Console.ReadLine();
            } while (productName.Length < 5 || productName.Length > 20);

            do
            {
                Console.Write("Input Product Price [1000-1000000]: ");
                productPrice = Convert.ToInt32(Console.ReadLine());
            } while (productPrice < 1000 || productPrice > 1000000);

            do
            {
                Console.Write("Input Product Quantity [1-1000]: ");
                productQty = Convert.ToInt32(Console.ReadLine());
            } while (productQty < 1 || productQty > 1000);

            product.productID = productID;
            product.productName = productName;
            product.productPrice = productPrice;
            product.productQty = productQty;

            productRepository.insertProduct(product);

            Console.WriteLine("The product has been successfully inserted!");
            Console.Write("Press enter to continue...");
            Console.ReadKey();

            numOfProduct = productRepository.viewProduct().Count;

        }

        void updateProduct()
        {
            Console.Clear();

            ProductModel product = new ProductModel();
            int productID, productPrice, productQty;
            string productName;
            Boolean flag;

            Console.WriteLine("Update Product");
            Console.WriteLine("==============");

            do
            {
                do
                {
                    Console.Write("Input Product ID [1 - {0}]: ", getLastProductID());
                    productID = Convert.ToInt32(Console.ReadLine());
                } while (productID < 1 || productID > getLastProductID());

                flag = searchItem(productID);

                if (!flag) Console.WriteLine("Item has been deleted");

            } while (!flag);

            do
            {
                Console.Write("Input Product Name [Length between 5-20]: ");
                productName = Console.ReadLine();
            } while (productName.Length < 5 || productName.Length > 20);

            do
            {
                Console.Write("Input Product Price [1000-1000000]: ");
                productPrice = Convert.ToInt32(Console.ReadLine());
            } while (productPrice < 1000 || productPrice > 1000000);

            do
            {
                Console.Write("Input Product Quantity [1-1000]: ");
                productQty = Convert.ToInt32(Console.ReadLine());
            } while (productQty < 1 || productQty > 1000);

            product.productID = productID;
            product.productName = productName;
            product.productPrice = productPrice;
            product.productQty = productQty;

            productRepository.updateProduct(product);

            Console.WriteLine("The product has been successfully updated!");
            Console.Write("Press enter to continue...");
            Console.ReadKey();
        }

        void deleteProduct()
        {
            Console.Clear();

            List<ProductModel> listProductAvailable = productRepository.viewProduct();
            int productID, idx = -1;
            string deleteFlag;
            Boolean flag;

            Console.WriteLine("Delete Product");
            Console.WriteLine("==============");

            do
            {
                do
                {
                    Console.Write("Input Product ID [1 - {0}]: ", getLastProductID());
                    productID = Convert.ToInt32(Console.ReadLine());
                } while (productID < 1 || productID > getLastProductID());

                flag = searchItem(productID);

                if (!flag) Console.WriteLine("Item has been deleted");

            } while (!flag);
            
            Console.WriteLine("");

            idx = indexOfItem(productID);

            Console.WriteLine("Product ID           : " + listProductAvailable[idx].productID);
            Console.WriteLine("Product Name         : " + listProductAvailable[idx].productName);
            Console.WriteLine("Product Quantity     : " + listProductAvailable[idx].productQty);
            Console.WriteLine("Product Price        : " + listProductAvailable[idx].productPrice);

            Console.WriteLine("");

            do
            {
                Console.Write("Are you sure want to delete this product? [Yes | No]: ");
                deleteFlag = Console.ReadLine();
            } while (!deleteFlag.Equals("Yes") && !deleteFlag.Equals("No"));

            if (deleteFlag.Equals("Yes"))
            {
                productRepository.deleteProduct(productID);
                Console.WriteLine("The product has been successfully deleted!");
            }

            Console.Write("Press enter to continue...");
            Console.ReadKey();

            numOfProduct = productRepository.viewProduct().Count;

            listProductAvailable.Clear();

        }

        void viewTransaction()
        {
            Console.Clear();

            List<TransactionModel> listTransaction = transactionRepository.viewTransaction();
            int total, i;

            Console.WriteLine("View Transactions");
            Console.WriteLine("=================");
            
            foreach(TransactionModel transaction in listTransaction)
            {
                i = total = 0;
                Console.WriteLine("Transaction ID           : " + transaction.transactionID);
                Console.WriteLine("|No |    Product Name    | Quantity |  Price  |");
                foreach(ProductModel product in transaction.listProduct)
                {
                    Console.WriteLine("|{0,-3}| {1,-19}|{2,10}|  {3,-7}|", ++i, product.productName, product.productQty, product.productPrice);
                    total += (product.productPrice * product.productQty);                
                }

                Console.WriteLine("");
                Console.WriteLine("Grand Total              : {0} by {1}", total, transaction.paymentMethod);
                Console.WriteLine("");
            }

            Console.Write("Press enter to continue...");
            Console.ReadKey();
        }

        int getLastProductID()
        {
            return productRepository.viewProduct()[productRepository.viewProduct().Count - 1].productID;
        }

        int getNumOfTransaction()
        {
            return transactionRepository.viewTransaction().Count;
        }

        Boolean searchItem(int productID)
        {
            List<ProductModel> listProductAvailable = productRepository.viewProduct();
            for (int i = 0; i < numOfProduct; i++)
                if (listProductAvailable[i].productID == productID)
                    return true;

            return false;
        }

        int indexOfItem(int productID)
        {
            List<ProductModel> listProductAvailable = productRepository.viewProduct();
            for (int i = 0; i < numOfProduct; i++)
                if (listProductAvailable[i].productID == productID)
                    return i;

            return -1;
        }

        static void Main(string[] args)
        {
            new Program();
        }
    }
}
