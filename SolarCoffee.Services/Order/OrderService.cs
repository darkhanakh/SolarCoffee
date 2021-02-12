using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolarCoffee.Data;
using SolarCoffee.Data.Models;
using SolarCoffee.Services.Inventory;
using SolarCoffee.Services.Product;

namespace SolarCoffee.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly SolarDbContext _db;
        private readonly IProductService _productService;
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<OrderService> _logger;

        
        public OrderService(
            SolarDbContext dbContext, 
            IProductService productService, 
            IInventoryService inventoryService,
            ILogger<OrderService> logger
            )
        {
            _db = dbContext;
            _productService = productService;
            _inventoryService = inventoryService;
            _logger = logger;
        }
        
        /// <summary>
        /// Get all SalesOrders in System
        /// </summary>
        /// <returns></returns>
        public List<SalesOrder> GetOrders()
        {
            return _db.SalesOrders
                .Include(order => order.Customer)
                    .ThenInclude(customer => customer.PrimaryAddress)
                .Include(order => order.SalesOrderItems)
                    .ThenInclude(item => item.Product)
                .ToList();
        }
        
        /// <summary>
        /// Creates an open SalesOrder
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public ServiceResponse<bool> GenerateOpenOrder(SalesOrder order)
        {
            _logger.LogInformation("Generating new order");
            foreach (var item in order.SalesOrderItems)
            {
                item.Product = _productService
                    .GetProductById(item.Product.Id);
                var inventoryId = _inventoryService
                    .GetByProductId(item.Product.Id).Id;
                _inventoryService
                    .UpdateUnitAvailable(inventoryId, -item.Quantity);
            }

            try
            {
                _db.SalesOrders.Add(order);
                _db.SaveChanges();
                
                return new ServiceResponse<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Open order is created",
                    Time = DateTime.UtcNow
                };
            }
            catch (Exception e)
            {
                return new ServiceResponse<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    Message = e.StackTrace,
                    Time = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Marks an open order as paid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServiceResponse<bool> MarkFulfilled(int id)
        {
            var now = DateTime.UtcNow;
            var order = _db.SalesOrders.Find(id);
            order.UpdatedOn = now;
            order.IsPaid = true;

            try
            {
                _db.SalesOrders.Update(order);
                _db.SaveChanges();
                
                return new ServiceResponse<bool>
                {
                    IsSuccess = true,
                    Message = $"Order {order.Id} marked as paid",
                    Data = true,
                    Time = now,
                };
            }
            catch (Exception e)
            {
                return new ServiceResponse<bool>
                {
                    IsSuccess = false,
                    Message = e.StackTrace,
                    Data = false,
                    Time = now
                };
            }
        }
    }
}