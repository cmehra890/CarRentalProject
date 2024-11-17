using CarRentalProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PayPal.Api;

namespace CarRentalProject.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly S22024Group3ProjectContext _context;

        public PaymentController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, S22024Group3ProjectContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _context = context;

        }

        public IActionResult Index()
        {
            return View();
        }

        public ActionResult PaymentWithPaypal(string Cancel = null, string blogId = "", string PayerID = "", string guid = "")
        {
            //getting the apiContext  
            var ClientID = _configuration.GetValue<string>("PayPal:Key");
            var ClientSecret = _configuration.GetValue<string>("PayPal:Secret");
            var mode = _configuration.GetValue<string>("PayPal:mode");
            APIContext apiContext = PaypalConfiguration.GetAPIContext(ClientID, ClientSecret, mode);
            // apiContext.AccessToken="Bearer access_token$production$j27yms5fthzx9vzm$c123e8e154c510d70ad20e396dd28287";
            try
            {

                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = PayerID;
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = this.Request.Scheme + "://" + this.Request.Host + "/Home/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guidd = Convert.ToString((new Random()).Next(100000));
                    guid = guidd;
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid, blogId);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    _httpContextAccessor.HttpContext.Session.SetString("payment", createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  

                    var paymentId = _httpContextAccessor.HttpContext.Session.GetString("payment");
                    var executedPayment = ExecutePayment(apiContext, payerId, paymentId as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {

                        return View("PaymentFailed");
                    }
                    var blogIds = executedPayment.transactions[0].item_list.items[0].sku;

                    var transactionId = paymentId + "/" + executedPayment.id;

                    var internalPaymentId = Convert.ToInt32(_httpContextAccessor.HttpContext.Session.GetString("internal_payment_id"));

                    using (var transaction = _context.Database.BeginTransaction())
                    {

                        var payment = _context.Payments.Where(x => x.PaymentId == internalPaymentId).FirstOrDefault();

                        if(payment is not null)
                        {
                            payment.PaymentMethod = "paypal";
                            payment.TransactionId = transactionId;
                            payment.Amount = Convert.ToDecimal(_httpContextAccessor.HttpContext.Session.GetString("amount_to_pay"));

                            _context.Payments.Attach(payment);
                            _context.Entry(payment).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                            _context.SaveChangesAsync();

                            Models.Invoice invoice = new Models.Invoice()
                            {
                                BookingId = payment.BookingId,
                                GeneratedDate = DateTime.Now,
                                Amount = payment.Amount,
                                DueAmount = 0,
                            };

                            _context.Invoices.Add(invoice);
                            _context.SaveChangesAsync();

                            transaction.Commit();
                        }

                    }

                    return View("PaymentSuccess");
                }
            }
            catch (Exception ex)
            {
                return View("PaymentFailed");
            }
            //on successful payment, show success page to user.  
            return View("SuccessView");
        }
        private PayPal.Api.Payment payment;
        private PayPal.Api.Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new PayPal.Api.Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private PayPal.Api.Payment CreatePayment(APIContext apiContext, string redirectUrl, string blogId)
        {
            //create itemlist and add item objects to it  

            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  
            itemList.items.Add(new Item()
            {
                name = "Car Rent From Wheel On Rent",
                currency = "NZD",
                price = _httpContextAccessor.HttpContext.Session.GetString("amount_to_pay"),
                quantity = "1",
                sku = "asd"
            });
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            //var details = new Details()
            //{
            //    tax = "1",
            //    shipping = "1",
            //    subtotal = "1"
            //};
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "NZD",
                total = _httpContextAccessor.HttpContext.Session.GetString("amount_to_pay"), // Total must be equal to sum of tax, shipping and subtotal.  
                //details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = Guid.NewGuid().ToString(), //Generate an Invoice No  
                amount = amount,
                item_list = itemList
            });
            this.payment = new PayPal.Api.Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }

    }
}
