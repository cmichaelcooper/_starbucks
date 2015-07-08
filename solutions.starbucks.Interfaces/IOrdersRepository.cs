using solutions.starbucks.Model.Pocos;
using System;
using System.Collections.Generic;

namespace solutions.starbucks.Interfaces
{
    public interface IOrdersRepository
    {
        IEnumerable<Orders> GetOrdersByAccount(string accountNumber);
        IEnumerable<Orders> GetOrdersByCustomer(string accountNumber, string zip);

        void SaveOrder(Orders order);
        void AddToOrder(Orders order, OrderItems orderItemToAdd);

        bool ExportDailyOrderReport(string templatePath, string outputPath, string marketingTemplatePath, string marketingOutputPath);

        Orders GetOrder(Guid orderID);
        Orders GetOrderInBag();
        void UpdateOrder(Orders order);
        void UpdateOrderItem(OrderItems orderItem);
        void DeleteOrderItem(OrderItems orderItem);
        int GetNextDailyOrderSequence();
        int GetNextOrderNumber();
        int TotalItemsInCart();
    }
}