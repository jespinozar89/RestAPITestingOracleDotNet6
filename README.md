# Tecnologias del aplicativo

esta aplicacion contiene las siguientes tecnologias:

- .NET6
- Oracle 21c
- Entity Framework Core - oracle
- API REST
- Patron Repository
- Inyeccion de dependencia
- DTO (Data Transfer Object)

# Crear Proyecto en Visual Studio Code
## 1. Comando crear Api .net6

Abrir terminal y ejecutar el siguiente comando:

```bash
dotnet new webapi -n MyApiProject -f net6.0
```

## 2. Configurar Entity Framework Core

1.- instalaremos los siguientes paquetes:

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Oracle.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

2.- Instalar dotnet-ef

```bash
dotnet tool install --global dotnet-ef
```

3.- verificar instalaccion dotnet-ef

```bash
dotnet ef --version
```

## 3. Crear archivo global.json .net6

Para trabajar con una versión específica de .NET en nuestro proyecto, debemos crear un archivo `global.json` en la raiz del proyecto y especificaremos la versión del `SDK`.

Ejemplo para trabajar en la version .NET6:

```bash
{
  "sdk": {
    "version": "6.0.428"
  }
}
```

## 4. Crear Modelo y Contexto

Esquema general del proyecto:

```bash
YourProject/
│
├── Controllers/         <- API Controllers
│   ├── CustomersController.cs
│   ├── OrderController.cs
│   ├── OrderItemController.cs
│
├── Data/                <- DbContext and migrations
│   ├── AppDbContext.cs
│
├── Models/              <- Entities and DTOs
│   ├── Entities/
│   │   ├── Customer.cs
│   │   ├── Order.cs
│   │   ├── OrderItem.cs
│   ├── DTOs/
│       ├── CustomerDTO.cs
│       ├── OrderDTO.cs
│       ├── OrderItemDTO.cs
│
├── Repository/          <- Interfaces and repository classes
│   ├── Interfaces/
│   │   ├── ICustomerRepository.cs
│   │   ├── IOrderRepository.cs
│   │   ├── IOrderItemRepository.cs
│   ├── Classes/
│       ├── CustomerRepository.cs
│       ├── OrderRepository.cs
│       ├── OrderItemRepository.cs
│
│── appsettings.Development.json
│── appsettings.json
│── global.json
│── MyApiORACLE.csproj
│── Program.cs
```

1. Crear carpeta `models/entities`.
2. Crear nuestros entidades `models/entities/Customer`, ejemplo:

```bash
namespace MiApiORACLE.Models
{
    [Table("CUSTOMERS")]
    public class Customer
    {
        [Column("CUSTOMER_ID")]
        public int CustomerId { get; set; }//PK
        [Column("EMAIL_ADDRESS")]
        public string? EmailAdress { get; set; }
        [Column("FULL_NAME")]
        public string? FullName { get; set; }

        //Relacion: un Clinete puede tener muchos Pedidos
        public ICollection<Order>? Orders { get; set; }

    }
}
```
3. Crea carpeta `Data` y añadir arhivo `AppDbContext.cs`, ejemplo:

```bash
namespace MyApiProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : 
        base(options) { }

        public DbSet<ExampleModel> Examples { get; set; }
    }
}
```

4. Configurar modelo BD en la clase `AppDbContext.cs`:

```bash
protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Configuracion de clave primaria compuesta para OrderItem
            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => new { oi.OrderId, oi.LineItemId });

            //Configuracion de relaciones

            //Order -> Customer
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId);

            //OrderItem -> Order
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);
        }
```
## 5. Configurar conexion a Oracle

1. Abre el archivo `appsettings.json` y agrega la cadena de conexión:

```bash
"ConnectionStrings": {
    "DefaultConnection": "User Id=<usuario>;Password=<clave>;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=<localhost>)(PORT=<puerto>)))(CONNECT_DATA=(SID=<sid>)));"
  }
```

2. Registra el contexto en `Program.cs`:

```bash
using MyApiProject.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configura EF Core con Oracle 
builder.Services.AddDbContext<AppDbContext>(options => options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
```

## 6. Crear y aplicar migraciones

1. Genera una migración inicial:

```bash
dotnet ef migrations add InitialCreate
```
2. Aplica la migración para crear las tablas en Oracle:

```bash
dotnet ef database update
```

3. Revisar la BD

## 7. Crear DTO

1. Crearemos la carpeta `DTO` en `Models`
2. Crearemos nuestros archivos DTO, ejemplo `CustomerDTO.cs`

```bash
namespace MiApiORACLE.DTO
{
    public class CustomerDTO
    {
        [Required]
        public int CustomerId { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? EmailAdress { get; set; }
        [Required]
        [MaxLength(50)]
        public string? FullName { get; set; }
    }
}
```

## 8. Aplicar Patron Repository

1.Crearemos la carpetas `Reposiroty` y dentro de ella `Classes` y `Interfaces`.

2.Crearemos nuestra clase `IRepository.cs` en `Interfaces`

```bash
namespace MyApiProject.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    }
}
```

3. Crea el archivo `Repository.cs`:

```bash
namespace MyApiProject.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task AddAsync(T entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();
    }
}
```

4.Crear repositorios específicos por entidad: Ejemplo Customer:

`Interfaces`:

```bash
namespace MiApiORACLE.Repositories.IRepositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        
    }

    //Aqui agregar las firma de los metodos
}
```

`Classes`

```bash
namespace MiApiORACLE.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext context) : base(context)
        {
        }

        //Aqui agregar la implementaciones de los metodos de la intefaz

    }
}
```

## 9. Inyeccion de Dependencias

1. Registrar servicios en Program.cs: En Program.cs, registra tus repositorios como servicios:

```bash
using MyApiProject.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Registro de repositorios 
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();

var app = builder.Build();
```

## 10. Inyectar repositorios en los controladores

1. Crear clases api controller en la carpeta `Controllers`
2. Usa el constructor de tus controladores para recibir las dependencias:

```bash
namespace MyApiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerRepository _customerRepository;

        public CustomersController(CustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _customerRepository.GetAllAsync();
            return Ok(customers);
        }
    }
}
```

3. usas tus DTOs: ejemplo metodo `HttpPost` con `try-catch`

```bash
[HttpPost]
public async Task<IActionResult> CreateCustomer(CustomerDTO customer)
{
    try
    {

        if (customer == null)
        {
            return BadRequest();
        }

        var newCustomer = new Customer
        {
            EmailAdress = customer.EmailAdress ?? string.Empty,
            FullName = customer.FullName ?? string.Empty
        };

        await _customerRepository.AddAsync(newCustomer);
        return CreatedAtAction(nameof(GetCustomerById), new { id = customer.CustomerId }, customer);
    }
    catch (DbUpdateException ex) when (ex.InnerException is OracleException oraEx)
    {

        return oraEx.Number switch
        {
            1 => Conflict("El registro ya existe."), // HTTP 409
            2291 => BadRequest("Referencia inválida (FK)."), // HTTP 400
            _ => StatusCode(500, "Error de base de datos.") // HTTP 500
        };
    }
    catch (OracleException ex)
    {
        return StatusCode(500, $"Error específico de Oracle: {ex.Message}");
    }
    catch (Exception)
    {
        return StatusCode(500, "Error interno del servidor."); // HTTP 500
    }
}
```