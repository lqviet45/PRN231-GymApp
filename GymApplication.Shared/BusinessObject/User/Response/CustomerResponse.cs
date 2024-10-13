﻿namespace GymApplication.Shared.BusinessObject.User.Response;

public sealed class CustomerResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int TotalSpentTime { get; set; }
    public decimal TotalPayment { get; set; }
}