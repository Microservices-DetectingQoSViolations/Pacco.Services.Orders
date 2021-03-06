using Convey.Exceptions;
using System;

namespace Pacco.Services.Orders.Core.Exceptions
{
    public class InvalidOrderPriceException : DomainException
    {
        public override string Code => "invalid_order_price";

        public InvalidOrderPriceException(Guid id, decimal price)
            : base($"Order with id: '{id}' has an invalid price: {price}")
        {
        }
    }
}