using System.Runtime.Intrinsics.X86;
using System.Text.Json;
using BE.Service;
using Lib.DTO.Cart;
using Lib.Models;
using Lib.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers.Cart;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<Lib.Models.Bill> _billRepository;
    private readonly IRepository<BillDetail> _billDetailRepository;
    private readonly IVnPayService _vnPayService;


    public CartController(IVnPayService vnPayService,IRepository<Lib.Models.Bill> billRepository,IRepository<Course> courseRepository, IRepository<BillDetail> billDetailRepository)
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
    public async Task<IActionResult> FinishPayment()
    {
        var response = _vnPayService.PaymentExecute(Request.Query);
        var cartListJson = Request.Cookies["cartList"];
        if (response.Success == true)
        {
            if (!string.IsNullOrEmpty(cartListJson))
            {
                cartList = JsonSerializer.Deserialize<Dictionary<int, CartDTO>>(cartListJson);
                var totalPayment = cartList.Sum(x=>x.Value.Course.Price);
                _billRepository.AddAsync(new Lib.Models.Bill
                {
                    TotalPayment = totalPayment,
                    UserId = 1
                });
                var billID = _billRepository.GetLastAsync(x => x.Id).Id;
                foreach (var item in cartList)
                {
                    _billDetailRepository.AddAsync(new BillDetail
                    {
                        BillId = billID,
                        CourseId = item.Value.Course.Id
                    });
                }
            } 
            return Ok("Finish Payment Successfully");
        }
       

        return BadRequest();
    }
    
}

