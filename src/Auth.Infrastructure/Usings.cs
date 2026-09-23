// Global using directives

global using System.Data;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;
global using Auth.Application.Common.Interfaces;
global using Auth.Domain.Entities;
global using Auth.Domain.Options;
global using Auth.Infrastructure.Persistence;
global using Auth.Infrastructure.Repository;
global using Auth.Infrastructure.Services;
global using Azure.Storage.Blobs;
global using Azure.Storage.Blobs.Models;
global using Dapper;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;
global using Npgsql;