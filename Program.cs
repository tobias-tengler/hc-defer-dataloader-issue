using GreenDonut;
using HotChocolate;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>();

var app = builder.Build();

app.MapGraphQL();

app.Run();

public class Query
{
    public async Task<string> Field1()
    {
        await Task.Delay(2500);

        return "field1";
    }

    public async Task<string> Field2()
    {
        await Task.Delay(2500);

        return "field2";
    }

    public List<Wrapper> GetWrappers() => new()
    {
        new Wrapper { Id = 1 },
        new Wrapper { Id = 2 },
        new Wrapper { Id = 3 }
    };

    public DeferTest GetComplexType() => new();
}

public class DeferTest
{
    public async Task<string> ComplexField1()
    {
        await Task.Delay(2500);

        return "complexfield1";
    }

    public async Task<string> ComplexField2()
    {
        await Task.Delay(2500);

        return "complexfield2";
    }
}

public class Wrapper
{
    public int Id { get; set; }

    public async Task<string> GetValueAsync([Parent] Wrapper wrapper, [DataLoader] ValueDataLoader dataLoader)
    {
        await Task.Delay(2000);

        return $"MyValue:{wrapper.Id}";

        // happens with and without the DataLoader
        //return dataLoader.LoadAsync(wrapper.Id);
    }
}

public class ValueDataLoader : BatchDataLoader<int, string>
{
    public ValueDataLoader(IBatchScheduler batchScheduler, DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
    }

    protected override async Task<IReadOnlyDictionary<int, string>> LoadBatchAsync(IReadOnlyList<int> keys, CancellationToken cancellationToken)
    {
        await Task.Delay(2000);

        return keys.ToDictionary(k => k, k => $"MyValue:{k}");
    }
}