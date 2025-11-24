using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiniSO.API.Models
{
    public class ClientDto
    {
        public int ClientId { get; set; }
        public string? CustomerName { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? State { get; set; }
        public string? PostCode { get; set; }
    }

    public class ItemDto
    {
        public int ItemId { get; set; }
        public string? ItemCode { get; set; }
        public string? Description { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class SalesOrderDetailDto
    {
        public int? SalesOrderDetailId { get; set; }
        
        [Required]
        public int ItemId { get; set; }
        
        // Auto-populated from Item table - nullable
        public string? ItemCode { get; set; }
        public string? Description { get; set; }
        
        public string? Note { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal Quantity { get; set; }
        
        public decimal UnitPrice { get; set; }
        
        [Required]
        [Range(0, 100, ErrorMessage = "Tax rate must be between 0 and 100")]
        public decimal TaxRate { get; set; }
        
        public decimal ExclAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal InclAmount { get; set; }
        public int LineNumber { get; set; }
    }

    public class CreateUpdateSalesOrderDto
    {
        public int? SalesOrderId { get; set; }
        
        [Required(ErrorMessage = "Invoice number is required")]
        [StringLength(50, ErrorMessage = "Invoice number cannot exceed 50 characters")]
        public string InvoiceNo { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Invoice date is required")]
        public DateTime InvoiceDate { get; set; }
        
        [StringLength(100, ErrorMessage = "Reference number cannot exceed 100 characters")]
        public string? ReferenceNo { get; set; }
        
        [Required(ErrorMessage = "Client is required")]
        public int ClientId { get; set; }
        
        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(200, ErrorMessage = "Customer name cannot exceed 200 characters")]
        public string CustomerName { get; set; } = string.Empty;
        
        [StringLength(200, ErrorMessage = "Address 1 cannot exceed 200 characters")]
        public string? Address1 { get; set; }
        
        [StringLength(200, ErrorMessage = "Address 2 cannot exceed 200 characters")]
        public string? Address2 { get; set; }
        
        [StringLength(200, ErrorMessage = "Address 3 cannot exceed 200 characters")]
        public string? Address3 { get; set; }
        
        [StringLength(100, ErrorMessage = "State cannot exceed 100 characters")]
        public string? State { get; set; }
        
        [StringLength(20, ErrorMessage = "Post code cannot exceed 20 characters")]
        public string? PostCode { get; set; }
        
        [Required(ErrorMessage = "At least one order item is required")]
        [MinLength(1, ErrorMessage = "Order must contain at least one item")]
        public List<SalesOrderDetailDto> OrderDetails { get; set; } = new List<SalesOrderDetailDto>();
    }

    public class SalesOrderDto
    {
        public int SalesOrderId { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public string? ReferenceNo { get; set; }
        public int ClientId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? State { get; set; }
        public string? PostCode { get; set; }
        public decimal TotalExclAmount { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal TotalInclAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<SalesOrderDetailDto> OrderDetails { get; set; } = new List<SalesOrderDetailDto>();
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public static ApiResponse<T> SuccessResponse(T data, string message = "Operation successful")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = new List<string>()
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errors ?? new List<string>()
            };
        }
    }
}