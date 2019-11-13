# entity-framework

Provides extensions to help use entity framework with certain design patterns.

## .IncludeEntities

When wrapping Entity Framework behind a repository, allows usage of a param array of related entities to be included on the query.

If you have the following data models:

<pre>
public class Customer {
  public int CustomerId { get; set; }
  ...
  public IEnumerable&lt;Order> Orders { get; set; }
  public 
}
  
public class Order {
  public int OrderId { get; set; }
  ...
  public Customer Customer { get; set; }
  public IEnumerable&lt;OrderItem> OrderItems { get; set; }
}

public class OrderItem {
  public int OrderItemId { get; set; }
  ...
  public Order Order { get; set; }
  public InventoryItem InventoryItem { get; set; }
}

public class InventoryItem {
  public int InventoryItem { get; set; }
  ...
  public IEnumerable&lt;OrderItem> OrderItems { get; set; }
  public IEnumerable&lt;Warehouse> Warehouses { get; set; }
}

public class Warehouse {
  public int WarehouseId { get; set; }
  ...
  public IEnumerable&lt;InventoryItem> InventoryItems { get; set; }
}
</pre>

In the Order repository, a method would load a single Order:

<pre>
  public async Task&lt;Order> LoadAsync(int orderId, params Expression&lt;Func&lt;Order, object>>[] includedEntities) {
    IQueryable&lt;Order> query = TheDbContext.Orders
      .Where(o => o.OrderId == orderId)
      .IncludeEntities(includedEntities);
      
    return await query.SingleOrDefaultAsync();
  }
</pre>

The application logic can then call this method, loading only the navigation properties that are needed.

<pre>
  // Loading just the Order
  var order = await OrderRepository.LoadAsync(1);
  
  // Loading Order and the items
  var order = await OrderRepository.LoadAsync(1, o => o.OrderItems);
  
  // Loading Order with items and customer
  var order = await OrderRepository.LoadAsync(1, 
    o => o.OrderItems,
    o => o.Customer);
    
  // Loading everything
  var order = await OrderRepository.LoadAsync(1, 
    o => o.OrderItems.Select(oi => oi.InventoryItem.Warehouses),
    o => o.Customer);
</pre>

## Releases

<ul>
  <li>1.0.1
    <ul>
      <li>Include documentation for Intellisense</li>
    </ul>
  </li>
  <li>1.0.0</li>
</ul>
