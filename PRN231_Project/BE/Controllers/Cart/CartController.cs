﻿using System.Text.Json;
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

    [HttpPost("AddItem")]
    public async Task<IActionResult> AddItem(int courseID)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, // Only accessible by the server
            Secure = true, // Send only over HTTPS
            Expires = DateTimeOffset.UtcNow.AddDays(7) // Set expiration time
        };
        var cartListJson = Request.Cookies["cartList"];
        var courseToAdd = await _courseRepository.GetByIdAsync(courseID);
        if (!string.IsNullOrEmpty(cartListJson))
        {
            cartList = JsonSerializer.Deserialize<Dictionary<int, CartDTO>>(cartListJson);
            if (cartList.ContainsKey(courseID))
            {
                return Conflict("Course Already Add");
            }
            else
            {
                cartList.Add(courseID, new CartDTO { Course = courseToAdd });
                Response.Cookies.Append("cartList", JsonSerializer.Serialize(cartList), cookieOptions);
            }
        }
        else
        {
            cartList = new Dictionary<int, CartDTO>();
            cartList.Add(courseID, new CartDTO { Course = courseToAdd });
            // Store the serialized JSON in the cookie
            Response.Cookies.Append("cartList", JsonSerializer.Serialize(cartList), cookieOptions);
        }

        return Ok();
    }

    [HttpGet("GetAllItemsinCart")]
    public async Task<IActionResult> GetAllItemsinCart()
    {
        var cartListJson = Request.Cookies["cartList"];
        if (!string.IsNullOrEmpty(cartListJson))
        {
            cartList = JsonSerializer.Deserialize<Dictionary<int, CartDTO>>(cartListJson);
            var respone = cartList.Select(x=>x.Value).ToList();
            return Ok(respone);
        }
        return Ok("no Items");
    }
    
    [HttpPost("CreatePaymentUrl")]
    public async Task<IActionResult> CreatePaymentUrl(PaymentInformationModel model)
    {
        var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            
        return Ok(url);
    }
    
}

