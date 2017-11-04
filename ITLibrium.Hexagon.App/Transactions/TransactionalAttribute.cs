using System;

namespace ITLibrium.Hexagon.App.Transactions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class TransactionalAttribute : Attribute
    {
        
    }
}