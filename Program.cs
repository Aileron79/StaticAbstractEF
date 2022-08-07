using System.Data.Entity;

public interface IDbContext<T> : IDisposable where T : class
{
    public DbSet<T> GetDbSet();
}

public interface IABEntity<T> where T : class
{
    public int EntityId { get; set; }
    public T Object { get; set; }
}

public class ABEntity<T>  where T: IABObject
{
    public int EntityId;
    public T Object;

    public ABEntity(T obj, int uuid)
    {
        this.Object = obj;
        this.EntityId = uuid;
    }
    public static List<ABEntity<T>> GetObjects()
    {
        List<ABEntity<T>> results = new List<ABEntity<T>>();
        using (IDbContext<T> dbContext = T.GetDbContext())
        {
            var x = dbContext.GetDbSet().Select(x => new ABEntity<T>((T)x, 0)).ToList();
            results.AddRange(x);
        }
        return results;
    }
}

public class Company : IABObject
{
    public int ObjectId { get; set; }
    public string CompanyName { get; set; }
    public static IDbContext<IABObject> GetDBContext()
    {
        return (IDbContext<IABObject>)new ABContextClient();
    }
}

public class Client : IABObject
{
    public int ObjectId { get; set; }
    public string Email { get; set; }

    public static IDbContext<IABObject> GetDBContext()
    {
        return (IDbContext<IABObject>) new  ABContextCompany();
    }
}

public interface IABObject
{
    public int ObjectId { get; set; }
    public abstract static IDbContext<IABObject> GetDBContext();
}

public class ABContextClient : DbContext, IDbContext<Client>
{
   public DbSet<Client> Clients { get; set; }

    public DbSet<Client> GetDbSet()
    {
        return Clients;
    }
}

public class ABContextCompany : DbContext, IDbContext<Company>
{
    public DbSet<Company> Companies { get; set; }

    public DbSet<Company> GetDbSet()
    {
        return Companies;
    }
}

class Program
{
    static void Main(string[] args)
    {
        using (var dbContext = new ABContextCompany())
        {
            var company = new Company { ObjectId = 0, CompanyName = "GenericTester" };
            dbContext.Companies.Add(company);
            dbContext.SaveChanges();
        }
        using (var dbContext = new ABContextClient())
        {
            var client = new Client { ObjectId = 0, Email = "some@email.com" };
            dbContext.Clients.Add(client);
            dbContext.SaveChanges();
        }

        List<ABEntity<Company>> companyList = ABEntity<Company>.GetObjects();
        List<ABEntity<Client>> clientList = ABEntity<Client>.GetObjects();

    }
}
