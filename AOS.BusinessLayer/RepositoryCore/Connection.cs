using AOS.DataAccessLayer;
using AOS.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS.BusinessLayer
{
    public partial interface IBusinessLayer
    {
        IList<Connection> GetAllConnections();
        Connection GetConnectionById(int? connectionId);
        Connection GetConnectionByURL(string url);
        Connection GetConnectionByContextIdentifier(string contextIdentifier);
        void AddConnection(params Connection[] connections);
        void UpdateConnection(params Connection[] connections);
        void RemoveConnection(params Connection[] connections);
    }

    public partial class BusinessLayer : IBusinessLayer
    {
        public IList<Connection> GetAllConnections()
        {
            var connections = _ConnectionRepository.GetAll();

            // Decrypt syspassword
            if (connections != null)
            {
                foreach (var connection in connections)
                {
                    connection.SysPassword = Utils.UnprotectServicePassword(connection.SysPassword);
                }
            }

            return connections;
        }

        public Connection GetConnectionById(int? connectionId)
        {
            var connection = _ConnectionRepository.GetSingle(d => d.ConnectionID.Equals(connectionId));

            // Decrypt syspassword
            if (connection != null)
                connection.SysPassword = Utils.UnprotectServicePassword(connection.SysPassword);

            return connection;
        }

        public Connection GetConnectionByURL(string url)
        {
            var connection = _ConnectionRepository.GetSingle(d => d.URL.ToLower().Equals(url.ToLower()));

            // Decrypt syspassword
            if (connection != null)
                connection.SysPassword = Utils.UnprotectServicePassword(connection.SysPassword);

            return connection;
        }

        public Connection GetConnectionByContextIdentifier(string contextIdentifier)
        {
            var allConns = GetAllConnections();

            Connection currConnection = null;

            foreach (var item in allConns)
            {
                if (item.ContextIdentifier != null)
                {
                    if (item.ContextIdentifier.ToLower() == contextIdentifier.ToLower())
                    {
                        currConnection = item;
                        break;
                    }
                }
            }

            if (currConnection != null && !string.IsNullOrEmpty(currConnection.SysPassword))
                currConnection.SysPassword = Utils.UnprotectServicePassword(currConnection.SysPassword);

            return currConnection;
        }

        public void AddConnection(params Connection[] connections)
        {
            // Encrypt syspassword
            foreach (var connection in connections)
            {
                connection.SysPassword = Utils.ProtectServicePassword(connection.SysPassword);
            }

            // Fill createdate and createuser fields
            foreach (var item in connections)
            {
                item.CreateDate = DateTime.Now;
                item.CreateUser = _CurrentUser;
            }

            // Set createdate and createuser
            _ConnectionRepository.Add(connections);
        }

        public void UpdateConnection(params Connection[] connections)
        {
            // Encrypt syspassword
            foreach (var connection in connections)
            {
                connection.SysPassword = Utils.ProtectServicePassword(connection.SysPassword);
            }

            foreach (var item in connections)
            {
                item.UpdateDate = DateTime.Now;
                item.UpdateUser = _CurrentUser;
            }

            _ConnectionRepository.Update(connections);
        }

        public void RemoveConnection(params Connection[] connections)
        {
            /* Validation and error handling omitted */
            _ConnectionRepository.Remove(connections);
        }
    }
}
