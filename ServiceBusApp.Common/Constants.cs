namespace ServiceBusApp.Common;

public static class Constants
{
    public const string ConnectionString = "Endpoint=sb://ongtest.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=NU+E+5HH4oSkSo/kUwZcHBuuilCev2PCL+ASbIONjrM=";

    public const string OrderCreatedQueue = "OrderCreatedQueue";
    public const string OrderDeletedQueue = "OrderDeletedQueue";

    public const string OrderTopic = "OrderTopic";
    public const string OrderCreatedSubName = "OrderCreatedSub";
    public const string OrderDeletedSubName = "OrderDeletedSub";
}
