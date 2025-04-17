using static MiniMart.Application.DTO.BankLinkDto;
using static MiniMart.Application.DTO.OrderDto;

namespace MiniMart.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var orders = new List<ProductRequest>();

            while (true)
            {
                Console.Write("Do you wish to Continue [y/n]: ");
                string y = Console.ReadLine().Trim();
                if (y == "y" || y == "Y")
                {
                    Start:
                    Console.Clear();
                    Console.WriteLine("1 to View available goods");
                    Console.WriteLine("2 to Purchase goods");
                    Console.WriteLine("3 to exit");
                    Console.WriteLine();
                    Console.Write(">>> ");

                    string input = Console.ReadLine().Trim();

                    bool isRightValue = int.TryParse(input, out int inputValue);

                    if (!isRightValue)
                    {
                        Console.WriteLine("Please enter a valid input");
                        goto Start;
                    }

                    Console.WriteLine();

                    if (inputValue == 1)
                    {
                        Console.Clear();
                        MiniMartConsole.GetProduct().Wait();
                    }
                    else if (inputValue == 2)
                    {
                        Console.Clear();
                        MiniMartConsole.GetProduct().Wait();

                    ItemSelection:
                        Console.Write("Enter Item S/N: ");
                        bool isValidId = int.TryParse(Console.ReadLine().Trim(), out int itemId);
                        if (!isValidId)
                        {
                            Console.WriteLine("Please enter a digit");
                            goto ItemSelection;
                        }

                    Quantity:
                        Console.Write("Enter Quantity: ");
                        bool isValidQuantity = int.TryParse(Console.ReadLine().Trim(), out int quantity);

                        if (!isValidQuantity)
                        {
                            Console.WriteLine("Please enter a digit");
                            goto Quantity;
                        }

                        var isIdPresent = MiniMartConsole.ProductMapping.TryGetValue(itemId, out var productMapping);

                        if (!isIdPresent)
                        {
                            Console.WriteLine("Invalid serial number, kindly try again");
                            goto ItemSelection;
                        }

                        orders.Add(new ProductRequest
                        {
                            ProductId = productMapping,
                            Quantity = quantity
                        });

                    NewItemSelection:
                        Console.Write("Do you want to add another Item [y/n]: ");
                        string addNewItem = Console.ReadLine().Trim();

                        if (addNewItem.Equals("y", StringComparison.OrdinalIgnoreCase))
                        {
                            goto ItemSelection;
                        }
                        else if (addNewItem.Equals("n", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.Clear();
                            decimal grandTotal = MiniMartConsole.DisplayOrder(new PurchaseProductDto { Products = orders }).Result;

                        Payment:
                            Console.WriteLine("Press 1 to generate Account number for payment");
                            Console.WriteLine("Press 2 to cancel order");
                            Console.Write(">>> ");
                            bool isValidInput = int.TryParse(Console.ReadLine().Trim(), out int paymentInput);

                            if (!isValidInput)
                            {
                                Console.WriteLine("Please enter a valid input");
                                goto Payment;
                            }

                            if (paymentInput == 1)
                            {
                                Console.Clear();
                                var (traceId, errorMessage) = MiniMartConsole.GenerateAccountNumber(new GenerateAccountNumberRequestDto { Amount = grandTotal }).Result;

                                if (!string.IsNullOrEmpty(errorMessage))
                                {
                                    Console.WriteLine($"{errorMessage}");
                                    goto Payment;
                                }

                            ConfirmPayment:
                                Console.WriteLine("Press 1 to confirm payment");
                                Console.WriteLine("Press 2 to print order");
                                Console.Write(">>> ");
                                bool isValidPaymentInput = int.TryParse(Console.ReadLine().Trim(), out int paymentConfirmation);

                                if (!isValidPaymentInput)
                                {
                                    Console.WriteLine("Please enter a valid input");
                                    goto ConfirmPayment;
                                }

                                if (paymentConfirmation == 1)
                                {
                                    MiniMartConsole.PaymentConfirmation(traceId).Wait();
                                    goto ConfirmPayment;
                                }
                                if (paymentConfirmation == 2)
                                {
                                    MiniMartConsole.PurchaseOrder(new PurchaseProductDto { Products = orders }).Wait();
                                    orders = [];
                                }
                                else
                                {
                                    goto ConfirmPayment;
                                }
                            }
                            else if (paymentInput == 2)
                            {
                                orders = [];
                                Console.Clear();
                            }
                            else
                            {
                                goto Payment;
                            }

                            //goto AccountGeneration;
                        }
                        else
                        {
                            goto NewItemSelection;
                        }
                        }
                    else if (inputValue == 3)
                    {
                        break;
                    }
                    else
                    {
                        goto Start;
                    }
                }
                else if (y == "n" || y == "N")
                    break;
                continue;
            }
        }
    }
}
