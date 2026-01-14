using System.Reflection;
using BulkValidation.Base.Interfaces;
using BulkValidation.Core.Abstractions;
using BulkValidation.Core.Interfaces;
using BulkValidation.Core.Models;
using BulkValidation.Pgsql.Abstractions;
using BulkValidation.Pgsql.DbValidators;
using BulkValidation.Pgsql.Executors;
using BulkValidation.Pgsql.Factories;
using BulkValidation.Pgsql.MetadataResolvers;
using BulkValidation.Pgsql.SqlBuilders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace BulkValidation.Pgsql.Extensions;

public static class DiExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPgsqlDbValidators<TContext>() where TContext : DbContext
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.AddScoped<SharedCounter>();
        
        
            //sql builders
            services.AddSqlBuilders(assembly);
            services.AddScoped<CombinedSqlBuilderBase<NpgsqlParameter>, PgsqlCombinedSqlBuilder>();
            services.AddScoped<IRuleSqlBuilderFactory<NpgsqlParameter>, PgsqlRuleSqlBuilderFactory>();
            
            //Metadata resolver
            services.AddScoped<IMetadataResolver<TContext>, MetadataResolver<TContext>>();
        
            //Sql executor
            services.AddScoped<ISqlExecutor<NpgsqlParameter>, PgsqlSqlExecutor<TContext>>();
            
            //Db validators
            services.AddScoped<IDbValidator<TContext, NpgsqlParameter>, PgsqlDbValidator<TContext>>();
            return services;
        }

        private void AddSqlBuilders(Assembly assembly)
        {
            var builderTypes = assembly
                .GetTypes()
                .Where(t => !t.IsAbstract
                            && t is { IsInterface: false, BaseType.IsGenericType: true }
                            && t.BaseType.GetGenericTypeDefinition() == typeof(PgsqlRuleSqlBuilderBase<>))
                .ToList();

            foreach (var type in builderTypes)
                services.AddScoped(typeof(RuleSqlBuilderBase<NpgsqlParameter>), type);
        }
    }
}