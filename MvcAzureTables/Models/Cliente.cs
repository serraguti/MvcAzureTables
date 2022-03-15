using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcAzureTables.Models
{
    public class Cliente: TableEntity
    {
        //TODA TABLE ENTITY DEBE CONTENER UN ROWKEY
        //NECESITAMOS ALMACENAR EL ROWKEY CUANDO 
        //CAMBIEMOS LA PROPIEDAD IDCLIENTE
        //NECESITAMOS UNA PROPIEDAD EXTENDIDA
        private string _IdCliente;
        public string IdCliente
        {
            get { return this._IdCliente; }
            set {
                this._IdCliente = value;
                this.RowKey = value;
            }
        }
        //OPCIONAL, PODRIA CONTENER UNA CATEGORIA/GRUPO 
        //LLAMADA PARTITION KEY
        private string _Empresa;
        public string Empresa
        {
            get { return this._Empresa; }
            set {
                this._Empresa = value;
                this.PartitionKey = value;
            }
        }
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public int Salario { get; set; }
    }
}
