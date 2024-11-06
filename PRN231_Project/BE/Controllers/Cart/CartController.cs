using System.Runtime.Intrinsics.X86;
using System.Text.Json;
using System.Web;
using BE.Service;
using Lib.DTO.Cart;
using Lib.Models;
using Lib.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace BE.Controllers.Cart;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<Lib.Models.Bill> _billRepository;
    private readonly IRepository<BillDetail> _billDetailRepository;
    private readonly IVnPayService _vnPayService;


    public CartController(IVnPayService vnPayService, IRepository<Lib.Models.Bill> billRepository,
        IRepository<Course> courseRepository, IRepository<BillDetail> billDetailRepository)
    {
        _vnPayService = vnPayService;
        _courseRepository = courseRepository;
        _billRepository = billRepository;
        _billDetailRepository = billDetailRepository;
    }

    public Dictionary<int, CartDTO> cartList;

    [HttpPost("CreatePaymentUrl")]
    public async Task<IActionResult> CreatePaymentUrl(PaymentInformationModel model)
    {
        var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

        return Ok(url);
    }

    [HttpPost("FinishPayment")]
    public async Task<IActionResult> FinishPayment([FromBody] PaymentRequestDTO paymentRequest, string cartListJson)
    {
        var queryDictionary = new Dictionary<string, StringValues>
        {
            { "vnp_Amount", paymentRequest.vnp_Amount.ToString() },
            { "vnp_BankCode", paymentRequest.vnp_BankCode },
            { "vnp_BankTranNo", paymentRequest.vnp_BankTranNo },
            { "vnp_CardType", paymentRequest.vnp_CardType },
            { "vnp_OrderInfo", paymentRequest.vnp_OrderInfo },
            { "vnp_PayDate", paymentRequest.vnp_PayDate },
            { "vnp_ResponseCode", paymentRequest.vnp_ResponseCode },
            { "vnp_TmnCode", paymentRequest.vnp_TmnCode },
            { "vnp_TransactionNo", paymentRequest.vnp_TransactionNo },
            { "vnp_TransactionStatus", paymentRequest.vnp_TransactionStatus },
            { "vnp_TxnRef", paymentRequest.vnp_TxnRef },
            { "vnp_SecureHash", paymentRequest.vnp_SecureHash }
        };

        var rq = new QueryCollection(queryDictionary);


        // Execute the payment and capture the response using paymentRequest properties
        var response = _vnPayService.PaymentExecute(rq); // Adjust this method accordingly

        // Retrieve the cart list from cookies
        if (response.Success)
        {
            if (!string.IsNullOrEmpty(cartListJson))
            {
                // Deserialize the cart list from JSON
                string decodedCartListJson = HttpUtility.UrlDecode(cartListJson);
                var cartList = JsonSerializer.Deserialize<Dictionary<int, CartDTO>>(decodedCartListJson);
                var totalPayment = cartList.Sum(x => x.Value.price);

                // Create and save the bill
                var bill = new Lib.Models.Bill
                {
                    TotalPayment = totalPayment,
                    UserId = 1 // Consider replacing with the actual user ID
                };
                await _billRepository.AddAsync(bill);

                // Add bill details for each course in the cart
                foreach (var item in cartList)
                {
                    var billDetail = new BillDetail
                    {
                        BillId = bill.Id,
                        CourseId = item.Value.CourseId
                    };
                    await _billDetailRepository.AddAsync(billDetail);
                }
            }

            return Ok("Finish Payment Successfully");
        }

        // Handle case where payment was not successful
        return BadRequest("Payment failed or invalid cart data.");
    }
}
