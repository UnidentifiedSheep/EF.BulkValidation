using System.Reflection;
using BulkValidation.Base.Interfaces;
using BulkValidation.Core.Abstractions;
using BulkValidation.Core.Interfaces;
using BulkValidation.Core.Models;
using BulkValidation.Pgsql.Abstractions;
using BulkValidation.Pgsql.DbValidators;
using BulkValidation.Pgsql.Executors;
using BulkValidation.Pgsql.MetadataResolvers;
using BulkValidation.Pgsql.SqlBuilders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace BulkValidation.Pgsql.Extensions;

public static class DiExtensions
{
    public static IServiceCollection AddPgsqlDbValidators<TContext>(this IServiceCollection services) where TContext : DbContext
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddScoped<SharedCounter>();

        services.AddScoped<IMetadataResolver<TContext>, MetadataResolver<TContext>>();

            
        var builderTypes = assembly
            .GetTypes()
            .Where(t => !t.IsAbstract &&
                        !t.IsInterface &&
                        t.BaseType != null &&
                        t.BaseType.IsGenericType &&
                        t.BaseType.GetGenericTypeDefinition() == typeof(PgsqlRuleSqlBuilderBase<>))
            .ToList();

        foreach (var type in builderTypes)
        {
            var closedType = type.MakeGenericType(typeof(TContext));
            services.AddScoped(typeof(RuleSqlBuilderBase<NpgsqlParameter>), closedType);
        }

        var combinedBuilderType = assembly.GetTypes()
            .FirstOrDefault(t => typeof(CombinedSqlBuilderBase<NpgsqlParameter>).IsAssignableFrom(t));

        if (combinedBuilderType != null)
            services.AddScoped<CombinedSqlBuilderBase<NpgsqlParameter>, PgsqlCombinedSqlBuilder>();

        var factoryType = assembly.GetTypes()
            .FirstOrDefault(t => typeof(IRuleSqlBuilderFactory<NpgsqlParameter>).IsAssignableFrom(t));

        if (factoryType != null)
            services.AddScoped(typeof(IRuleSqlBuilderFactory<NpgsqlParameter>), factoryType);

        services.AddScoped<ISqlExecutor<NpgsqlParameter>, PgsqlSqlExecutor<TContext>>();
        services.AddScoped<IDbValidator<TContext, NpgsqlParameter>, PgsqlDbValidator<TContext>>();

        return services;
    }
}