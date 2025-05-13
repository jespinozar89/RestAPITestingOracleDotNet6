using MiApiORACLE.Data;
using MiApiORACLE.Models.DTO;
using MiApiORACLE.Repositories;
using MiApiORACLE.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MiApiORACLE.Repository.Classes
{
    public class ReportRepository : Repository<ReportDTO>, IReportRepository
    {
        private readonly AppDbContext _context;
        public ReportRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ReportDTO>> GetCustomerById(int id)
        {
            var report = await _context.Customers
                .Where(customer => customer.CustomerId == id)
                .Join(_context.Orders,
                    customer => customer.CustomerId,
                    order => order.CustomerId,
                    (customer, order) => new { customer, order })
                .Join(_context.OrderItems,
                    customerOrder => customerOrder.order.OrderId,
                    orderItem => orderItem.OrderId,
                    (customerOrder, orderItem) => new { customerOrder, orderItem })
                .Select(report => new ReportDTO
                {
                    FullName = report.customerOrder.customer.FullName,
                    OrderTimestamp = report.customerOrder.order.OrderTimestamp,
                    ProductId = report.orderItem.ProductId,
                    UnitPrice = report.orderItem.UnitPrice
                })
                .OrderByDescending(report => report.OrderTimestamp)
                .ToListAsync();

            return report;
        }

        public async Task<List<ReportDTO>> GetGroupByJoinCountMax()
        {
            var report = await _context.Customers
                .Join(_context.Orders,
                    customer => customer.CustomerId,
                    order => order.CustomerId,
                    (customer, order) => new { customer, order })
                .Where(report => report.order.OrderTimestamp >= DateTime.Now.AddMonths(-12))
                .GroupBy(report => report.customer.FullName)
                .Where(group => group.Count() > 10)
                .Select(group => new ReportDTO
                {
                    FullName = group.Key,
                    OrderId = group.Count(),
                    OrderTimestamp = group.Max(report => report.order.OrderTimestamp)

                })
                .OrderByDescending(report => report.OrderId)
                .ToListAsync();

            return report;
        }

        public async Task<List<ReportDTO>> GetGroupByJoinFullName()
        {
            var report = await _context.Customers
                .Join(_context.Orders,
                    customer => customer.CustomerId,
                    order => order.CustomerId,
                    (customer, order) => new { customer, order })
                .Join(_context.OrderItems,
                    customerOrder => customerOrder.order.OrderId,
                    orderItem => orderItem.OrderId,
                    (customerOrder, orderItem) => new { customerOrder, orderItem })
                .Where(report => report.customerOrder.order.OrderTimestamp >= DateTime.Now.AddMonths(-6))
                .GroupBy(report => report.customerOrder.customer.FullName)
                .Select(group => new ReportDTO
                {
                    FullName = group.Key,
                    TotalPrice = group.Sum(report => ((int)report.orderItem.UnitPrice) * report.orderItem.Quantity)
                })
                .ToListAsync();

            return report;

        }

        public async Task<List<ReportDTO>> GetGroupByJoinSum()
        {
            var report = await _context.Orders
                .Join(_context.OrderItems,
                    order => order.OrderId,
                    orderItem => orderItem.OrderId,
                    (order, orderItem) => new { order, orderItem })
                .GroupBy(report => report.order.OrderId)
                .Where(group => group.Sum(report => report.orderItem.Quantity) > 5 &&
                        group.Sum(report => report.orderItem.Quantity) > 100)
                .Select(group => new ReportDTO
                {
                    OrderId = group.Key,
                    TotalQuantity = group.Sum(report => report.orderItem.Quantity),
                    TotalPrice = group.Sum(report => ((int)report.orderItem.UnitPrice) * report.orderItem.Quantity)
                })
                .ToListAsync();

            return report;

        }

        public async Task<List<ReportDTO>> GetGroupByLeftJoinHaving()
        {

            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            var now = DateTime.Now;

            var report = await _context.Customers
                    .GroupJoin(_context.Orders
                    .Where(o => o.OrderTimestamp >= sixMonthsAgo && o.OrderTimestamp <= now),
                        c => c.CustomerId,
                        o => o.CustomerId,
                        (c, orders) => new { c.FullName, orders })
                    .SelectMany(x => x.orders.DefaultIfEmpty(), (x, order) => new
                    {
                        FullName = x.FullName,
                        Order = order
                    })
                    .GroupBy(x => x.FullName)
                    .Where(group => !group.Any(g => g.Order != null))//HAVING MAX(ORDER_TMS) IS NULL
                    .Select(group => new ReportDTO
                    {
                        FullName = group.Key,
                        //OrderTimestamp = group.Max(x => x.Order.OrderTimestamp)
                    })
                    .ToListAsync();

            return report;
        }

        public Task<List<ReportDTO>> GetSumProduct()
        {
            var report = _context.OrderItems
                .Where(orderItem => orderItem.ShipmentId == 1)
                .GroupBy(orderItem => orderItem.ProductId)
                .Where(group => group.Sum(orderItem => orderItem.Quantity) > 1)
                .Select(group => new ReportDTO
                {
                    ProductId = group.Key,
                    TotalQuantity = group.Sum(orderItem => orderItem.Quantity),
                    TotalPrice = group.Sum(orderItem => ((int)orderItem.UnitPrice) * orderItem.Quantity)
                })
                .ToListAsync();

            return report;
        }

        public async Task<List<ReportDTO>> GetGroupByLeftJoinCount()
        {
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            var now = DateTime.Now;

            var report = await _context.Customers
                .GroupJoin(
                    _context.Orders
                        .Where(o => o.OrderTimestamp >= sixMonthsAgo && o.OrderTimestamp <= now),
                    c => c.CustomerId,
                    o => o.CustomerId,
                    (c, orders) => new { Customer = c, Orders = orders }
                )
                .SelectMany(
                    x => x.Orders.DefaultIfEmpty(),
                    (x, o) => new
                    {
                        FullName = x.Customer.FullName,
                        Order = o
                    })
                .Where(x => x.Order == null)
                .GroupBy(x => x.FullName)
                .Select(g => new ReportDTO
                {
                    FullName = g.Key,
                    OrderId = g.Count()  // Todos serán 0 en teoría, pero se mantiene la estructura
                })
                .ToListAsync();

            return report;

        }

        public async Task<List<ReportDTO>> GetGroupByJoinBetweenSum()
        {
            var report = await _context.OrderItems
                .GroupJoin(_context.Orders
                    .Where(o => o.OrderTimestamp >= DateTime.Now.AddMonths(-6)),
                    oi => oi.OrderId,  // Clave en OrderItem
                    o => o.OrderId,    // Clave en Order
                    (oi, orders) => new { OrderItem = oi, Orders = orders }) // Agrupar correctamente
                .SelectMany(x => x.Orders.DefaultIfEmpty(), (x, order) => new
                {
                    OrderItem = x.OrderItem,
                    Order = order
                })
                .Where(x => x.Order == null) // Filtrar clientes sin pedidos recientes
                .GroupBy(g => g.OrderItem.OrderId) // Agrupar por ID de la orden
                .Select(g => new ReportDTO
                {
                    OrderId = g.Key, // Corregir uso de Key en GroupBy
                    UnitPrice = g.Sum(x => x.OrderItem.UnitPrice * x.OrderItem.Quantity) // Obtener Suma total precios
                })
                .ToListAsync();

            return report;
        }

        public async Task<List<ReportDTO>> GetGroupByJoinHavingWhere()
        {
            var report = await _context.Orders
                .Join(_context.OrderItems,
                    o => o.OrderId,
                    oi => oi.OrderId,
                    (o, oi) => new { o, oi })
                .Where(x => x.o.OrderStatus == "COMPLETE")
                .GroupBy(g => g.o.OrderId)
                .Where(g => g.Sum(x => x.oi.Quantity) > 5)
                .Select(g => new ReportDTO
                {
                    OrderId = g.Key,
                    Quantity = g.Sum(x => x.oi.Quantity),
                    UnitPrice = g.Sum(x => x.oi.UnitPrice)
                })
                .ToListAsync();

            return report;
        }


        public async Task<List<ReportDTO>> GetGroupByJoinX2HavingWhere()
        {
            /*¿Cómo puedes formular una consulta SQL que muestre los clientes
            de la tabla CO.CUSTOMERS que hayan gastado más de 5000 dólares
             en un periodo de 6 meses considerando los montos de CO.ORDER_ITEMS, 
             y que además hayan realizado al menos una compra en los últimos 2 meses,
              utilizando la relación con CO.ORDERS? 😊🚀*/

            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            var twoMonthsAgo = DateTime.Now.AddMonths(-2);

            var report = await _context.Customers
                .Join(_context.Orders,
                    c => c.CustomerId,
                    o => o.CustomerId,
                    (c, o) => new { Customer = c, Order = o })
                .Join(_context.OrderItems,
                    co => co.Order.OrderId,
                    oi => oi.OrderId,
                    (co, oi) => new { co.Customer, co.Order, OrderItem = oi })
                .Where(x => x.Order.OrderTimestamp >= sixMonthsAgo && x.Order.OrderTimestamp <= DateTime.Now)
                .GroupBy(x => new { x.Customer.CustomerId, x.Customer.FullName })
                .Where(g => g.Sum(x => x.OrderItem.UnitPrice * x.OrderItem.Quantity) > 5000 &&
                            g.Max(x => x.Order.OrderTimestamp) >= twoMonthsAgo)
                .Select(g => new ReportDTO
                {
                    FullName = g.Key.FullName,
                    TotalPrice = g.Sum(x => (int)(x.OrderItem.UnitPrice) * x.OrderItem.Quantity)
                })
                .ToListAsync();

            return report;

        }

        public async Task<List<ReportDTO>> GetGroupByJoinCountDistinctHavingWhere()
        {
            var report = await _context.Orders
                .Join(_context.OrderItems,
                    o => o.OrderId,
                    oi => oi.OrderId,
                    (o, oi) => new { o, oi })
                .Where(x => x.o.OrderTimestamp >= DateTime.Now.AddMonths(-2))
                .GroupBy(g => new { g.o.OrderId, g.o.OrderTimestamp })
                .Where(g => g.Select(x => x.oi.ProductId).Distinct().Count() > 3)
                .Select(g => new ReportDTO
                {
                    OrderId = g.Key.OrderId,
                    OrderTimestamp = g.Key.OrderTimestamp,
                    Quantity = g.Select(x => x.oi.ProductId).Distinct().Count()
                })
                .ToListAsync();

            return report;
        }

        private void FiltrarProductosXNombreYCategoria()
        {
            //Linq
            // var productosFiltrados = await _context.Productos
            //     .Where(p => p.Nombre.Contains(nombreBuscado) 
            //          && p.Categoria == categoriaBuscada)
            //     .ToListAsync();

            //SQL
            // SELECT * FROM CO.PRODUCTOS
            // WHERE NOMBRE LIKE '%' || :nombreBuscado || '%'
            // AND CATEGORIA = :categoriaBuscada;

        }

        private void FiltrarProductosXNombreOCategoriaODescripcionOPrecio()
        {
            // var productos = await _context.Productos
            //     .Where(p => 
            //         (string.IsNullOrEmpty(nombreBuscado) || p.Nombre.Contains(nombreBuscado)) &&
            //         (string.IsNullOrEmpty(descripcionBuscada) || p.Descripcion.Contains(descripcionBuscada)) &&
            //         (string.IsNullOrEmpty(categoriaBuscada) || p.Categoria == categoriaBuscada) &&
            //         (!precioMin.HasValue || p.Precio >= precioMin) &&
            //         (!precioMax.HasValue || p.Precio <= precioMax))
            //     .ToListAsync();

            // SQL
            // SELECT * FROM CO.PRODUCTOS
            // WHERE 
            //     (:nombreBuscado IS NULL OR NOMBRE LIKE '%' || :nombreBuscado || '%')
            // AND (:descripcionBuscada IS NULL OR DESCRIPCION LIKE '%' || :descripcionBuscada || '%')
            // AND (:categoriaBuscada IS NULL OR CATEGORIA = :categoriaBuscada)
            // AND (:precioMin IS NULL OR PRECIO >= :precioMin)
            // AND (:precioMax IS NULL OR PRECIO <= :precioMax);


        }
        public async Task<List<ReportDTO>> EjercicioCO2_N1()
        {
            var report = await _context.Customers
                .Join(_context.Orders,
                    c => c.CustomerId,
                    o => o.CustomerId,
                    (c, o) => new { Customer = c, Order = o })
                .Join(_context.OrderItems,
                    o => o.Order.OrderId,
                    oi => oi.OrderId,
                    (o, oi) => new
                    {
                        Customer = o.Customer,
                        Order = o.Order,
                        OrderItem = oi
                    })
                .Where(x => x.Order.OrderStatus == "REFUNDED")
                .GroupBy(g => new { g.Customer.CustomerId, g.Customer.FullName })
                .Where(g => g.Sum(x => x.OrderItem.UnitPrice * x.OrderItem.Quantity) > 100)
                .Select(g => new ReportDTO
                {
                    FullName = g.Key.FullName,
                    TotalPrice = g.Sum(x => (int)(x.OrderItem.UnitPrice) * x.OrderItem.Quantity)
                })
                .ToListAsync();

            return report;

        }

        public async Task<List<ReportDTO>> EjercicioCO2_N2()
        {
            var report = await _context.OrderItems
                .GroupBy(g => new { g.ProductId })
                .Where(g => g.Sum(x => x.Quantity) > 50)
                .Select(g => new ReportDTO
                {
                    ProductId = g.Key.ProductId,
                    Quantity = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(g => g.Quantity)
                .ToListAsync();

            return report;
        }

        public async Task<List<ReportDTO>> EjercicioCO2_N3()
        {
            var fechaCorte = DateTime.Now.AddMonths(-6);

            var idsClientesConPedidosAntiguos = _context.Orders
                .Where(o => o.OrderTimestamp < fechaCorte)
                .Select(o => o.CustomerId)
                .Distinct();

            var report = await _context.Customers
                .GroupJoin(_context.Orders
                    .Where(o => o.OrderTimestamp >= fechaCorte),
                    c => c.CustomerId,
                    o => o.CustomerId,
                    (c, o) => new { c, o })
                .SelectMany(x => x.o.DefaultIfEmpty(),
                    (x, order) => new
                    {
                        Customer = x.c,
                        Order = order
                    })
                .Where(x => x.Order == null &&
                    idsClientesConPedidosAntiguos.Contains(x.Customer.CustomerId))
                .Select(x => new ReportDTO
                {
                    FullName = x.Customer.FullName
                })
                .ToListAsync();

            return report;
        }

        public async Task<List<ReportDTO>> EjercicioCO2_N4()
        {
            var report = await _context.Orders
                .Join(_context.OrderItems,
                    o => o.OrderId,
                    oi => oi.OrderId,
                    (o, oi) => new { o, oi })
                .Join(_context.Customers,
                    o => o.o.CustomerId,
                    c => c.CustomerId,
                    (o, c) => new
                    {
                        Customer = c,
                        Order = o.o,
                        OrderItem = o.oi
                    })
                .Where(x => x.OrderItem.ShipmentId == null)
                .Select(x => new ReportDTO
                {
                    FullName = x.Customer.FullName,
                    OrderTimestamp = x.Order.OrderTimestamp,
                    OrderId = x.Order.OrderId
                })
                .ToListAsync();

            return report;
        }

        public async Task<List<ReportDTO>> EjercicioCO2_N5()
        {
            var report = await _context.Customers
                .Join(_context.Orders,
                    c => c.CustomerId,
                    o => o.CustomerId,
                    (c, o) => new { c, o })
                .Join(_context.OrderItems,
                    co => co.o.OrderId,
                    oi => oi.OrderId,
                    (co, oi) => new
                    {
                        Customer = co.c,
                        Order = co.o,
                        OrderItem = oi
                    })
                .GroupBy(group => new { group.Customer.CustomerId, group.Customer.FullName })
                .Where(group => group.Average(x => x.OrderItem.UnitPrice * x.OrderItem.Quantity) > 500)
                .Select(g => new ReportDTO
                {
                    FullName = g.Key.FullName,
                    TotalPrice = (int)g.Average(x => x.OrderItem.UnitPrice * x.OrderItem.Quantity)
                })
                .ToListAsync();

            return report;
        }

        public async Task<List<ReportDTO>> EjercicioCO2_N6()
        {
            var report = await _context.Customers
                .Join(_context.Orders,
                    c => c.CustomerId,
                    o => o.CustomerId,
                    (c, o) => new { c, o })
                .Join(_context.OrderItems,
                    co => co.o.OrderId,
                    oi => oi.OrderId,
                    (co, oi) => new
                    {
                        Customer = co.c,
                        Order = co.o,
                        OrderItem = oi
                    })
                .GroupBy(g => new
                { g.Customer.CustomerId, g.Customer.FullName })
                .Select(x => new
                {
                    FullName = x.Key.FullName,
                    ShipmentCount = x.Count(x => x.OrderItem.ShipmentId != null) // ✅ Contar ShipmentId
                })
                .Where(g => g.ShipmentCount > 2) // ✅ Usando ShipmentCount en el filtro
                .Select(g => new ReportDTO
                {
                    FullName = g.FullName,
                    ShipmentId = g.ShipmentCount // ✅ ShipmentCount ahora se puede usar directamente
                })
                .ToListAsync();

            return report;
        }

        public async Task<List<ReportDTO>> EjercicioCO2_N7()
        {
            var report = await _context.Orders
                .Include(o => o.Customer)//Join con Customers
                .Include(o => o.OrderItems)// JOIN con OrderItems 
                .Where(o => o.OrderStatus == "COMPLETE")
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderTimestamp,
                    o.Customer.FullName,
                    TotalPrice = o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity)
                })
                .Where(x => x.TotalPrice > 2000)
                .Select(x => new ReportDTO
                {
                    FullName = x.FullName,
                    OrderId = x.OrderId,
                    OrderTimestamp = x.OrderTimestamp
                })
                .ToListAsync();

            return report;
        }

        public async Task<List<ReportDTO>> EjercicioCO2_N8()
        {
            var report = await _context.OrderItems
                 .Include(o => o.Order)
                 .GroupBy(g => new { g.ProductId })
                 .Select(g => new
                 {
                     g.Key.ProductId,
                     OrderCount = g.Select(x => x.OrderId).Distinct().Count(),
                     QuantitySum = g.Sum(x => x.Quantity)
                 })
                 .Where(g => g.QuantitySum < 5)
                 .Select(g => new ReportDTO
                 {
                     ProductId = g.ProductId,
                     OrderId = g.OrderCount,
                     Quantity = g.QuantitySum
                 })
                 .OrderByDescending(x => x.Quantity)
                 .ToListAsync();

            return report;
        }

        public async Task<List<ReportDTO>> EjercicioCO2_N9()
        {
            var threeMonthsAgo = DateTime.Now.AddMonths(-3);

            var report = await _context.Orders
                .Where(o => o.OrderTimestamp >= threeMonthsAgo)
                .GroupBy(o => o.Customer != null ? o.Customer.FullName : "Unknown")
                .Select(g => new
                {
                    FullName = g.Key,
                    Months = g.GroupBy(o => new { o.OrderTimestamp.Year, o.OrderTimestamp.Month })
                              .Count(),
                    TotalSpent = g.Sum(o => o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity)),
                    TotalOrderId = g.Select(x => x.OrderId)
                                    .Distinct()
                                    .Count()
                })
                .Where(x => x.Months >= 3 && x.TotalOrderId >= 3)
                .Select(x => new ReportDTO
                {
                    FullName = x.FullName,
                    UnitPrice = x.TotalSpent
                })
                .OrderByDescending(x => x.UnitPrice)
                .ToListAsync();

            return report;

        }
        
        public async Task<List<ReportDTO>> EjercicioCO2_N10()
        { 
            var report = await _context.Orders
                .Where(o => o.OrderTimestamp >= DateTime.Now.AddMonths(-6))
                .GroupBy(o => new { o.CustomerId, o.Customer.FullName })
                .Select(g => new
                {
                    g.Key.FullName,
                    TotalGastos = g
                        .Where(o => o.OrderTimestamp >= DateTime.Now.AddMonths(-3))
                        .Sum(o => o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity)),
                    TotalGastosOld = g
                        .Where(o => o.OrderTimestamp < DateTime.Now.AddMonths(-3))
                        .Sum(o => o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity))
                })
                .Where(g => g.TotalGastos * 1.5m > g.TotalGastosOld)
                .Select(g => new ReportDTO
                {
                    FullName = g.FullName,
                    UnitPrice = g.TotalGastos
                })
                .OrderByDescending(g => g.UnitPrice)
                .ToListAsync();

            return report;
        }

    }
}