using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Tables;
using MvcAzureTables.Models;

namespace MvcAzureTables.Services
{
    public class ServiceTableStorage
    {
        private CloudTable tablaClientes;

        public ServiceTableStorage(string azureKeys)
        {
            //PARA ACCEDER A LAS TABLAS Y LOS SERVICIOS NECESITAMOS 
            //UNA CLASE DE AZURESTORAGE
            CloudStorageAccount account =
                CloudStorageAccount.Parse(azureKeys);
            CloudTableClient client = account.CreateCloudTableClient();
            this.tablaClientes = client.GetTableReference("clientes");
            Task.Run(async () =>
            {
                await this.tablaClientes.CreateIfNotExistsAsync();
            });
        }

        public async Task CreateClienteAsync(string id, string empresa
            , string nombre, int edad, int salario)
        {
            Cliente cliente = new Cliente
            {
               IdCliente = id, Empresa = empresa, Nombre = nombre
               , Edad = edad, Salario = salario
            };
            //LAS CONSULTAS DE ACCION SE REALIZAN MEDIANTE OBJETOS
            //DE TIPO TableOperation
            //POSTERIORMENTE, SE EJECUTAN ESTAS CONSULTAS SOBRE LA PROPIA TABLE
            TableOperation insert = TableOperation.Insert(cliente);
            await this.tablaClientes.ExecuteAsync(insert);
        }

        public async Task<List<Cliente>> GetClientesAsync()
        {
            //PARA REALIZAR BUSQUEDAS SOBRE UNA TABLA ENTITY
            //DEBEMOS UTILIZAR LA CLASE TableQuery<T>
            TableQuery<Cliente> query = new TableQuery<Cliente>();
            //LAS CONSULTAS DE SELECCION SE REALIZAN MEDIANTE 
            //SEGMENTOS, QUE INTERNAMENTE VA CONSTRUYENDO LA CONSULTA
            //DE OBJETOS JSON A CLASES
            TableQuerySegment<Cliente> segment =
                await this.tablaClientes.ExecuteQuerySegmentedAsync(query, null);
            List<Cliente> clientes = segment.Results;
            return clientes;
        }

        public async Task<List<Cliente>> GetClientesEmpresaAsync(string empresa)
        {
            TableQuery<Cliente> query = new TableQuery<Cliente>();
            TableQuerySegment<Cliente> segment =
                await this.tablaClientes.ExecuteQuerySegmentedAsync(query, null);
            //FILTRO SE REALIZA SOBRE EL PROPIO SEGMENT
            List<Cliente> clientes = segment.Where(x => x.Empresa == empresa)
                .ToList();
            return clientes;
        }

        //PARA BUSCAR OBJETOS UNICOS DENTRO DE TABLEENTITY PODEMOS 
        //HACERLO COMO EL METODO ANTERIOR O PODEMOS HACERLO POR 
        //SU ROWKEY.  NO EXISTE UN METODO DE BUSQUEDA SOLAMENTE POR ROWKEY
        //NOS PIDE TAMBIEN PARTITIONKEY
        public async Task<Cliente> FindClienteAsync(string rowkey
            , string partitionkey)
        {
            TableOperation select =
                TableOperation.Retrieve<Cliente>(partitionkey, rowkey);
            TableResult result =
                await this.tablaClientes.ExecuteAsync(select);
            Cliente cliente = result.Result as Cliente;
            return cliente;
        }

        public async Task DeleteClienteAsync(string rowkey, string partitionkey)
        {
            Cliente cliente = await this.FindClienteAsync(rowkey, partitionkey);
            TableOperation delete = TableOperation.Delete(cliente);
            await this.tablaClientes.ExecuteAsync(delete);
        }

        public async Task UpdateClienteAsync(string rowkey, string partitionkey
            , string nombre, int edad, int salario)
        {
            Cliente cliente = await this.FindClienteAsync(rowkey, partitionkey);
            cliente.Nombre = nombre;
            cliente.Edad = edad;
            cliente.Salario = salario;
            TableOperation update = TableOperation.Replace(cliente);
            await this.tablaClientes.ExecuteAsync(update);
        }
    }
}
